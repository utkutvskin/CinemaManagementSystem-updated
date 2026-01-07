using System.Xml.Serialization;
using CinemaManagementSystem.Enums;
using CinemaManagementSystem.Exceptions;
using CinemaManagementSystem.People.ContractType;

namespace CinemaManagementSystem.People.Roles;

public class EmployeeRole
{
    //Attributes 
        private DateTime _startDate;
        private DateTime? _endDate;
        
        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                if(value > DateTime.Now )
                    throw new ArgumentException("Start date cannot be greater than today.");
                _startDate = value;
            }
        }
        public DateTime? EndDate
        {
            get => _endDate;
            set
            {
                if(value > DateTime.Now || value < StartDate)
                    throw new ArgumentException("End date cannot be greater than today or less than start date.");
                _endDate = value;
            }
        }
        
        [XmlIgnore]
        private FullTimeContract? _fullTime;
        [XmlIgnore]
        public FullTimeContract? FullTime => _fullTime;
        
        [XmlIgnore]
        private PartTimeContract? _partTime;
        [XmlIgnore]
        public PartTimeContract? PartTime => _partTime;
        
        [XmlIgnore]
        private InternContract? _intern;
        [XmlIgnore]
        public InternContract? Intern => _intern;

        private bool isFullTime => _fullTime != null;
        private bool isPartTime => _partTime != null;
        private bool isIntern => _intern != null;
        
        internal void SetFullTime(FullTimeContract fullTime)
        {
            if (fullTime == null)
                throw new ArgumentException("Full time cannot be null");
            
            if(isFullTime || isPartTime || isIntern)
                throw new InvalidOperationException("There is another contract type");
            
            _fullTime = fullTime;
        }
        public void ChangeToFullTime(double salary )
        {
            if(isFullTime)
                throw new InvalidOperationException("It has already full time");
            
            if(isPartTime)
            {
                _partTime.RemoveFromExtent();
                _partTime = null;
            }
            else if(isIntern)
            {
                _intern.RemoveFromExtent();
                _intern = null;
            }

            _fullTime = new FullTimeContract(this, salary);
        }

        public void SetFullTime(double salary)
        {
            if(isFullTime || isPartTime || isIntern)
                throw new InvalidOperationException("There is another contract type");
            _fullTime = new FullTimeContract(this, salary);
        }

        internal void SetPartTime(PartTimeContract partTime)
        {
            if (partTime == null)
                throw new ArgumentException("Part time cannot be null");
            
            if(isFullTime || isPartTime || isIntern)
                throw new InvalidOperationException("There is another contract type");

            _partTime = partTime;
        }
        public void ChangeToPartTime(double hourlyRate)
        {
            if(isPartTime)
                throw new InvalidOperationException("It has already part time");
            
            if(isFullTime)
            {
                _fullTime.RemoveFromExtent();
                _fullTime = null;
            }
            else if(isIntern)
            {
                _intern.RemoveFromExtent();
                _intern = null;
            }

            _partTime = new PartTimeContract(this, hourlyRate);
        }
        public void SetPartTime(double hourlyRate)
        {
            if(isFullTime || isPartTime || isIntern)
                throw new InvalidOperationException("There is another contract type");
            _partTime = new PartTimeContract(this, hourlyRate);
        }

        internal void SetIntern(InternContract intern)
        {
            if (intern == null)
                throw new ArgumentException("Intern cannot be null");
            if(isFullTime || isPartTime || isIntern)
                throw new InvalidOperationException("There is another contract type");

            _intern = intern;
        }
        public void ChangeToIntern(string universityName, double? dailySalary = null)
        {
            if(isIntern)
                throw new InvalidOperationException("It has already intern");
            
            if(isFullTime)
            {
                _fullTime.RemoveFromExtent();
                _fullTime = null;
            }
            else if(isPartTime)
            {
                _partTime.RemoveFromExtent();
                _partTime = null;
            }

            _intern = new InternContract(this, universityName, dailySalary);
        }
        public void SetIntern(string universityName, double? dailySalary = null)
        {
            if(isFullTime || isPartTime || isIntern)
                throw new InvalidOperationException("There is another contract type");
            _intern = new InternContract(this, universityName, dailySalary);
        }
        [XmlIgnore]
        private Employee _employee;
        [XmlIgnore]
        public Employee Employee => _employee;
        
        private void SetEmployee(Employee employee)
        {
            if(employee == null)
                throw new ArgumentException("Employee cannot be null");
            if(Employee != null)
                throw new InvalidOperationException("Employee is already assigned");
            if(employee.IsFired)
                throw new InvalidOperationException("Employee is already fired");

            employee.SetRole(this);
            _employee = employee;
        }
        
        // Class extent 
        private static List<EmployeeRole> _employeeRoles = new List<EmployeeRole>();
        public static IReadOnlyList<EmployeeRole> EmployeeRoles => _employeeRoles.AsReadOnly();

        private static void AddEmployeeRole(EmployeeRole employee)
        {
            if (employee == null)
                throw new ArgumentException("employee cannot be null");

            _employeeRoles.Add(employee);
        }
        
        //  Constructors 

        protected EmployeeRole(){}
        
        public EmployeeRole(Employee employee)
        {
            StartDate = DateTime.Now;
            SetEmployee(employee);
            AddEmployeeRole(this);
        }
        
        //  Methods 
        public override string ToString()
        {
            string end = EndDate. HasValue ? EndDate.Value.ToShortDateString() : "Present";
            return $" Started: {StartDate: dd/MM/yyyy}, End: {end}";
        }

        //  Persistence 
        public static void Save(string filePath)
        {
            var serializer = new XmlSerializer(typeof(List<EmployeeRole>));
            using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            serializer.Serialize(fs, _employeeRoles);
        }

        public static void Load(string filePath)
        {
            if (! File.Exists(filePath))
                throw new FileNotFoundException("Employee file not found.");

            XmlSerializer serializer = new XmlSerializer(typeof(List<EmployeeRole>));
            using (StreamReader reader = new StreamReader(filePath))
            {
                var loaded = (List<EmployeeRole>)serializer.Deserialize(reader);
                _employeeRoles = loaded ??  new List<EmployeeRole>();
            }
        }
}