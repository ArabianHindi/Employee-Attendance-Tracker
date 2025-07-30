using Employee_Attendance_Tracker.Models.Entities;
using Employee_Attendance_Tracker.Services.Interfaces;

namespace Employee_Attendance_Tracker.Services.Implementations
{
    public class AttendanceService : IAttendanceService
    {
        public Task<bool> CanMarkAttendanceForDateAsync(DateTime date)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CreateAttendanceAsync(Attendance attendance)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAttendanceAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Attendance>> GetAllAttendancesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Attendance> GetAttendanceByEmployeeAndDateAsync(int employeeId, DateTime date)
        {
            throw new NotImplementedException();
        }

        public Task<Attendance> GetAttendanceByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetAttendanceStatusAsync(int employeeId, DateTime date)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Attendance>> GetFilteredAttendancesAsync(int? departmentId, int? employeeId, DateTime? startDate, DateTime? endDate)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAttendanceAsync(Attendance attendance)
        {
            throw new NotImplementedException();
        }
    }
}
