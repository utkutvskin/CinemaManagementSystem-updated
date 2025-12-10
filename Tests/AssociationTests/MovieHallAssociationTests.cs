using CinemaManagementSystem.AssociationClasses;
using CinemaManagementSystem.Enums;
using CinemaManagementSystem.Exceptions;
using NUnit.Framework;

namespace CinemaManagementSystem.Tests.AssociationTests;


[TestFixture]
public class MovieHallAssociationTests
{
    [SetUp]
    public void Setup()
    {
        Movie.ClearExtent();
    }

    [Test]
    public void CreateAssociationBetweenMovieAndHall_ShouldCreateAssociation_WhenUseScreeningClass()
    {
        var directors = new List<string> { "Christopher Nolan" };
        var genres = new List<GenreEnum> { GenreEnum.Sci_fi, GenreEnum.Thriller };

        var movie = new Movie("Inception", directors, genres, ScreeningEnum.IMAX, 148, new DateTime(2025, 12, 3));
        
        Hall hall = new Hall(10);
        
        Screening.Create(movie, hall, new DateTime(2026, 12, 3), new TimeSpan(12,0,0), "English");
        
        Assert.That(movie.Screenings.Count, Is.EqualTo(1));
        Assert.That(movie.Screenings.Count, Is.EqualTo(1));
        
    }
    
    [Test]
    public void CreateAssociationBetweenMovieAndHall_ShouldCreateAssociation_WhenUseMovieClass()
    {
        var directors = new List<string> { "Christopher Nolan" };
        var genres = new List<GenreEnum> { GenreEnum.Sci_fi, GenreEnum.Thriller };

        var movie = new Movie("Inception", directors, genres, ScreeningEnum.IMAX, 148, new DateTime(2025, 12, 3));
        
        Hall hall = new Hall(10);
        
        movie.ScheduleScreening(hall, new DateTime(2026, 12, 3), new TimeSpan(12,0,0), "English");
        
        Assert.That(movie.Screenings.Count, Is.EqualTo(1));
        Assert.That(movie.Screenings.Count, Is.EqualTo(1));
        
    }
    
    [Test]
    public void CreateAssociationBetweenMovieAndHall_ShouldCreateAssociation_WhenUseHallClass()
    {
        var directors = new List<string> { "Christopher Nolan" };
        var genres = new List<GenreEnum> { GenreEnum.Sci_fi, GenreEnum.Thriller };

        var movie = new Movie("Inception", directors, genres, ScreeningEnum.IMAX, 148, new DateTime(2025, 12, 3));
        
        Hall hall = new Hall(10);
        
        hall.AddScreening(movie, new DateTime(2026, 12, 3), new TimeSpan(12,0,0), "English");
        
        Assert.That(movie.Screenings.Count, Is.EqualTo(1));
        Assert.That(movie.Screenings.Count, Is.EqualTo(1));
        
    }
    
    [Test]
    public void CreateAssociationBetweenMovieAndHall_ShouldThrowException_WhenOverlapsOccur()
    {
        var directors = new List<string> { "Christopher Nolan" };
        var genres = new List<GenreEnum> { GenreEnum.Sci_fi, GenreEnum.Thriller };

        var movie = new Movie("Inception", directors, genres, ScreeningEnum.IMAX, 148, new DateTime(2025, 12, 3));
        var movie2 = new Movie("NewMovie", directors, genres, ScreeningEnum.IMAX, 148, new DateTime(2025, 12, 3));
        
        Hall hall = new Hall(10);
        
        hall.AddScreening(movie, new DateTime(2026, 12, 3), new TimeSpan(12,0,0), "English");

        Assert.Throws<InvalidOperationException>(() =>
            movie2.ScheduleScreening(hall, new DateTime(2026, 12, 3), new TimeSpan(13, 0, 0), "English"));

    }

