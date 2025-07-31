using Employee_Attendance_Tracker.Models.Entities;

namespace Employee_Attendance_Tracker.Services.Interfaces
{
    public interface IAttendanceService
    {
        Task<IEnumerable<Attendance>> GetFilteredAttendancesAsync(int? departmentId, int? employeeId, DateTime? startDate, DateTime? endDate);
        Task<Attendance?> GetAttendanceByIdAsync(int id);
        Task<Attendance?> GetAttendanceByEmployeeAndDateAsync(int employeeId, DateTime date);
        Task<bool> CreateAttendanceAsync(Attendance attendance);
        Task<bool> UpdateAttendanceAsync(Attendance attendance);
        Task<bool> DeleteAttendanceAsync(int id);
        Task<bool> CanMarkAttendanceForDateAsync(DateTime date);
        Task<string> GetAttendanceStatusAsync(int employeeId, DateTime date);
    }
}
