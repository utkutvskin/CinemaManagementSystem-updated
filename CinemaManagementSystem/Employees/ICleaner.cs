using CinemaManagementSystem.AssociationClasses;
using CinemaManagementSystem.Enums;

namespace CinemaManagementSystem.Employees;

public interface ICleaner
{
    CleaningTypeEnum CleaningType { get; set; }
    IReadOnlyCollection<CleanerAssignment> Assignments { get; }

    CleanerAssignment Clean(CleanableArea area);
    List<CleanableArea> GetListOfAreaThatNeedToBeCleaned();
}