using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Linq;
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

        public GenderEnum Gender
        {
            get => _gender;
            set
            {
                if (value == null)
                    throw new ArgumentException("Gender can't be null");
                
                _gender =value;
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
        
        
        //Basic Association 
        [XmlIgnore]
        private readonly HashSet<Movie> _movies = new HashSet<Movie>();

        [XmlIgnore]
        public IReadOnlyCollection<Movie> Movies => _movies;
        
        public void AddMovie(Movie movie)
        {
            if (movie == null)
                throw new ArgumentException("Movie cannot be null.");

            if (_movies.Contains(movie))
                throw new InvalidOperationException("Movie is already in actor's filmography.");

            _movies.Add(movie);

            
            if (!movie.Actors.Contains(this))
            {
                movie.AddActorInternal(this);
            }
        }

        public void RemoveMovie(Movie movie)
        {
            if (movie == null)
                throw new ArgumentException("Movie cannot be null.");

            if (!_movies.Contains(movie))
                throw new InvalidOperationException("Movie is not in actor's filmography.");

            _movies.Remove(movie);

            
            if (movie.Actors.Contains(this))
            {
                movie.RemoveActorInternal(this);
            }
        }
        
        internal void AddMovieInternal(Movie movie)
        {
            _movies.Add(movie);
        }

        internal void RemoveMovieInternal(Movie movie)
        {
            _movies.Remove(movie);
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

        

        //Constructors
        public Actor() { }

        public Actor(string name, string surname, GenderEnum gender, DateTime birthDate)
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

        public void Delete()
        {
            foreach (var movie in new List<Movie>(_movies))
            {
                movie.RemoveActorInternal(this);
            }
            _movies.Clear();

            _actors.Remove(this);
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
            if (!File.Exists(filePath))
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
                catch 
                {
                    _actors.Clear();
                    return false;
                }
            }

            return true;
        }
        
        //for testing
        public static void ClearAllActors()
        {
            foreach (var actor in new List<Actor>(_actors))
            {
                actor.Delete();
            }
        }
    }
}