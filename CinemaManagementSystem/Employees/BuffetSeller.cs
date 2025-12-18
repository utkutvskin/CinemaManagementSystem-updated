using System;
using System.Xml.Serialization;

namespace CinemaManagementSystem
{
    [Serializable]
    public class BuffetSeller 
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

         public BuffetSeller(decimal initialSales = 0)
        {
            TotalSales = initialSales;
        }
        
        public void SellItem()
        {
            //main logic will be implemented later
            //for now this method is used to calculate total sales

            TotalSales += 1;
        }

        public override string ToString()
        {
            return $"BuffetSeller - Total Sales: {TotalSales:C}";
        }
    }
}
