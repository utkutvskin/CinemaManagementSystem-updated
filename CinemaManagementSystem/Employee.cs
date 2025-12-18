using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using CinemaManagementSystem.ContractTypeForEmployee;
using CinemaManagementSystem.Employees;
using CinemaManagementSystem.PersistenceForAllClasses;

namespace CinemaManagementSystem
{
    [Serializable]
    [XmlInclude(typeof(Cleaner))]
    [XmlInclude(typeof(Manager))]
    [XmlInclude(typeof(BuffetSeller))]
    [XmlInclude(typeof(Displayer))]
    [XmlInclude(typeof(Receptionist))]
    public class Employee :IExtent<Employee>
    {
        [XmlIgnore]
        private static double _minSalary = 3000;
        
        //Attributes 
        private string _name;
        private string _surname;
        private DateTime _birthDate;
        private DateTime _startDate;
        private DateTime? _endDate;
        private double _salary;

        private ContractType _contractType;
        public ContractType ContractType
        {
            get => _contractType;
            set
            {
                if(value == null)
                    throw new ArgumentException("Contract type cannot be null");
                _contractType = value;
            }
        }


// DYNAMIC + COMPLETE:  Her employee'nin bir rolü olmalı ve değiştirilebilir
        private object _currentRole;
        
        [XmlElement("Cleaner", typeof(Cleaner))]
        [XmlElement("Manager", typeof(Manager))]
        [XmlElement("Displayer", typeof(Displayer))]
        [XmlElement("BuffetSeller", typeof(BuffetSeller))]
        [XmlElement("Receptionist", typeof(Receptionist))]
        public object CurrentRole
        {
            get => _currentRole;
            set
            {
                // COMPLETE constraint: role cannot be null
                if (value == null)
                    throw new ArgumentException("Employee must have a role.  Role cannot be null.");
                
                // Validate role type
              if (!(value is Cleaner || value is Manager || value is BuffetSeller || 
              value is Receptionist || value is Displayer))
            throw new ArgumentException("Invalid role type.");
        
        _currentRole = value;
    }
}

        /// <summary>
        /// Changes employee's role (DYNAMIC)
        /// </summary>
        public void ChangeRole(object newRole)
        {
            if (newRole == null)
                throw new ArgumentException("New role cannot be null.  Employee must always have a role.");
            
            // Validate role type
            if (!(newRole is Cleaner || newRole is Manager || newRole is BuffetSeller || 
                  newRole is Receptionist || newRole is Displayer))
                throw new ArgumentException("Invalid role type.");
            
            _currentRole = newRole;
        }

        /// <summary>
        /// Checks if employee currently has a specific role type
        /// </summary>
        public bool IsInRole<T>() where T : class
        {
            return _currentRole is T;
        }

        /// <summary>
        /// Gets current role as specific type
        /// </summary>
        public T GetCurrentRole<T>() where T : class
        {
            return _currentRole as T;
        }

        /// <summary>
        /// Gets current role name
        /// </summary>
        public string GetCurrentRoleName()
        {
            return _currentRole?. GetType().Name ?? "No Role";
        }




        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Name cannot be empty.");
                _name = value;
            }
        }
        
        public string Surname
        {
            get => _surname;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Surname cannot be empty.");
                _surname = value;
            }
        }

        public DateTime BirthDate
        {
            get => _birthDate;
            set
            {
                if(value > DateTime.Now.AddYears(-16)) 
                    throw new ArgumentException("You must be older than 16 years old"); 
                _birthDate = value;
                
            }
        }

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

        public double Salary
        {
            get => _salary;
            set
            {
                if(value < _minSalary)
                    throw new ArgumentException("Salary cannot be less than minimum salary.");
                _salary = value;
            }
        }

        //  Derived Attributes 
        [XmlIgnore]
        public int Age
        {
            get
            {
                int age = DateTime.Now.Year - BirthDate.Year;
                if (DateTime.Now.DayOfYear < BirthDate.DayOfYear)
                    age--;
                return age;
            }
        }

        [XmlIgnore]
        public int YearsOfService
        {
            get
            {
                var end = EndDate ?? DateTime.Now;
                int years = end.Year - StartDate.Year;
                if (end.DayOfYear < StartDate.DayOfYear)
                    years--;
                return years;
            }
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
        
        
        public static void ClearAllEmployees()
        {
            _employees.Clear();
        }

      

        //  Constructors 
   
      public Employee() { }

     public Employee(string name, string surname, DateTime birthDate, DateTime startDate, double salary, object initialRole, DateTime? endDate = null)
        {
            Name = name;
            Surname = surname;
            BirthDate = birthDate;
            StartDate = startDate;
            EndDate = endDate;
            Salary = salary;

            // COMPLETE constraint: must have initial role
            CurrentRole = initialRole; // This will validate through property setter

            AddEmployee(this);
        }

        //  Methods 
        public override string ToString()
        {
            string end = EndDate.HasValue ? EndDate.Value.ToShortDateString() : "Present";
            string role = GetCurrentRoleName(); 
            return $"{Name} {Surname}, Age: {Age}, Salary: {Salary}€, Started: {StartDate:dd/MM/yyyy}, End: {end}, Years of Service:  {YearsOfService}, Current Role: {role}";
        }

        //  Persistence 
        private static XmlSerializer GetSerializer()
        {
            return new XmlSerializer(
                typeof(List<Employee>),
                new Type[]
                {
                    typeof(Cleaner),
                    typeof(Manager),
                    typeof(BuffetSeller),
                    typeof(Receptionist),
                    typeof(Displayer) ,
                });
        }
        public static void Save(string filePath)
        {
            var serializer = GetSerializer();
            using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            serializer.Serialize(fs, _employees);
        }

        public static void Load(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Employee file not found.");

            XmlSerializer serializer = GetSerializer();
            using (StreamReader reader = new StreamReader(filePath))
            {
                var loaded = (List<Employee>)serializer.Deserialize(reader);
                _employees = loaded ?? new List<Employee>();
            }
        }


        public List<Employee> GetExtent() => _employees;

        public void ReplaceExtent(List<Employee> newExtent)
        {
            _employees = newExtent ?? new List<Employee>();
        }
    }

}


