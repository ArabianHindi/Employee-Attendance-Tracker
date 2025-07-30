using Employee_Attendance_Tracker.Data;
using Employee_Attendance_Tracker.Models.Entities;
using Employee_Attendance_Tracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Employee_Attendance_Tracker.Services.Implementations
{
    public class EmployeeService : IEmployeeService
    {
        private readonly ApplicationDbContext _context;

        public EmployeeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateEmployeeAsync(Employee employee)
        {
            try
            {
                // Validate business rules
                if (await IsEmailUniqueAsync(employee.Email))
                {
                    return false; // Email already exists
                }

                // Validate full name format
                if (!IsValidFullName(employee.FullName))
                {
                    return false; // Invalid name format
                }

                // Validate department exists
                if (!await DepartmentExistsAsync(employee.DepartmentId))
                {
                    return false; // Department doesn't exist
                }

                // Generate unique employee code
                employee.EmployeeCode = await GenerateEmployeeCodeAsync();

                _context.Employees.Add(employee);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            try
            {
                var employee = await _context.Employees
                    .Include(e => e.Attendances)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (employee == null)
                {
                    return false;
                }

                // Note: Attendance records will be cascade deleted due to FK relationship
                _context.Employees.Remove(employee);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<int> GenerateEmployeeCodeAsync()
        {
            var maxCode = await _context.Employees
                .MaxAsync(e => (int?)e.EmployeeCode) ?? 1000;

            // Return next available code
            return maxCode + 1;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Attendances)
                .OrderBy(e => e.EmployeeCode)
                .ToListAsync();
        }

        public async Task<(int Present, int Absent, decimal Percentage)> GetCurrentMonthAttendanceSummaryAsync(int employeeId)
        {
            try
            {
                var currentDate = DateTime.Now;
                var startOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
                var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

                var attendances = await _context.Attendances
                    .Where(a => a.EmployeeId == employeeId &&
                               a.Date >= startOfMonth &&
                               a.Date <= endOfMonth)
                    .ToListAsync();

                if (!attendances.Any())
                {
                    return (0, 0, 0);
                }

                var presentCount = attendances.Count(a => a.IsPresent);
                var absentCount = attendances.Count(a => !a.IsPresent);
                var totalDays = attendances.Count;

                var percentage = totalDays > 0 ? Math.Round((decimal)presentCount / totalDays * 100, 2) : 0;

                return (presentCount, absentCount, percentage);
            }
            catch
            {
                return (0, 0, 0);
            }
        }

        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Attendances)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null)
        {
            var query = _context.Employees.Where(e => e.Email.ToLower() == email.ToLower());

            if (excludeId.HasValue)
            {
                query = query.Where(e => e.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> UpdateEmployeeAsync(Employee employee)
        {
            try
            {
                var existingEmployee = await _context.Employees.FindAsync(employee.Id);
                if (existingEmployee == null)
                {
                    return false;
                }

                // Validate business rules
                if (await IsEmailUniqueAsync(employee.Email, employee.Id))
                {
                    return false; // Email already exists
                }

                // Validate full name format
                if (!IsValidFullName(employee.FullName))
                {
                    return false; // Invalid name format
                }

                // Validate department exists
                if (!await DepartmentExistsAsync(employee.DepartmentId))
                {
                    return false; // Department doesn't exist
                }

                // Update properties (Employee Code should not be updated)
                existingEmployee.FullName = employee.FullName;
                existingEmployee.Email = employee.Email;
                existingEmployee.DepartmentId = employee.DepartmentId;

                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return false;

            // Regex pattern: exactly 4 names, each at least 2 characters, letters and spaces only
            var pattern = @"^([A-Za-z]{2,}\s){3}[A-Za-z]{2,}$";
            return Regex.IsMatch(fullName.Trim(), pattern);
        }

        private async Task<bool> DepartmentExistsAsync(int departmentId)
        {
            return await _context.Departments.AnyAsync(d => d.Id == departmentId);
        }
    }
}
