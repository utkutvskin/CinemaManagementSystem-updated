using System.Reflection.Metadata;
using System.Xml.Serialization;
using CinemaManagementSystem.AssociationClasses;
using CinemaManagementSystem.Exceptions;

namespace CinemaManagementSystem.Employees
{
    [Serializable]
    public class Receptionist : IReceptionist
    {
        internal Employee employee { get; }
        private int _deskNumber;

        public int DeskNumber
        {
            get => _deskNumber;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Desk number must be positive.");
                _deskNumber = value;
            }
        }

        public Receptionist() { }

        internal Receptionist(Employee employee, int deskNumber)
        {
            this.employee = employee ?? throw new ArgumentNullException(nameof(employee));
            DeskNumber = deskNumber;
        }

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

        public void SellTicketPayedByCard(DateTime dateTimeOfCreation, CardInfo cardInfo)
        {
            if (!Orders.ContainsKey(dateTimeOfCreation))
                throw new ExistenceException($"Order with date of purchase {dateTimeOfCreation}");
            
            Orders[dateTimeOfCreation].DateOfPurchase = DateTime.Now.Date + DateTime.Now.TimeOfDay;
            Orders[dateTimeOfCreation].cardInfo = cardInfo;
        }
        
        public void SellTicketPayedByCash(DateTime dateTimeOfCreation)
        {
            if (!Orders.ContainsKey(dateTimeOfCreation))
                throw new ExistenceException($"Order with date of purchase {dateTimeOfCreation}");
            
            Orders[dateTimeOfCreation].DateOfPurchase = DateTime.Now.Date + DateTime.Now.TimeOfDay;
        }

        public void CancelOrder(DateTime dateTimeOfCreation)
        {
            if (!Orders.ContainsKey(dateTimeOfCreation))
                throw new ExistenceException($"Order with date of purchase {dateTimeOfCreation}");

            if (Orders[dateTimeOfCreation].DateOfPurchase != null)
                throw new CancelOrderException(Orders[dateTimeOfCreation]);
            
            Orders[dateTimeOfCreation].Cancel();
        }

        public void ApplyCustomerStampCardToOrder(Order order, Customer customer)
        {
            if(order == null)
                throw new ArgumentNullException(nameof(order));
            
            if(customer == null)
                throw new ArgumentNullException(nameof(customer));
            
            customer.ApplyStampCardToOrder(order);
        }
    }
}