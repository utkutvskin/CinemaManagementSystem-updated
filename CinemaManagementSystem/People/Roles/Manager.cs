
using CinemaManagementSystem. Enums;

namespace CinemaManagementSystem.People.Roles
{
    [Serializable]
    public class Manager : EmployeeRole
    {
        private readonly HashSet<Employee> _managedEmployees = new();
        public IReadOnlyCollection<Employee> ManagedEmployees => _managedEmployees;

        public Manager(Employee employee) :base(employee)
        {
        }

        //PartTime
        public Manager(Employee employee, double salary) :base(employee, salary)
        {
        }
        //FullTime
        public Manager(Employee employee, double hourlyRate, double hoursPerMonth) 
            :base(employee, hourlyRate, hoursPerMonth)
        {     
        }
        //Intern
        public Manager(Employee employee, string universityName, double? salary = null) 
            :base(employee, universityName, salary)
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