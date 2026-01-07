using System.Xml.Serialization;
using CinemaManagementSystem.Exceptions;
using CinemaManagementSystem.Person;

namespace CinemaManagementSystem.Person.ContractType
{
    [Serializable]
    public class PartTimeContract 
    {
        private static double MinHourlyRate = 27;
        
        private double _hourlyRate;
        
        public double HourlyRate
        {
            get => _hourlyRate;
            set
            {
                if(value < MinHourlyRate)
                    throw new ArgumentException("HourlyRate cannot be less than MinHourlyRate");
                _hourlyRate = value;
            }
        }
        
        
        //Derived
        [XmlIgnore]
        public double HoursPerMonth => HourlyRate * 1;
        
        [XmlIgnore]
        public double SalaryPerMonth => HourlyRate * HoursPerMonth;
        
        
        [XmlIgnore]
        private Employee _employee;
        [XmlIgnore]
        public Employee Employee => _employee;
        
        private void SetEmployee(Employee employee)
        {
            if(employee == null)
                throw new ArgumentNullException(nameof(employee));
            
            if(employee == _employee)
                throw new DuplicateException(employee, this);

            employee.SetPartTime(this);
            
            _employee = employee;
        }

        //  Class extent 
        private static List<PartTimeContract> _contracts = new List<PartTimeContract>();
        public static IReadOnlyList<PartTimeContract> Contracts => _contracts.AsReadOnly();

        private static void AddContract(PartTimeContract contract)
        {
            if (contract == null)
                throw new ArgumentException("contract cannot be null");

            _contracts.Add(contract);
        }

        internal void RemoveFromExtent()
        {
            _contracts.Remove(this);
        }
        
        public PartTimeContract() { }

        public PartTimeContract(Employee employee, double hourlyRate)
        {
            HourlyRate = hourlyRate;
            
            SetEmployee(employee);
            AddContract(this);
        }

        public override string ToString()
        {
            return base.ToString() + 
                   $", Part-Time (Hourly rate: {HourlyRate}, Salary per month: {SalaryPerMonth}, Hours per month: {HoursPerMonth})";
        }

    }
}