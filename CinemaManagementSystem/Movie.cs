using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using CinemaManagementSystem.AssociationClasses;
using CinemaManagementSystem.Enums;

namespace CinemaManagementSystem
{
    [Serializable]
    public class Movie
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
            set => _releaseDate = value;
        }
        
        
        //Basic association
        [XmlIgnore]
        private readonly HashSet<Actor> _actors = new HashSet<Actor>();

        [XmlIgnore]
        public IReadOnlyCollection<Actor> Actors => _actors;
        
        
        public void AddActor(Actor actor)
        {
            if (actor == null)
                throw new ArgumentException("Actor cannot be null.");

            if (_actors.Contains(actor))
                throw new InvalidOperationException("Actor is already assigned to this movie.");

            _actors.Add(actor);

            
            if (!actor.Movies.Contains(this))
            {
                actor.AddMovieInternal(this);
            }
        }

        public void RemoveActor(Actor actor)
        {
            if (actor == null)
                throw new ArgumentException("Actor cannot be null.");

            if (!_actors.Contains(actor))
                throw new InvalidOperationException("Actor is not assigned to this movie.");

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

        
        
        //Attribute association
        [XmlIgnore]
        private readonly List<Screening> _screenings = new();

        [XmlIgnore]
        public IReadOnlyCollection<Screening> Screenings => _screenings;

        public Screening ScheduleScreening(Hall hall, DateTime date, TimeSpan hour, string language)
        {
            if (hall == null)
                throw new ArgumentException("Hall cannot be null.");

            return Screening.Create(this, hall, date, hour, language);
        }

        
        internal void AddScreeningInternal(Screening screening)
        {
            if (screening != null)
                _screenings.Add(screening);
        }

        internal void RemoveScreeningInternal(Screening screening)
        {
            if (screening != null)
                _screenings.Remove(screening);
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

      

        public override string ToString()
        {
            string directors = string.Join(", ", Directors);
            string genres = string.Join(", ", Genres);
            return $"{Title} ({genres}) directed by {directors}, {ScreeningType}, {Duration} min";
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

            _movies.Remove(this);
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
    }
}
