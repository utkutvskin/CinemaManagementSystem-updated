using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

namespace CinemaManagementSystem
{
    [Serializable]
    public class Actor
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
        // Age is computed on-the-fly from BirthDate. It is read-only.
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
        private static List<Actor> _actors = new List<Actor>();
        public static IReadOnlyList<Actor> Actors => _actors.AsReadOnly();

        public static void ClearAllActors()
        {
            _actors.Clear();
        }

        // ---------- Constructors ----------
        public Actor() { } // parameterless ctor for serializer

        public Actor(string name, string surname, string gender, DateTime birthDate)
        {
            // Use property setters so validation is reused
            Name = name;
            Surname = surname;
            Gender = gender;
            BirthDate = birthDate;

            _actors.Add(this);
        }

        // ---------- Methods ----------
        public override string ToString()
        {
            return $"{Name} {Surname}, {Gender}, Age: {Age}";
        }

        // ---------- Persistence ----------
        public static void Save(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Actor>));
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, _actors);
            }
        }

        public static void Load(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Actor file not found.");

            XmlSerializer serializer = new XmlSerializer(typeof(List<Actor>));
            using (StreamReader reader = new StreamReader(filePath))
            {
                var loaded = (List<Actor>)serializer.Deserialize(reader);
                _actors = loaded ?? new List<Actor>();
            }

            // Age is computed from BirthDate (get-only), so no manual recalculation is necessary here.
        }
    }
}
