using System.Xml.Serialization;
using CinemaManagementSystem.Exceptions;
using CinemaManagementSystem.People.Roles;

namespace CinemaManagementSystem.People.Contract
{
    [Serializable]
    public class InternContract 
    {
        
        private string _universityName;
        private double? _dailySalary;

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
        public double? DailySalary
        {
            get => _dailySalary;
            set
            {
                if(value <= 0)
                    throw new ArgumentException("HourlyRate cannot be less than 0");
                _dailySalary = value;
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
        
        
        public InternContract(EmployeeRole employee, string universityName, double? dailySalary = null)
        {
            UniversityName = universityName;
            DailySalary = dailySalary;
            
            SetEmployeeRole(employee);
            AddContract(this);
        }

        public override string ToString()
        {
            return base.ToString() + 
                   $", Intern (University {UniversityName} )";
        }
    }
}