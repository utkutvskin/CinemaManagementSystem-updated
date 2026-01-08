
namespace CinemaManagementSystem.People.Roles
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
        //PartTime
        public BuffetSeller(Employee employee, double salary) :base(employee, salary)
        {
            TotalSales = 0;
        }
        //FullTime
        public BuffetSeller(Employee employee, double hourlyRate, double hoursPerMonth) 
            :base(employee, hourlyRate, hoursPerMonth)
        {
            TotalSales = 0;
        }
        //Intern
        public BuffetSeller(Employee employee, string universityName, double? salary = null) 
            :base(employee, universityName, salary)
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