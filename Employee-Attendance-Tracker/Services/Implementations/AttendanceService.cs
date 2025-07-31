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

        public async Task<IEnumerable<Attendance>> GetFilteredAttendancesAsync(int? departmentId, int? employeeId, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Attendances
                .Include(a => a.Employee)
                .ThenInclude(e => e.Department)
                .AsQueryable();

            if (departmentId.HasValue && departmentId > 0)
                query = query.Where(a => a.Employee.DepartmentId == departmentId);

            if (employeeId.HasValue && employeeId > 0)
                query = query.Where(a => a.EmployeeId == employeeId);

            if (startDate.HasValue)
                query = query.Where(a => a.Date.Date >= startDate.Value.Date);

            if (endDate.HasValue)
                query = query.Where(a => a.Date.Date <= endDate.Value.Date);

            return await query
                .OrderByDescending(a => a.Date)
                .ThenBy(a => a.Employee.FullName)
                .ToListAsync();
        }

        public async Task<Attendance?> GetAttendanceByIdAsync(int id)
        {
            return await _context.Attendances
                .Include(a => a.Employee)
                .ThenInclude(e => e.Department)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Attendance?> GetAttendanceByEmployeeAndDateAsync(int employeeId, DateTime date)
        {
            return await _context.Attendances
                .Include(a => a.Employee)
                .ThenInclude(e => e.Department)
                .FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.Date.Date == date.Date);
        }

        public async Task<bool> CanMarkAttendanceForDateAsync(DateTime date)
        {
            return date.Date <= DateTime.Today;
        }

        public async Task<string> GetAttendanceStatusAsync(int employeeId, DateTime date)
        {
            var attendance = await GetAttendanceByEmployeeAndDateAsync(employeeId, date);
            if (attendance == null) return "Not marked";
            return attendance.IsPresent ? "Present" : "Absent";
        }

        public async Task<bool> CreateAttendanceAsync(Attendance attendance)
        {
            attendance.Date = attendance.Date.Date;

            if (!await CanMarkAttendanceForDateAsync(attendance.Date)) return false;

            if (!await _context.Employees.AnyAsync(e => e.Id == attendance.EmployeeId)) return false;

            if (await _context.Attendances.AnyAsync(a => a.EmployeeId == attendance.EmployeeId && a.Date.Date == attendance.Date))
                return false;

            _context.Attendances.Add(attendance);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAttendanceAsync(Attendance attendance)
        {
            var existing = await _context.Attendances.FindAsync(attendance.Id);
            if (existing == null) return false;

            if (!await CanMarkAttendanceForDateAsync(attendance.Date)) return false;

            if (!await _context.Employees.AnyAsync(e => e.Id == attendance.EmployeeId)) return false;

            if ((existing.EmployeeId != attendance.EmployeeId || existing.Date.Date != attendance.Date.Date) &&
                await _context.Attendances.AnyAsync(a =>
                    a.EmployeeId == attendance.EmployeeId && a.Date.Date == attendance.Date.Date && a.Id != attendance.Id))
                return false;

            existing.EmployeeId = attendance.EmployeeId;
            existing.Date = attendance.Date.Date;
            existing.IsPresent = attendance.IsPresent;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAttendanceAsync(int id)
        {
            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance == null) return false;

            _context.Attendances.Remove(attendance);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
