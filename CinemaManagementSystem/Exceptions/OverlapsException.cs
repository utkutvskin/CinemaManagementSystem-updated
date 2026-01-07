using CinemaManagementSystem.Area;

namespace CinemaManagementSystem.Exceptions;

public class OverlapsException(Hall hall, DateTime date, TimeSpan hour)
    : Exception($"This {hall} is already occupied at {date.Date + hour}.")
{ }