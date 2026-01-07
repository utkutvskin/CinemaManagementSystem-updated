using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using CinemaManagementSystem.Area;
using CinemaManagementSystem.Exceptions;
using CinemaManagementSystem.PersistenceForAllClasses;

namespace CinemaManagementSystem.AssociationClasses;

[Serializable]
    public class Screening :IExtent<Screening>
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
        
        
        //Class extent
        private static List<Screening> _screenings = new();
        public static IReadOnlyList<Screening> Screenings => _screenings.AsReadOnly();

        private void AddScreening(Screening screening)
        {
            if(screening == null)
                throw new ArgumentException("Screening cannot be null.");
            _screenings.Add(screening);
        }
        
        public static void ClearExtent()
        {
            foreach (var s in new List<Screening>(_screenings))
            {
                s.Cancel();
            }
        }
        
        
        
        //constructors
        public Screening() { }

        
        private Screening(Movie movie, Hall hall, DateTime date, TimeSpan hour, string language)
        {
            //assign movie and hall to this screening
            _movie = movie;
            _hall = hall;

            Date = date;
            Hour = hour;
            Language = language;

            AddScreening(this);
        }

        //attribute association Movie - Hall
        [XmlIgnore]
        private Movie _movie;

        [XmlIgnore]
        private Hall _hall;
        
        [XmlIgnore]
        public Movie Movie => _movie;

        [XmlIgnore]
        public Hall Hall => _hall;
        
        //creating a screening (association between Movie - Hall) (from screening side)
        //only this method can be used for creating association from screening side (constructor is private)
        public static Screening Create(Movie movie, Hall hall, DateTime date, TimeSpan hour, string language)
        {
            if (movie == null) throw new ArgumentException("Movie cannot be null.");
            if (hall == null) throw new ArgumentException("Hall cannot be null.");

            
            Screening? duplicate = _screenings
                .FirstOrDefault(s => s.Movie == movie 
                                     && s.Date == date 
                                     && s.Hour == hour 
                                     && s.Language == language
                                     && s.Hall == hall);
            
            //checking duplicates
            if (duplicate != null)
                throw new DuplicateException(duplicate, movie, hall);
            
            
            if (date.Date < movie.ReleaseDate.Date)
                throw new ArgumentException("Screening date cannot be earlier than movie release date.");
            
            //check if hall is free at this date and time
            if(CheckOverlaps(hall, date, hour, movie.Duration))
                throw new OverlapsException(hall, date, hour);

            //if all is correct then we create Screening using constructor
            var screening = new Screening(movie, hall, date, hour, language);

            //assign this screening to the movie and hall
            movie.AddScreeningInternal(screening);
            hall.AddScreeningInternal(screening);

            return screening;
        }
        
        
        //method for checking if hall is free at this date and time
        private static bool CheckOverlaps(Hall hall, DateTime date, TimeSpan hour, int movieDuration)
        {
            DateTime newStart = date.Date + hour;
            DateTime newEnd = newStart + TimeSpan.FromMinutes(movieDuration);
            
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
                    return true;
            }
            
            return false;
        }

        //removing a screening (association between Movie - Hall) (can be done only from screening side)
        public static void RemoveScreening(Movie movie, Hall hall, DateTime date, TimeSpan hour)
        {
            if (movie == null) throw new ArgumentException("Movie cannot be null.");
            if (hall == null) throw new ArgumentException("Hall cannot be null.");
            
            Screening? screening = _screenings
                .FirstOrDefault(s => s.Movie == movie 
                                     && s.Date == date 
                                     && s.Hour == hour 
                                     && s.Hall == hall);
            
            if (screening != null)
            {
                screening.Cancel();
            }
            else throw new ExistenceException("Screening" );
            
        }
        
        
        //Delete association 
        public void Cancel()
        {
            _screenings.Remove(this);
            _movie.RemoveScreeningInternal(this);
            _hall.RemoveScreeningInternal(this);
            foreach (var t in new List<Ticket>(_tickets) )
            {
                t.Cancel();
            }
        }

        public override string ToString()
        {
            return $"{Movie?.Title} on {Date:dd/MM/yyyy} at {Hour:hh\\:mm} in hall {Hall?.Number} ({Language})";
        }
        
        //
        
        //attribute association Ticket
        [XmlIgnore]
        private readonly List<Ticket> _tickets = new();

        [XmlIgnore]
        public IReadOnlyCollection<Ticket> Tickets => _tickets.AsReadOnly();

        internal void AddTicketInternal(Ticket ticket)
        {
            if (ticket == null)
                throw new ArgumentException("Ticket cannot be null.");
            _tickets.Add(ticket);
        }

        internal void RemoveTicketInternal(Ticket ticket)
        {
            if (ticket != null)
                _tickets.Remove(ticket);
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

        public List<Screening> GetExtent() => _screenings;

        public void ReplaceExtent(List<Screening> newExtent)
        {
            _screenings = newExtent ?? new List<Screening>();
        }
    }
