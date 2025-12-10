namespace CinemaManagementSystem.Exceptions;

public class CapacityException(string elements, int maxCapacity)
    : Exception($"The number of {elements} is exceed the maximum number {maxCapacity}");