using CinemaManagementSystem.Enums;
using CinemaManagementSystem.Exceptions;
using CinemaManagementSystem.Person;
using NUnit.Framework;

namespace CinemaManagementSystem.Tests.AssociationTests;

[TestFixture]
public class MovieActorTest
{

    [SetUp]
    public void Setup()
    {
        Movie.ClearExtent();
    }
    
    [Test]
    public void Constructor_CreateMovieWithActors()
    {
        var directors = new List<string> { "Christopher Nolan" };
        var genres = new List<GenreEnum> { GenreEnum.Sci_fi, GenreEnum.Thriller };



        var actors = new List<Actor>
        {
            new Actor("Actor1", "Surname1", GenderEnum.Men, new DateTime(1999, 12, 2)),
            new Actor("Actor2", "Surname2", GenderEnum.Men, new DateTime(1999, 1, 2)),
        };
        var movie = new Movie("Inception", directors, genres, ScreeningEnum.IMAX, 148, new DateTime(2025, 12, 3),
            actors);

        Assert.That(movie.Actors.Count, Is.EqualTo(2));
        Assert.That(movie.Actors, Is.EqualTo(actors));
    }

    [Test]
    public void AddActor_FromMovieSide()
    {
        var directors = new List<string> { "Christopher Nolan" };
        var genres = new List<GenreEnum> { GenreEnum.Sci_fi, GenreEnum.Thriller };

        var actor = new Actor("John", "Doe", GenderEnum.Men, new DateTime(1980, 1, 1));
        var movie = new Movie("Inception", directors, genres, ScreeningEnum.IMAX, 148, new DateTime(2025, 12, 3));

        movie.AddActor(actor);


        Assert.IsTrue(movie.Actors.Contains(actor));
        Assert.IsTrue(actor.Movies.Contains(movie));
    }

    [Test]
    public void AddMovie_FromActorSide()
    {
        var directors = new List<string> { "Christopher Nolan" };
        var genres = new List<GenreEnum> { GenreEnum.Sci_fi, GenreEnum.Thriller };


        var actor = new Actor("Alice", "Smith", GenderEnum.Female, new DateTime(1990, 5, 5));
        var movie = new Movie("Inception", directors, genres, ScreeningEnum.IMAX, 148, new DateTime(2025, 12, 3));

        actor.AddMovie(movie);

        Assert.IsTrue(movie.Actors.Contains(actor));
        Assert.IsTrue(actor.Movies.Contains(movie));

    }


    [Test]
    public void RemoveActor_FromMovieSide()
    {
        var directors = new List<string> { "Christopher Nolan" };
        var genres = new List<GenreEnum> { GenreEnum.Sci_fi, GenreEnum.Thriller };

        var actor1 = new Actor("A", "One", GenderEnum.Men, new DateTime(1980, 1, 1));
        var actor2 = new Actor("B", "Two", GenderEnum.Men, new DateTime(1985, 2, 2));
        
        var movie = new Movie("Inception", directors, genres, ScreeningEnum.IMAX, 148, new DateTime(2025, 12, 3), new List<Actor> { actor1, actor2 });

        movie.RemoveActor(actor1);

        Assert.IsFalse(movie.Actors.Contains(actor1));
        Assert.IsFalse(actor1.Movies.Contains(movie));

        Assert.IsTrue(movie.Actors.Contains(actor2));
    }

    [Test]
    public void RemoveActor_LastActor_ThrowsException()
    {
        
        var directors = new List<string> { "Christopher Nolan" };
        var genres = new List<GenreEnum> { GenreEnum.Sci_fi, GenreEnum.Thriller };

        var actor = new Actor("Single", "Actor", GenderEnum.Men,  new DateTime(1970, 1, 1));

        var movie = new Movie("Inception", directors, genres, ScreeningEnum.IMAX, 148, new DateTime(2025, 12, 3), new List<Actor>() {actor});

        Assert.Throws<MultiplicityException>(() => { movie.RemoveActor(actor); });
    }

    [Test]
    public void RemoveMovie_FromActorSide()
    {
        var directors = new List<string> { "Christopher Nolan" };
        var genres = new List<GenreEnum> { GenreEnum.Sci_fi, GenreEnum.Thriller };

        var actor1 = new Actor("A", "One", GenderEnum.Men, new DateTime(1980, 1, 1));
        var actor2 = new Actor("B", "Two", GenderEnum.Men, new DateTime(1985, 2, 2));
        
        var movie = new Movie("Inception", directors, genres, ScreeningEnum.IMAX, 148, new DateTime(2025, 12, 3), new List<Actor> { actor1, actor2 });


        actor1.RemoveMovie(movie); 

        Assert.IsFalse(movie.Actors.Contains(actor1));
        Assert.IsFalse(actor1.Movies.Contains(movie));

        Assert.IsTrue(movie.Actors.Contains(actor2));
        
    }
    

}