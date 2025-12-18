namespace CinemaManagementSystem.Employees;

public interface IBuffetSeller
{
    decimal TotalSales { get; set; }
    void SellItem();
}