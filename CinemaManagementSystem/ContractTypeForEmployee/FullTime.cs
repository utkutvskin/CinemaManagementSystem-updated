using System.Xml.Serialization;
using CinemaManagementSystem.Exceptions;

namespace CinemaManagementSystem.ContractTypeForEmployee
{
    [Serializable]
    public class FullTimeContract
    {
        private readonly Dictionary<DateTime, double> _bonuses = new();

        public IReadOnlyDictionary<DateTime, double> Bonuses => _bonuses;
        
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
        
        public FullTimeContract(Employee employee)
        {
            SetEmployee(employee);
            
            AddContract(this);
        }

        public void AddBonus(double bonus)
        {
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