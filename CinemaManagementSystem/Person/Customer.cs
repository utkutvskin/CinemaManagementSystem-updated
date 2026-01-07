using System.Xml;
using System.Xml.Serialization;
using CinemaManagementSystem.AssociationClasses;
using CinemaManagementSystem.Enums;
using CinemaManagementSystem.Exceptions;
using CinemaManagementSystem.PersistenceForAllClasses;

namespace CinemaManagementSystem.Person
{
    [Serializable]
    public class Customer: Person, IExtent<Customer>
    {
        private HashSet<CardInfo>? _cards;

        public HashSet<CardInfo>? Cards
        {
            get => _cards;
            set => _cards = value;
        }

        //Class extent 
        private static List<Customer> _customers = new List<Customer>();
        public static IReadOnlyList<Customer> Customers => _customers.AsReadOnly();

        private static void AddCustomer(Customer customer)
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

       
        public Customer(string name, string surname, GenderEnum gender, DateTime birthDate)
            : base(name, surname, gender, birthDate)
        {
            AddCustomer(this);
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
        //
        
        //Qualified Association: Customer - Stampcard

        private Dictionary<DateTime, Stampcard> _stampcards = new Dictionary<DateTime, Stampcard>();
        public IReadOnlyDictionary<DateTime, Stampcard> Stampcards => _stampcards;
        
        public bool HasActiveStampcard()
        {
            foreach (var keyValuePair in _stampcards)
            {
                if(keyValuePair.Value.Status == StampCardStatus.Active || keyValuePair.Value.Status == StampCardStatus.ReadyForFreeMovie)
                    return true;
            }
            
            return false;
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
        //
        
        
        //  Methods 
        public override string ToString()
        {
            if (Cards == null || Cards.Count == 0)
                return base.ToString();
                
            string str =  base.ToString() + $"\nCards: \n";

            foreach (var card in Cards)
            {
                str += $"{card}\n";
            }
            
            return str;
        }

        public void AddNewCard(CardInfo cardInfo)
        {
            if (cardInfo == null)
                throw new ArgumentException("Card cannot be null");

            Cards ??= new HashSet<CardInfo>();
            
            if(!Cards.Add(cardInfo))
                throw new DuplicateException(cardInfo, this );
            
        }

        public void RemoveCard(string cardNumber, string PINcode)
        {
            if(cardNumber == null)
                throw new ArgumentException("Card cannot be null");

            if (Cards?.FirstOrDefault(c => c.Number == cardNumber && c.PINcode == PINcode) == null)
                throw new ExistenceException($"Card with {cardNumber}");
            
            Cards.RemoveWhere(c => c.Number == cardNumber);
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
            
            Order order = Orders[dateTimeOfCreation];
            
            if (order.DateOfPurchase != null)
                throw new OrderException(order, "modified");
            
            Orders[dateTimeOfCreation].AddTicket(screening, seat);
        }

        public void RemoveTicket(Screening screening, Seat seat, DateTime dateTimeOfCreation)
        {
            if (!Orders.ContainsKey(dateTimeOfCreation))
                throw new ExistenceException($"Order with date of purchase {dateTimeOfCreation}");
            
            Order order = Orders[dateTimeOfCreation];
            
            if (order.DateOfPurchase != null)
                throw new OrderException(order, "modified");
            
            Orders[dateTimeOfCreation].RemoveTicket(screening, seat);
        }

        public void PayForOrder(DateTime dateTimeOfCreation, CardInfo cardInfo)
        {
            if (!Orders.ContainsKey(dateTimeOfCreation))
                throw new ExistenceException($"Order with date of purchase {dateTimeOfCreation}");
            
            Order order = Orders[dateTimeOfCreation];
            
            if (order.DateOfPurchase != null)
                throw new OrderException(order, "payed");
            
            if(Cards == null || !Cards.Contains(cardInfo))
                throw new ExistenceException(cardInfo, this);
            
            Orders[dateTimeOfCreation].DateOfPurchase = DateTime.Now.Date + DateTime.Now.TimeOfDay;
            Orders[dateTimeOfCreation].cardInfo = cardInfo;
        }

        public void CancelOrder(DateTime dateTimeOfCreation)
        {
            if (!Orders.ContainsKey(dateTimeOfCreation))
                throw new ExistenceException($"Order with date of purchase {dateTimeOfCreation}");

            if (Orders[dateTimeOfCreation].DateOfPurchase != null)
                throw new OrderException(Orders[dateTimeOfCreation], "cancelled");
            
            Orders[dateTimeOfCreation].Cancel();
        }

        public void ApplyStampCardToOrder(DateTime dateTimeOfCreation)
        {
            Stampcard stamp = _stampcards.FirstOrDefault(s => s.Value.Status == StampCardStatus.Active).Value;
            
            if(stamp == null)
                throw new StampException(this, " does not contain active stampcard");
            
            if(!_orders.ContainsKey(dateTimeOfCreation))
                throw new ExistenceException($"Order with date of purchase {dateTimeOfCreation}");
            
            Order order = Orders[dateTimeOfCreation];
            if(order.DateOfPurchase != null)
                throw new StampException(stamp, order);
            
            order.ApplyStampCard(stamp);
        }

        public int CheckNumberOfStamps(DateTime dateofPurchase)
        {
            if(!_stampcards.ContainsKey(dateofPurchase))
                throw new ExistenceException($"Stamp card with date of purchase {dateofPurchase}");
            
            return _stampcards[dateofPurchase].NumberOfStamps;
        }
        
        public Stampcard RequestNewStampcard()
        {
            if(HasActiveStampcard())
                throw new StampException(this, "already has active stampcard");
            
            var stamp = Stampcard.CreateStampcard(this);
            
            return stamp;
            
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

