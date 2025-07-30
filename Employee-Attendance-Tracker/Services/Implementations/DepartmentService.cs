using Employee_Attendance_Tracker.Data;
using Employee_Attendance_Tracker.Models.Entities;
using Employee_Attendance_Tracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Employee_Attendance_Tracker.Services.Implementations
{
    public class DepartmentService : IDepartmentService
    {

        private readonly ApplicationDbContext _context;

        public DepartmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateDepartmentAsync(Department department)
        {
            try
            {
                // Validate business rules
                if (await IsDepartmentNameUniqueAsync(department.Name))
                {
                    return false;
                }

                if (await IsDepartmentCodeUniqueAsync(department.Code))
                {
                    return false;
                }

                // Additional validation
                if (!IsValidDepartmentCode(department.Code))
                {
                    return false;
                }

                _context.Departments.Add(department);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteDepartmentAsync(int id)
        {
            try
            {
                var department = await _context.Departments
                    .Include(d => d.Employees)
                    .FirstOrDefaultAsync(d => d.Id == id);

                if (department == null)
                {
                    return false;
                }

                // Business rule: Cannot delete department with employees
                if (department.Employees != null && department.Employees.Any())
                {
                    return false;
                }

                _context.Departments.Remove(department);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
        {
            return await _context.Departments
                .Include(d => d.Employees)
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        public async Task<Department> GetDepartmentByIdAsync(int id)
        {
            return await _context.Departments
                .Include(d => d.Employees)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<int> GetEmployeeCountByDepartmentAsync(int departmentId)
        {
            return await _context.Employees
                .CountAsync(e => e.DepartmentId == departmentId);
        }

        public async Task<bool> IsDepartmentCodeUniqueAsync(string code, int? excludeId = null)
        {
            var query = _context.Departments.Where(d => d.Code.ToUpper() == code.ToUpper());

            if (excludeId.HasValue)
            {
                query = query.Where(d => d.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> IsDepartmentNameUniqueAsync(string name, int? excludeId = null)
        {
            var query = _context.Departments.Where(d => d.Name.ToLower() == name.ToLower());

            if (excludeId.HasValue)
            {
                query = query.Where(d => d.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> UpdateDepartmentAsync(Department department)
        {
            try
            {
                var existingDepartment = await _context.Departments.FindAsync(department.Id);
                if (existingDepartment == null)
                {
                    return false;
                }

                // Validate business rules
                if (await IsDepartmentNameUniqueAsync(department.Name, department.Id))
                {
                    return false;
                }

                if (await IsDepartmentCodeUniqueAsync(department.Code, department.Id))
                {
                    return false;
                }

                // Additional validation
                if (!IsValidDepartmentCode(department.Code))
                {
                    return false; // Invalid code format
                }

                // Update properties
                existingDepartment.Name = department.Name;
                existingDepartment.Code = department.Code;
                existingDepartment.Location = department.Location;

                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidDepartmentCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return false;

            if (code.Length != 4)
                return false;

            return code.All(c => char.IsLetter(c) && char.IsUpper(c));
        }
    }
}
