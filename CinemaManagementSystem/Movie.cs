using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
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
        private List<string> _genres;
        private ScreeningEnum _screeningType;
        private int _duration;
        
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

        public List<string> Genres
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

        public Movie(string title, List<string> directors, List<string> genres, ScreeningEnum screeningType, int duration)
        {

            Title = title;
            Directors = directors;
            Genres = genres;
            ScreeningType = screeningType;
            Duration = duration;

            AddMovie(this);
        }

      

        public override string ToString()
        {
            string directors = string.Join(", ", Directors);
            string genres = string.Join(", ", Genres);
            return $"{Title} ({genres}) directed by {directors}, {ScreeningType}, {Duration} min";
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


        public static void ClearExtent()
        {
            _movies.Clear();
        }
    }
}
