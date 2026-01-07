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
        private bool _isFired;
        private List<EmployeeRole>? _prevoiusEmployeeRoles;
        
        public bool IsFired
        {
            get => _isFired;
            set => _isFired = value;
        }
        public List<EmployeeRole>? PrevoiusEmployeeRoles
        {
            get => _prevoiusEmployeeRoles;
            set => _prevoiusEmployeeRoles = value;
        }
        
        [XmlIgnore]
        private EmployeeRole? _currentRole;
        [XmlIgnore]
        public EmployeeRole? CurrentRole => _currentRole;

        internal void SetRole(EmployeeRole role)
        {
            if(role == null)
                throw new ArgumentException("role cannot be null");
            if(_currentRole != null)
                throw new InvalidOperationException("Current role already set");
            if (role.EndDate != null)
                throw new InvalidOperationException("Cannot set an already-ended role as current");

            _currentRole = role;
        }

        private void ChangeRole()
        {
            if (_currentRole == null)
                throw new InvalidOperationException("Employee has no current role.");

            _currentRole.EndDate = DateTime.Now;

            _prevoiusEmployeeRoles ??= new List<EmployeeRole>();
            _prevoiusEmployeeRoles.Add(_currentRole);

            _currentRole = null;
        }
        
        public EmployeeRole ChangeRoleToReceptionist(int deskNumber)
        {
            ChangeRole();
            Receptionist rec = new Receptionist(deskNumber, this);
            return rec;
        }
        public EmployeeRole ChangeRoleToManager()
        {
            ChangeRole();
            
            Manager rec = new Manager( this);
            return rec;
        }
        public EmployeeRole ChangeRoleToCleaner(CleaningTypeEnum cleaningType)
        {
            ChangeRole();
            
            Cleaner rec = new Cleaner(cleaningType, this);
            return rec;
        }
        public EmployeeRole ChangeRoleToDisplayer()
        {
            ChangeRole();
            
            Displayer rec = new Displayer( this);
            return rec;
        }
        public EmployeeRole ChangeRoleToBuffetSeller()
        {
            ChangeRole();
            
            BuffetSeller rec = new BuffetSeller( this);
            return rec;
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
            IsFired = false;
            
            AddEmployee(this);
        }
        
        //  Methods 
        public override string ToString()
        {
            return $"{Name} {Surname}, Age:  {Age}";
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
