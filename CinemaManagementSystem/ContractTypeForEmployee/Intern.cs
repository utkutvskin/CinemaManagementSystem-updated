using System.Xml.Serialization;
using CinemaManagementSystem.Exceptions;

namespace CinemaManagementSystem.ContractTypeForEmployee
{
    [Serializable]
    public class InternContract 
    {
        private string _universityName;
        private int _duration;

        public string UniversityName
        {
            get => _universityName;
            set
            {
                if(string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("University name cannot be empty");
                _universityName = value;
            }
        }

        public int Duration
        {
            get => _duration;
            set
            {
                if(value < 0)
                    throw new ArgumentException("Duration cannot be negative");
                _duration = value;
            }
        }

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

            employee.SetIntern(this);
            
            _employee = employee;
        }

        //  Class extent 
        private static List<InternContract> _contracts = new List<InternContract>();
        public static IReadOnlyList<InternContract> Contracts => _contracts.AsReadOnly();

        private static void AddContract(InternContract contract)
        {
            if (contract == null)
                throw new ArgumentException("contract cannot be null");

            _contracts.Add(contract);
        }

        internal void RemoveFromExtent()
        {
            _contracts.Remove(this);
        }
        
        
        public InternContract(string universityName, int duration, Employee employee)
        {
            _universityName = universityName;
            _duration = duration;
            
            SetEmployee(employee);
            
            AddContract(this);
        }

        public override string ToString()
        {
            return base.ToString() + 
                   $", Intern (University {UniversityName}, Duration: {Duration})";
        }
    }
}