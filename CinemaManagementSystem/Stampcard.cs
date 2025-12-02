using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace CinemaManagementSystem
{
    [Serializable]
    public class Stampcard
    {
        // ---------- Backing fields ----------
        private DateTime _dateOfPurchase;
        private bool _isCompleted;
        private int _numberOfStamps;

        // ---------- Attributes (preserve original public names) ----------
        public DateTime DateOfPurchase
        {
            get => _dateOfPurchase;
            set
            {
                if (value > DateTime.Now)
                    throw new ArgumentException("Date of purchase cannot be in the future.");
                _dateOfPurchase = value;
            }
        }

        public bool IsCompleted
        {
            get => _isCompleted;
            set => _isCompleted = value;
        }

        public int NumberOfStamps
        {
            get => _numberOfStamps;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Number of stamps cannot be negative.");
                if (value > MaxStamps)
                    throw new ArgumentException($"Number of stamps cannot exceed {MaxStamps}.");
                _numberOfStamps = value;
                if (_numberOfStamps >= MaxStamps)
                    _isCompleted = true;
            }
        }

        // ---------- Class extent ----------
        private static List<Stampcard> _stampcards = new List<Stampcard>();
        public static IReadOnlyList<Stampcard> Stampcards => _stampcards.AsReadOnly();

        // ---------- Constants ----------
        private const int MaxStamps = 10;

        // Backwards-compatible clear methods
        public static void ClearAllStampcards() => _stampcards.Clear();
        public static void ClearExtent() => ClearAllStampcards();

        // ---------- Constructors ----------
        public Stampcard()
        {
            DateOfPurchase = DateTime.Now;
            IsCompleted = false;
            NumberOfStamps = 0;
            _stampcards.Add(this);
        }

        // ---------- Methods ----------
        public void AddStamp()
        {
            if (IsCompleted)
                throw new InvalidOperationException("This stamp card is already completed.");

            NumberOfStamps++; // uses setter to update IsCompleted when needed
        }

        public int CheckNumberOfStamps() => NumberOfStamps;

        public override string ToString()
        {
            return $"Stampcard - Purchased: {DateOfPurchase:dd/MM/yyyy}, Stamps: {NumberOfStamps}, Completed: {IsCompleted}";
        }

        // ---------- Persistence ----------
        public static void Save(string filePath)
        {
            var serializer = new XmlSerializer(typeof(List<Stampcard>));

            var dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                serializer.Serialize(fs, _stampcards);
                fs.Flush();
            }
        }

        public static void Load(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Stampcard file not found.");

            var serializer = new XmlSerializer(typeof(List<Stampcard>));
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var loaded = (List<Stampcard>)serializer.Deserialize(fs);
                _stampcards = loaded ?? new List<Stampcard>();
            }
        }
    }
}
