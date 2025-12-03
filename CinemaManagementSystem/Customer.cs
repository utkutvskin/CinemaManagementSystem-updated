using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace CinemaManagementSystem
{
    [Serializable]
    public class Customer
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

        // ---------- Qualified Association: Customer → Stampcard ----------

        private Dictionary<DateTime, Stampcard> _stampcards = new Dictionary<DateTime, Stampcard>();
        public IReadOnlyDictionary<DateTime, Stampcard> Stampcards => _stampcards;

        public bool HasStampcard(DateTime date) => _stampcards.ContainsKey(date.Date);

        public void AddStampcard(Stampcard card)
        {
            if (card == null)
                throw new ArgumentNullException(nameof(card));

            // Customer can hold only ONE active stampcard
            if (_stampcards.Count >= 1)
                throw new InvalidOperationException("Customer cannot have more than one active stampcard at a time.");

            DateTime key = card.DateOfPurchase.Date;

            if (_stampcards.ContainsKey(key))
                throw new InvalidOperationException("A stampcard with this purchase date already exists for this customer.");

            _stampcards[key] = card;

            // Reverse connection
            if (card.Customer != this)
                card.SetCustomer(this);
        }
        internal void ForceAddStampcard(Stampcard card)
        {
            DateTime key = card.DateOfPurchase.Date;
            _stampcards[key] = card;
        }

        
        
        public void RemoveStampcard(Stampcard card)
        {
            if (card == null)
                throw new ArgumentNullException(nameof(card));

            DateTime key = card.DateOfPurchase.Date;

            if (!_stampcards.ContainsKey(key))
                throw new InvalidOperationException("This stampcard is not associated with this customer.");

            _stampcards.Remove(key);

            if (card.Customer == this)
                card.SetCustomer(null);
        }

        internal void ForceRemoveStampcard(Stampcard card)
        {
            DateTime key = card.DateOfPurchase.Date;
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
    }

}
