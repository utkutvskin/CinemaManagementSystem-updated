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
public class WC :CleanableArea, IExtent<WC>
{
    private WCTypeEnum _type;

    public WCTypeEnum Type
    {
        get => _type;
        set => _type = value;
    }

    //composition association (Floor)
    [XmlIgnore]           
    private Floor _floor;

    [XmlIgnore]
    public Floor Floor => _floor;

    internal void SetFloor(Floor floor)
    {
        if (floor == null)
            throw new ArgumentException("floor cannot be null for a WC.");

        if(floor == _floor)
            throw new DuplicateException("Floor",  floor.ToString());
        
        floor.SetWc(this);
        
        _floor = floor;
    }
        

    internal static void RemoveFromExtent(WC wc)
    {
        _wcs.Remove(wc);
    }

    //
    
    //Class extent
    private static List<WC> _wcs = new();
    public static IReadOnlyList<WC> WCs => _wcs;

    private void AddWC(WC wc)
    {
        if(wc == null)
            throw new ArgumentException("WC cannot be null.");
        _wcs.Add(wc);
    }
    
    
    
    public WC() { }

    private WC(WCTypeEnum type)
    {
        
        Type = type;

        AddWC(this);
    }

    public WC(WCTypeEnum type, Floor floor) : base($"WC {type} in floor {floor.Number}", TimeSpan.FromHours(1))
    {
        Type = type;

        AddWC(this);
        
        SetFloor(floor);
        RegisterArea(this);
    }
    
    
    //Persistence 
    public static void Save(string filePath)
    {
        StreamWriter sw = File.CreateText(filePath);
        XmlSerializer serializer = new XmlSerializer(typeof(List<WC>));
        using (XmlTextWriter writer = new XmlTextWriter(sw))
        {
            serializer.Serialize(writer, _wcs);
        }
    }

    public static bool Load(string filePath)
    {
        if (!File.Exists(filePath))
        {
            _wcs.Clear();
            return false;
        }

        XmlSerializer serializer = new XmlSerializer(typeof(List<WC>));
        using (XmlTextReader reader = new XmlTextReader(filePath))
        {
            try
            {
                _wcs = (List<WC>)serializer.Deserialize(reader);
            }
            catch 
            {
                _wcs.Clear();
                return false;
            }
        }

        return true;
    }

    public override string ToString()
    {
        return $"WC: {Type}";
    }
    
    public List<WC> GetExtent() => _wcs;

    public void ReplaceExtent(List<WC> newExtent)
    {
        _wcs = newExtent ?? new List<WC>();
    }
}