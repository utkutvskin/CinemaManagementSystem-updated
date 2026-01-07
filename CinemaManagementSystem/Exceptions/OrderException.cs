namespace CinemaManagementSystem.Exceptions;

public class OrderException(Order order, string message) 
    :Exception($"This order: {order} cannot be {message} as it is already payed.")
{
    
}