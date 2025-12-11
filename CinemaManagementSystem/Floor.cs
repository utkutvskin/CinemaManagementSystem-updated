using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using CinemaManagementSystem.Enums;
using CinemaManagementSystem.Exceptions;
using CinemaManagementSystem.PersistenceForAllClasses;

namespace CinemaManagementSystem;

[Serializable]
public class Floor :CleanableArea, IExtent<Floor>
{
    // Attributes
    private int _number;

    public int Number
    {
        get => _number;
        set
        {
            if (value < 0) 
                throw new ArgumentException("Floor cannot be negative");
            foreach (var f in _floors)
            {
                if (f.Number == value)
                    throw new ArgumentException("This floor already exists");
            }
                
            _number = value;
        }
    }
    
    
    //composition association (Hall)
    [XmlIgnore]
    private readonly HashSet<Hall> _halls = new HashSet<Hall>();

    [XmlIgnore]
    public IReadOnlyCollection<Hall> Halls => _halls;
        
    public Hall AddHall( int number)
    {
        var newHall = new Hall(number, this);
            
        return newHall;
    }
        
    internal void SetHall(Hall hall)
    {
        if (hall == null)
            throw new ArgumentException("hall cannot be null.");
        
        foreach (var hl in _halls)
        {
            if (hl.Number == hall.Number)
                throw new DuplicateException($"Hall", hall.ToString());
        }
        _halls.Add(hall);
    }


    public void RemoveHall(Hall hall)
    {
        if (hall == null)
            throw new ArgumentException("hall cannot be null.");

        if (!_halls.Contains(hall))
            throw new ExistenceException("hall", hall.ToString(), "Floor");

        _halls.Remove(hall);
            
        Hall.RemoveFromExtent(hall);  
    }

    internal void InternalRemoveHall(Hall hall)
    {
        _halls.Remove(hall);
    }
    //
    
    
    //composition association (WC)
    [XmlIgnore]
    private readonly HashSet<WC> _wcs = new HashSet<WC>();

    [XmlIgnore]
    public IReadOnlyCollection<WC> WCs => _wcs;
        
    public WC AddWC( WCTypeEnum type)
    {

        var newWC = new WC(type, this);
            
        return newWC;
    }
        
    internal void SetWc(WC wc)
    {
        if (wc == null)
            throw new ArgumentException("wc cannot be null.");
        
        if (_wcs.Count >= 2) 
            throw new MultiplicityException();
        
        foreach (var existing in _wcs)
        {
            if (existing.Type == wc.Type)
                throw new DuplicateException("WC", wc.ToString());
        }
        
        _wcs.Add(wc);
    }


    public void RemoveWc(WC wc)
    {
        if (wc == null)
            throw new ArgumentException("wc cannot be null.");

        if (!_wcs.Contains(wc))
            throw new ExistenceException("wc", wc.ToString(), "Floor");

        _wcs.Remove(wc);
            
        WC.RemoveFromExtent(wc);  
    }
    //
    
    

    // Extent
    private static List<Floor> _floors = new();
    public static IReadOnlyList<Floor> Floors => _floors.AsReadOnly();

    private void AddFloor(Floor floor)
    {
        if(floor == null)
            throw new ArgumentException("Floor cannot be null");
        _floors.Add(floor);
    }
    
    internal static void RemoveFromExtent(Floor floor)
    {
        _floors.Remove(floor);
    }
    
    public void DeleteFloor()
    {
        Hall.ClearExtent();

        _halls.Clear();
        
        foreach (var wc in _wcs)
        {
            WC.RemoveFromExtent(wc);
        }

        _wcs.Clear();

        RemoveFromExtent(this);
    }

    //for tests
    public static void ClearExtent()
    {
        foreach (var floor in new List<Floor>(_floors))
        {
            floor.DeleteFloor();
        }
    }
    
    //constructor
    public Floor() { }

    public Floor(int number) :base($"Floor {number}", TimeSpan.FromHours(4))
    {
        Number = number;
        AddFloor(this);
        RegisterArea(this);
    }
    
    
    //Persistence 
    public static void Save(string filePath)
    {
        StreamWriter sw = File.CreateText(filePath);
        XmlSerializer serializer = new XmlSerializer(typeof(List<Floor>));
        using (XmlTextWriter writer = new XmlTextWriter(sw))
        {
            serializer.Serialize(writer, _floors);
        }
    }

    public static bool Load(string filePath)
    {
        StreamReader file;
        try
        {
            file = File.OpenText(filePath);
        }
        catch (FileNotFoundException)
        {
            _floors.Clear();
            return false;
        }

        XmlSerializer serializer = new XmlSerializer(typeof(List<Floor>));
        using (XmlTextReader reader = new XmlTextReader(filePath))
        {
            try
            {
                _floors = (List<Floor>)serializer.Deserialize(reader);
            }
            catch (Exception)
            {
                _floors.Clear();
                return false;
            }
        }

        return true;
    }

    public override string ToString()
    {
        return $"Floor {Number}";
    }

    public List<Floor> GetExtent() => _floors;

    public void ReplaceExtent(List<Floor> newExtent)
    {
        _floors = newExtent ?? new List<Floor>();
    }
}