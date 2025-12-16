namespace CinemaManagementSystem.Exceptions;

public class ExistenceException : Exception
{
    public ExistenceException(Object info1, Object info2)
    :base($"{info1.GetType()} : ({info1.ToString()}) is not assigned to this {info2.GetType()} : ({info2.ToString()}") { }

    public ExistenceException(string class1)  : base($"This {class1} doesn't not exist") { }
}