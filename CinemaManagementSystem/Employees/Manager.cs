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

        public void ChangeRoleForEmployee(Employee employee, double newSalary, HashSet<Employee>? managedEmployees = null)
        {
            if (employee.GetType() == typeof(Manager))
                throw new ArgumentException("This employee is already manager");
            
            var e = new Manager(employee.Name, employee.Surname, employee.BirthDate, DateTime.Now, Salary);
            employee.EndDate = DateTime.Now;

            if (managedEmployees != null)
            {
                foreach (var managedEmployee in managedEmployees)
                    e.AddManagedEmployee(managedEmployee);
            }
            
        }
        
        public void ChangeRoleForEmployee(Employee employee, double newSalary, int deskNumber)
        {
            if (employee.GetType() == typeof(Receptionist))
                throw new ArgumentException("This employee is already receptionist");
            
            var e = new Receptionist(employee.Name, employee.Surname, employee.BirthDate, DateTime.Now, newSalary, deskNumber);
            employee.EndDate = DateTime.Now;
            
        }

        public void ChangeRoleForEmployee(Employee employee, double newSalary, CleaningTypeEnum type)
        {
            if (employee.GetType() == typeof(Cleaner))
                throw new ArgumentException("This employee is already cleaner");
            var e  = new Cleaner(type, employee.Name, employee.Surname, employee.BirthDate, DateTime.Now, newSalary);
            employee.EndDate = DateTime.Now;
        }

        public void ChangeRoleForEmployee(Employee employee, double newSalary)
        {
            if (employee.GetType() == typeof(BuffetSeller))
                throw new ArgumentException("This employee is already buffet seller");
            var e = new BuffetSeller(employee.Name, employee.Surname, employee.BirthDate, DateTime.Now, newSalary);
            employee.EndDate = DateTime.Now;
        }
        
    }
}
