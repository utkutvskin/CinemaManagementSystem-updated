using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace CinemaManagementSystem.AssociationClasses;

[Serializable]
    public class Screening
    {
        
        private DateTime _date;
        private TimeSpan _hour;
        private string _language;

        public DateTime Date
        {
            get => _date.Date;
            set
            {
               if(value.Date < DateTime.Today.Date)
                    throw new ArgumentException("Date cannot be less than the today date.");
                _date = value;
            }
        }

        public TimeSpan Hour
        {
            get => _hour;
            set
            {
                if (value < TimeSpan.Zero || value >= TimeSpan.FromDays(1))
                    throw new ArgumentException("Hour must be between 00:00 and 23:59.");
                _hour = value;
            }
        }

        public string Language
        {
            get => _language;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Language cannot be empty.");
                _language = value;
            }
        }

        [XmlIgnore]
        private Movie _movie;

        [XmlIgnore]
        private Hall _hall;
        
        [XmlIgnore]
        public Movie Movie => _movie;

        [XmlIgnore]
        public Hall Hall => _hall;

        
        //Class extent
        private static List<Screening> _screenings = new();
        public static IReadOnlyList<Screening> Screenings => _screenings.AsReadOnly();

        private void AddScreening(Screening screening)
        {
            if(screening == null)
                throw new ArgumentException("Screening cannot be null.");
            _screenings.Add(screening);
        }
        
        //constructors
        public Screening() { }

        
        private Screening(Movie movie, Hall hall, DateTime date, TimeSpan hour, string language)
        {
            _movie = movie ?? throw new ArgumentException("Movie cannot be null.");
            _hall = hall ?? throw new ArgumentException("Hall cannot be null.");

            Date = date;
            Hour = hour;
            Language = language;

            AddScreening(this);
        }

        //creating a screening
        public static Screening Create(Movie movie, Hall hall, DateTime date, TimeSpan hour, string language)
        {
            if (movie == null) throw new ArgumentException("Movie cannot be null.");
            if (hall == null) throw new ArgumentException("Hall cannot be null.");

            if (date.Date < movie.ReleaseDate.Date)
                throw new ArgumentException("Screening date cannot be earlier than movie release date.");
            
            DateTime newStart = date.Date + hour;
            DateTime newEnd = newStart + TimeSpan.FromMinutes(movie.Duration);
            
            foreach (var s in _screenings)
            {
                if (s.Hall != hall) 
                    continue;
                
                if (s.Date.Date != date.Date)
                    continue;
                
                DateTime existingStart = s.Date.Date + s.Hour;
                DateTime existingEnd = existingStart + TimeSpan.FromMinutes(s.Movie.Duration);

                bool overlaps = newStart < existingEnd && existingStart < newEnd;

                if (overlaps)
                {
                    throw new InvalidOperationException(
                        $"Hall {hall.Number} is already occupied between " +
                        $"{existingStart:HH\\:mm} and {existingEnd:HH\\:mm} on {date:dd/MM/yyyy}.");
                }
            }

            var screening = new Screening(movie, hall, date, hour, language);

            movie.AddScreeningInternal(screening);
            hall.AddScreeningInternal(screening);

            return screening;
        }

        //Delete association 
        public void Cancel()
        {
            _screenings.Remove(this);
            _movie?.RemoveScreeningInternal(this);
            _hall?.RemoveScreeningInternal(this);
        }

        public override string ToString()
        {
            return $"{Movie?.Title} on {Date:dd/MM/yyyy} at {Hour:hh\\:mm} in hall {Hall?.Number} ({Language})";
        }

        public static void ClearExtent()
        {
            foreach (var s in new List<Screening>(_screenings))
            {
                s.Cancel();
            }
        }
        
        //Persistence 
        public static void Save(string filePath)
        {
            StreamWriter sw = File.CreateText(filePath);
            XmlSerializer serializer = new XmlSerializer(typeof(List<Screening>));
            using (XmlTextWriter writer = new XmlTextWriter(sw))
            {
                serializer.Serialize(writer, _screenings);
            }
        }

        public static bool Load(string filePath)
        {
            if (!File.Exists(filePath))
            {
                _screenings.Clear();
                return false;
            }

            XmlSerializer serializer = new XmlSerializer(typeof(List<Screening>));
            using (XmlTextReader reader = new XmlTextReader(filePath))
            {
                try
                {
                    _screenings = (List<Screening>)serializer.Deserialize(reader);
                }
                catch 
                {
                    _screenings.Clear();
                    return false;
                }
            }

            return true;
        }
    }