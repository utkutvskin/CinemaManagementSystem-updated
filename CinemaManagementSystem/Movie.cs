using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using CinemaManagementSystem.AssociationClasses;
using CinemaManagementSystem.Enums;
using CinemaManagementSystem.Exceptions;
using CinemaManagementSystem.PersistenceForAllClasses;

namespace CinemaManagementSystem
{
    [Serializable]
    public class Movie :IExtent<Movie>
    {
        //  Attributes 
        private string _title;

        // Multi-value attributes
        private List<string> _directors;
        private List<GenreEnum> _genres;
        
        private ScreeningEnum _screeningType;
        private int _duration;
        private DateTime _releaseDate;
        
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
        
        
        
        
        //Basic association Actor
        [XmlIgnore]
        private readonly HashSet<Actor> _actors = new HashSet<Actor>();

        [XmlIgnore]
        public IReadOnlyCollection<Actor> Actors => _actors;


        public void AddActor(Actor actor)
        {
            if (actor == null)
                throw new ArgumentException("Actor cannot be null.");

            if (_actors.Contains(actor))
                throw new DuplicateException("Actor", actor.ToString());

            _actors.Add(actor);

            
            if (!actor.Movies.Contains(this))
            {
                actor.AddMovieInternal(this);
            }
        }

        public void RemoveActor(Actor actor)
        {
            if (_actors.Count == 1)
                throw new MultiplicityException();
            
            if (actor == null)
                throw new ArgumentException("Actor cannot be null.");
        
            if (!_actors.Contains(actor))
                throw new ExistenceException("Actor", actor.ToString(), "Movie");
        
            _actors.Remove(actor);
        
            if (actor.Movies.Contains(this))
            {
                actor.RemoveMovieInternal(this);
            }
        }
        

        internal void AddActorInternal(Actor actor)
        {
            
            _actors.Add(actor);
        }

        internal void RemoveActorInternal(Actor actor)
        {
            _actors.Remove(actor);
        }
        
        //
        
        
        
        
        //Attribute association
        [XmlIgnore]
        private readonly List<Screening> _screenings = new();

        [XmlIgnore]
        public IReadOnlyCollection<Screening> Screenings => _screenings;

        public Screening ScheduleScreening(Hall hall, DateTime date, TimeSpan hour, string language)
        {
            return Screening.Create(this, hall, date, hour, language);
        }

        public void RemoveScreening(Hall hall, DateTime date, TimeSpan hour)
        {
            Screening.RemoveScreening(this, hall, date, hour);
        }
        
        internal void AddScreeningInternal(Screening screening)
        {
            if (screening == null) throw new ArgumentException("Screening cannot be null.");
            _screenings.Add(screening);
        }

        internal void RemoveScreeningInternal(Screening screening)
        {
            if (screening == null) throw new ArgumentException("Screening cannot be null.");
            _screenings.Remove(screening);
        }
        //

        
         
         // Reflexive association: sequels / prequels
        [XmlIgnore]
        private readonly HashSet<Movie> _sequels = new HashSet<Movie>();
        [XmlIgnore]
        public IReadOnlyCollection<Movie> Sequels => _sequels;

        [XmlIgnore]
        private readonly HashSet<Movie> _prequels = new HashSet<Movie>();
        [XmlIgnore]
        public IReadOnlyCollection<Movie> Prequels => _prequels;
        

        public void AddSequel(Movie sequel)
        {
            if (sequel == null) 
                throw new ArgumentException("Sequel cannot be null.");
            
            if (sequel == this) 
                throw new InvalidOperationException("A movie cannot be a sequel to itself.");
            
            if (_sequels.Contains(sequel)) 
                throw new DuplicateException("Sequel",  sequel.ToString());

            _sequels.Add(sequel);

            if (!sequel._prequels.Contains(this))
            {
                sequel.AddPrequelInternal(this);
            }
        }

        public void RemoveSequel(Movie sequel)
        {
            if (sequel == null) 
                throw new ArgumentException("Sequel cannot be null.");
            
            if (!_sequels.Contains(sequel)) 
                throw new ExistenceException("Sequel" , sequel.ToString(), "Movie");

            _sequels.Remove(sequel);

            if (sequel._prequels.Contains(this))
            {
                sequel.RemovePrequelInternal(this);
            }
        }

