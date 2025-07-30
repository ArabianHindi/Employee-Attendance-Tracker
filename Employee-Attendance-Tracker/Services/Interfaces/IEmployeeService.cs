using Employee_Attendance_Tracker.Models.Entities;

namespace Employee_Attendance_Tracker.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<Employee>> GetAllEmployeesAsync();
        Task<Employee> GetEmployeeByIdAsync(int id);
        Task<bool> CreateEmployeeAsync(Employee employee);
        Task<bool> UpdateEmployeeAsync(Employee employee);
        Task<bool> DeleteEmployeeAsync(int id);
        Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null);
        Task<int> GenerateEmployeeCodeAsync();
        Task<(int Present, int Absent, decimal Percentage)> GetCurrentMonthAttendanceSummaryAsync(int employeeId);
    }
}
