using Employee_Attendance_Tracker.Models.Entities;
using Employee_Attendance_Tracker.Models.ViewModels;
using Employee_Attendance_Tracker.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Employee_Attendance_Tracker.Presentation.Controllers
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

        // GET: /Attendance
        public async Task<IActionResult> Index(AttendanceFilterViewModel filter)
        {
            filter.Departments = await GetDepartmentSelectListAsync();
            filter.Employees = await GetEmployeeSelectListAsync();

            var attendances = await _attendanceService.GetFilteredAttendancesAsync(
                filter.DepartmentId, filter.EmployeeId, filter.StartDate, filter.EndDate);

            filter.FilteredAttendances = attendances.Select(a => new AttendanceViewModel
            {
                Id = a.Id,
                EmployeeId = a.EmployeeId,
                EmployeeName = a.Employee.FullName,
                DepartmentName = a.Employee.Department?.Name,
                Date = a.Date,
                IsPresent = a.IsPresent
            }).ToList();

            return View(filter);
        }

        // GET: /Attendance/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new AttendanceViewModel
            {
                Date = DateTime.Today,
                Employees = await GetEmployeeSelectListAsync()
            };

            return View(viewModel);
        }

        // POST: /Attendance/Create
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

                ModelState.AddModelError("", "Failed to mark attendance. It may already exist or the date is invalid.");
            }

            viewModel.Employees = await GetEmployeeSelectListAsync();
            return View(viewModel);
        }

        // GET: /Attendance/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var attendance = await _attendanceService.GetAttendanceByIdAsync(id);
            if (attendance == null) return NotFound();

            var viewModel = new AttendanceViewModel
            {
                Id = attendance.Id,
                EmployeeId = attendance.EmployeeId,
                Date = attendance.Date,
                IsPresent = attendance.IsPresent,
                Employees = await GetEmployeeSelectListAsync()
            };

            return View(viewModel);
        }

        // POST: /Attendance/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AttendanceViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var attendance = new Attendance
                {
                    Id = viewModel.Id,
                    EmployeeId = viewModel.EmployeeId,
                    Date = viewModel.Date,
                    IsPresent = viewModel.IsPresent
                };

                var result = await _attendanceService.UpdateAttendanceAsync(attendance);
                if (result)
                {
                    TempData["SuccessMessage"] = "Attendance updated successfully.";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "Failed to update attendance.");
            }

            viewModel.Employees = await GetEmployeeSelectListAsync();
            return View(viewModel);
        }

        // POST: /Attendance/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _attendanceService.DeleteAttendanceAsync(id);
            if (result)
                TempData["SuccessMessage"] = "Attendance deleted successfully.";
            else
                TempData["ErrorMessage"] = "Failed to delete attendance.";

            return RedirectToAction(nameof(Index));
        }

        // GET: /Attendance/GetAttendanceStatus?employeeId=1&date=2025-07-31
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

        // POST: /Attendance/UpdateAttendanceStatus
        [HttpPost]
        public async Task<IActionResult> UpdateAttendanceStatus(int employeeId, string date, bool isPresent)
        {
            if (!DateTime.TryParse(date, out DateTime parsedDate))
                return Json(new { success = false, message = "Invalid date" });

            var existing = await _attendanceService.GetAttendanceByEmployeeAndDateAsync(employeeId, parsedDate);

            bool result;
            if (existing == null)
            {
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
                existing.IsPresent = isPresent;
                result = await _attendanceService.UpdateAttendanceAsync(existing);
            }

            if (result)
            {
                return Json(new { success = true, status = isPresent ? "Present" : "Absent" });
            }

            return Json(new { success = false, message = "Failed to update attendance" });
        }

        // Helpers
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
