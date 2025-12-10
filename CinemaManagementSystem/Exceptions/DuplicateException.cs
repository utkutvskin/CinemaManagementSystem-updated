namespace CinemaManagementSystem.Exceptions;

public class DuplicateException(string type, string @class) 
    : Exception($"{type} : ({@class}) is already exists.");