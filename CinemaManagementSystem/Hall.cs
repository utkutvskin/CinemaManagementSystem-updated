using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using CinemaManagementSystem.AssociationClasses;
using CinemaManagementSystem.Exceptions;
using CinemaManagementSystem.PersistenceForAllClasses;

namespace CinemaManagementSystem
{
    [Serializable]
    public class Hall : CleanableArea, IExtent<Hall>
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

        [XmlIgnore] public static readonly int MaxCapacity = 100;
        


        //attribute association Displayer - Hall
        [XmlIgnore]
        private readonly List<DisplayerAssigment> _assigments = new();

        [XmlIgnore]
        public IReadOnlyCollection<DisplayerAssigment> Assigments => _assigments;
        
        
        public void AddDisplayerAssignmentInternal(DisplayerAssigment assigment)
        {
            _assigments.Add(assigment);
        }
        
        public void RemoveDisplayerAssignmentInternal(DisplayerAssigment assigment)
        {
            _assigments.Remove(assigment);
        }

        //

        //composition association (hall - seat )

        [XmlIgnore] private readonly HashSet<Seat> _seats = new HashSet<Seat>();
        
        [XmlIgnore] public IReadOnlyCollection<Seat> Seats => _seats;
        

        public Seat AddSeat(int number, char row)
        {
            var newSeat = new Seat(number, char.ToUpper(row), this);

            return newSeat;
        }


        internal void SetSeat(Seat seat)
        {
            if (seat == null)
                throw new ArgumentException("Seat cannot be null.");

            //for tests I use 3 instead of MaxCapacity as it is too big 
            if (_seats.Count >= 3) //MaxCapacity
                throw new CapacityException("seats", MaxCapacity);
                  
            foreach (var s in _seats)
            {
                if (s.Number == seat.Number && s.Row == seat.Row)
                    throw new DuplicateException("Seat", seat.ToString());
            }
            _seats.Add(seat);
        }


        public void RemoveSeat(Seat seat)
        {
            if (seat == null)
                throw new ArgumentException("Seat cannot be null.");

            if (!_seats.Contains(seat))
                throw new ExistenceException(seat, this);

            _seats.Remove(seat);

            seat.RemoveFromExtent();
        }


        internal void InternalClearSeats()
        {
            _seats.Clear();
        }
        
        //
        
        

        //Composition association Floor
        [XmlIgnore] private Floor _floor;

        [XmlIgnore]
        public Floor Floor => _floor;

        internal void SetFloor(Floor flr)
        {
            if (flr == null)
                throw new ArgumentException("floor cannot be null for a hall.");


            if(flr == _floor)
                throw new DuplicateException("Floor",  flr.ToString());
            
            flr.SetHall(this);
            
            _floor = flr;
        }
        
        //


        //attribute association Movie - Hall
        [XmlIgnore] private readonly List<Screening> _screenings = new();

        [XmlIgnore] public IReadOnlyCollection<Screening> Screenings => _screenings;

        internal void AddScreeningInternal(Screening screening)
        {
            _screenings.Add(screening);
        }

        internal void RemoveScreeningInternal(Screening screening)
        {
            _screenings.Remove(screening);
        }
        //

        //  Class extent 
        private static List<Hall> _halls = new List<Hall>();
        public static IReadOnlyList<Hall> Halls => _halls.AsReadOnly();

        private static void AddHall(Hall hall)
        {
            if (hall == null)
                throw new ArgumentException("hall cannot be null");

            _halls.Add(hall);
        }

        public void DeleteHall()
        {
            foreach (var screening in new List<Screening>(_screenings))
            {
                screening.Cancel();
            }

            _screenings.Clear();

            foreach (var seat in _seats)
            {
                seat.RemoveFromExtent();
            }

            _seats.Clear();

            if(_floor != null)
            {
                _floor.InternalRemoveHall(this);
            }


            foreach (var assigment in new List<DisplayerAssigment>(_assigments))
            {
                assigment.Cancel();
            }

            RemoveFromExtent(this);
        }
        
        internal static void RemoveFromExtent(Hall hall)
        {
            _halls.Remove(hall);
        }
        


        //  Constructors 
        public Hall()
        {
        }

        public Hall(int number) :
            base($"Hall {number}", TimeSpan.FromHours(3))
        {
            Number = number;
            AddHall(this);
        }

        public Hall(int number, Floor floor) :
            this(number)
        {
            SetFloor(floor);
            
            RegisterArea(this);
        }


        //  Methods 
        public override string ToString()
        {
            return $"Hall {Number} (Max Capacity: {MaxCapacity})";
        }

        //for tests
        public static void ClearExtent()
        {
            foreach (var hall in new List<Hall>(_halls))
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

        public List<Hall> GetExtent() => _halls;

        public void ReplaceExtent(List<Hall> newExtent)
        {
            _halls = newExtent ?? new List<Hall>();
        }
    }

}
