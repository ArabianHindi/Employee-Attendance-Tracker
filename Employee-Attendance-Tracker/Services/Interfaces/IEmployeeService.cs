using Employee_Attendance_Tracker.Models.DTOs;
using Employee_Attendance_Tracker.Models.Entities;
using Employee_Attendance_Tracker.Models.ViewModels;

namespace Employee_Attendance_Tracker.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<List<EmployeeViewModel>> GetAllAsync();
        Task<EmployeeViewModel> GetByIdAsync(int id);
        Task AddAsync(EmployeeDTO employeeDto);
        Task UpdateAsync(int id, EmployeeDTO employeeDto);
        Task DeleteAsync(int id);
    }
}