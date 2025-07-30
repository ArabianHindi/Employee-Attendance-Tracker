using Employee_Attendance_Tracker.Data;
using Employee_Attendance_Tracker.Models.Entities;
using Employee_Attendance_Tracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Employee_Attendance_Tracker.Services.Implementations
{
    public class AttendanceService : IAttendanceService
    {
        private readonly ApplicationDbContext _context;

        public AttendanceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CanMarkAttendanceForDateAsync(DateTime date)
        {
            return date.Date <= DateTime.Now.Date;
        }

        public async Task<bool> CreateAttendanceAsync(Attendance attendance)
        {
            try
            {
                // Normalize date to remove time component
                attendance.Date = attendance.Date.Date;

                // Validate business rules
                if (!await CanMarkAttendanceForDateAsync(attendance.Date))
                {
                    return false;
                }

                // Validate employee exists
                if (!await EmployeeExistsAsync(attendance.EmployeeId))
                {
                    return false;
                }

                // Check if attendance already exists for this employee and date
                var existingAttendance = await GetAttendanceByEmployeeAndDateAsync(attendance.EmployeeId, attendance.Date);
                if (existingAttendance != null)
                {
                    return false;
                }

                _context.Attendances.Add(attendance);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteAttendanceAsync(int id)
        {
            try
            {
                var attendance = await _context.Attendances.FindAsync(id);
                if (attendance == null)
                {
                    return false;
                }

                _context.Attendances.Remove(attendance);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<Attendance>> GetAllAttendancesAsync()
        {
            return await _context.Attendances
                .Include(a => a.Employee)
                .ThenInclude(e => e.Department)
                .OrderByDescending(a => a.Date)
                .ThenBy(a => a.Employee.FullName)
                .ToListAsync();
        }

        public async Task<Attendance> GetAttendanceByEmployeeAndDateAsync(int employeeId, DateTime date)
        {
            var normalizedDate = date.Date;

            return await _context.Attendances
                .Include(a => a.Employee)
                .ThenInclude(e => e.Department)
                .FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.Date.Date == normalizedDate);
        }

        public async Task<Attendance> GetAttendanceByIdAsync(int id)
        {
            return await _context.Attendances
                .Include(a => a.Employee)
                .ThenInclude(e => e.Department)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<string> GetAttendanceStatusAsync(int employeeId, DateTime date)
        {
            try
            {
                var attendance = await GetAttendanceByEmployeeAndDateAsync(employeeId, date);

                if (attendance == null)
                {
                    return "Not marked";
                }

                return attendance.IsPresent ? "Present" : "Absent";
            }
            catch
            {
                return "Error";
            }
        }

        public async Task<IEnumerable<Attendance>> GetFilteredAttendancesAsync(int? departmentId, int? employeeId, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var query = _context.Attendances
                    .Include(a => a.Employee)
                    .ThenInclude(e => e.Department)
                    .AsQueryable();

                // Filter by department
                if (departmentId.HasValue && departmentId.Value > 0)
                {
                    query = query.Where(a => a.Employee.DepartmentId == departmentId.Value);
                }

                // Filter by employee
                if (employeeId.HasValue && employeeId.Value > 0)
                {
                    query = query.Where(a => a.EmployeeId == employeeId.Value);
                }

                // Filter by date range
                if (startDate.HasValue)
                {
                    var normalizedStartDate = startDate.Value.Date;
                    query = query.Where(a => a.Date.Date >= normalizedStartDate);
                }

                if (endDate.HasValue)
                {
                    var normalizedEndDate = endDate.Value.Date;
                    query = query.Where(a => a.Date.Date <= normalizedEndDate);
                }

                return await query
                    .OrderByDescending(a => a.Date)
                    .ThenBy(a => a.Employee.FullName)
                    .ToListAsync();
            }
            catch
            {
                return new List<Attendance>();
            }
        }

        public async Task<bool> UpdateAttendanceAsync(Attendance attendance)
        {
            try
            {
                var existingAttendance = await _context.Attendances.FindAsync(attendance.Id);
                if (existingAttendance == null)
                {
                    return false;
                }

                // Normalize date to remove time component
                attendance.Date = attendance.Date.Date;

                // Validate business rules
                if (!await CanMarkAttendanceForDateAsync(attendance.Date))
                {
                    return false;
                }

                // Validate employee exists
                if (!await EmployeeExistsAsync(attendance.EmployeeId))
                {
                    return false;
                }

                // If date or employee changed, check for duplicates
                if (existingAttendance.EmployeeId != attendance.EmployeeId ||
                    existingAttendance.Date.Date != attendance.Date.Date)
                {
                    var duplicateAttendance = await GetAttendanceByEmployeeAndDateAsync(attendance.EmployeeId, attendance.Date);
                    if (duplicateAttendance != null && duplicateAttendance.Id != attendance.Id)
                    {
                        return false;
                    }
                }

                // Update properties
                existingAttendance.EmployeeId = attendance.EmployeeId;
                existingAttendance.Date = attendance.Date;
                existingAttendance.IsPresent = attendance.IsPresent;

                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> EmployeeExistsAsync(int employeeId)
        {
            return await _context.Employees.AnyAsync(e => e.Id == employeeId);
        }
    }
}