        internal void AddSequelInternal(Movie sequel)
        {
            _sequels.Add(sequel);
        }

        internal void RemoveSequelInternal(Movie sequel)
        {
            _sequels.Remove(sequel);
        }
        
        public void AddPrequel(Movie prequel)
        {
            if (prequel == null) 
                throw new ArgumentException("Prequel cannot be null.");
            
            if (prequel == this) 
                throw new InvalidOperationException("A movie cannot be a prequel to itself.");
            
            if (_prequels.Contains(prequel)) 
                throw new DuplicateException("Prequel", prequel.ToString());

            _prequels.Add(prequel);

            if (!prequel._sequels.Contains(this))
            {
                prequel.AddSequelInternal(this);
            }
        }

        public void RemovePrequel(Movie prequel)
        {
            if (prequel == null)
                throw new ArgumentException("Prequel cannot be null.");
            
            if (!_prequels.Contains(prequel)) 
                throw new ExistenceException("Prequel", prequel.ToString(), "Movie");

            _prequels.Remove(prequel);

            if (prequel._sequels.Contains(this))
            {
                prequel.RemoveSequelInternal(this);
            }
        }

        internal void AddPrequelInternal(Movie prequel)
        {
            _prequels.Add(prequel);
        }

        internal void RemovePrequelInternal(Movie prequel)
        {
            _prequels.Remove(prequel);
        }
        

        public void RemoveReflexiveAssociations()
        {
            // Remove this movie from each prequel's sequels (use a copy to avoid modifying while iterating)
            foreach (var prequel in new List<Movie>(_prequels))
            {
                // Use internal helper on the other side so we don't trigger validation/exceptions there
                prequel.RemoveSequelInternal(this);
            }
            // Clear local prequel collections and ids
            _prequels.Clear();

            // Remove this movie from each sequel's prequels
            foreach (var sequel in new List<Movie>(_sequels))
            {
                sequel.RemovePrequelInternal(this);
            }
            // Clear local sequel collections and ids
            _sequels.Clear();
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
            
            foreach (var actor in new List<Actor>(_actors))
            {
                actor.RemoveMovieInternal(this);
            }
            _actors.Clear();

            
            foreach (var screening in new List<Screening>(_screenings))
            {
                screening.Cancel();
            }
            _screenings.Clear();

          
            foreach (var prequel in new List<Movie>(_prequels))
            {
                prequel.RemoveSequelInternal(this);
            }
            _prequels.Clear();

            
            foreach (var sequel in new List<Movie>(_sequels))
            {
                sequel.RemovePrequelInternal(this);
            }
            _sequels.Clear();

          
            _movies.Remove(this);
        }

        
        
        //  Constructors 
        public Movie() { }
        

        public Movie(string title, List<string> directors, List<GenreEnum> genres, ScreeningEnum screeningType, int duration, DateTime releaseDate)
        {

            Title = title;
            Directors = directors;
            Genres = genres;
            ScreeningType = screeningType;
            Duration = duration;
            ReleaseDate = releaseDate;

            AddMovie(this);
            
        }
        
        public Movie(string title, List<string> directors, List<GenreEnum> genres, ScreeningEnum screeningType, int duration, DateTime releaseDate, List<Actor> actors)
        :this(title, directors, genres, screeningType, duration, releaseDate)
        {

            foreach (var actor in actors)
            {
                AddActor(actor);
            }
        }

      

        public override string ToString()
        {
            string directors = string.Join(", ", Directors);
            string genres = string.Join(", ", Genres);
            return $"{Title} ({genres}) directed by {directors}, {ScreeningType}, {Duration} min";
        }

        

        
      
        
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

        //For tests 
        public static void ClearExtent()
        {
            foreach (var movie in new List<Movie>(_movies))
            {
                movie.Delete();
            }
        }
        
        public List<Movie> GetExtent() => _movies;

        public void ReplaceExtent(List<Movie> newExtent)
        {
            _movies = newExtent ?? new List<Movie>();
        }
    }
}




