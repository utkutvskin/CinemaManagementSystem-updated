using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using CinemaManagementSystem.AssociationClasses;
using CinemaManagementSystem.PersistenceForAllClasses;

namespace CinemaManagementSystem
{
    [Serializable]
    public class Order :IExtent<Order>
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

        
        //attribute association Ticket
        [XmlIgnore]
        private readonly List<Ticket> _tickets = new();

        [XmlIgnore]
        public IReadOnlyCollection<Ticket> Tickets => _tickets.AsReadOnly();

        internal void AddTicketInternal(Ticket ticket)
        {
            if (ticket == null)
                throw new ArgumentException("Ticket cannot be null.");
            _tickets.Add(ticket);
        }

        internal void RemoveTicketInternal(Ticket ticket)
        {
            if (ticket == null)
                throw new ArgumentException("Ticket cannot be null.");
            _tickets.Remove(ticket);
        }

        public Ticket AddTicket(Screening screening, Seat seat, double price)
        {
            return Ticket.CreateTicket(price, screening, this, seat);
        }
        
        
        //association customer
        [XmlIgnore]
        private Customer _customer;

        [XmlIgnore]
        public Customer Customer
        {
            get => _customer;
            set => _customer = value;
        }

        public void AddCustomer(Customer customer)
        {
            if (Customer == customer)
                return;
            
            if (Customer != null)
            {
                var old = Customer;
                Customer = null;
                old.ForceRemoveOrder(this);
            }

            Customer = customer;

            if (customer != null)
            {
                customer.ForceAddOrder(this);
            }
        }
        public static Order Create(Customer customer, CardInfo cardInfo)
        {
            if (customer == null) throw new ArgumentNullException(nameof(customer));
            var order = new Order(cardInfo);
            order.AddCustomer(customer);    
            return order;
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

        public List<Order> GetExtent() => _orders;

        public void ReplaceExtent(List<Order> newExtent)
        {
            _orders = newExtent ?? new List<Order>();
        }
    }
}
