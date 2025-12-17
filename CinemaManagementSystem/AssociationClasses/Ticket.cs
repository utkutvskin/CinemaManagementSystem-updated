using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Serialization;
using CinemaManagementSystem.Exceptions;
using CinemaManagementSystem.PersistenceForAllClasses;

namespace CinemaManagementSystem.AssociationClasses;

[Serializable]
public class Ticket :IExtent<Ticket>
{
    public static double FeeForOnlinePurchase = 1.5; 

    //basic association 
    [XmlIgnore]
    private Seat _seat;
        
    [XmlIgnore]
    public Seat Seat => _seat;

    
    //attribute association 
    [XmlIgnore] private Screening _screening;

    [XmlIgnore] private Order _order;
    
    [XmlIgnore] public Screening Screening => _screening;
    [XmlIgnore] public Order Order => _order;
    
    
    //Class extent
    private static List<Ticket> _tickets = new();
    
    public static IReadOnlyList<Ticket> Tickets => _tickets.AsReadOnly();

    public void AddTicket(Ticket ticket)
    {
        if(ticket is null)
            throw new ArgumentException("Ticket cannot be null");
        _tickets.Add(ticket);
    }
    
    //constructors
    public Ticket() {}

    private Ticket(Screening screening, Order order, Seat seat)
    {
        _screening = screening;
        _order = order;
        _seat = seat;
        
        AddTicket(this);
    }

    internal static Ticket CreateTicket(Screening screening, Order order, Seat seat)
    {
       
        var ticket = new Ticket(screening, order, seat);
        
        screening.AddTicketInternal(ticket);
        order.AddTicketInternal(ticket);
        seat.SetTicket(ticket);
        
        return ticket;
    }

    internal static void RemoveTicket(Screening screening, Order order, Seat seat)
    {
        
        Ticket? ticket = _tickets.FirstOrDefault(t => t.Screening == screening && t.Seat == seat && t.Order == order);
        
        if (ticket != null)
        {
            ticket.Cancel();
        }
        else throw new ExistenceException("Ticket" );
    }
    
    

    public void Cancel()
    {
        _tickets.Remove(this);
        _screening.RemoveTicketInternal(this);
        _order.RemoveTicketInternal(this);
        _seat.RemoveTicket(this);
    }
    
    
    public override string ToString()
    {
        return $"Ticket for {Screening}, Order: {Order.DateOfPurchase}";
    }

    public static void ClearExtent()
    {
        foreach (var s in new List<Ticket>(_tickets))
        {
            s.Cancel();
        }
    }
    
    //Persistence 
    public static void Save(string filePath)
    {
        StreamWriter sw = File.CreateText(filePath);
        XmlSerializer serializer = new XmlSerializer(typeof(List<Ticket>));
        using (XmlTextWriter writer = new XmlTextWriter(sw))
        {
            serializer.Serialize(writer, _tickets);
        }
    }

    public static bool Load(string filePath)
    {
        if (!File.Exists(filePath))
        {
            _tickets.Clear();
            return false;
        }

        XmlSerializer serializer = new XmlSerializer(typeof(List<Ticket>));
        using (XmlTextReader reader = new XmlTextReader(filePath))
        {
            try
            {
                _tickets = (List<Ticket>)serializer.Deserialize(reader);
            }
            catch 
            {
                _tickets.Clear();
                return false;
            }
        }

        return true;
    }

    public List<Ticket> GetExtent() => _tickets;
    public void ReplaceExtent(List<Ticket> newExtent)
    {
       _tickets = newExtent ?? new List<Ticket>();
    }
}