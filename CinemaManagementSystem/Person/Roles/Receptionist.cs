using System.Xml.Serialization;
using CinemaManagementSystem.AssociationClasses;
using CinemaManagementSystem.Enums;
using CinemaManagementSystem.Exceptions;

namespace CinemaManagementSystem.Person.Roles
{
    [Serializable]
    public class Receptionist : EmployeeRole
    {
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

        public Receptionist(int deskNumber, Employee employee) :base(employee)
        {
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
                throw new OrderException(Orders[dateTimeOfCreation], "canceled");
            
            Orders[dateTimeOfCreation].Cancel();
        }

        public void ApplyCustomerStampCardToOrder(DateTime dateTimeOfCreation, Customer customer)
        {
            
            if(customer == null)
                throw new ArgumentNullException(nameof(customer));
            
            customer.ApplyStampCardToOrder(dateTimeOfCreation);
        }
    }
}