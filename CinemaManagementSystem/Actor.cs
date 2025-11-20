using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

namespace CinemaManagementSystem
{
    [Serializable]
    public class Actor
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
        private static List<Actor> _actors = new List<Actor>();
        public static IReadOnlyList<Actor> Actors => _actors.AsReadOnly();

        
        public static void ClearAllActors()
        {
            _actors.Clear();
        }

        // ---------- Constructors ----------
        public Actor() { }

        public Actor(string name, string surname, string gender, DateTime birthDate)
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
        }
    }
}