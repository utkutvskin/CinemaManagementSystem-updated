using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace CinemaManagementSystem
{
    [Serializable]
    public class Order
    {
        // ---------- Backing fields ----------
        private string _cardInfo;
        private DateTime _dateOfPurchase;

        // ---------- Properties with validation (preserve original public names) ----------
        public string CardInfo
        {
            get => _cardInfo;
            set
            {
                var trimmed = value?.Trim();
                if (string.IsNullOrWhiteSpace(trimmed))
                    throw new ArgumentException("Card information cannot be empty.");
                _cardInfo = trimmed;
            }
        }

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

        // ---------- Class extent ----------
        private static List<Order> _orders = new List<Order>();
        public static IReadOnlyList<Order> Orders => _orders.AsReadOnly();

        // Backwards-compatible clear methods
        public static void ClearAllOrders() => _orders.Clear();
        public static void ClearExtent() => ClearAllOrders();

        // ---------- Constructors ----------
        public Order() { } // XML serializer için gerekli

        public Order(string cardInfo)
        {
            // Use property setter for validation/normalization
            CardInfo = cardInfo;
            DateOfPurchase = DateTime.Now;

            _orders.Add(this);
        }

        // ---------- Methods ----------
        public override string ToString()
        {
            return $"Order made on {DateOfPurchase:dd/MM/yyyy HH:mm}, Card Info: {CardInfo}";
        }

        // ---------- Persistence ----------
        public static void Save(string filePath)
        {
            var serializer = new XmlSerializer(typeof(List<Order>));

            var dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                serializer.Serialize(fs, _orders);
                fs.Flush();
            }
        }

        public static void Load(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Order file not found.");

            var serializer = new XmlSerializer(typeof(List<Order>));
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var loaded = (List<Order>)serializer.Deserialize(fs);
                _orders = loaded ?? new List<Order>();
            }
        }
    }
}
