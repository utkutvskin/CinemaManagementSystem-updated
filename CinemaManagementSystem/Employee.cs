using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

namespace CinemaManagementSystem
{
    [Serializable]
    public class Employee
    {
        // ---------- Backing fields (preserve original attribute names) ----------
        private string _name;
        private string _surname;
        private DateTime _birthDate;
        private DateTime _startDate;
        private DateTime? _endDate;
        private double _salary;

        // ---------- Properties with validation (same public names as original) ----------
        public string Name
        {
            get => _name;
            set
            {
                var trimmed = value?.Trim();
                if (string.IsNullOrWhiteSpace(trimmed))
                    throw new ArgumentException("Name cannot be empty.");
                _name = trimmed;
            }
        }

        public string Surname
        {
            get => _surname;
            set
            {
                var trimmed = value?.Trim();
                if (string.IsNullOrWhiteSpace(trimmed))
                    throw new ArgumentException("Surname cannot be empty.");
                _surname = trimmed;
            }
        }

        public DateTime BirthDate
        {
            get => _birthDate;
            set
            {
                if (value > DateTime.Now)
                    throw new ArgumentException("Birth date cannot be in the future.");
                _birthDate = value;
            }
        }

        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                if (value > DateTime.Now)
                    throw new ArgumentException("Start date cannot be in the future.");
                _startDate = value;
            }
        }

        public DateTime? EndDate
        {
            get => _endDate;
            set
            {
                if (value.HasValue && value.Value < StartDate)
                    throw new ArgumentException("End date cannot be before start date.");
                _endDate = value;
            }
        }

        public double Salary
        {
            get => _salary;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Salary must be positive.");
                _salary = value;
            }
        }

        // ---------- Computed properties (get-only) ----------
        // Read-only properties are computed on the fly from stored fields.
        public int Age
        {
            get
            {
                var today = DateTime.Today;
                int age = today.Year - BirthDate.Year;
                if (BirthDate > today.AddYears(-age))
                    age--;
                return age;
            }
        }

        public int YearsOfService
        {
            get
            {
                var end = EndDate ?? DateTime.Today;
                var today = end.Date;
                int years = today.Year - StartDate.Year;
                if (today < StartDate.AddYears(years))
                    years--;
                return years;
            }
        }

        // ---------- Class extent ----------
        private static List<Employee> _employees = new List<Employee>();
        public static IReadOnlyList<Employee> Employees => _employees.AsReadOnly();

        public static void ClearAllEmployees() => _employees.Clear();

        // ---------- Constructors (preserve original signature) ----------
        public Employee() { } // XmlSerializer için gerekli

        public Employee(string name, string surname, DateTime birthDate, DateTime startDate, double salary, DateTime? endDate = null)
        {
            // Use property setters so validation/normalization is applied
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
            var serializer = new XmlSerializer(typeof(List<Employee>));
            using (var writer = new StreamWriter(filePath))
                serializer.Serialize(writer, _employees);
        }

        public static void Load(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Employee file not found.");

            var serializer = new XmlSerializer(typeof(List<Employee>));
            using (var reader = new StreamReader(filePath))
            {
                var loaded = (List<Employee>)serializer.Deserialize(reader);
                _employees = loaded ?? new List<Employee>();
            }

            // Computed properties (Age, YearsOfService) use stored fields, no post-load recalculation required.
        }
    }
}
