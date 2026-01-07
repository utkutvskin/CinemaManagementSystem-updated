using System.Xml.Serialization;
using CinemaManagementSystem.Exceptions;
using CinemaManagementSystem.Person.Roles;

namespace CinemaManagementSystem.Person.ContractType
{
    [Serializable]
    public class FullTimeContract
    {
        private static double MinSalary = 3000;
        
        private Dictionary<DateTime, double> _bonuses;
        private double _salary;
        
        public Dictionary<DateTime, double> Bonuses
        {
            get => _bonuses;
            set => _bonuses = value;
        }

        public double Salary
        {
            get => _salary;
            set
            {
                if(value < MinSalary)
                    throw new ArgumentException("Salary cannot be less than MinSalary");
                _salary = value;
            }
        }
        
        [XmlIgnore]
        private EmployeeRole _employee;
        [XmlIgnore]
        public EmployeeRole Employee => _employee;

        private void SetEmployeeRole(EmployeeRole employee)
        {
            if(employee == null)
                throw new ArgumentNullException(nameof(employee));
            
            if(employee == _employee)
                throw new DuplicateException(employee, this);

            employee.SetFullTime(this);
            
            _employee = employee;
        }
        
        //  Class extent 
        private static List<FullTimeContract> _contracts = new List<FullTimeContract>();
        public static IReadOnlyList<FullTimeContract> Contracts => _contracts.AsReadOnly();

        private static void AddContract(FullTimeContract contract)
        {
            if (contract == null)
                throw new ArgumentException("contract cannot be null");

            _contracts.Add(contract);
        }

        internal void RemoveFromExtent()
        {
            _contracts.Remove(this);
        }
        
        public FullTimeContract(EmployeeRole employee, double salary)
        {
            Salary = salary;
            Bonuses = new Dictionary<DateTime, double>();
            
            SetEmployeeRole(employee);
            AddContract(this);
        }

        public void AddBonus(double bonus)
        {
            if(bonus <= 0)
                throw new ArgumentException("Bonus cannot be less than 0");
            
            _bonuses.Add(DateTime.Now, bonus);
        }

        public override string ToString()
        {
            var str = "";
            foreach (var bonus in _bonuses)
            {
                str += $"Got {bonus.Value} bonus at {bonus.Key}\n";
            }
            return str;
        }
    }
}