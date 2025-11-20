using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace CinemaManagementSystem
{
    [Serializable]
    public class Order
    {
        // ---------- Attributes ----------
        public string CardInfo { get; set; }
        public DateTime DateOfPurchase { get; set; }

        // ---------- Class extent ----------
        private static List<Order> _orders = new List<Order>();
        public static IReadOnlyList<Order> Orders => _orders.AsReadOnly();

        // ---------- Constructors ----------
        public Order() { } // XML için zorunlu

        public Order(string cardInfo)
        {
            if (string.IsNullOrWhiteSpace(cardInfo))
                throw new ArgumentException("Card information cannot be empty.");

            CardInfo = cardInfo;
            DateOfPurchase = DateTime.Now;
            _orders.Add(this);
        }

        // ---------- Methods ----------
        public override string ToString()
        {
            return $"Order made on {DateOfPurchase:dd/MM/yyyy HH:mm}, Card Info: {CardInfo}";
        }

        public static void ClearExtent()
        {
            _orders.Clear();
        }

        // ---------- Persistence ----------
        public static void Save(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Order>));
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                serializer.Serialize(fs, _orders);
            }
        }

        public static void Load(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Order file not found.");

            XmlSerializer serializer = new XmlSerializer(typeof(List<Order>));
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                _orders = (List<Order>)serializer.Deserialize(fs);
            }
        }
    }
}
