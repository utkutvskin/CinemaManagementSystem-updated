using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

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
                if (value > MaxCapacity || value <= 0)
                    throw new ArgumentException("Number can't be greater than max capacity or less than 0.");
                _number = value;
            }
        }

        [XmlIgnore]
        public static readonly int MaxCapacity = 100;

        //  Class extent 
        private static List<Hall> _halls = new List<Hall>();
        public static IReadOnlyList<Hall> Halls => _halls.AsReadOnly();
        
        private static void AddHall(Hall hall)
        {
            if (hall == null)
                throw new ArgumentException("hall cannot be null");

            _halls.Add(hall);
        }

        //  Constructors 
        public Hall() { } 

        public Hall(int number)
        {

            Number = number;
            AddHall(this);
        }

        //  Methods 
        public override string ToString()
        {
            return $"Hall {Number} (Max Capacity: {MaxCapacity})";
        }

        public static void ClearExtent()
        {
            _halls.Clear();
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
