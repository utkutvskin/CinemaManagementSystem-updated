using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace CinemaManagementSystem
{
    [Serializable]
    public class Hall
    {
        // ---------- Attributes ----------
        public int Number { get; set; }

        [XmlIgnore]
        public static readonly int MaxCapacity = 100;

        // ---------- Class extent ----------
        private static List<Hall> _halls = new List<Hall>();
        public static IReadOnlyList<Hall> Halls => _halls.AsReadOnly();

        // ---------- Constructors ----------
        public Hall() { } // XML serialization için gerekli

        public Hall(int number)
        {
            if (number <= 0)
                throw new ArgumentException("Hall number must be positive.");

            foreach (var hall in _halls)
            {
                if (hall.Number == number)
                    throw new ArgumentException($"Hall with number {number} already exists.");
            }

            Number = number;
            _halls.Add(this);
        }

        // ---------- Methods ----------
        public override string ToString()
        {
            return $"Hall {Number} (Max Capacity: {MaxCapacity})";
        }

        public static void ClearExtent()
        {
            _halls.Clear();
        }

        // ---------- Persistence ----------
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
