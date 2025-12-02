using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace CinemaManagementSystem
{
    [Serializable]
    public class Hall
    {
        // ---------- Backing fields ----------
        private int _number;

        // ---------- Attributes (preserve original public names) ----------
        public int Number
        {
            get => _number;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Hall number must be positive.");

                // ensure uniqueness among existing halls
                foreach (var hall in _halls)
                {
                    if (!ReferenceEquals(hall, this) && hall.Number == value)
                        throw new ArgumentException($"Hall with number {value} already exists.");
                }

                _number = value;
            }
        }

        
        public static readonly int MaxCapacity = 100;

        // ---------- Class extent ----------
        private static List<Hall> _halls = new List<Hall>();
        public static IReadOnlyList<Hall> Halls => _halls.AsReadOnly();

        // Backwards-compatible clear method names (tests expect ClearAllHalls in other files)
        public static void ClearAllHalls() => _halls.Clear();

        // Keep original ClearExtent name as an alias for compatibility
        public static void ClearExtent() => ClearAllHalls();

        // ---------- Constructors ----------
        public Hall() { } // XML serialization için gerekli

        public Hall(int number)
        {
            // Use property setter so validation/uniqueness is applied
            Number = number;
            _halls.Add(this);
        }

        // ---------- Methods ----------
        public override string ToString()
        {
            return $"Hall {Number} (Max Capacity: {MaxCapacity})";
        }

        // ---------- Persistence ----------
        public static void Save(string filePath)
        {
            var serializer = new XmlSerializer(typeof(List<Hall>));

            // Ensure directory exists
            var dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                serializer.Serialize(fs, _halls);
                fs.Flush(); // ensure file fully written
            }
        }

        public static void Load(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Hall file not found.");

            var serializer = new XmlSerializer(typeof(List<Hall>));

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var loaded = (List<Hall>)serializer.Deserialize(fs);
                _halls = loaded ?? new List<Hall>();
            }
        }
    }
}
