using CinemaManagementSystem.AssociationClasses;

namespace CinemaManagementSystem.Employees;

public interface IReceptionist
{
    int DeskNumber { get; set; }
    void CreateOrder(Screening screening, Seat seat);
    void RemoveOrder(DateTime dateTimeOfCreation);
    void ChooseNewTicket(Screening screening, Seat seat, DateTime dateTimeOfCreation);
    void RemoveTicket(Screening screening, Seat seat, DateTime dateTimeOfCreation);
    void SellTicketPayedByCard(DateTime dateTimeOfCreation, CardInfo cardInfo);
    void SellTicketPayedByCash(DateTime dateTimeOfCreation);
    void CancelOrder(DateTime dateTimeOfCreation);
    void ApplyCustomerStampCardToOrder(Order order, Customer customer);
}