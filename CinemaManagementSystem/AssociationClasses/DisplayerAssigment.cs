using System.Xml;
using CinemaManagementSystem.Employees;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using CinemaManagementSystem.Area;
using CinemaManagementSystem.Exceptions;
using CinemaManagementSystem.PersistenceForAllClasses;

namespace CinemaManagementSystem.AssociationClasses;
[Serializable]
public class DisplayerAssigment 
{
    private DateTime _date;
    private TimeSpan _time;
    private string? _problemDescription;

    public DateTime Date
    {
        get => _date.Date;
        set
        {
            if (value.Date > DateTime.Today)
                throw new ArgumentException("Managing date cannot be in the future.");
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
    
    public string? ProblemDescription
    {
        get => _problemDescription;
        set => _problemDescription = value;
    }


    [XmlIgnore] private Displayer _displayer;

    [XmlIgnore] private Hall _hall;
    
    [XmlIgnore] public Displayer Displayer => _displayer;

    [XmlIgnore] public Hall Hall => _hall;

    
    
    //extent 
    private static List<DisplayerAssigment> _assignments = new();
    public static IReadOnlyList<DisplayerAssigment> Assignments => _assignments.AsReadOnly();

    private void AddDisplayerAssignmentInternal(DisplayerAssigment cleaner)
    {
        if(cleaner == null)
            throw new ArgumentNullException(nameof(cleaner));
        _assignments.Add(cleaner);
    }

    
    //constructors
    public DisplayerAssigment() { } 

    
    private DisplayerAssigment(Displayer displayer, Hall hall, DateTime date, TimeSpan time, string? problemDescription)
    {

        _problemDescription = problemDescription;
        Date = date;
        Time = time;

        AddDisplayerAssignmentInternal(this);
    }

    internal static DisplayerAssigment Create(Displayer displayer, Hall hall, DateTime date, TimeSpan time, string? problemDescription = null)
    {
        if (displayer == null) throw new ArgumentException("displayer cannot be null.");
        if (hall == null) throw new ArgumentException("hall cannot be null.");
        
        DisplayerAssigment? duplicate = _assignments
            .FirstOrDefault(s => s._displayer == displayer 
                                 && s.Date == date 
                                 && s.Time == time 
                                 && s.Hall == hall);
            
        //checking duplicates
        if (duplicate != null)
            throw new DuplicateException(duplicate, displayer, hall);
        
        
        var assigment = new DisplayerAssigment(displayer, hall, date, time, problemDescription);
        
        displayer.AddDisplayerAssignmentInternal(assigment);
        hall.AddDisplayerAssignmentInternal(assigment);
        
        return assigment;
    }
    

    public void Cancel()
    {
        _assignments.Remove(this);
        _displayer?.RemoveDisplayerAssignmentInternal(this);
        _hall?.RemoveDisplayerAssignmentInternal(this);
    }
    
    public override string ToString()
    {
        return $"{Displayer.Name} {Displayer.Surname} clean {Hall.Description} on {Date:dd/MM/yyyy} at {Time:hh\\:mm})";
    }
    
    public static void ClearExtent()
    {
        foreach (var s in new List<DisplayerAssigment>(_assignments))
        {
            s.Cancel();
        }
    }
    
    //Persistence 
    public static void Save(string filePath)
    {
        StreamWriter sw = File.CreateText(filePath);
        XmlSerializer serializer = new XmlSerializer(typeof(List<DisplayerAssigment>));
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

        XmlSerializer serializer = new XmlSerializer(typeof(List<DisplayerAssigment>));
        using (XmlTextReader reader = new XmlTextReader(filePath))
        {
            try
            {
                _assignments = (List<DisplayerAssigment>)serializer.Deserialize(reader);
            }
            catch 
            {
                _assignments.Clear();
                return false;
            }
        }

        return true;
    }

}
