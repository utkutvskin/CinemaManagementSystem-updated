using CinemaManagementSystem.AssociationClasses;
using CinemaManagementSystem.Enums;
using CinemaManagementSystem.Exceptions;
using NUnit.Framework;

namespace CinemaManagementSystem.Tests.AssociationTests;

[TestFixture]
public class OrderScreeningTests
{
    [SetUp]
    public void Setup()
    {
        Order.ClearExtent();
    }
    
    [Test]
    public void CreateOrder_ShouldCreateNewOrder()
    {
        var card = new CardInfo("John Doe", "1234567890121111", DateTime.Now.AddYears(1), "123");
        var customer = new Customer("John", "Doe", new DateTime(1990, 5, 10));
        
        var directors = new List<string> { "Christopher Nolan" };
        var genres = new List<GenreEnum> { GenreEnum.Sci_fi, GenreEnum.Thriller };

        var movie = new Movie("Inception", directors, genres, ScreeningEnum.IMAX, 148, new DateTime(2025, 12, 3));
        
        Hall hall = new Hall(10);
        
        var screening = Screening.Create(movie, hall, new DateTime(2026, 12, 3), new TimeSpan(12,0,0), "English");

        var seat = new Seat(1, 'A', hall);
        
        var order = Order.Create(customer, card, screening, seat, 27);
        
        Assert.That(order.Tickets.Count, Is.EqualTo(1));
        Assert.That(order.Tickets.First(t => t.Screening == screening).Seat, Is.EqualTo(seat));
        
        
    }
    
    [Test]
    public void AddTicketToOrder_ShouldAddNewTicket()
    {
        var card = new CardInfo("John Doe", "1234567890121111", DateTime.Now.AddYears(1), "123");
        var customer = new Customer("John", "Doe", new DateTime(1990, 5, 10));
        
        var directors = new List<string> { "Christopher Nolan" };
        var genres = new List<GenreEnum> { GenreEnum.Sci_fi, GenreEnum.Thriller };

        var movie = new Movie("Inception", directors, genres, ScreeningEnum.IMAX, 148, new DateTime(2025, 12, 3));
        
        Hall hall = new Hall(10);
        Hall hall1 = new Hall(11);
        
        var screening = Screening.Create(movie, hall, new DateTime(2026, 12, 3), new TimeSpan(12,0,0), "English");
        var screening1 = Screening.Create(movie, hall1, new DateTime(2026, 12, 4), new TimeSpan(12,0,0), "English");
        
        var seat = new Seat(1, 'A', hall);
        var seat1 = new Seat(1, 'A', hall1);
        
        var order = Order.Create(customer, card, screening, seat, 27);
        order.AddTicket(screening1, seat1, 30);
        
        Assert.That(order.Tickets.Count, Is.EqualTo(2));
        Assert.That(order.Tickets.First(t => t.Screening == screening1).Seat, Is.EqualTo(seat1));
    }
    
    [Test]
    public void RemoveTicketFromOrder_ShouldRemoveTicket()
    {
        var card = new CardInfo("John Doe", "1234567890121111", DateTime.Now.AddYears(1), "123");
        var customer = new Customer("John", "Doe", new DateTime(1990, 5, 10));
        
        var directors = new List<string> { "Christopher Nolan" };
        var genres = new List<GenreEnum> { GenreEnum.Sci_fi, GenreEnum.Thriller };

        var movie = new Movie("Inception", directors, genres, ScreeningEnum.IMAX, 148, new DateTime(2025, 12, 3));
        
        Hall hall = new Hall(10);
        Hall hall1 = new Hall(11);
        
        var screening = Screening.Create(movie, hall, new DateTime(2026, 12, 3), new TimeSpan(12,0,0), "English");
        var screening1 = Screening.Create(movie, hall1, new DateTime(2026, 12, 4), new TimeSpan(12,0,0), "English");
        
        var seat = new Seat(1, 'A', hall);
        var seat1 = new Seat(1, 'A', hall1);
        
        var order = Order.Create(customer, card, screening, seat, 27);
        order.AddTicket(screening1, seat1, 30);
        
        order.RemoveTicket(screening, seat);
        
        Assert.That(order.Tickets.Count, Is.EqualTo(1));
        Assert.That(order.Tickets.First(t => t.Screening == screening1).Seat, Is.EqualTo(seat1));
        Assert.That(seat1.Tickets.Count, Is.EqualTo(1));
    }
    
    [Test]
    public void RemoveOrder_ShouldRemoveOrder()
    {
        var card = new CardInfo("John Doe", "1234567890121111", DateTime.Now.AddYears(1), "123");
        var customer = new Customer("John", "Doe", new DateTime(1990, 5, 10));
        
        var directors = new List<string> { "Christopher Nolan" };
        var genres = new List<GenreEnum> { GenreEnum.Sci_fi, GenreEnum.Thriller };

        var movie = new Movie("Inception", directors, genres, ScreeningEnum.IMAX, 148, new DateTime(2025, 12, 3));
        
        Hall hall = new Hall(10);
        Hall hall1 = new Hall(11);
        
        var screening = Screening.Create(movie, hall, new DateTime(2026, 12, 3), new TimeSpan(12,0,0), "English");
        var screening1 = Screening.Create(movie, hall1, new DateTime(2026, 12, 4), new TimeSpan(12,0,0), "English");
        
        var seat = new Seat(1, 'A', hall);
        var seat1 = new Seat(1, 'A', hall1);
        
        var order = Order.Create(customer, card, screening, seat, 27);
        order.AddTicket(screening1, seat1, 30);
        
        Order.RemoveOrder(customer, DateTime.Now.Date);
        
        Assert.That(order.Tickets.Count, Is.EqualTo(0));
        Assert.That(screening.Tickets.Count, Is.EqualTo(0));
        Assert.That(seat.Tickets.Count, Is.EqualTo(0));
    }
    
