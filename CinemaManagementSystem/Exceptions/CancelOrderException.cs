namespace CinemaManagementSystem.Exceptions;

public class CancelOrderException(Order order) 
    :Exception($"This order: {order} cannot be cancelled as it is already payed.")
{
    
}