using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using CinemaManagementSystem.AssociationClasses;
using CinemaManagementSystem.Exceptions;
using CinemaManagementSystem.PersistenceForAllClasses;

namespace CinemaManagementSystem
{
    [Serializable]
    public class Seat : IExtent<Seat>
    {
        //  Attributes 
        private int _number;
        private char _row;

        public int Number
        {
            get => _number;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Seat number must be positive.");
                
                _number = value;
            }
        }

        public char Row
        {
            get => _row;
            set
            {
                if (!char.IsLetter(value))
                    throw new ArgumentException("Row must be a letter (A-Z).");
                
                _row = char.ToUpper(value);
            }
        }
        
        
        
        //composition association
        [XmlIgnore]           
        private Hall _hall;

        [XmlIgnore]
        public Hall Hall => _hall;

        private void SetHall(Hall hall)
        {
            if (hall == null)
                throw new ArgumentException("Hall cannot be null for a seat.");
            
            if(hall == _hall)
                throw new DuplicateException( hall, this);
            
            hall.SetSeat(this);
            
            _hall = hall;
        }
        
        internal void RemoveFromExtent()
        {
            foreach (var ticket in new List<Ticket>(_tickets))
                ticket.Cancel();
            
            _seats.Remove(this);
        }

        //
        
        //basic association Ticket
        [XmlIgnore]
        private readonly HashSet<Ticket> _tickets = new HashSet<Ticket>();

        [XmlIgnore]
        public IReadOnlyCollection<Ticket> Tickets => _tickets;

        internal void SetTicket(Ticket ticket)
        {
            if (ticket == null)
                throw new ArgumentException("Ticket cannot be null.");
            _tickets.Add(ticket);
        }

        internal void RemoveTicket(Ticket ticket)
        {
            if (ticket == null)
                throw new ArgumentException("Ticket cannot be null.");
            _tickets.Remove(ticket);
        }


        //  Class extent 
        private static List<Seat> _seats = new List<Seat>();
        public static IReadOnlyList<Seat> Seats => _seats.AsReadOnly();
        
        private static void AddSeat(Seat seat)
        {
            if (seat == null)
                throw new ArgumentException("movie cannot be null");

            _seats.Add(seat);
        }
        
        

        //  Constructors 
        public Seat() { }

        //only for tests
        public Seat(int number, char row)
        {

            Number = number;
            Row = char.ToUpper(row);

            AddSeat(this);
        }
        
        public Seat(int number, char row, Hall hall) : this(number, row)
        {
            SetHall(hall);
        }

        
        
        //  Methods 
        public override string ToString()
        {
            return $"Seat {Row}{Number}";
        }

        
        
        
        //  Persistence 
        public static void Save(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Seat>));
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, _seats);
            }
        }

        public static void Load(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Seat file not found.");

            XmlSerializer serializer = new XmlSerializer(typeof(List<Seat>));
            using (StreamReader reader = new StreamReader(filePath))
            {
                var loaded = (List<Seat>)serializer.Deserialize(reader);
                _seats = loaded ?? new List<Seat>();
            }
        }
        
        //for tests only
        public static void ClearAllSeatsForTesting()
        {
            foreach (var hall in Hall.Halls)
                hall.InternalClearSeats();

            _seats.Clear();
            
        }

        public List<Seat> GetExtent() => _seats;

        public void ReplaceExtent(List<Seat> newExtent)
        {
            _seats = newExtent ?? new List<Seat>();
        }
    }
}