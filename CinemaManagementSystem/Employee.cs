using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace CinemaManagementSystem
{
    [Serializable]
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
        public static void Save(string filePath)
        {
            StreamWriter sw = File.CreateText(filePath);
            XmlSerializer serializer = new XmlSerializer(typeof(List<Employee>));
            using (XmlTextWriter writer = new XmlTextWriter(sw))
            {
                serializer.Serialize(writer, _employees);
            }
        }

        public static void Load(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Employee file not found.");

            XmlSerializer serializer = new XmlSerializer(typeof(List<Employee>));
            using (StreamReader reader = new StreamReader(filePath))
            {
                var loaded = (List<Employee>)serializer.Deserialize(reader);
                _employees = loaded ?? new List<Employee>();
            }
        }
    }
}