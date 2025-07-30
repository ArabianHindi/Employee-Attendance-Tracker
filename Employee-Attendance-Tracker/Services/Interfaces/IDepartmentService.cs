using Employee_Attendance_Tracker.Models.Entities;

namespace Employee_Attendance_Tracker.Services.Interfaces
{
    public interface IDepartmentService
    {
        Task<IEnumerable<Department>> GetAllDepartmentsAsync();
        Task<Department> GetDepartmentByIdAsync(int id);
        Task<bool> CreateDepartmentAsync(Department department);
        Task<bool> UpdateDepartmentAsync(Department department);
        Task<bool> DeleteDepartmentAsync(int id);
        Task<bool> IsDepartmentNameUniqueAsync(string name, int? excludeId = null);
        Task<bool> IsDepartmentCodeUniqueAsync(string code, int? excludeId = null);
        Task<int> GetEmployeeCountByDepartmentAsync(int departmentId);
    }
}
