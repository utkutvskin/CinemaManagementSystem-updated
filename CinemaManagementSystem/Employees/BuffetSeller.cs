using System;
using System.Xml.Serialization;

namespace CinemaManagementSystem
{
    [Serializable]
    public class BuffetSeller : Employee
    {
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

        public BuffetSeller(string name, string surname, DateTime birthDate,
            DateTime startDate, double salary)
            : base(name, surname, birthDate, startDate, salary)
        {
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