using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

namespace CinemaManagementSystem
{
    [Serializable]
    public class Employee
    {
        // ---------- Attributes ----------
        public string Name { get; set; }         
        public string Surname { get; set; }      
        public DateTime BirthDate { get; set; }  
        public DateTime StartDate { get; set; }  
        public DateTime? EndDate { get; set; }   
        public double Salary { get; set; }       

        // ---------- Derived Attributes ----------
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

        // ---------- Class extent ----------
        private static List<Employee> _employees = new List<Employee>();
        public static IReadOnlyList<Employee> Employees => _employees.AsReadOnly();

        
        public static void ClearAllEmployees()
        {
            _employees.Clear();
        }

        // ---------- Constructors ----------
        public Employee() { } // XmlSerializer için gerekli

        public Employee(string name, string surname, DateTime birthDate, DateTime startDate, double salary, DateTime? endDate = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty.");
            if (string.IsNullOrWhiteSpace(surname))
                throw new ArgumentException("Surname cannot be empty.");
            if (birthDate > DateTime.Now)
                throw new ArgumentException("Birth date cannot be in the future.");
            if (startDate > DateTime.Now)
                throw new ArgumentException("Start date cannot be in the future.");
            if (endDate.HasValue && endDate < startDate)
                throw new ArgumentException("End date cannot be before start date.");
            if (salary <= 0)
                throw new ArgumentException("Salary must be positive.");

            Name = name;
            Surname = surname;
            BirthDate = birthDate;
            StartDate = startDate;
            EndDate = endDate;
            Salary = salary;

            _employees.Add(this);
        }

        // ---------- Methods ----------
        public void AccessShiftsList()
        {
            Console.WriteLine($"{Name} {Surname} is accessing the shift list...");
        }

        public override string ToString()
        {
            string end = EndDate.HasValue ? EndDate.Value.ToShortDateString() : "Present";
            return $"{Name} {Surname}, Age: {Age}, Salary: {Salary}€, Started: {StartDate:dd/MM/yyyy}, End: {end}, Years of Service: {YearsOfService}";
        }

        // ---------- Persistence ----------
        public static void Save(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Employee>));
            using (StreamWriter writer = new StreamWriter(filePath))
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