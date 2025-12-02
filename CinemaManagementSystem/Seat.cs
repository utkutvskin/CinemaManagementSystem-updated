using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

namespace CinemaManagementSystem
{
    [Serializable]
    public class Seat
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
                
                foreach (var seat in _seats)
                {
                    if (seat.Number == value && seat.Row == _row)
                        throw new ArgumentException($"Seat {_row}{value} already exists.");
                }
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

                foreach (var seat in _seats)
                {
                    if (seat.Number == _number && seat.Row == char.ToUpper(value))
                        throw new ArgumentException($"Seat {char.ToUpper(value)}{_number} already exists.");
                }
                
                _row = char.ToUpper(value);
            }
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

        
        public static void ClearAllSeats()
        {
            _seats.Clear();
        }

        //  Constructors 
        public Seat() { }

        public Seat(int number, char row)
        {

            Number = number;
            Row = char.ToUpper(row);

            AddSeat(this);
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
    }
}