namespace CinemaManagementSystem.Exceptions;

public class StampException
    :Exception
{
    public StampException(Stampcard card) : base($"This stampcard {card} cannot be stamped")
    { }
    public StampException(Stampcard card, Order order) : base($"This stampcard {card} cannot be stamped for this order: {order} as it is already payed")
    { }
    
    public StampException(Customer customer, string message) : base($"This customer {customer} {message}")
    { }
}