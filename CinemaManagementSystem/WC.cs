using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using CinemaManagementSystem.Enums;

namespace CinemaManagementSystem;

[Serializable]
public class WC
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

            
        if (_floor != null && _floor != floor)
            throw new InvalidOperationException("wc is already assigned to another hall.");

        _floor = floor;
    }
        

    internal static void RemoveFromExtent(WC wc)
    {
        if (wc != null)
            _wcs.Remove(wc);
    }


    
    //Class extent
    private static List<WC> _wcs = new();
    public static IReadOnlyList<WC> WCs => _wcs;

    private void AddWC(WC wc)
    {
        if(wc == null)
            throw new ArgumentException("WC cannot be null.");
        _wcs.Add(wc);
    }

    
    //for tests only
    public static void ClearAllWCsForTesting()
    {
        foreach (var floor in Floor.Floors)
            floor.InternalClearWCs();

        _wcs.Clear();
    }
    
    
    
    public WC() { }

    private WC(WCTypeEnum type)
    {
        
        Type = type;

        AddWC(this);
    }

    public WC(WCTypeEnum type, Floor floor) : this(type)
    {
        SetFloor(floor);
        
        floor.AddWCInternal(this);
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
}