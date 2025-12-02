using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

namespace CinemaManagementSystem
{
    [Serializable]
    public class Customer
    {
        // ---------- Backing fields ----------
        private string _name;
        private string _surname;
        private string _gender;
        private DateTime _birthDate;

        // ---------- Properties with validation ----------
        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Name cannot be empty.");
                _name = value.Trim();
            }
        }

        public string Surname
        {
            get => _surname;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Surname cannot be empty.");
                _surname = value.Trim();
            }
        }

        public string Gender
        {
            get => _gender;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Gender cannot be empty.");
                _gender = value.Trim();
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

        // ---------- Calculated Age (get-only) ----------
        // Age is computed on-the-fly from BirthDate and thus will not be serialized.
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

        // ---------- Class extent ----------
        private static List<Customer> _customers = new List<Customer>();
        public static IReadOnlyList<Customer> Customers => _customers.AsReadOnly();

        public static void ClearAllCustomers()
        {
            _customers.Clear();
        }

        // ---------- Constructors ----------
        public Customer() { } // parameterless ctor for serializer

        public Customer(string name, string surname, string gender, DateTime birthDate)
        {
            Name = name;
            Surname = surname;
            Gender = gender;
            BirthDate = birthDate;

            _customers.Add(this);
        }

        // ---------- Methods ----------
        public override string ToString()
        {
            return $"{Name} {Surname}, {Gender}, Age: {Age}";
        }

        // ---------- Persistence ----------
        public static void Save(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Customer>));
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, _customers);
            }
        }

        public static void Load(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Customer file not found.");

            XmlSerializer serializer = new XmlSerializer(typeof(List<Customer>));
            using (StreamReader reader = new StreamReader(filePath))
            {
                var loaded = (List<Customer>)serializer.Deserialize(reader);
                _customers = loaded ?? new List<Customer>();
            }

            // Age is computed from BirthDate (get-only), so no manual recalculation is needed here.
        }
    }
}
