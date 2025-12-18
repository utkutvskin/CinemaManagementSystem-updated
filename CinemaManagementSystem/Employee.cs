using System;
using System.Collections. Generic;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using CinemaManagementSystem.ContractTypeForEmployee;
using CinemaManagementSystem.Employees;
using CinemaManagementSystem.Enums;
using CinemaManagementSystem.PersistenceForAllClasses;

namespace CinemaManagementSystem
{
    [Serializable]
    public class Employee : IExtent<Employee>
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

        [XmlIgnore]
        private FullTimeContract? _fullTime;
        [XmlIgnore]
        public FullTimeContract? FullTime => _fullTime;

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

        public void ChangeToFullTime()
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

            _fullTime = new FullTimeContract(this);
        }
        
        [XmlIgnore]
        private PartTimeContract? _partTime;
        [XmlIgnore]
        public PartTimeContract? PartTime => _partTime;

        internal void SetPartTime(PartTimeContract partTime)
        {
            if (partTime == null)
                throw new ArgumentException("Part time cannot be null");
            
            if(isFullTime || isPartTime || isIntern)
                throw new InvalidOperationException("There is another contract type");

            _partTime = partTime;
        }
        
        public void ChangeToPartTime(int hoursPerWeek)
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

            _partTime = new PartTimeContract(hoursPerWeek, this);
        }
        
        [XmlIgnore]
        private InternContract? _intern;
        [XmlIgnore]
        public InternContract? Intern => _intern;

        internal void SetIntern(InternContract intern)
        {
            if (intern == null)
                throw new ArgumentException("Intern cannot be null");
            if(isFullTime || isPartTime || isIntern)
                throw new InvalidOperationException("There is another contract type");

            _intern = intern;
        }
        public void ChangeToIntern(string universityName, int duration)
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

            _intern = new InternContract(universityName, duration, this);
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
                if(value > DateTime.Now. AddYears(-16)) 
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
                if (DateTime.Now. DayOfYear < BirthDate. DayOfYear)
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
            string end = EndDate. HasValue ? EndDate.Value. ToShortDateString() : "Present";
            return $"{Name} {Surname}, Age:  {Age}, Salary: {Salary}€, Started: {StartDate: dd/MM/yyyy}, End: {end}, Years of Service: {YearsOfService}";
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
