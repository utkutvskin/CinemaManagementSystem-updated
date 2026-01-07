using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.AccessControl;
using System.Xml;
using System.Xml.Serialization;
using CinemaManagementSystem.AssociationClasses;
using CinemaManagementSystem.Enums;
using CinemaManagementSystem.Exceptions;
using CinemaManagementSystem.PersistenceForAllClasses;
using CinemaManagementSystem.Person;

namespace CinemaManagementSystem
{
    [Serializable]
    public class Movie :IExtent<Movie>
    {
        //  Attributes 
        private string _title;

        private List<string> _directors;
        private List<GenreEnum> _genres;
        
        private ScreeningEnum _screeningType;
        private int _duration;
        private DateTime _releaseDate;
        private double _price;
        
        public string Title
        {
            get => _title;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Title cannot be empty.");
                _title = value;
            }
            
        }
        
        // Multi-value attributes
        public List<string> Directors
        {
            get => _directors;
            set
            {
                if (value == null)
                    throw new ArgumentException("At least one director must be specified.");
                _directors = value;
            }
        }

        public List<GenreEnum> Genres
        {
            get => _genres;
            set
            {
                if (value == null)
                    throw new ArgumentException("At least one genre must be specified.");
                _genres = value;
            }
        }

        public ScreeningEnum ScreeningType
        {
            get => _screeningType;
            set => _screeningType = value;
        }

        public int Duration
        {
            get => _duration;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Duration must be positive.");
                _duration = value;
            }
        }

        public DateTime ReleaseDate
        {
            get => _releaseDate;
            set
            {
                if(value > DateTime.Now.AddYears(1))
                    throw new ArgumentException("Release date cannot be in deep future.");
            }
        }

        public double Price
        {
            get => _price;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Price must be positive.");
                _price = value;
            }
        }
        
        
        //Attribute association Movie - Hall 
        [XmlIgnore]
        private readonly List<Screening> _screenings = new();

        [XmlIgnore]
        public IReadOnlyCollection<Screening> Screenings => _screenings;
        
        
        internal void AddScreeningInternal(Screening screening)
        {
            _screenings.Add(screening);
        }

        internal void RemoveScreeningInternal(Screening screening)
        {
            _screenings.Remove(screening);
        }
        //

        
         
         // Reflexive association: sequels / prequels
        [XmlIgnore] private Movie? _sequels;
        [XmlIgnore]
        public Movie? Sequels => _sequels;

        [XmlIgnore] private Movie? _prequels;
        [XmlIgnore]
        public Movie? Prequels => _prequels;
        

        //Adding sequel to this movie
        public void AddSequel(Movie sequel)
        {
            if (sequel == null) 
                throw new ArgumentException("Sequel cannot be null.");
            
            if (sequel == this) 
                throw new ReflexAssociationException("A movie cannot be a sequel to itself.");

            //check if the sequel is not already assigned to this movie
            if (_sequels == sequel)
                return; //if it is assigned, immediately exit the method to avoid recursion
            
            if(_sequels != null && _sequels != sequel)
                throw new ReflexAssociationException("There is already a sequel");
                

            _sequels = sequel;

             sequel.AddPrequel(this); // assign this movie as prequel
        }
        
        public void RemoveSequel(Movie sequel)
        {
            if (sequel == null) 
                throw new ArgumentException("Sequel cannot be null.");
            
            //check if there is a sequel to this movie
            if(_sequels == null)
                return; //if it is not assigned, immediately exit the method 
            
            if (_sequels != sequel) 
                throw new ExistenceException(sequel, this);

            _sequels = null;

            sequel.RemovePrequel(this); 
        }
        
        //Adding prequel to this movie
        public void AddPrequel(Movie prequel)
        {
            if (prequel == null) 
                throw new ArgumentException("Prequel cannot be null.");
            
            if (prequel == this) 
                throw new ReflexAssociationException("A movie cannot be a prequel to itself.");
            
            //check if the prequel is not already assigned to this movie
            if (_prequels == prequel) 
                return;  //if it is assigned, immediately exit the method to avoid recursion
            
            if(_prequels != null && _prequels != prequel)
                throw new ReflexAssociationException("There is already a sequel");

            _prequels = prequel;

            prequel.AddSequel(this); // assign this movie as sequel
        }

        public void RemovePrequel(Movie prequel)
        {
            if (prequel == null)
                throw new ArgumentException("Prequel cannot be null.");
            
            //check if there is a prequel to this movie
            if(_prequels == null)
                return; //if it is not assigned, immediately exit the method 
            
            if (_prequels != prequel) 
                throw new ExistenceException(prequel, this);

            _prequels = null;

            prequel.RemoveSequel(this);
        }


        public void RemoveReflexiveAssociations()
        {
            if(_sequels != null)
            {
                _sequels.RemovePrequel(this);
                _sequels = null;
            }

            if (_prequels != null)
            {
                _prequels.RemoveSequel(this);
                _prequels = null;
            }
        }
        
        

        //  Class extent
        private static List<Movie> _movies = new List<Movie>();
        public static IReadOnlyList<Movie> Movies => _movies.AsReadOnly();
        
        private static void AddMovie(Movie movie)
        {
            if (movie == null)
                throw new ArgumentException("movie cannot be null");

            _movies.Add(movie);
        }
        
        public void Delete()
        {
            
            foreach (var screening in new List<Screening>(_screenings))
            {
                screening.Cancel();
            }
            _screenings.Clear();

          
            RemoveReflexiveAssociations();

          
            _movies.Remove(this);
        }
        
        //For tests 
        public static void ClearExtent()
        {
            foreach (var movie in new List<Movie>(_movies))
            {
                movie.Delete();
            }
        }

        
        
        //  Constructors 
        public Movie() { }
        

        public Movie(string title, List<string> directors, List<GenreEnum> genres, ScreeningEnum screeningType, int duration, DateTime releaseDate, double price)
        {

            Title = title;
            Directors = directors;
            Genres = genres;
            ScreeningType = screeningType;
            Duration = duration;
            ReleaseDate = releaseDate;
            Price = price;
            
            AddMovie(this);
            
        }
        
        
        public override string ToString()
        {
            string directors = string.Join(", ", Directors);
            string genres = string.Join(", ", Genres);
            return $"{Title} ({genres}) directed by {directors}, {ScreeningType}, {Duration} min";
        }

        
      
        //methods
        public void AddDirector(string director)
        {

            if (string.IsNullOrWhiteSpace(director))
                throw new ArgumentException("director cannot be empty");
            
            if(_directors.Contains(director))
                throw new ArgumentException("director already exists");
                
            _directors.Add(director);
        }

        public void AddGenres(GenreEnum genre)
        {
            if (_genres.Contains(genre))
                throw new ArgumentException("genre already exists");
            _genres.Add(genre);
        }

        
        
        
        //  Persistence 
        public static void Save(string filePath)
        {
            StreamWriter sw = File.CreateText(filePath);
            XmlSerializer serializer = new XmlSerializer(typeof(List<Movie>));
            using (XmlTextWriter writer = new XmlTextWriter(sw))
            {
                serializer.Serialize(writer, _movies);
            }
        }

        public static void Load(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Movie file not found.");

            XmlSerializer serializer = new XmlSerializer(typeof(List<Movie>));
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var loaded = (List<Movie>)serializer.Deserialize(fs);
                _movies = loaded ?? new List<Movie>();
            }
            
        }
        
        public List<Movie> GetExtent() => _movies;

        public void ReplaceExtent(List<Movie> newExtent)
        {
            _movies = newExtent ?? new List<Movie>();
        }
    }
}




