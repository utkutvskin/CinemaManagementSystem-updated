using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

namespace CinemaManagementSystem
{
    [Serializable]
    public class Customer
    {
        // ---------- Attributes ----------
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Gender { get; set; }  
        public DateTime BirthDate { get; set; }  

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

        // ---------- Class extent ----------
        private static List<Customer> _customers = new List<Customer>();
        public static IReadOnlyList<Customer> Customers => _customers.AsReadOnly();

        
        public static void ClearAllCustomers()
        {
            _customers.Clear();
        }

        // ---------- Constructors ----------
        public Customer() { }  // XML serialization için

       
        public Customer(string name, string surname, string gender, DateTime birthDate)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty.");
            if (string.IsNullOrWhiteSpace(surname))
                throw new ArgumentException("Surname cannot be empty.");
            if (string.IsNullOrWhiteSpace(gender))
                throw new ArgumentException("Gender cannot be empty.");
            if (birthDate > DateTime.Now)
                throw new ArgumentException("Birth date cannot be in the future.");

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
        }
    }
}