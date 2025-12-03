using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using CinemaManagementSystem.AssociationClasses;

namespace CinemaManagementSystem
{
    [Serializable]
    public class Hall
    {
        //  Attributes 
        private int _number;
        
        public int Number
        {
            get => _number;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Number can't be less than 0.");
                _number = value;
            }
        }

        [XmlIgnore]
        public static readonly int MaxCapacity = 100;
        
        
        
        //composition association (Seat)
        [XmlIgnore]
        private readonly HashSet<Seat> _seats = new HashSet<Seat>();

        [XmlIgnore]
        public IReadOnlyCollection<Seat> Seats => _seats;
        
        public Seat AddSeat(int number, char row)
        {
            var newSeat = new Seat(number, char.ToUpper(row), this);
            
            return newSeat;
        }
        
        internal void AddSeatInternal(Seat seat)
        {
            if (seat == null)
                throw new ArgumentException("Seat cannot be null.");
            
            if (_seats.Count >= MaxCapacity)
                throw new InvalidOperationException(
                    $"Hall {Number} has reached its maximum capacity of {MaxCapacity} seats.");

            foreach (var s in _seats)
            {
                if (s.Number == seat.Number && s.Row == seat.Row)
                    throw new ArgumentException($"Seat {seat.Row}{seat.Number} already exists in this hall.");
            }
            _seats.Add(seat);
        }


        public void RemoveSeat(Seat seat)
        {
            if (seat == null)
                throw new ArgumentException("Seat cannot be null.");

            if (!_seats.Contains(seat))
                throw new InvalidOperationException("Seat does not belong to this hall.");

            _seats.Remove(seat);
            
            Seat.RemoveFromExtent(seat);  
        }

        public void DeleteHall()
        {
            foreach (var screening in _screenings)
            {
                screening.Cancel();
            }
            _screenings.Clear();
            
            foreach (var seat in _seats)
            {
                Seat.RemoveFromExtent(seat);
            }

            _seats.Clear();
            
            if (_floor != null)
            {
                _floor.InternalRemoveHall(this); 
            }

            RemoveFromExtent(this);
        }

        internal void InternalClearSeats()
        {
            _seats.Clear();
        }
        
        
        //composition association (Floor)
        [XmlIgnore]           
        private Floor _floor;

        [XmlIgnore]
        public Floor FLoor => _floor;

        internal void SetFloor(Floor floor)
        {
            if (floor == null)
                throw new ArgumentException("floor cannot be null for a hall.");

            
            if (_floor != null && _floor != floor)
                throw new InvalidOperationException("Hall is already assigned to another floor.");

            _floor = floor;
        }
        
        
        
        //attribute association
        [XmlIgnore]
        private readonly List<Screening> _screenings = new();

        [XmlIgnore]
        public IReadOnlyCollection<Screening> Screenings => _screenings;

        public Screening AddScreening(Movie movie, DateTime date, TimeSpan hour, string language)
        {
            if (movie == null)
                throw new ArgumentException("Movie cannot be null.");

            return Screening.Create(movie, this, date, hour, language);
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
        private static List<Hall> _halls = new List<Hall>();
        public static IReadOnlyList<Hall> Halls => _halls.AsReadOnly();
        
        private static void AddHall(Hall hall)
        {
            if (hall == null)
                throw new ArgumentException("hall cannot be null");

            _halls.Add(hall);
        }
        
        internal static void RemoveFromExtent(Hall hall)
        {
            _halls.Remove(hall);
        }

        
        
        //  Constructors 
        public Hall() { } 

        public Hall(int number)
        {

            Number = number;
            AddHall(this);
        }

        public Hall(int number, Floor floor) : this(number)
        {
            SetFloor(floor);
            
            floor.AddHallInternal(this);
        }

        
        
        //  Methods 
        public override string ToString()
        {
            return $"Hall {Number} (Max Capacity: {MaxCapacity})";
        }

        //for tests
        public static void ClearExtent()
        {
            foreach (var hall in _halls)
            {
                hall.DeleteHall();
            }
        }

        
        
        //  Persistence 
        public static void Save(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Hall>));

            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                serializer.Serialize(fs, _halls);
                fs.Flush();
            }
        }

        public static void Load(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Hall file not found.");

            XmlSerializer serializer = new XmlSerializer(typeof(List<Hall>));

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var loaded = (List<Hall>)serializer.Deserialize(fs);
                _halls.Clear();
                _halls.AddRange(loaded);
            }
        }
    }
}
