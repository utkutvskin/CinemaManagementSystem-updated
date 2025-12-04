using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using CinemaManagementSystem.PersistenceForAllClasses;

namespace CinemaManagementSystem
{
    [Serializable]
    public class Stampcard :IExtent<Stampcard>
    {
        //  Attributes 
        private DateTime _dateOfPurchase;
        private int _numberOfStamps;
        
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
        
        [XmlIgnore]
        public bool IsCompleted => NumberOfStamps >= MaxStamps;

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
        public Stampcard()
        {
            DateOfPurchase = DateTime.Now;
            NumberOfStamps = 0;
            AddStampcard(this);
        }

        //  Methods 
        public void AddStamp()
        {
            if (IsCompleted)
                throw new InvalidOperationException("This stamp card is already completed.");

            NumberOfStamps++;
        }

        public static void ClearExtent() => _stampcards.Clear();

        public override string ToString()
        {
            return $"Stampcard - Purchased: {DateOfPurchase:dd/MM/yyyy}, Stamps: {NumberOfStamps}, Completed: {IsCompleted}";
        }

     // ---------- Association: Stampcard → Customer (passive side) ----------

        public Customer Customer { get; private set; }
        public void SetCustomer(Customer customer)
        {
            if (Customer == customer)
                return;

            if (Customer != null)
            {
                var old = Customer;
                Customer = null;
                old.ForceRemoveStampcard(this);
            }

            Customer = customer;

            if (customer != null)
                customer.ForceAddStampcard(this);
        }

        public void RemoveCustomer()
        {
            if (Customer == null)
                throw new InvalidOperationException("This stampcard has no customer to remove.");

            var old = Customer;
            Customer = null;
            old.ForceRemoveStampcard(this);
        }

        
        
        
        
        
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

