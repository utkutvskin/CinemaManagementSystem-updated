namespace CinemaManagementSystem.Exceptions;

public class DuplicateException : Exception
{
    public DuplicateException(Object info1, Object info2) : 
        base($"{info1.GetType()} : ({info1.ToString()}) is already assigned to {info2.GetType()} : ({info2.ToString()}).") { }
    public DuplicateException(Object info1, Object info2, Object info3) : 
        base($"{info1.GetType()} : ({info1.ToString()}) is already assigned to {info2.GetType()} : ({info2.ToString()}) and {info3.GetType()} : ({info3.ToString()}).") { }
}