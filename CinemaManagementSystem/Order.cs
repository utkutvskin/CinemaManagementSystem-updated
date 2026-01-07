using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using CinemaManagementSystem.AssociationClasses;
using CinemaManagementSystem.Employees;
using CinemaManagementSystem.Enums;
using CinemaManagementSystem.Exceptions;
using CinemaManagementSystem.PersistenceForAllClasses;
using CinemaManagementSystem.Person;
using CinemaManagementSystem.Person.Roles;

namespace CinemaManagementSystem
{
    [Serializable]
    public class Order : IExtent<Order>
    {
        //Attributes
        private CardInfo? _cardInfo;
        private DateTime? _dateOfPurchase;
        
        private DateTime _dateTimeOfCreation;

        public CardInfo? cardInfo
        {
            get => _cardInfo;
            set => _cardInfo = value;
        }

        public DateTime? DateOfPurchase
        {
            get => _dateOfPurchase;
            set
            {
                if (value > DateTime.Now)
                    throw new ArgumentException("Date of purchase cannot be in the future.");
                _dateOfPurchase = value;
            }
        }

        public DateTime DateTimeOfCreation
        {
            get => _dateTimeOfCreation;
            set
            {
                if (value > DateTime.Now)
                    throw new ArgumentException("Date of creation cannot be in the future.");
                _dateTimeOfCreation = value;
            }
        }

        //Derived
        [XmlIgnore]
        public double FinalPrice
        {
            get
            {
                double finalPrice = 0;

                if (Stampcard != null && Stampcard.Status == StampCardStatus.ReadyForFreeMovie)
                    return finalPrice;
                
                foreach (var ticket in Ticket.Tickets)
                {
                    finalPrice += ticket.Screening.Movie.Price;
                }

                if(Customer != null )
                    finalPrice += (Tickets.Count * Ticket.FeeForOnlinePurchase);
                
                return finalPrice;
            }
        }
        

        //attribute association Ticket
        [XmlIgnore] private readonly List<Ticket> _tickets = new();

        [XmlIgnore] public IReadOnlyCollection<Ticket> Tickets => _tickets.AsReadOnly();

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


        public void AddTicket(Screening screening, Seat seat)
        {
            if (screening == null)
                throw new ArgumentException("Screening cannot be null.");
            if (seat == null)
                throw new ArgumentException("Seat cannot be null.");

            if (!screening.Hall.Seats.Contains(seat))
                throw new ExistenceException(seat, screening);

            bool occupied = Ticket.Tickets.Any(t => t.Screening == screening && t.Seat == seat);

            if (occupied)
                throw new InvalidOperationException($"Seat {seat} is already taken for this screening.");

            Ticket.CreateTicket(screening, this, seat);

        }

        public void RemoveTicket(Screening screening, Seat seat)
        {
            if (screening == null)
                throw new ArgumentException("Screening cannot be null.");
            if (seat == null)
                throw new ArgumentException("Seat cannot be null.");
            
            if (_tickets.Count <= 1)
                throw new MultiplicityException();
            
            Ticket.RemoveTicket(screening, this, seat);
        }

        [XmlIgnore]
        private Customer? _customer;

        [XmlIgnore] public Customer? Customer => _customer;
        
        [XmlIgnore]
        private Receptionist? _receptionist;

        [XmlIgnore] public Receptionist? Receptionist => _receptionist;

        internal static Order Create(Customer customer, Screening screening, Seat seat)
        {
            if (customer == null) 
                throw new ArgumentException("Customer cannot be null.");
            if(screening == null)
                throw new ArgumentException("Screening cannot be null.");
            if (seat == null)
                throw new ArgumentException("Seat cannot be null."); 
            
            
            if (!screening.Hall.Seats.Contains(seat))
                throw new ExistenceException( seat, screening);

            bool occupied = Ticket.Tickets.Any(t => t.Screening == screening && t.Seat == seat);
        
            if (occupied)
                throw new InvalidOperationException($"Seat {seat} is already taken for this screening.");
            
            var order = new Order( customer, screening, seat);

            customer.AddOrderInternal(order);
            return order;
        }

        internal static Order Create(Receptionist receptionist, Screening screening, Seat seat)
        {
            if (receptionist == null) 
                throw new ArgumentException("receptionist cannot be null.");
            if(screening == null)
                throw new ArgumentException("Screening cannot be null.");
            if (seat == null)
                throw new ArgumentException("Seat cannot be null."); 
            
            if (!screening.Hall.Seats.Contains(seat))
                throw new ExistenceException( seat, seat);

            bool occupied = Ticket.Tickets.Any(t => t.Screening == screening && t.Seat == seat);
        
            if (occupied)
                throw new InvalidOperationException($"Seat {seat} is already taken for this screening.");
            
            var order = new Order(receptionist, screening, seat);

            receptionist.AddOrderInternal(order);
            return order;
        }
        
        internal static void RemoveOrder(Customer customer, DateTime dateTimeOfCreation)
        {
            if (customer == null) 
                throw new ArgumentException("Customer cannot be null.");
            
            Order? order = _orders.FirstOrDefault(o => o.Customer == customer && o.DateTimeOfCreation == dateTimeOfCreation);
            
            if (order == null)
                throw new ExistenceException("Order");
            
            order.Cancel();
        }
        
        internal static void RemoveOrder(Receptionist receptionist, DateTime dateTimeOfCreation)
        {
            if (receptionist == null) 
                throw new ArgumentException("receptionist cannot be null.");
            
            Order? order = _orders.FirstOrDefault(o => o.Receptionist == receptionist && o.DateTimeOfCreation == dateTimeOfCreation);
            
            if (order == null)
                throw new ExistenceException("Order");
            
            order.Cancel();
        }

        public void Cancel()
        {
            _orders.Remove(this);
            foreach (var ticket in new List<Ticket>(_tickets))
            {
                ticket.Cancel();
            }
            _customer?.RemoveOrderInternal(this);
            _receptionist?.RemoveOrderInternal(this);
        }
        
        //StampCard -Order
        [XmlIgnore] public Stampcard? _stampcard;
        [XmlIgnore] public Stampcard? Stampcard => _stampcard;

        internal void ApplyStampCard(Stampcard stampcard)
        {

            if (Stampcard == stampcard)
                return;
            
            stampcard.AddOrder(this);
            
            DateOfPurchase = DateTime.Now.Date + DateTime.Now.TimeOfDay;
            
            _stampcard = stampcard;
            
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

        public void ClearExtents()
        {
            foreach (var o in new List<Order>(_orders))
            {
                o.Cancel();
            }
        }

        // Constructors
        public Order() { } 
        

        
        private Order(Customer customer, Screening screening, Seat seat)
        {
            DateTimeOfCreation = DateTime.Now.Date + DateTime.Now.TimeOfDay;
            
            _customer = customer;
            
            Ticket.CreateTicket(screening, this, seat);
             
            AddOrder(this);
        }
        
        private Order( Receptionist receptionist, Screening screening, Seat seat)
        {
            DateTimeOfCreation = DateTime.Now.Date + DateTime.Now.TimeOfDay;
            
            _receptionist = receptionist;
            
            Ticket.CreateTicket(screening, this, seat);
             
            AddOrder(this);
        }

        // Methods 
        public override string ToString()
        {
            return $"Order made on {DateTimeOfCreation:dd/MM/yyyy HH:mm}, Card Info: {cardInfo}";
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

