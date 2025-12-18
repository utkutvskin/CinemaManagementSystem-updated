using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using CinemaManagementSystem.AssociationClasses;
using CinemaManagementSystem.Enums;
using CinemaManagementSystem.Exceptions;
using CinemaManagementSystem.PersistenceForAllClasses;

namespace CinemaManagementSystem
{
    [Serializable]
    public class Customer: IExtent<Customer>
    {
        //Attributes
        private string _name;
        private string _surname;
        private DateTime _dateOfBirth;
        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Name cannot be empty.");
                _name = value;
            }
        }

        public string Surname
        {
            get => _surname;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Surname cannot be empty.");
                _surname = value;
            }
        }

        public DateTime DateOfBirth
        {
            get => _dateOfBirth;
            set
            {
                if(value > DateTime.Now.AddYears(-16))
                    throw new ArgumentException("You must be older than 16 years old");
                _dateOfBirth = value;
            }
        }  

        
        //Derived
        [XmlIgnore]
        public int Age
        {
            get
            {
                int age = DateTime.Now.Year - DateOfBirth.Year;
                if (DateTime.Now.DayOfYear < DateOfBirth.DayOfYear)
                    age--;
                return age;
            }
        }

        //Class extent 
        private static List<Customer> _customers = new List<Customer>();
        public static IReadOnlyList<Customer> Customers => _customers.AsReadOnly();

        private void AddCustomer(Customer customer)
        {
            if (customer == null)
                throw new ArgumentException("Actor cannot be null");
            
            _customers.Add(customer);
        }

        
        public static void ClearAllCustomers()
        {
            _customers.Clear();
        }

        //  Constructors 
        public Customer() { }  

       
        public Customer(string name, string surname, DateTime birthDate)
        {

            Name = name;
            Surname = surname;
            DateOfBirth = birthDate;

            AddCustomer(this);
        }

        //  Methods 
        public override string ToString()
        {
            return $"{Name} {Surname}, Age: {Age}";
        }
        
        
        //Order association
        [XmlIgnore]
        private Dictionary<DateTime, Order> _orders = new();
        [XmlIgnore]
        public IReadOnlyDictionary<DateTime, Order> Orders => _orders.AsReadOnly();


        internal void AddOrderInternal(Order order)
        { 
            if (order == null) 
                throw new ArgumentNullException(nameof(order));
            if (_orders.ContainsKey(order.DateTimeOfCreation)) 
                throw new DuplicateException( order, this);

            _orders.Add(order.DateTimeOfCreation, order);
        }

        internal void RemoveOrderInternal(Order order)
        {
            if (order == null) 
                throw new ArgumentNullException(nameof(order));
            
            if (!_orders.ContainsKey(order.DateTimeOfCreation)) 
                throw new ExistenceException(order, this);
            
            _orders.Remove(order.DateTimeOfCreation);
        }

        public void CreateOrder(Screening screening, Seat seat)
        {
            Order.Create(this, screening, seat);
        }

        public void RemoveOrder(DateTime dateTimeOfCreation)
        {
            Order.RemoveOrder(this, dateTimeOfCreation);
        }

        public void ChooseNewTicket(Screening screening, Seat seat, DateTime dateTimeOfCreation)
        {
            if (!Orders.ContainsKey(dateTimeOfCreation))
                throw new ExistenceException($"Order with date of purchase {dateTimeOfCreation}");
            
            Orders[dateTimeOfCreation].AddTicket(screening, seat);
        }

        public void RemoveTicket(Screening screening, Seat seat, DateTime dateTimeOfCreation)
        {
            if (!Orders.ContainsKey(dateTimeOfCreation))
                throw new ExistenceException($"Order with date of purchase {dateTimeOfCreation}");
            
            Orders[dateTimeOfCreation].RemoveTicket(screening, seat);
        }

        public void PayForOrder(DateTime dateTimeOfCreation, CardInfo cardInfo)
        {
            if (!Orders.ContainsKey(dateTimeOfCreation))
                throw new ExistenceException($"Order with date of purchase {dateTimeOfCreation}");
            
            Orders[dateTimeOfCreation].DateOfPurchase = DateTime.Now.Date + DateTime.Now.TimeOfDay;
            Orders[dateTimeOfCreation].cardInfo = cardInfo;
        }

        public void CancelOrder(DateTime dateTimeOfCreation)
        {
            if (!Orders.ContainsKey(dateTimeOfCreation))
                throw new ExistenceException($"Order with date of purchase {dateTimeOfCreation}");

            if (Orders[dateTimeOfCreation].DateOfPurchase != null)
                throw new CancelOrderException(Orders[dateTimeOfCreation]);
            
            Orders[dateTimeOfCreation].Cancel();
        }

        public void ApplyStampCardToOrder(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));
            
            Stampcard stamp = _stampcards.FirstOrDefault(s => s.Value.Status == StampCardStatus.Active).Value;
            
            if(stamp == null)
                throw new StampException(this, "doesn't have active stampcard");
            
            if(!_orders.ContainsKey(order.DateTimeOfCreation))
                throw new ExistenceException(order, this);
            
            if(order.DateOfPurchase != null)
                throw new StampException(stamp, order);
            
            order.ApplyStampCard(stamp);

        }

        //Qualified Association: Customer - Stampcard

        private Dictionary<DateTime, Stampcard> _stampcards = new Dictionary<DateTime, Stampcard>();
        public IReadOnlyDictionary<DateTime, Stampcard> Stampcards => _stampcards;
        
        public bool HasActiveStampcard()
        {
            foreach (var keyValuePair in _stampcards)
            {
                if(keyValuePair.Value.Status == StampCardStatus.Active)
                    return true;
            }
            
            return false;
        }

        
        public Stampcard RequestNewStampcard()
        {
            if(HasActiveStampcard())
                throw new StampException(this, "already has active stampcard");
            
            var stamp = Stampcard.CreateStampcard(this);
            
            return stamp;
            
        }

        internal void SetStampcardInternal(Stampcard stampcard)
        {
            if (stampcard == null) 
                throw new ArgumentNullException(nameof(stampcard));
            

            _stampcards.Add(stampcard.DateOfPurchase, stampcard);
        }

       
        
        
        internal void RemoveStampcardInternal(Stampcard card)
        {
            if (card == null)
                throw new ArgumentNullException(nameof(card));

            DateTime key = card.DateOfPurchase.Date;

            if (!_stampcards.ContainsKey(key))
                throw new ExistenceException(card, this);
            
            _stampcards.Remove(key);
            
        }
        
        
        // Persistence 
        public static void Save(string filePath)
        {
            StreamWriter sw = File.CreateText(filePath);
            XmlSerializer serializer = new XmlSerializer(typeof(List<Customer>));
            using (XmlTextWriter writer = new XmlTextWriter(sw))
            {
                serializer.Serialize(writer, _customers);
            }
        }

        public static void Load(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Customer file not found.");

            XmlSerializer serializer = new XmlSerializer(typeof(List<Customer>));
            using (StreamReader reader = new StreamReader(filePath))
            {
                var loaded = (List<Customer>)serializer.Deserialize(reader);
                _customers = loaded ?? new List<Customer>();
            }
        }

        public List<Customer> GetExtent() => _customers;

        public void ReplaceExtent(List<Customer> newExtent)
        {
            _customers = newExtent ?? new List<Customer>();
        }
    }

}

