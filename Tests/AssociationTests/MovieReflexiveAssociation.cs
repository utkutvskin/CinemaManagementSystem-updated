using CinemaManagementSystem.Enums;
using CinemaManagementSystem.Exceptions;
using NUnit.Framework;

namespace CinemaManagementSystem.Tests.AssociationTests;

[TestFixture]
public class MovieReflexiveAssociation
{

    [SetUp]
    public void Setup()
    {
        Movie.ClearExtent();
    }

    [Test]
    public void AddSequel_CreatesReversePrequel()
    {
        var a = new Movie("A", new List<string> { "D" }, new List<GenreEnum> { GenreEnum.Action }, ScreeningEnum.TwoD,
            100, DateTime.Now.AddYears(-1));
        var b = new Movie("B", new List<string> { "D" }, new List<GenreEnum> { GenreEnum.Action }, ScreeningEnum.TwoD,
            95, DateTime.Now.AddYears(-1));

        a.AddSequel(b);

        Assert.That(a.Sequels, Is.EqualTo(b));
        Assert.That(b.Prequels, Is.EqualTo(a));
    }


    [Test]
    public void RemoveSequel_RemovesReversePrequel()
    {
        var a = new Movie("A", new List<string> { "D" }, new List<GenreEnum> { GenreEnum.Comedy }, ScreeningEnum.TwoD,
            100, DateTime.Now.AddYears(-1));
        var b = new Movie("B", new List<string> { "D" }, new List<GenreEnum> { GenreEnum.Comedy }, ScreeningEnum.TwoD,
            95, DateTime.Now.AddYears(-1));
        a.AddSequel(b);

        a.RemoveSequel(b);

        Assert.That(a.Sequels, Is.EqualTo(null));
        Assert.That(b.Prequels, Is.EqualTo(null));
    }


    [Test]
    public void AddSequel_ErrorCases_ThrowsAppropriateExceptions()
    {
        var a = new Movie("A", new List<string> { "D" }, new List<GenreEnum> { GenreEnum.Drama }, ScreeningEnum.TwoD,
            100, DateTime.Now.AddYears(-1));
        var b = new Movie("B", new List<string> { "D" }, new List<GenreEnum> { GenreEnum.Drama }, ScreeningEnum.TwoD,
            95, DateTime.Now.AddYears(-1));

        // null
        Assert.Throws<ArgumentException>(() => a.AddSequel(null));

        // self
        Assert.Throws<InvalidOperationException>(() => a.AddSequel(a));

        // duplicate
        a.AddSequel(b);
        Assert.Throws<DuplicateException>(() => a.AddSequel(b));
    }


    [Test]
    public void RemoveReflexiveAssociations_ClearsBothSides()
    {
        var a = new Movie("A", new List<string> { "D" }, new List<GenreEnum> { GenreEnum.Action }, ScreeningEnum.TwoD,
            100, DateTime.Now.AddYears(-1));
        var b = new Movie("B", new List<string> { "D" }, new List<GenreEnum> { GenreEnum.Action }, ScreeningEnum.TwoD,
            95, DateTime.Now.AddYears(-1));
        var c = new Movie("C", new List<string> { "D" }, new List<GenreEnum> { GenreEnum.Action }, ScreeningEnum.TwoD,
            97, DateTime.Now.AddYears(-1));

        a.AddSequel(b);
        a.AddPrequel(c);

        // sanity
        Assert.That(a.Sequels, Is.EqualTo(b));
        Assert.That(a.Prequels, Is.EqualTo(c));

        a.RemoveReflexiveAssociations();

        Assert.That(a.Sequels, Is.EqualTo(null));
        Assert.That(a.Prequels, Is.EqualTo(null));
        Assert.That(c.Sequels, Is.EqualTo(null));
        Assert.That(b.Prequels, Is.EqualTo(null));
    }


    [Test]
    public void ReplaceSequel_RemovesOldReverseAndAddsNewReverse()
    {
        var a = new Movie("A", new List<string> { "D" }, new List<GenreEnum> { GenreEnum.Action }, ScreeningEnum.TwoD,
            100, DateTime.Now.AddYears(-1));
        var b = new Movie("B", new List<string> { "D" }, new List<GenreEnum> { GenreEnum.Action }, ScreeningEnum.TwoD,
            95, DateTime.Now.AddYears(-1));
        var c = new Movie("C", new List<string> { "D" }, new List<GenreEnum> { GenreEnum.Action }, ScreeningEnum.TwoD,
            97, DateTime.Now.AddYears(-1));

        a.AddSequel(b);
        Assert.That(b.Prequels, Is.EqualTo(a));

        a.RemoveSequel(b);
        a.AddSequel(c);

        Assert.That(a.Sequels, Is.EqualTo(c));
        Assert.That(c.Prequels, Is.EqualTo(a));
    }


    [Test]
    public void Delete_RemovesMovieFromOtherMoviesReflexiveLists()
    {
        var a = new Movie("A", new List<string> { "D" }, new List<GenreEnum> { GenreEnum.Comedy }, ScreeningEnum.TwoD,
            100, DateTime.Now.AddYears(-1));
        var b = new Movie("B", new List<string> { "D" }, new List<GenreEnum> { GenreEnum.Comedy }, ScreeningEnum.TwoD,
            95, DateTime.Now.AddYears(-1));
        a.AddSequel(b);

        a.Delete();

        Assert.That(b.Prequels, Is.EqualTo(null));
        Assert.IsFalse(Movie.Movies.Contains(a), "Deleted movie should be removed from extent");
    }
}