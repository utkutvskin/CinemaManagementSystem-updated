using CinemaManagementSystem.ContractTypeForEmployee;
using CinemaManagementSystem.Enums;

namespace CinemaManagementSystem.Employees
{
    [Serializable]
    public class Manager : Employee
    {
        private readonly HashSet<Employee> _managedEmployees = new();
        public IReadOnlyCollection<Employee> ManagedEmployees => _managedEmployees;

        public Manager() { }

        public Manager(string name, string surname, DateTime birthDate,
                       DateTime startDate, double salary)
            : base(name, surname, birthDate, startDate, salary) { }

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
        
        
        public void ApplyBonusesToFullTimeEmployee(Employee employee, double bonus)
        {
            if (employee.ContractType is not FullTimeContract fullTime)
                throw new ArgumentException("Employee must have full time contract to gain bonuses");

            if (bonus <= 0)
                throw new ArgumentException("Bonus must be positive.");

            fullTime.AddBonus(bonus);
        }
    }
}
