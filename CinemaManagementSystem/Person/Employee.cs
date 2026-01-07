using System.Xml.Serialization;
using CinemaManagementSystem.Employees;
using CinemaManagementSystem.Enums;
using CinemaManagementSystem.Exceptions;
using CinemaManagementSystem.PersistenceForAllClasses;
using CinemaManagementSystem.Person.ContractType;
using CinemaManagementSystem.Person.Roles;

namespace CinemaManagementSystem.Person
{
    [Serializable]
    public class Employee : Person, IExtent<Employee>
    {
        //Attributes 
        private DateTime _startDate;
        private DateTime? _endDate;
        private Role _role;
        
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
        public Role Role {
            get => _role;
            set => _role = value;
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
        
        //Reflex Association
        [XmlIgnore]
        private HashSet<Employee>? _previousRoles = new HashSet<Employee>();
        [XmlIgnore]
        public IReadOnlySet<Employee>? PreviousRoles => _previousRoles;
        
        [XmlIgnore]
        private Employee? _futureRole;
        [XmlIgnore]
        public Employee? FutureRole => _futureRole;

        public void ChangeRoleToReceptionist(int deskNumber)
        {
            if(Role == Role.Receptionist)
                throw new RoleException(Role.Receptionist);
            
            Receptionist rec = new Receptionist(Name, Surname, BirthDate, Gender, deskNumber);
            EndDate = DateTime.Now;

            ChangeRole(rec);
        }
        public void ChangeRoleToManager()
        {
            if(Role == Role.Manager)
                throw new RoleException(Role.Manager);
            
            Manager man = new Manager(Name, Surname, BirthDate, Gender);
            EndDate = DateTime.Now;

            ChangeRole(man);
        }
        public void ChangeRoleToBuffetSeller()
        {
            if(Role == Role.BuffetSeller)
                throw new RoleException(Role.BuffetSeller);
            
            BuffetSeller bs = new BuffetSeller(Name, Surname, BirthDate, Gender);
            EndDate = DateTime.Now;

            ChangeRole(bs);
        }
        public void ChangeRoleToCleaner(CleaningTypeEnum type)
        {
            if(Role == Role.Cleaner)
                throw new RoleException(Role.Cleaner);
            
            Cleaner cl = new Cleaner(Name, Surname, BirthDate, Gender, type);
            EndDate = DateTime.Now;

            ChangeRole(cl);
        }
        public void ChangeRoleToDisplayer( )
        {
            if(Role == Role.Displayer)
                throw new RoleException(Role.Displayer);
            
            Displayer ds = new Displayer(Name, Surname, BirthDate, Gender);
            EndDate = DateTime.Now;

            ChangeRole(ds);
        }

        private void ChangeRole(Employee empl)
        {
            empl._previousRoles = new HashSet<Employee>();

            if (_previousRoles != null)
            {
                foreach (var employee in _previousRoles)
                {
                    empl._previousRoles.Add(employee);
                    employee._futureRole = empl;
                }
                _previousRoles.Clear();
            }
            
            _futureRole = empl;
            empl._previousRoles.Add(this);
        }
        
        // Class extent 
        private static List<Employee> _employees = new List<Employee>();
        public static IReadOnlyList<Employee> Employees => _employees.AsReadOnly();

        private static void AddEmployee(Employee employee)
        {
            if (employee == null)
                throw new ArgumentException("employee cannot be null");

            _employees.Add(employee);
        }
        
        //  Constructors 
        public Employee() { } 

        public Employee(string name, string surname, DateTime birthDate, GenderEnum gender, Role role)
            :base(name, surname, gender, birthDate)
        {
            StartDate = DateTime.Now;
            Role = role;
            
            AddEmployee(this);
        }
        
        //  Methods 
        public override string ToString()
        {
            string end = EndDate. HasValue ? EndDate.Value.ToShortDateString() : "Present";
            return $"{Name} {Surname}, Age:  {Age}, Started: {StartDate: dd/MM/yyyy}, End: {end}";
        }

        //  Persistence 
        public static void Save(string filePath)
        {
            var serializer = new XmlSerializer(typeof(List<Employee>));
            using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            serializer.Serialize(fs, _employees);
        }

        public static void Load(string filePath)
        {
            if (! File.Exists(filePath))
                throw new FileNotFoundException("Employee file not found.");

            XmlSerializer serializer = new XmlSerializer(typeof(List<Employee>));
            using (StreamReader reader = new StreamReader(filePath))
            {
                var loaded = (List<Employee>)serializer.Deserialize(reader);
                _employees = loaded ??  new List<Employee>();
            }
        }


        public List<Employee> GetExtent() => _employees;

        public void ReplaceExtent(List<Employee> newExtent)
        {
            _employees = newExtent ??  new List<Employee>();
        }
    }

}
