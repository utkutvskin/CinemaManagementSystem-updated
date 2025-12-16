using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Linq;
using System.Xml;
using CinemaManagementSystem.Enums;
using CinemaManagementSystem.Exceptions;
using CinemaManagementSystem.PersistenceForAllClasses;

namespace CinemaManagementSystem
{
    [Serializable]
    public class Actor :IExtent<Actor>
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
        
        
        //Basic Association Movie
        [XmlIgnore]
        private readonly HashSet<Movie> _movies = new HashSet<Movie>();

        [XmlIgnore]
        public IReadOnlyCollection<Movie> Movies => _movies;

        //method for adding the movie to this actor(from actor side)
        public void AddMovie(Movie movie)
        {
            if (movie == null)
                throw new ArgumentException("Movie cannot be null.");

            //check if movie is not already added to this actor 
            if (_movies.Contains(movie))
                return; //if so, immediately exit the method to avoid duplicates and recursion 

            _movies.Add(movie);

            movie.AddActor(this); //add this actor to the movie
        }

        //method for removing movie from this actor (from actor side)
        public void RemoveMovie(Movie movie)
        {
            if (movie == null)
                throw new ArgumentException("Movie cannot be null.");
            
            //check if movie is added to this actor
            if (!_movies.Contains(movie))
                return; //if it is not added, immediately exit the method as it means that we've already removed this movie 
            
            //check if it is not the last actor because movie must have at least one actor
            
            //Here we additionally check movie.Actors.Contains(this)
            //because this function can be called from the Movie class,
            //where we have already removed this actor and the number of actors in the movie could become 1.
            //If it were not for this check, in this case we would remove the actor from the movie
            //but not the movie from the actor due to multiplicity exception.
            if (movie.Actors.Count == 1 && movie.Actors.Contains(this))
                throw new MultiplicityException();

            _movies.Remove(movie);
            
            movie.RemoveActor(this); // remove this actor from movie
        }

        //this method is used if we want to delete movie, as we check for multiplicity in simple method removeMovie, we can't delete last actor
        internal void RemoveMovieIgnoreMultiplicity(Movie movie)
        {
            if (movie == null)
                throw new ArgumentException("Movie cannot be null.");
            
            //check if movie is added to this actor
            if (!_movies.Contains(movie))
                return; //if it is not added, immediately exit the method as it means that we've already removed this movie 

            _movies.Remove(movie);
            
            movie.RemoveActorIgnoreMultiplicity(this); // remove this actor from movie
        }
        //

        
        //Class extent
        private static List<Actor> _actors = new List<Actor>();
        public static IReadOnlyList<Actor> Actors => _actors.AsReadOnly();
        
        private static void AddActor(Actor actor)
        {
            if (actor == null)
                throw new ArgumentException("Actor cannot be null");

            _actors.Add(actor);
        }
        
        public void Delete()
        {
            foreach (var movie in new List<Movie>(_movies))
            {
                movie.RemoveActor(this);
            }
            _movies.Clear();

            _actors.Remove(this);
        }
        
        //for testing
        public static void ClearAllActors()
        {
            foreach (var actor in new List<Actor>(_actors))
            {
                actor.Delete();
            }
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

        public List<Actor> GetExtent() => _actors;

        public void ReplaceExtent(List<Actor> newExtent)
        {
            _actors = newExtent ?? new List<Actor>();
        }
    }
}