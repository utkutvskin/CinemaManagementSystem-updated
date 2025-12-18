using CinemaManagementSystem.AssociationClasses;

namespace CinemaManagementSystem.Employees;

public interface IDisplayer
{
    int NumbersOfScreensManaged { get; }
    IReadOnlyCollection<DisplayerAssigment> Assigments { get; }

    void ManageHall(Hall hall, string? description = null);
}