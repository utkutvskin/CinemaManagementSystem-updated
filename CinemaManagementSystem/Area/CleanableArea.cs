using System.Xml.Serialization;
using CinemaManagementSystem.AssociationClasses;

namespace CinemaManagementSystem.Area;

[Serializable]
//INHERITANCE IMPLEMENTATION: Polymorphism Support for XML Serialization
//Since CleanableArea is abstract, the XmlSerializer needs to know exactly 
// which concrete classes (Hall, Floor, WC) it might encounter in the list.
[XmlInclude(typeof(Hall))]
[XmlInclude(typeof(Floor))]
[XmlInclude(typeof(WC))]

public abstract class CleanableArea
{
    private string _description;
    private TimeSpan _periodBetweenCleanings;

    public string Description
    {
        get => _description;
        set
        {
            if(string.IsNullOrWhiteSpace(value)) 
                throw new ArgumentException("Description cannot be null or empty");
            _description = value;
        }
    }

    public TimeSpan PeriodBetweenCleanings
    {
        get => _periodBetweenCleanings;
        set
        {
            if (value < TimeSpan.Zero || value >= TimeSpan.FromDays(1))
                throw new ArgumentException("Time must be between 00:00 and 23:59.");
            _periodBetweenCleanings = value;
        }
    }
    
    [XmlIgnore]
    public bool IsNeedToBeCleaned
    {
        get
        {
            if (_cleanerAssignments.Count == 0)
            {
                return true;
            }

            DateTime lastCleaning = _cleanerAssignments
                .Max(a => a.CleaningDateTime);

            return DateTime.Now - lastCleaning > PeriodBetweenCleanings;
        }
    }
    
    [XmlIgnore]
    private static readonly List<CleanableArea> _areas = new();
    [XmlIgnore]
    public static IReadOnlyList<CleanableArea> Areas => _areas.AsReadOnly();

    protected void RegisterArea(CleanableArea area)
    {
        _areas.Add(area);
    }

    // Attribute association 
    [XmlIgnore]
    private readonly List<CleanerAssignment> _cleanerAssignments = new();

    [XmlIgnore]
    public IReadOnlyCollection<CleanerAssignment> CleanerAssignments => _cleanerAssignments.AsReadOnly();

    internal void AddCleanerAssignmentInternal(CleanerAssignment assignment)
    {
        _cleanerAssignments.Add(assignment);
    }

    internal void RemoveCleanerAssignmentInternal(CleanerAssignment assignment)
    {
        _cleanerAssignments.Remove(assignment);
    }
    


    // methods
    public static List<CleanableArea> GenerateListOfAreaToClean()
    {
        return _areas.Where(a => a.IsNeedToBeCleaned).ToList();
    }

    protected CleanableArea() {}
    
    protected CleanableArea(string description, TimeSpan periodBetweenCleanings)
    {
        Description = description;
        PeriodBetweenCleanings = periodBetweenCleanings;
        
    }
}
