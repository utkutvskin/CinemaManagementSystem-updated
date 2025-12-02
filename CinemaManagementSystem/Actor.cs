using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using CinemaManagementSystem.Enums;

namespace CinemaManagementSystem
{
    [Serializable]
    public class Actor
    {
        //Attributes
        private string _name;
        private string _surname;
        private GenderEnum _gender;
        private DateTime _birthDate;
        
        
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

        public string Gender
        {
            get => _gender.ToString();
            set
            {
                if (string.IsNullOrWhiteSpace(value) || !Enum.IsDefined(typeof(GenderEnum), value))
                    throw new ArgumentException("Gender not correct");
                
                _gender = Enum.Parse<GenderEnum>(value);
            }
        }

        public DateTime BirthDate
        {
            get => _birthDate;
            set
            {
                if(value > DateTime.Now)
                    throw new ArgumentException("Birth day cannot be greater than today.");
                _birthDate = value;
            }
        }

        //Derived
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

        //Class extent
        private static List<Actor> _actors = new List<Actor>();
        public static IReadOnlyList<Actor> Actors => _actors.AsReadOnly();
        
        private static void AddActor(Actor actor)
        {
            if (actor == null)
                throw new ArgumentException("Actor cannot be null");

            _actors.Add(actor);
        }

        //for testing
        public static void ClearAllActors()
        {
            _actors.Clear();
        }

        //Constructors
        public Actor() { }

        public Actor(string name, string surname, string gender, DateTime birthDate)
        {

            Name = name;
            Surname = surname;
            Gender = gender;
            BirthDate = birthDate;

            AddActor(this);
        }

        //Methods 
        public override string ToString()
        {
            return $"{Name} {Surname}, {Gender}, Age: {Age}";
        }

        //Persistence 
        public static void Save(string filePath)
        {
            StreamWriter sw = File.CreateText(filePath);
            XmlSerializer serializer = new XmlSerializer(typeof(List<Actor>));
            using (XmlTextWriter writer = new XmlTextWriter(sw))
            {
                serializer.Serialize(writer, _actors);
            }
        }

        public static bool Load(string filePath)
        {
            StreamReader file;
            try
            {
                file = File.OpenText(filePath);
            }
            catch (FileNotFoundException)
            {
                _actors.Clear();
                return false;
            }

            XmlSerializer serializer = new XmlSerializer(typeof(List<Actor>));
            using (XmlTextReader reader = new XmlTextReader(filePath))
            {
                try
                {
                    _actors = (List<Actor>)serializer.Deserialize(reader);
                }
                catch (InvalidCastException)
                {
                    _actors.Clear();
                    return false;
                }
                catch (Exception)
                {
                    _actors.Clear();
                    return false;
                }
            }

            return true;
        }
    }
}