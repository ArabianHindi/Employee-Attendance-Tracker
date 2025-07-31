using Employee_Attendance_Tracker.Data;
using Employee_Attendance_Tracker.Models.DTOs;
using Employee_Attendance_Tracker.Models.Entities;
using Employee_Attendance_Tracker.Models.ViewModels;
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

        private int GenerateEmployeeCode()
        {
            return _context.Employees.Any()
                ? _context.Employees.Max(e => e.EmployeeCode) + 1
                : 1000;
        }

        public async Task<List<EmployeeViewModel>> GetAllAsync()
        {
            var employees = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Attendances)
                .ToListAsync();

            var thisMonth = DateTime.Now.Month;
            var thisYear = DateTime.Now.Year;

            return employees.Select(e =>
            {
                var monthlyAttendances = e.Attendances?
                    .Where(a => a.Date.Month == thisMonth && a.Date.Year == thisYear)
                    .ToList();

                var presents = monthlyAttendances?.Count(a => a.IsPresent) ?? 0;
                var absents = monthlyAttendances?.Count(a => !a.IsPresent) ?? 0;

                return new EmployeeViewModel
                {
                    Id = e.Id,
                    EmployeeCode = e.EmployeeCode,
                    FullName = e.FullName,
                    Email = e.Email,
                    DepartmentId = e.DepartmentId,
                    DepartmentName = e.Department?.Name,
                    Presents = presents,
                    Absents = absents
                };
            }).ToList();
        }

        public async Task<EmployeeViewModel> GetByIdAsync(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Attendances)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null) return null;

            var thisMonth = DateTime.Now.Month;
            var thisYear = DateTime.Now.Year;

            var monthlyAttendances = employee.Attendances?
                .Where(a => a.Date.Month == thisMonth && a.Date.Year == thisYear)
                .ToList();

            var presents = monthlyAttendances?.Count(a => a.IsPresent) ?? 0;
            var absents = monthlyAttendances?.Count(a => !a.IsPresent) ?? 0;

            return new EmployeeViewModel
            {
                Id = employee.Id,
                EmployeeCode = employee.EmployeeCode,
                FullName = employee.FullName,
                Email = employee.Email,
                DepartmentId = employee.DepartmentId,
                DepartmentName = employee.Department?.Name,
                Presents = presents,
                Absents = absents
            };
        }

        public async Task AddAsync(EmployeeDTO dto)
        {
            if (await _context.Employees.AnyAsync(e => e.Email == dto.Email))
                throw new Exception("Email already exists.");

            var employee = new Employee
            {
                EmployeeCode = GenerateEmployeeCode(),
                FullName = dto.FullName,
                Email = dto.Email,
                DepartmentId = dto.DepartmentId
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(int id, EmployeeDTO dto)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                throw new Exception("Employee not found.");

            if (await _context.Employees.AnyAsync(e => e.Email == dto.Email && e.Id != id))
                throw new Exception("Email already exists.");

            employee.FullName = dto.FullName;
            employee.Email = dto.Email;
            employee.DepartmentId = dto.DepartmentId;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                throw new Exception("Employee not found.");

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
        }
    }
}
