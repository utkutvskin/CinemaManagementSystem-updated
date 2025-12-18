using System;
using System. Xml.Serialization;
using CinemaManagementSystem.Employees;

namespace CinemaManagementSystem
{
    [Serializable]
    public class BuffetSeller : IBuffetSeller
    {
        internal Employee employee { get; }
        
        private decimal _totalSales;


        public decimal TotalSales
        {
            get => _totalSales;
            set
            {
                if (value < 0)
                    throw new ArgumentException("TotalSales cannot be less than 0");
                _totalSales = value;
            }
        }
    

        public BuffetSeller() { }

        public BuffetSeller(Employee employee)
        {
            this.employee = employee ?? throw new ArgumentNullException(nameof(employee));

            
            TotalSales = 0;
        }

        public void SellItem()
        {
            //main logic will be implemented later
            //for now this method is used to calculate total sales

            TotalSales += 1;
        }
    }
}
