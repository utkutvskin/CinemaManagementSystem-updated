using System.Xml.Serialization;
using CinemaManagementSystem.Exceptions;

namespace CinemaManagementSystem.ContractTypeForEmployee
{
    [Serializable]
    public class PartTimeContract 
    {
        private int _hoursPerWeek;

        public int HoursPerWeek
        {
            get => _hoursPerWeek;
            set
            {
                if (value <= 0 || value > MaxHours * 5)
                    throw new ArgumentException("Part-time employee must work between 1 and 30 hours.");
                _hoursPerWeek = value;
            }
        }
        
        private static int MaxHours = 6;
        
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

        public PartTimeContract(int hoursPerWeek, Employee employee)
        {
            HoursPerWeek = hoursPerWeek;
            
            SetEmployee(employee);
            
            AddContract(this);
        }

        public override string ToString()
        {
            return base.ToString() + 
                   $", Part-Time ({HoursPerWeek}h/week)";
        }

    }
}