using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace CinemaManagementSystem
{
    [Serializable]
    public class Seat
    {
        // ---------- Backing fields ----------
        private int _number;
        private char _row;

        // ---------- Properties with validation (preserve public names) ----------
        public int Number
        {
            get => _number;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Seat number must be positive.");

                // ensure uniqueness among existing seats (excluding this instance)
                foreach (var seat in _seats)
                {
                    if (!ReferenceEquals(seat, this) && seat.Number == value && seat.Row == Row)
                        throw new ArgumentException($"Seat {Row}{value} already exists.");
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

                var upper = char.ToUpper(value);

                // ensure uniqueness among existing seats (excluding this instance)
                foreach (var seat in _seats)
                {
                    if (!ReferenceEquals(seat, this) && seat.Number == Number && seat.Row == upper)
                        throw new ArgumentException($"Seat {upper}{Number} already exists.");
                }

                _row = upper;
            }
        }

        // ---------- Class extent ----------
        private static List<Seat> _seats = new List<Seat>();
        public static IReadOnlyList<Seat> Seats => _seats.AsReadOnly();

        public static void ClearAllSeats() => _seats.Clear();

        // ---------- Constructors ----------
        public Seat() { } // XmlSerializer için gerekli

        public Seat(int number, char row)
        {
            // Use property setters so validation and normalization are applied
            Number = number;
            Row = row;

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
            var serializer = new XmlSerializer(typeof(List<Seat>));

            var dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                serializer.Serialize(fs, _seats);
                fs.Flush();
            }
        }

        public static void Load(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Seat file not found.");

            var serializer = new XmlSerializer(typeof(List<Seat>));
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var loaded = (List<Seat>)serializer.Deserialize(fs);
                _seats = loaded ?? new List<Seat>();
            }
        }
    }
}
