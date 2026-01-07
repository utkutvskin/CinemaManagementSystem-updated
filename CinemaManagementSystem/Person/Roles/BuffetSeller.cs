using System;
using System. Xml.Serialization;
using CinemaManagementSystem.Enums;
using CinemaManagementSystem.Person;
using CinemaManagementSystem.Person.Roles;

namespace CinemaManagementSystem
{
    [Serializable]
    public class BuffetSeller : EmployeeRole
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
        

        public BuffetSeller(Employee employee) :base(employee)
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