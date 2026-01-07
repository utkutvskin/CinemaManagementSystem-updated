
using CinemaManagementSystem. Enums;
using CinemaManagementSystem.Person;
using CinemaManagementSystem.Person.Roles;

namespace CinemaManagementSystem.Employees
{
    [Serializable]
    public class Manager : EmployeeRole
    {
        private readonly HashSet<Employee> _managedEmployees = new();
        public IReadOnlyCollection<Employee> ManagedEmployees => _managedEmployees;

        public Manager(Employee employee) :base(employee)
        {
        }

        public void AddManagedEmployee(Employee employee)
        {
            if (employee == null)
                throw new ArgumentException("Employee cannot be null.");
            _managedEmployees.Add(employee);
        }

        public void RemoveManagedEmployee(Employee employee)
        {
            if (employee == null)
                throw new ArgumentException("Employee cannot be null.");
            _managedEmployees.Remove(employee);
        }
        
    }
}