    [Test]
    public void RemoveAssociationBetweenMovieAndHall_ShouldRemoveAssociation_WhenUseScreeningClass()
    {
        var directors = new List<string> { "Christopher Nolan" };
        var genres = new List<GenreEnum> { GenreEnum.Sci_fi, GenreEnum.Thriller };

        var movie = new Movie("Inception", directors, genres, ScreeningEnum.IMAX, 148, new DateTime(2025, 12, 3));
        var movie2 = new Movie("NewMovie", directors, genres, ScreeningEnum.IMAX, 148, new DateTime(2025, 12, 3));
        
        Hall hall = new Hall(10);
        
        hall.AddScreening(movie, new DateTime(2026, 12, 3), new TimeSpan(12,0,0), "English");
        movie2.ScheduleScreening(hall, new DateTime(2026, 12, 4), new TimeSpan(13, 0, 0), "English");
        
        Screening.RemoveScreening(movie, hall, new DateTime(2026, 12, 3), new TimeSpan(12,0,0));   
        Screening.RemoveScreening(movie2, hall, new DateTime(2026, 12, 4), new TimeSpan(13, 0, 0));
        
        Assert.That(movie.Screenings.Count, Is.EqualTo(0));
        Assert.That(movie2.Screenings.Count, Is.EqualTo(0));
        Assert.That(hall.Screenings.Count, Is.EqualTo(0));

    }
    
    [Test]
    public void RemoveAssociationBetweenMovieAndHall_ShouldRemoveAssociation_WhenUseMovieClass()
    {
        var directors = new List<string> { "Christopher Nolan" };
        var genres = new List<GenreEnum> { GenreEnum.Sci_fi, GenreEnum.Thriller };

        var movie = new Movie("Inception", directors, genres, ScreeningEnum.IMAX, 148, new DateTime(2025, 12, 3));
        Hall hall = new Hall(10);
        
        hall.AddScreening(movie, new DateTime(2026, 12, 3), new TimeSpan(12,0,0), "English");
        
        movie.RemoveScreening(hall, new DateTime(2026, 12, 3), new TimeSpan(12,0,0));   
        
        Assert.That(movie.Screenings.Count, Is.EqualTo(0));
        Assert.That(hall.Screenings.Count, Is.EqualTo(0));

    }
    
    [Test]
    public void RemoveAssociationBetweenMovieAndHall_ShouldRemoveAssociation_WhenUseHallClass()
    {
        var directors = new List<string> { "Christopher Nolan" };
        var genres = new List<GenreEnum> { GenreEnum.Sci_fi, GenreEnum.Thriller };

        var movie = new Movie("Inception", directors, genres, ScreeningEnum.IMAX, 148, new DateTime(2025, 12, 3));
        
        Hall hall = new Hall(10);
        
        hall.AddScreening(movie, new DateTime(2026, 12, 3), new TimeSpan(12,0,0), "English");
        
        hall.RemoveScreening(movie, new DateTime(2026, 12, 3), new TimeSpan(12,0,0));   
        
        Assert.That(movie.Screenings.Count, Is.EqualTo(0));
        Assert.That(hall.Screenings.Count, Is.EqualTo(0));

    }
    
    [Test]
    public void RemoveAssociationBetweenMovieAndHall_ShouldThrowException_WhenScreeningNotExists()
    {
        var directors = new List<string> { "Christopher Nolan" };
        var genres = new List<GenreEnum> { GenreEnum.Sci_fi, GenreEnum.Thriller };

        var movie = new Movie("Inception", directors, genres, ScreeningEnum.IMAX, 148, new DateTime(2025, 12, 3));
        
        Hall hall = new Hall(10);
        
        hall.AddScreening(movie, new DateTime(2026, 12, 3), new TimeSpan(12,0,0), "English");
        
        Assert.Throws<ExistenceException>(() =>
                hall.RemoveScreening(movie, new DateTime(2026, 11, 3), new TimeSpan(12,0,0)));

    }
}