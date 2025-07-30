using Employee_Attendance_Tracker.Models.Entities;
using Employee_Attendance_Tracker.Models.ViewModels;
using Employee_Attendance_Tracker.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Employee_Attendance_Tracker.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;

        public EmployeeController(IEmployeeService employeeService, IDepartmentService departmentService)
        {
            _employeeService = employeeService;
            _departmentService = departmentService;
        }

        // GET: Employee
        public async Task<IActionResult> Index()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();

            // Convert Entity to ViewModel with attendance summary
            var employeeViewModels = new List<EmployeeViewModel>();

            foreach (var employee in employees)
            {
                var attendanceSummary = await _employeeService.GetCurrentMonthAttendanceSummaryAsync(employee.Id);

                employeeViewModels.Add(new EmployeeViewModel
                {
                    Id = employee.Id,
                    EmployeeCode = employee.EmployeeCode,
                    FullName = employee.FullName,
                    Email = employee.Email,
                    DepartmentId = employee.DepartmentId,
                    DepartmentName = employee.Department?.Name,
                    PresentDays = attendanceSummary.Present,
                    AbsentDays = attendanceSummary.Absent,
                    AttendancePercentage = attendanceSummary.Percentage
                });
            }

            return View(employeeViewModels);
        }

        // GET: Employee/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            var attendanceSummary = await _employeeService.GetCurrentMonthAttendanceSummaryAsync(employee.Id);

            // Convert Entity to ViewModel
            var viewModel = new EmployeeViewModel
            {
                Id = employee.Id,
                EmployeeCode = employee.EmployeeCode,
                FullName = employee.FullName,
                Email = employee.Email,
                DepartmentId = employee.DepartmentId,
                DepartmentName = employee.Department?.Name,
                PresentDays = attendanceSummary.Present,
                AbsentDays = attendanceSummary.Absent,
                AttendancePercentage = attendanceSummary.Percentage
            };

            return View(viewModel);
        }

        // GET: Employee/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new EmployeeViewModel
            {
                Departments = await GetDepartmentSelectListAsync()
            };

            return View(viewModel);
        }

        // POST: Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Convert ViewModel to Entity
                var employee = new Employee
                {
                    FullName = viewModel.FullName.Trim(),
                    Email = viewModel.Email.Trim().ToLower(),
                    DepartmentId = viewModel.DepartmentId
                };

                var result = await _employeeService.CreateEmployeeAsync(employee);
                if (result)
                {
                    TempData["SuccessMessage"] = "Employee created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Failed to create employee. Email might already exist or invalid data provided.");
                }
            }

            // Reload departments for dropdown
            viewModel.Departments = await GetDepartmentSelectListAsync();
            return View(viewModel);
        }

        // GET: Employee/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            // Convert Entity to ViewModel
            var viewModel = new EmployeeViewModel
            {
                Id = employee.Id,
                EmployeeCode = employee.EmployeeCode,
                FullName = employee.FullName,
                Email = employee.Email,
                DepartmentId = employee.DepartmentId,
                Departments = await GetDepartmentSelectListAsync()
            };

            return View(viewModel);
        }

        // POST: Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmployeeViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Convert ViewModel to Entity
                var employee = new Employee
                {
                    Id = viewModel.Id,
                    EmployeeCode = viewModel.EmployeeCode,
                    FullName = viewModel.FullName.Trim(),
                    Email = viewModel.Email.Trim().ToLower(),
                    DepartmentId = viewModel.DepartmentId
                };

                var result = await _employeeService.UpdateEmployeeAsync(employee);
                if (result)
                {
                    TempData["SuccessMessage"] = "Employee updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Failed to update employee. Email might already exist or invalid data provided.");
                }
            }

            // Reload departments for dropdown
            viewModel.Departments = await GetDepartmentSelectListAsync();
            return View(viewModel);
        }

        // GET: Employee/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            var attendanceSummary = await _employeeService.GetCurrentMonthAttendanceSummaryAsync(employee.Id);

            // Convert Entity to ViewModel
            var viewModel = new EmployeeViewModel
            {
                Id = employee.Id,
                EmployeeCode = employee.EmployeeCode,
                FullName = employee.FullName,
                Email = employee.Email,
                DepartmentId = employee.DepartmentId,
                DepartmentName = employee.Department?.Name,
                PresentDays = attendanceSummary.Present,
                AbsentDays = attendanceSummary.Absent,
                AttendancePercentage = attendanceSummary.Percentage
            };

            return View(viewModel);
        }

        // POST: Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _employeeService.DeleteEmployeeAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Employee deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete employee.";
            }

            return RedirectToAction(nameof(Index));
        }

        // Helper method to get department dropdown list
        private async Task<SelectList> GetDepartmentSelectListAsync()
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            return new SelectList(departments, "Id", "Name");
        }
    }
}
