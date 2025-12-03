using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using CinemaManagementSystem.ContractTypeForEmployee;
using CinemaManagementSystem.Employees;

namespace CinemaManagementSystem
{
    [Serializable]
    [XmlInclude(typeof(Cleaner))]
    [XmlInclude(typeof(Manager))]
    [XmlInclude(typeof(BuffetSeller))]
    [XmlInclude(typeof(Receptionist))]
    public class Employee
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

        // ---------- Reflexive Association: Manager - Employee ----------
        [XmlIgnore]
        private Employee _manager;
        
        [XmlIgnore]
        private readonly HashSet<Employee> _directReports = new();
        
        [XmlIgnore]
        public Employee Manager => _manager;
        
        [XmlIgnore]
        public IReadOnlyCollection<Employee> DirectReports => _directReports;
        
        // Assign manager to employee
        public void AssignManager(Employee manager)
        {
            if (manager == null)
                throw new ArgumentNullException(nameof(manager), "Manager cannot be null.");
        
            if (manager == this)
                throw new InvalidOperationException("An employee cannot manage themselves!");
        
            if (_manager == manager)
                return;
        
            _manager?.RemoveEmployeeInternal(this); // remove from old manager if exists
        
            _manager = manager;
        
            manager.AddEmployeeInternal(this); // reverse
        }
        
        public void RemoveManager()
        {
            if (_manager == null)
                throw new InvalidOperationException("This employee does not have a manager!");
        
            var oldManager = _manager;
            _manager = null;
        
            oldManager.RemoveEmployeeInternal(this);
        }
        
        internal void AddEmployeeInternal(Employee employee)
        {
            if (!_directReports.Contains(employee))
                _directReports.Add(employee);
        }
        
        internal void RemoveEmployeeInternal(Employee employee)
        {
            if (_directReports.Contains(employee))
                _directReports.Remove(employee);
        }


        //  Constructors 
        public Employee() { } 

        public Employee(string name, string surname, DateTime birthDate, DateTime startDate, double salary, DateTime? endDate = null)
        {

            Name = name;
            Surname = surname;
            BirthDate = birthDate;
            StartDate = startDate;
            EndDate = endDate;
            Salary = salary;

            AddEmployee(this);
        }

        //  Methods 
        public override string ToString()
        {
            string end = EndDate.HasValue ? EndDate.Value.ToShortDateString() : "Present";
            return $"{Name} {Surname}, Age: {Age}, Salary: {Salary}€, Started: {StartDate:dd/MM/yyyy}, End: {end}, Years of Service: {YearsOfService}";
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
                    typeof(Receptionist)
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
    }

}
