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

        Assert.IsTrue(a.Sequels.Contains(b), "A should list B as sequel");
        Assert.IsTrue(b.Prequels.Contains(a), "B should list A as prequel (reverse connection)");
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

        Assert.IsFalse(a.Sequels.Contains(b));
        Assert.IsFalse(b.Prequels.Contains(a));
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
        a.AddSequel(c);

        // sanity
        Assert.IsTrue(b.Prequels.Contains(a));
        Assert.IsTrue(c.Prequels.Contains(a));

        a.RemoveReflexiveAssociations();

        Assert.IsEmpty(a.Sequels);
        Assert.IsEmpty(a.Prequels);
        Assert.IsFalse(b.Prequels.Contains(a));
        Assert.IsFalse(c.Prequels.Contains(a));
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
        Assert.IsTrue(b.Prequels.Contains(a));

        a.RemoveSequel(b);
        a.AddSequel(c);

        Assert.IsFalse(a.Sequels.Contains(b));
        Assert.IsFalse(b.Prequels.Contains(a));

        Assert.IsTrue(a.Sequels.Contains(c));
        Assert.IsTrue(c.Prequels.Contains(a));
    }


    [Test]
    public void Delete_RemovesMovieFromOtherMoviesReflexiveLists()
    {
        var a = new Movie("A", new List<string> { "D" }, new List<GenreEnum> { GenreEnum.Comedy }, ScreeningEnum.TwoD,
            100, DateTime.Now.AddYears(-1));
        var b = new Movie("B", new List<string> { "D" }, new List<GenreEnum> { GenreEnum.Comedy }, ScreeningEnum.TwoD,
            95, DateTime.Now.AddYears(-1));
        a.AddSequel(b);

        // call Delete() — expects implementation to clean reflexive associations
        a.Delete();

        Assert.IsFalse(b.Prequels.Contains(a),
            "After delete, other movies should no longer reference deleted movie as prequel");
        Assert.IsFalse(Movie.Movies.Contains(a), "Deleted movie should be removed from extent");
    }
}