    [Test]
    public void AddTicketToOrder_ShouldThrowException_WhenSeatIsOccupied()
    {
        var card = new CardInfo("John Doe", "1234567890121111", DateTime.Now.AddYears(1), "123");
        var customer = new Customer("John", "Doe", new DateTime(1990, 5, 10));
        
        var directors = new List<string> { "Christopher Nolan" };
        var genres = new List<GenreEnum> { GenreEnum.Sci_fi, GenreEnum.Thriller };

        var movie = new Movie("Inception", directors, genres, ScreeningEnum.IMAX, 148, new DateTime(2025, 12, 3));
        
        Hall hall = new Hall(10);
        Hall hall1 = new Hall(11);
        
        var screening = Screening.Create(movie, hall, new DateTime(2026, 12, 3), new TimeSpan(12,0,0), "English");
        var screening1 = Screening.Create(movie, hall1, new DateTime(2026, 12, 4), new TimeSpan(12,0,0), "English");
        
        var seat = new Seat(1, 'A', hall);
        var seat1 = new Seat(1, 'A', hall1);
        
        var order = Order.Create(customer, card, screening, seat, 27);
        order.AddTicket(screening1, seat1, 30);
        
        Assert.Throws<InvalidOperationException>(() => order.AddTicket(screening, seat, 30));
        
    }

    [Test]
    public void AddTicketToOrder_ShouldThrowException_WhenSeatIsNotExistsInThisHall()
    {
        var card = new CardInfo("John Doe", "1234567890121111", DateTime.Now.AddYears(1), "123");
        var customer = new Customer("John", "Doe", new DateTime(1990, 5, 10));

        var directors = new List<string> { "Christopher Nolan" };
        var genres = new List<GenreEnum> { GenreEnum.Sci_fi, GenreEnum.Thriller };

        var movie = new Movie("Inception", directors, genres, ScreeningEnum.IMAX, 148, new DateTime(2025, 12, 3));

        Hall hall = new Hall(10);
        Hall hall1 = new Hall(11);

        var screening = Screening.Create(movie, hall, new DateTime(2026, 12, 3), new TimeSpan(12, 0, 0), "English");
        var screening1 = Screening.Create(movie, hall1, new DateTime(2026, 12, 4), new TimeSpan(12, 0, 0), "English");

        var seat = new Seat(1, 'A', hall);
        var seat1 = new Seat(1, 'A', hall1);

        var order = Order.Create(customer, card, screening, seat, 27);
        order.AddTicket(screening1, seat1, 30);

        Assert.Throws<ExistenceException>(() => order.AddTicket(screening, seat1, 30));

    }
    
    [Test]
    public void RemoveTicketFromOrder_ShouldThrowException_WhenTicketIsLast()
    {
        var card = new CardInfo("John Doe", "1234567890121111", DateTime.Now.AddYears(1), "123");
        var customer = new Customer("John", "Doe", new DateTime(1990, 5, 10));
        
        var directors = new List<string> { "Christopher Nolan" };
        var genres = new List<GenreEnum> { GenreEnum.Sci_fi, GenreEnum.Thriller };

        var movie = new Movie("Inception", directors, genres, ScreeningEnum.IMAX, 148, new DateTime(2025, 12, 3));
        
        Hall hall = new Hall(10);
        Hall hall1 = new Hall(11);
        
        var screening = Screening.Create(movie, hall, new DateTime(2026, 12, 3), new TimeSpan(12,0,0), "English");
        var screening1 = Screening.Create(movie, hall1, new DateTime(2026, 12, 4), new TimeSpan(12,0,0), "English");
        
        var seat = new Seat(1, 'A', hall);
        var seat1 = new Seat(1, 'A', hall1);
        
        var order = Order.Create(customer, card, screening, seat, 27);
        order.AddTicket(screening1, seat1, 30);
        
        order.RemoveTicket(screening, seat);
        
        Assert.Throws<MultiplicityException>(() => order.RemoveTicket(screening1, seat1));
    }
    
    [Test]
    public void RemoveTicketFromOrder_ShouldThrowException_WhenTicketIsIsNotExists()
    {
        var card = new CardInfo("John Doe", "1234567890121111", DateTime.Now.AddYears(1), "123");
        var customer = new Customer("John", "Doe", new DateTime(1990, 5, 10));
        
        var directors = new List<string> { "Christopher Nolan" };
        var genres = new List<GenreEnum> { GenreEnum.Sci_fi, GenreEnum.Thriller };

        var movie = new Movie("Inception", directors, genres, ScreeningEnum.IMAX, 148, new DateTime(2025, 12, 3));
        
        Hall hall = new Hall(10);
        Hall hall1 = new Hall(11);
        
        var screening = Screening.Create(movie, hall, new DateTime(2026, 12, 3), new TimeSpan(12,0,0), "English");
        var screening1 = Screening.Create(movie, hall1, new DateTime(2026, 12, 4), new TimeSpan(12,0,0), "English");
        
        var seat = new Seat(1, 'A', hall);
        var seat1 = new Seat(1, 'A', hall1);
        
        var order = Order.Create(customer, card, screening, seat, 27);
        order.AddTicket(screening1, seat1, 30);
        
        
        Assert.Throws<ExistenceException>(() => order.RemoveTicket(screening1, seat));
    }

}