using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace CinemaManagementSystem
{
    [Serializable]
    public class Stampcard
    {
        // ---------- Attributes ----------
        public DateTime DateOfPurchase { get; set; } // public set; XML için gerekli
        public bool IsCompleted { get; set; }
        public int NumberOfStamps { get; set; }

        // ---------- Class extent ----------
        private static List<Stampcard> _stampcards = new List<Stampcard>();
        public static IReadOnlyList<Stampcard> Stampcards => _stampcards.AsReadOnly();

        // ---------- Constants ----------
        private const int MaxStamps = 10;

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

            NumberOfStamps++;

            if (NumberOfStamps >= MaxStamps)
                IsCompleted = true;
        }

        public int CheckNumberOfStamps() => NumberOfStamps;

        public static void ClearExtent() => _stampcards.Clear();

        public override string ToString()
        {
            return $"Stampcard - Purchased: {DateOfPurchase:dd/MM/yyyy}, Stamps: {NumberOfStamps}, Completed: {IsCompleted}";
        }

        // ---------- Persistence ----------
        public static void Save(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Stampcard>));
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                serializer.Serialize(fs, _stampcards);
            }
        }

        public static void Load(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Stampcard file not found.");

            XmlSerializer serializer = new XmlSerializer(typeof(List<Stampcard>));
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                _stampcards = (List<Stampcard>)serializer.Deserialize(fs);
            }
        }
    }
}
