using System.Xml.Serialization;
using CinemaManagementSystem.Exceptions;
using CinemaManagementSystem.People.Roles;

namespace CinemaManagementSystem.People.Contract
{
    [Serializable]
    public class PartTimeContract 
    {
        private static double MinHourlyRate = 27;
        private static double MaxHourlyRate = 60;
        
        private double _hourlyRate;
        private double _hoursPerMonth;
        
        public double HourlyRate
        {
            get => _hourlyRate;
            set
            {
                if(value < MinHourlyRate)
                    throw new ArgumentException("HourlyRate cannot be less than MinHourlyRate");
                if(value > MaxHourlyRate)
                    throw new ArgumentException("HourlyRate cannot be bigger that  MaxHourlyRate");
                _hourlyRate = value;
            }
        }
        
        public double HoursPerMonth
        {
            get => _hoursPerMonth;
            set
            {
                if(value < 0)
                    throw new ArgumentException("HoursPerMounth cannot be less than 0");
                _hoursPerMonth = value;
            }
        }
        
        [XmlIgnore]
        public double SalaryPerMonth => HourlyRate * HoursPerMonth;
        
        
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

        public PartTimeContract(EmployeeRole employee, double hourlyRate, double hoursPerMonth)
        {
            HourlyRate = hourlyRate;
            HoursPerMonth = hoursPerMonth;
            
            SetEmployeeRole(employee);
            AddContract(this);
        }

        public override string ToString()
        {
            return base.ToString() + 
                   $", Part-Time (Hourly rate: {HourlyRate}, Salary per month: {SalaryPerMonth}, Hours per month: {HoursPerMonth})";
        }

    }
}