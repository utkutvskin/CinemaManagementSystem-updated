using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace CinemaManagementSystem
{
    [Serializable]
    public class Movie
    {
        // ---------- Backing fields (preserve original public names) ----------
        private string _title;
        private List<string> _directors = new List<string>();
        private List<string> _genres = new List<string>();
        private string _screeningType;
        private int _duration;
        private DateTime _releaseDate;

        // ---------- Properties with validation ----------
        public string Title
        {
            get => _title;
            set
            {
                var trimmed = value?.Trim();
                if (string.IsNullOrWhiteSpace(trimmed))
                    throw new ArgumentException("Title cannot be empty.");
                _title = trimmed;
            }
        }

        public List<string> Directors
        {
            get => _directors;
            set
            {
                if (value == null || value.Count == 0)
                    throw new ArgumentException("At least one director must be specified.");
                // copy to avoid external mutation
                _directors = new List<string>(value);
            }
        }

        public List<string> Genres
        {
            get => _genres;
            set
            {
                if (value == null || value.Count == 0)
                    throw new ArgumentException("At least one genre must be specified.");
                _genres = new List<string>(value);
            }
        }

        public string ScreeningType
        {
            get => _screeningType;
            set
            {
                var trimmed = value?.Trim();
                if (string.IsNullOrWhiteSpace(trimmed))
                    throw new ArgumentException("Screening type cannot be empty.");
                _screeningType = trimmed;
            }
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
                if (value > DateTime.Now)
                    throw new ArgumentException("Release date cannot be in the future.");
                _releaseDate = value;
            }
        }

        // ---------- Derived attribute ----------
       
        public int AgeInYears
        {
            get
            {
                var today = DateTime.Today;
                int age = today.Year - ReleaseDate.Year;
                if (ReleaseDate > today.AddYears(-age))
                    age--;
                return age;
            }
        }

        // ---------- Class extent ----------
        private static List<Movie> _movies = new List<Movie>();
        public static IReadOnlyList<Movie> Movies => _movies.AsReadOnly();

        // Backwards-compatible clear methods
        public static void ClearAllMovies() => _movies.Clear();
        public static void ClearExtent() => ClearAllMovies();

        // ---------- Constructors ----------
        public Movie() { } // serializer

        public Movie(string title, List<string> directors, List<string> genres, string screeningType, int duration, DateTime releaseDate)
        {
            // Use property setters so validation and normalization are reused
            Title = title;
            Directors = directors;
            Genres = genres;
            ScreeningType = screeningType;
            Duration = duration;
            ReleaseDate = releaseDate;

            _movies.Add(this);
        }

        // Convenience factory preserved
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
            var serializer = new XmlSerializer(typeof(List<Movie>));

            var dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                serializer.Serialize(fs, _movies);
                fs.Flush();
            }
        }

        public static void Load(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Movie file not found.");

            var serializer = new XmlSerializer(typeof(List<Movie>));
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var loaded = (List<Movie>)serializer.Deserialize(fs);
                _movies = loaded ?? new List<Movie>();
            }
        }
    }
}
