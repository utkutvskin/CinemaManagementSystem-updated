namespace CinemaManagementSystem.Exceptions;

public class DuplicateException(string type, string info) 
    : Exception($"{type} : ({@info}) is already exists.");