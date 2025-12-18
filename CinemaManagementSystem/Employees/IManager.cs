namespace CinemaManagementSystem.Employees;

public interface IManager
{
    IReadOnlyCollection<Employee> ManagedEmployees { get; }
    void AddManagedEmployee(Employee employee);
    void RemoveManagedEmployee(Employee employee);
    void ApplyBonusesToFullTimeEmployee(Employee employee, double bonus);
}