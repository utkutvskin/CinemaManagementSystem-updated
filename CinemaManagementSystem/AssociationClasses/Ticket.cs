using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Serialization;
using CinemaManagementSystem.Exceptions;
using CinemaManagementSystem.PersistenceForAllClasses;

namespace CinemaManagementSystem.AssociationClasses;

[Serializable]
public class Ticket :IExtent<Ticket>
{
    private double _price;

    public double Price
    {
        get => _price;
        set
        {
            if(value <= 0)
                throw new ArgumentException("Price must be greater than zero");
            _price = value;
        }
    }
    
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

    private Ticket(double price, Screening screening, Order order, Seat seat)
    {
        _screening = screening;
        _order = order;
        _seat = seat;
        Price = price;
        
        AddTicket(this);
    }

    internal static Ticket CreateTicket(double price, Screening screening, Order order, Seat seat)
    {
       
        var ticket = new Ticket(price, screening, order, seat);
        
        screening.AddTicketInternal(ticket);
        order.AddTicketInternal(ticket);
        seat.SetTicket(ticket);
        
        return ticket;
    }

    internal static void RemoveTicket(Screening screening, Order order, Seat seat)
    {
        if (screening == null) 
            throw new ArgumentException("Screening cannot be null.");
        if (order == null)
            throw new ArgumentException("Order cannot be null.");
        if (seat == null)
            throw new ArgumentException("Seat cannot be null.");
        
        
        Ticket? ticket = _tickets.FirstOrDefault(t => t.Screening == screening && t.Seat == seat && t.Order == order);
        
        if (ticket != null)
        {
            ticket.Cancel();
        }
        else throw new ExistenceException("Screening" );
    }
    
    

    public void Cancel()
    {
        _tickets.Remove(this);
        _screening.RemoveTicketInternal(this);
        _order.RemoveTicketInternal(this);
        _seat.RemoveTicket(this);
    }

    
    //basic association 
    [XmlIgnore]
    private Seat _seat;
        
    [XmlIgnore]
    public Seat Seat => _seat;
    
    
    public override string ToString()
    {
        return $"Ticket Price: {Price} for {Screening.Movie}, Order: {Order.DateOfPurchase}";
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