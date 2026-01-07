using CinemaManagementSystem.Area;
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
    public void RemoveAssociationBetweenMovieAndHall_ShouldRemoveAssociation_WhenUseScreeningClass()
    {
        var directors = new List<string> { "Christopher Nolan" };
        var genres = new List<GenreEnum> { GenreEnum.Sci_fi, GenreEnum.Thriller };

        var movie = new Movie("Inception", directors, genres, ScreeningEnum.IMAX, 148, new DateTime(2025, 12, 3));
        
        Hall hall = new Hall(10);
        
        Screening.Create(movie, hall, new DateTime(2026, 12, 3), new TimeSpan(12,0,0), "English");
        
        Assert.That(movie.Screenings.Count, Is.EqualTo(1));
        Assert.That(movie.Screenings.Count, Is.EqualTo(1));
        
        Screening.RemoveScreening(movie, hall, new DateTime(2026, 12, 3), new TimeSpan(12,0,0));
        
        Assert.That(movie.Screenings.Count, Is.EqualTo(0));
        Assert.That(movie.Screenings.Count, Is.EqualTo(0));
        
    }
}