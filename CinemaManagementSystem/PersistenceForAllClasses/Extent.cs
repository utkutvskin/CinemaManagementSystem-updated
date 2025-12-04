using CinemaManagementSystem.AssociationClasses;

namespace CinemaManagementSystem.PersistenceForAllClasses
{
    [Serializable]
    public class Extent
    {
        public List<Movie> Movies { get; set; } = new();
        public List<Actor> Actors { get; set; } = new();
        public List<Customer> Customers { get; set; } = new();
        public List<Employee> Employees { get; set; } = new();
        public List<Hall> Halls { get; set; } = new();
        public List<Floor> Floors { get; set; } = new();
        public List<WC> WCs { get; set; } = new();
        public List<Seat> Seats { get; set; } = new();
        public List<Stampcard> Stampcards { get; set; } = new();
        public List<Order> Orders { get; set; } = new();
        public List<Screening> Screenings { get; set; } = new();
        public List<Ticket> Tickets { get; set; } = new();
        public List<CleanerAssignment> CleanerAssignments { get; set; } = new();

        public static Extent Capture()
        {
            var result = new Extent();
 
            result.Movies = new Movie().GetExtent().ToList();
            result.Actors = new Actor().GetExtent().ToList();
            result.Customers = new Customer().GetExtent().ToList();
            result.Employees = new Employee().GetExtent().ToList();
            result.Halls = new Hall().GetExtent().ToList();
            result.Floors = new Floor().GetExtent().ToList();
            result.WCs = new WC().GetExtent().ToList();
            result.Seats = new Seat().GetExtent().ToList();
            result.Stampcards = new Stampcard().GetExtent().ToList();
            result.Orders = new Order().GetExtent().ToList();
            result.Screenings = new Screening().GetExtent().ToList();
            result.Tickets = new Ticket().GetExtent().ToList();
            result.CleanerAssignments  = new CleanerAssignment().GetExtent().ToList();

            return result;
        }
 
        public void Apply()
        {
            new Movie().ReplaceExtent(Movies);
            new Actor().ReplaceExtent(Actors);
            new Customer().ReplaceExtent(Customers);
            new Employee().ReplaceExtent(Employees);
            new Hall().ReplaceExtent(Halls);
            new Floor().ReplaceExtent(Floors);
            new WC().ReplaceExtent(WCs);
            new Seat().ReplaceExtent(Seats);
            new Stampcard().ReplaceExtent(Stampcards);
            new Order().ReplaceExtent(Orders);
            new Screening().ReplaceExtent(Screenings);
            new Ticket().ReplaceExtent(Tickets);
            new CleanerAssignment().ReplaceExtent(CleanerAssignments);
        }
    }
}
