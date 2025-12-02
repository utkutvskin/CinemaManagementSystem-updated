using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace CinemaManagementSystem
{
    [Serializable]
    public class Order
    {
        //Attributes
        private CardInfo _cardInfo;
        private DateTime _dateOfPurchase;
        
        public CardInfo cardInfo
        {
            get => _cardInfo;
            set
            {
                if (value == null)
                    throw new ArgumentException("CardInfo cannot be null.");
                _cardInfo = value;
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


        //Class extent
        private static List<Order> _orders = new List<Order>();
        public static IReadOnlyList<Order> Orders => _orders.AsReadOnly();

        private void AddOrder(Order order)
        {
            if (order == null)
                throw new ArgumentException("Actor cannot be null");

            _orders.Add(order);
        }

        // Constructors
        public Order() { } 

        public Order(CardInfo cardInfo)
        {

            this.cardInfo = cardInfo;
            DateOfPurchase = DateTime.Now;
            AddOrder(this);
        }

        // Methods 
        public override string ToString()
        {
            return $"Order made on {DateOfPurchase:dd/MM/yyyy HH:mm}, Card Info: {cardInfo}";
        }

        public static void ClearExtent()
        {
            _orders.Clear();
        }

        // Persistence 
        public static void Save(string filePath)
        {
            StreamWriter sw = new StreamWriter(filePath);
            XmlSerializer serializer = new XmlSerializer(typeof(List<Order>));
            using (XmlTextWriter writer = new XmlTextWriter(sw))
            {
                serializer.Serialize(writer, _orders);
            }
        }

        public static bool Load(string filePath)
        {
            StreamReader file;
            try
            {
                file = File.OpenText(filePath);
            }
            catch (FileNotFoundException)
            {
                _orders.Clear();
                return false;
            }

            XmlSerializer serializer = new XmlSerializer(typeof(List<Order>));
            using (XmlTextReader reader = new XmlTextReader(filePath))
            {
                try
                {
                    _orders = (List<Order>)serializer.Deserialize(reader);
                }
                catch (InvalidCastException)
                {
                    _orders.Clear();
                    return false;
                }
                catch (Exception)
                {
                    _orders.Clear();
                    return false;
                }
            }

            return true;
        }
    }
}
