using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

namespace CinemaManagementSystem
{
    [Serializable]
    public class Seat
    {
        // ---------- Attributes ----------
        public int Number { get; set; }
        public char Row { get; set; }

        // ---------- Class extent ----------
        private static List<Seat> _seats = new List<Seat>();
        public static IReadOnlyList<Seat> Seats => _seats.AsReadOnly();

        
        public static void ClearAllSeats()
        {
            _seats.Clear();
        }

        // ---------- Constructors ----------
        public Seat() { } // XmlSerializer için gerekli

        public Seat(int number, char row)
        {
            if (number <= 0)
                throw new ArgumentException("Seat number must be positive.");

            if (!char.IsLetter(row))
                throw new ArgumentException("Row must be a letter (A-Z).");

            foreach (var seat in _seats)
            {
                if (seat.Number == number && seat.Row == row)
                    throw new ArgumentException($"Seat {row}{number} already exists.");
            }

            Number = number;
            Row = char.ToUpper(row);

            _seats.Add(this);
        }

        // ---------- Methods ----------
        public override string ToString()
        {
            return $"Seat {Row}{Number}";
        }

        // ---------- Persistence ----------
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