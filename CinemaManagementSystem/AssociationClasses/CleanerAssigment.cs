using System.Xml;
using CinemaManagementSystem.Employees;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using CinemaManagementSystem.PersistenceForAllClasses;

namespace CinemaManagementSystem.AssociationClasses;
[Serializable]
public class CleanerAssignment :IExtent<CleanerAssignment>
{
    private DateTime _date;
    private TimeSpan _time;

    public DateTime Date
    {
        get => _date.Date;
        set
        {
            if (value.Date > DateTime.Today)
                throw new ArgumentException("Cleaning date cannot be in the future.");
            _date = value.Date;
        }
    }

    public TimeSpan Time
    {
        get => _time;
        set
        {
            if (value < TimeSpan.Zero || value >= TimeSpan.FromDays(1))
                throw new ArgumentException("Time must be between 00:00 and 23:59.");
            _time = value;
        }
    }

    [XmlIgnore]
    public DateTime CleaningDateTime => Date.Date + Time;

    [XmlIgnore] private Cleaner _cleaner;

    [XmlIgnore] private CleanableArea _area;
    
    [XmlIgnore] public Cleaner Cleaner => _cleaner;

    [XmlIgnore] public CleanableArea Area => _area;

    
    
    //extent 
    private static List<CleanerAssignment> _assignments = new();
    public static IReadOnlyList<CleanerAssignment> Assignments => _assignments.AsReadOnly();

    private void AddCleanerAssignmentInternal(CleanerAssignment cleaner)
    {
        if(cleaner == null)
            throw new ArgumentNullException(nameof(cleaner));
        _assignments.Add(cleaner);
    }

    
    //constructors
    public CleanerAssignment() { } 

    private CleanerAssignment(Cleaner cleaner, CleanableArea area, DateTime date, TimeSpan time)
    {
        _cleaner = cleaner ?? throw new ArgumentException("Cleaner cannot be null.");
        _area = area ?? throw new ArgumentException("Area cannot be null.");

        Date = date;
        Time = time;

        AddCleanerAssignmentInternal(this);
    }

    public static CleanerAssignment Create(Cleaner cleaner, CleanableArea area, DateTime date, TimeSpan time)
    {
        if (cleaner == null) throw new ArgumentException("Cleaner cannot be null.");
        if (area == null) throw new ArgumentException("Area cannot be null.");
        
        var assigment = new CleanerAssignment(cleaner, area, date, time);
        
        cleaner.AddCleanerAssignmentInternal(assigment);
        area.AddCleanerAssignmentInternal(assigment);
        
        return assigment;
    }

    public void Cancel()
    {
        _assignments.Remove(this);
        _cleaner?.RemoveCleanerAssignmentInternal(this);
        _area?.RemoveCleanerAssignmentInternal(this);
    }
    
    public override string ToString()
    {
        return $"{Cleaner.employee.Name} {Cleaner.employee.Surname} clean {Area.Description} on {Date:dd/MM/yyyy} at {Time:hh\\:mm})";
    }
    
    public static void ClearExtent()
    {
        foreach (var s in new List<CleanerAssignment>(_assignments))
        {
            s.Cancel();
        }
    }
    
    //Persistence 
    public static void Save(string filePath)
    {
        StreamWriter sw = File.CreateText(filePath);
        XmlSerializer serializer = new XmlSerializer(typeof(List<CleanerAssignment>));
        using (XmlTextWriter writer = new XmlTextWriter(sw))
        {
            serializer.Serialize(writer, _assignments);
        }
    }

    public static bool Load(string filePath)
    {
        if (!File.Exists(filePath))
        {
            _assignments.Clear();
            return false;
        }

        XmlSerializer serializer = new XmlSerializer(typeof(List<CleanerAssignment>));
        using (XmlTextReader reader = new XmlTextReader(filePath))
        {
            try
            {
                _assignments = (List<CleanerAssignment>)serializer.Deserialize(reader);
            }
            catch 
            {
                _assignments.Clear();
                return false;
            }
        }

        return true;
    }

    public List<CleanerAssignment> GetExtent() => _assignments;

    public void ReplaceExtent(List<CleanerAssignment> newExtent)
    {
        _assignments = newExtent ?? new List<CleanerAssignment>();
    }
}
