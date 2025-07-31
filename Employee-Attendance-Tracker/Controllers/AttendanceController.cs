using Employee_Attendance_Tracker.Models.Entities;
using Employee_Attendance_Tracker.Models.ViewModels;
using Employee_Attendance_Tracker.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Employee_Attendance_Tracker.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly IAttendanceService _attendanceService;
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;

        public AttendanceController(
            IAttendanceService attendanceService,
            IEmployeeService employeeService,
            IDepartmentService departmentService)
        {
            _attendanceService = attendanceService;
            _employeeService = employeeService;
            _departmentService = departmentService;
        }

        // GET: Attendance
        public async Task<IActionResult> Index(AttendanceFilterViewModel filter)
        {
            // Load filter dropdown data
            filter.Departments = await GetDepartmentSelectListAsync();
            filter.Employees = await GetEmployeeSelectListAsync();

            // Get filtered attendances
            var attendances = await _attendanceService.GetFilteredAttendancesAsync(
                filter.DepartmentId, filter.EmployeeId, filter.StartDate, filter.EndDate);

            // Convert to ViewModels
            filter.FilteredAttendances = attendances.Select(a => new AttendanceViewModel
            {
                Id = a.Id,
                EmployeeId = a.EmployeeId,
                EmployeeName = a.Employee.FullName,
                DepartmentName = a.Employee.Department.Name,
                Date = a.Date,
                IsPresent = a.IsPresent
            }).ToList();

            return View(filter);
        }

        // GET: Attendance/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new AttendanceViewModel
            {
                Date = DateTime.Today,
                Employees = await GetEmployeeSelectListAsync()
            };

            return View(viewModel);
        }

        // POST: Attendance/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AttendanceViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var attendance = new Attendance
                {
                    EmployeeId = viewModel.EmployeeId,
                    Date = viewModel.Date,
                    IsPresent = viewModel.IsPresent
                };

                var result = await _attendanceService.CreateAttendanceAsync(attendance);
                if (result)
                {
                    TempData["SuccessMessage"] = "Attendance marked successfully.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Failed to mark attendance. Attendance may already exist for this date or date is invalid.");
                }
            }

            viewModel.Employees = await GetEmployeeSelectListAsync();
            return View(viewModel);
        }

        // AJAX method for dynamic attendance status
        [HttpGet]
        public async Task<IActionResult> GetAttendanceStatus(int employeeId, string date)
        {
            if (DateTime.TryParse(date, out DateTime parsedDate))
            {
                var status = await _attendanceService.GetAttendanceStatusAsync(employeeId, parsedDate);
                var canMark = await _attendanceService.CanMarkAttendanceForDateAsync(parsedDate);

                return Json(new { status, canMark });
            }

            return Json(new { status = "Error", canMark = false });
        }

        // AJAX method for updating attendance
        [HttpPost]
        public async Task<IActionResult> UpdateAttendanceStatus(int employeeId, string date, bool isPresent)
        {
            if (DateTime.TryParse(date, out DateTime parsedDate))
            {
                var existingAttendance = await _attendanceService.GetAttendanceByEmployeeAndDateAsync(employeeId, parsedDate);

                bool result;
                if (existingAttendance == null)
                {
                    // Create new attendance
                    var newAttendance = new Attendance
                    {
                        EmployeeId = employeeId,
                        Date = parsedDate,
                        IsPresent = isPresent
                    };
                    result = await _attendanceService.CreateAttendanceAsync(newAttendance);
                }
                else
                {
                    // Update existing attendance
                    existingAttendance.IsPresent = isPresent;
                    result = await _attendanceService.UpdateAttendanceAsync(existingAttendance);
                }

                if (result)
                {
                    var status = isPresent ? "Present" : "Absent";
                    return Json(new { success = true, status });
                }
            }

            return Json(new { success = false, message = "Failed to update attendance" });
        }

        // Helper methods
        private async Task<SelectList> GetEmployeeSelectListAsync()
        {
            var employees = await _employeeService.GetAllAsync();
            return new SelectList(employees.Select(e => new { e.Id, DisplayName = $"{e.FullName} ({e.EmployeeCode})" }), "Id", "DisplayName");
        }

        private async Task<SelectList> GetDepartmentSelectListAsync()
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            return new SelectList(departments, "Id", "Name");
        }
    }
}
