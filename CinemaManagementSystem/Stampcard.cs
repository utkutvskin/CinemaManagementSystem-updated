using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using CinemaManagementSystem.Enums;
using CinemaManagementSystem.Exceptions;
using CinemaManagementSystem.People;
using CinemaManagementSystem.PersistenceForAllClasses;

namespace CinemaManagementSystem
{
    [Serializable]
    public class Stampcard :IExtent<Stampcard>
    {
        //  Attributes 
        private DateTime _dateOfPurchase;
        private int _numberOfStamps;
        private StampCardStatus _status;
        
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
            }
        }

        public StampCardStatus Status
        {
            get => _status;
            set => _status = value;
        }
        
        //  Class extent 
        private static List<Stampcard> _stampcards = new List<Stampcard>();
        public static IReadOnlyList<Stampcard> Stampcards => _stampcards.AsReadOnly();

        private static void AddStampcard(Stampcard stampcard)
        {
            if (stampcard == null)
                throw new ArgumentException("stampcard cannot be null");

            _stampcards.Add(stampcard);
            
        }
        //  Constants 
        private const int MaxStamps = 4;

        //  Constructors 
        public Stampcard(){}
        
        private Stampcard(Customer customer)
        {
            DateOfPurchase = DateTime.Now;
            NumberOfStamps = 0;
            Status = StampCardStatus.Active;
            
            _customer = customer;
            
            AddStampcard(this);
        }

        //  Methods 
        public void AddStamp()
        {
            if (Status == StampCardStatus.Completed)
                throw new StampException(this);

            NumberOfStamps++;
        }

        public override string ToString()
        {
            return $"Stampcard - Purchased: {DateOfPurchase:dd/MM/yyyy}, Stamps: {NumberOfStamps}, Status: {Status}";
        }

       //Association: Stampcard - Customer 

       [XmlIgnore] private Customer _customer;
       [XmlIgnore] public  Customer Customer  => _customer;

       internal static Stampcard CreateStampcard(Customer customer)
       {
           if(customer == null)
               throw new ArgumentException("Customer cannot be null");
           
           var stampCard = new Stampcard(customer);
           
           customer.SetStampcardInternal(stampCard);
           
           return stampCard;
       }
       
       public static void RemoveStampcard(Customer customer, DateTime DateOfPurchase)
       {
           if(customer == null)
               throw new ArgumentException("Customer cannot be null");

           Stampcard? stampCard = _stampcards.Find(x => x.DateOfPurchase == DateOfPurchase
                                && x.Customer == customer);
           
           if (stampCard == null)
               throw new ExistenceException($"Stampcard with date of purchase {DateOfPurchase}");
           
           _stampcards.Remove(stampCard);
           
           customer.RemoveStampcardInternal(stampCard);
           
       }
       
       //
       
       //Stampcard - Order
       
       [XmlIgnore]
       private readonly HashSet<Order> _orders = new HashSet<Order>();

       [XmlIgnore]
       public IReadOnlyCollection<Order> Orders => _orders;

       internal void AddOrder(Order order)
       {
           if (_orders.Contains(order))
               return;

           if (Orders.Count == 5)
               throw new MultiplicityException();
           
           NumberOfStamps++;
           
           if (NumberOfStamps == MaxStamps)
               Status = StampCardStatus.ReadyForFreeMovie;
           
           if (NumberOfStamps == 5)
               Status = StampCardStatus.Completed;

           order.ApplyStampCard(this);
           
           _orders.Add(order);
           
       }
       
       //

       //  Persistence 
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

        public List<Stampcard> GetExtent() => _stampcards;

        public void ReplaceExtent(List<Stampcard> newExtent)
        {
            _stampcards = newExtent ?? new List<Stampcard>();
        }
    }
}



