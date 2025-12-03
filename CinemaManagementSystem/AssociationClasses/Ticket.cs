using System.Xml;
using System.Xml.Serialization;

namespace CinemaManagementSystem.AssociationClasses;

[Serializable]
public class Ticket
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

    public Ticket(double price, Screening screening, Order order, Seat seat)
    {
        _screening = screening;
        _order = order;
        _seat = seat;
        Price = price;
        
        AddTicket(this);
    }

    public static Ticket CreateTicket(double price, Screening screening, Order order, Seat seat)
    {
        if (screening == null) 
            throw new ArgumentException("Screening cannot be null.");
        if (order == null)
            throw new ArgumentException("Order cannot be null.");
        if (seat == null)
            throw new ArgumentException("Seat cannot be null.");

        if (!screening.Hall.Seats.Contains(seat))
            throw new InvalidOperationException("Seat does not belong to this hall.");

        bool occupied = Tickets.Any(t => t.Screening == screening && t.seat == seat);
        if (occupied)
            throw new InvalidOperationException($"Seat {seat} is already taken for this screening.");
       
        var ticket = new Ticket(price, screening, order, seat);
        
        screening.AddTicketInternal(ticket);
        order.AddTicketInternal(ticket);
        
        return ticket;
    }

    public void Cancel()
    {
        _tickets.Remove(this);
        _screening?.RemoveTicketInternal(this);
        _order?.RemoveTicketInternal(this);
    }

    
    //basic association 
    [XmlIgnore]
    private Seat _seat;
        
    [XmlIgnore]
    public Seat seat => _seat;
    
    
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
}