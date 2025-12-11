using System.Reflection.Metadata;
using System.Xml.Serialization;
using CinemaManagementSystem.Exceptions;

namespace CinemaManagementSystem.Employees
{
    [Serializable]
    public class Receptionist : Employee
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

        public Receptionist(int deskNumber)
        {
            DeskNumber = deskNumber;
        }

        public Receptionist(string name, string surname, DateTime birthDate,
            DateTime startDate, double salary, int deskNumber)
            : base(name, surname, birthDate, startDate, salary)
        {
            DeskNumber = deskNumber;
        }

        [XmlIgnore]
        private readonly List<Order> _orders = new();
        [XmlIgnore]
        public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();
        
        internal void AddOrder(Order order)
        { 
            if (order == null) 
                throw new ArgumentNullException(nameof(order));
            if (_orders.Contains(order)) 
                throw new DuplicateException("Order", order.ToString());

            _orders.Add(order);
        }

        internal void RemoveOrder(Order order)
        {
            if (order == null) 
                throw new ArgumentNullException(nameof(order));
            if (!_orders.Contains(order)) 
                throw new ExistenceException("Order", order.ToString(), "Customer");
            
            _orders.Remove(order);
        }
        
    }
}