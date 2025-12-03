using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using CinemaManagementSystem.Enums;

namespace CinemaManagementSystem;

[Serializable]
public class Floor
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
        
    internal void AddHallInternal(Hall hall)
    {
        if (hall == null)
            throw new ArgumentException("hall cannot be null.");
        
        foreach (var hl in _halls)
        {
            if (hl.Number == hall.Number)
                throw new ArgumentException($"Hall {hall.Number} already exists in this floor.");
        }
        _halls.Add(hall);
    }


    public void RemoveHall(Hall hall)
    {
        if (hall == null)
            throw new ArgumentException("hall cannot be null.");

        if (!_halls.Contains(hall))
            throw new InvalidOperationException("hall does not belong to this floor.");

        _halls.Remove(hall);
            
        Hall.RemoveFromExtent(hall);  
    }

    public void DeleteFloor()
    {
        foreach (var hall in _halls)
        {
            hall.DeleteHall();
        }

        _halls.Clear();
        
        foreach (var wc in _wcs)
        {
            WC.RemoveFromExtent(wc);
        }

        _wcs.Clear();

        RemoveFromExtent(this);
    }

    internal void InternalClearHalls()
    {
        _halls.Clear();
    }
    
    internal void InternalRemoveHall(Hall hall)
    {
        _halls.Remove(hall);
    }
    
    
    
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
        
    internal void AddWCInternal(WC wc)
    {
        if (wc == null)
            throw new ArgumentException("wc cannot be null.");
        
        foreach (var existing in _wcs)
        {
            if (existing.Type == wc.Type)
                throw new ArgumentException($"WC for {wc.Type} already exists on this floor.");
        }
        
        _wcs.Add(wc);
    }


    public void RemoveWC(WC wc)
    {
        if (wc == null)
            throw new ArgumentException("wc cannot be null.");

        if (!_wcs.Contains(wc))
            throw new InvalidOperationException("wc does not belong to this floor.");

        _wcs.Remove(wc);
            
        WC.RemoveFromExtent(wc);  
    }


    internal void InternalClearWCs()
    {
        _wcs.Clear();
    }
    
    

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

    
    //constructor
    public Floor() { }

    public Floor(int number)
    {
        Number = number;
        AddFloor(this);
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
}