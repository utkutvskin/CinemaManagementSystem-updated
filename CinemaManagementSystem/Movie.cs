using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace CinemaManagementSystem
{
    [Serializable]
    public class Movie
    {
        // ---------- Attributes ----------
        public string Title { get; set; }

        // Multi-value attributes
        public List<string> Directors { get; set; } = new List<string>();
        public List<string> Genres { get; set; } = new List<string>();

        public string ScreeningType { get; set; } // örn. "2D", "3D", "IMAX"
        public int Duration { get; set; }

        // Derived attribute example
        [XmlIgnore]
        public int AgeInYears
        {
            get
            {
                return DateTime.Now.Year - ReleaseDate.Year -
                    (DateTime.Now.DayOfYear < ReleaseDate.DayOfYear ? 1 : 0);
            }
        }

        public DateTime ReleaseDate { get; set; }

        // ---------- Class extent ----------
        private static List<Movie> _movies = new List<Movie>();
        public static IReadOnlyList<Movie> Movies => _movies.AsReadOnly();

        // ---------- Constructors ----------
        public Movie() { }

        public Movie(string title, List<string> directors, List<string> genres, string screeningType, int duration, DateTime releaseDate)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty.");
            if (directors == null || directors.Count == 0)
                throw new ArgumentException("At least one director must be specified.");
            if (genres == null || genres.Count == 0)
                throw new ArgumentException("At least one genre must be specified.");
            if (string.IsNullOrWhiteSpace(screeningType))
                throw new ArgumentException("Screening type cannot be empty.");
            if (duration <= 0)
                throw new ArgumentException("Duration must be positive.");
            if (releaseDate > DateTime.Now)
                throw new ArgumentException("Release date cannot be in the future.");

            Title = title;
            Directors = directors;
            Genres = genres;
            ScreeningType = screeningType;
            Duration = duration;
            ReleaseDate = releaseDate;

            _movies.Add(this);
        }

        // ---------- Methods ----------
        public static Movie AddMovie(string title, List<string> directors, List<string> genres, string screeningType, int duration, DateTime releaseDate)
        {
            return new Movie(title, directors, genres, screeningType, duration, releaseDate);
        }

        public override string ToString()
        {
            string directors = string.Join(", ", Directors);
            string genres = string.Join(", ", Genres);
            return $"{Title} ({genres}) directed by {directors}, {ScreeningType}, {Duration} min";
        }

        // ---------- Persistence ----------
        public static void Save(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Movie>));
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                serializer.Serialize(fs, _movies);
            }
        }

        public static void Load(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Movie file not found.");

            XmlSerializer serializer = new XmlSerializer(typeof(List<Movie>));
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                _movies = (List<Movie>)serializer.Deserialize(fs);
            }
        }

        public static void ClearExtent()
        {
            _movies.Clear();
        }
    }
}
