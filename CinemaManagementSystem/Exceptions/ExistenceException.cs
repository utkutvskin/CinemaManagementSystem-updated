namespace CinemaManagementSystem.Exceptions;

public class ExistenceException : Exception
{
    public ExistenceException(string class1, string info, string class2)
    :base($"{class1} : ({info}) is not assigned to this {class2}") { }

    public ExistenceException(string class1)  : base($"This {class1} doesn't not exist") { }
}