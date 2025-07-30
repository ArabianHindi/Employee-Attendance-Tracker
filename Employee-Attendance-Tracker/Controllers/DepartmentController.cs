using Employee_Attendance_Tracker.Models.Entities;
using Employee_Attendance_Tracker.Models.ViewModels;
using Employee_Attendance_Tracker.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Employee_Attendance_Tracker.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        // GET: Department
        public async Task<IActionResult> Index()
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();

            // Convert Entity to ViewModel
            var departmentViewModels = departments.Select(d => new DepartmentViewModel
            {
                Id = d.Id,
                Name = d.Name,
                Code = d.Code,
                Location = d.Location,
                EmployeeCount = d.Employees?.Count ?? 0
            }).ToList();

            return View(departmentViewModels);
        }

        // GET: Department/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var department = await _departmentService.GetDepartmentByIdAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            // Convert Entity to ViewModel
            var viewModel = new DepartmentViewModel
            {
                Id = department.Id,
                Name = department.Name,
                Code = department.Code,
                Location = department.Location,
                EmployeeCount = department.Employees?.Count ?? 0
            };

            return View(viewModel);
        }

        // GET: Department/Create
        public IActionResult Create()
        {
            return View(new DepartmentViewModel());
        }

        // POST: Department/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DepartmentViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Convert ViewModel to Entity
                var department = new Department
                {
                    Name = viewModel.Name,
                    Code = viewModel.Code.ToUpper(), // Ensure uppercase
                    Location = viewModel.Location
                };

                var result = await _departmentService.CreateDepartmentAsync(department);
                if (result)
                {
                    TempData["SuccessMessage"] = "Department created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Failed to create department. Name or Code might already exist.");
                }
            }

            return View(viewModel);
        }

        // GET: Department/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var department = await _departmentService.GetDepartmentByIdAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            // Convert Entity to ViewModel
            var viewModel = new DepartmentViewModel
            {
                Id = department.Id,
                Name = department.Name,
                Code = department.Code,
                Location = department.Location
            };

            return View(viewModel);
        }

        // POST: Department/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DepartmentViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Convert ViewModel to Entity
                var department = new Department
                {
                    Id = viewModel.Id,
                    Name = viewModel.Name,
                    Code = viewModel.Code.ToUpper(), // Ensure uppercase
                    Location = viewModel.Location
                };

                var result = await _departmentService.UpdateDepartmentAsync(department);
                if (result)
                {
                    TempData["SuccessMessage"] = "Department updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Failed to update department. Name or Code might already exist.");
                }
            }

            return View(viewModel);
        }

        // GET: Department/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var department = await _departmentService.GetDepartmentByIdAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            // Convert Entity to ViewModel
            var viewModel = new DepartmentViewModel
            {
                Id = department.Id,
                Name = department.Name,
                Code = department.Code,
                Location = department.Location,
                EmployeeCount = department.Employees?.Count ?? 0
            };

            return View(viewModel);
        }

        // POST: Department/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _departmentService.DeleteDepartmentAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Department deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Cannot delete department. It may contain employees or not exist.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
