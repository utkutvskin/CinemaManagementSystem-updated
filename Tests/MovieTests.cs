using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using CinemaManagementSystem;
using CinemaManagementSystem.Enums;

namespace CinemaManagementSystem.Tests
{
    [TestFixture]
    public class MovieTests
    {
        private string filePath = "movies_test.xml";

        [SetUp]
        public void Setup()
        {
            Movie.ClearExtent();
        }

        [Test]
        public void AddDirectorMethod_ShouldThrowException()
        {
            var directors = new List<string> { "Christopher Nolan" };
            var genres = new List<GenreEnum> { GenreEnum.Sci_fi, GenreEnum.Thriller };
            var movie = new Movie("Inception", directors, genres, ScreeningEnum.IMAX, 148, new DateTime(2025, 12, 3));

            Assert.Throws<ArgumentException>(() =>
                movie.AddDirector("  ")
            );
        }

        [Test]
        public void AddDirectorMethod_ShouldAddNewDirectorSuccessfully()
        {
            var directors = new List<string> { "Christopher Nolan" };
            var genres = new List<GenreEnum> { GenreEnum.Sci_fi, GenreEnum.Thriller };
            var movie = new Movie("Inception", directors, genres, ScreeningEnum.IMAX, 148, new DateTime(2025, 12, 3));

            movie.AddDirector("New Director");

            Assert.That(movie.Directors, Is.EqualTo(new List<string>()
            {
                "Christopher Nolan",
                "New Director"
            }));
        }

        [Test]
        public void AddDGenreMethod_ShouldThrowException()
        {
            var directors = new List<string> { "Christopher Nolan" };
            var genres = new List<GenreEnum> { GenreEnum.Sci_fi, GenreEnum.Thriller };
            var movie = new Movie("Inception", directors, genres, ScreeningEnum.IMAX, 148, new DateTime(2025, 4, 3));

            Assert.Throws<ArgumentException>(() =>
                movie.AddGenres(GenreEnum.Sci_fi)
            );
        }

        [Test]
        public void AddGenreMethod_ShouldAddNewGenreSuccessfully()
        {
            var directors = new List<string> { "Christopher Nolan" };
            var genres = new List<GenreEnum> { GenreEnum.Sci_fi, GenreEnum.Thriller };
            var movie = new Movie("Inception", directors, genres, ScreeningEnum.IMAX, 148, new DateTime(2025, 12, 3));

            movie.AddGenres(GenreEnum.Comedy);

            Assert.That(movie.Genres.Count, Is.EqualTo(3));
        }

        [Test]
        public void TitleSetterValidation_ShouldThrowException()
        {
            var directors = new List<string> { "Christopher Nolan" };
            var genres = new List<GenreEnum> { GenreEnum.Sci_fi, GenreEnum.Thriller };
            var movie = new Movie("Inception", directors, genres, ScreeningEnum.IMAX, 148, new DateTime(2025, 12, 3));

            Assert.Throws<ArgumentException>(() =>
                movie.Title = " "
            );
        }

        [Test]
        public void TitleSetterValidation_ShouldSetTitleSuccessfully()
        {
            var directors = new List<string> { "Christopher Nolan" };
            var genres = new List<GenreEnum> { GenreEnum.Sci_fi, GenreEnum.Thriller };
            var movie = new Movie("Inception", directors, genres, ScreeningEnum.IMAX, 148, new DateTime(2025, 12, 3));

            movie.Title = "Title";

            Assert.That(movie.Title, Is.EqualTo("Title"));
        }

        [Test]
        public void DirectorsSetterValidation_ShouldThrowException()
        {
            var directors = new List<string> { "Christopher Nolan" };
            var genres = new List<GenreEnum> { GenreEnum.Sci_fi, GenreEnum.Thriller };
            var movie = new Movie("Inception", directors, genres, ScreeningEnum.IMAX, 148, new DateTime(2025, 12, 3));

            Assert.Throws<ArgumentException>(() =>
                movie.Directors = null
            );
        }

        [Test]
        public void DirectorsSetterValidation_ShouldSetDirectorsSuccessfully()
        {
            var directors = new List<string> { "Christopher Nolan" };
            var genres = new List<GenreEnum> { GenreEnum.Sci_fi, GenreEnum.Thriller };
            var movie = new Movie("Inception", directors, genres, ScreeningEnum.IMAX, 148, new DateTime(2025, 12, 3));
            var d = new List<string> { "Director" };

            movie.Directors = d;

            Assert.That(movie.Directors, Is.EqualTo(d));
        }

        [Test]
        public void GenresSetterValidation_ShouldThrowException()
        {
            var directors = new List<string> { "Christopher Nolan" };
            var genres = new List<GenreEnum> { GenreEnum.Sci_fi, GenreEnum.Thriller };
            var movie = new Movie("Inception", directors, genres, ScreeningEnum.IMAX, 148, new DateTime(2025, 12, 3));

            Assert.Throws<ArgumentException>(() =>
                movie.Genres = null
            );
        }

        [Test]
        public void GenresSetterValidation_ShouldSetGenresSuccessfully()
        {
            var directors = new List<string> { "Christopher Nolan" };
            var genres = new List<GenreEnum> { GenreEnum.Sci_fi, GenreEnum.Thriller };
            var movie = new Movie("Inception", directors, genres, ScreeningEnum.IMAX, 148, new DateTime(2025, 12, 3));

            var gen = new List<GenreEnum> { GenreEnum.Sci_fi, GenreEnum.Comedy };

            movie.Genres = gen;

            Assert.That(movie.Genres, Is.EqualTo(gen));
        }

        [Test]
        public void DurationSetterValidation_ShouldThrowException()
        {
            var directors = new List<string> { "Christopher Nolan" };
            var genres = new List<GenreEnum> { GenreEnum.Sci_fi, GenreEnum.Thriller };
            var movie = new Movie("Inception", directors, genres, ScreeningEnum.IMAX, 148, new DateTime(2025, 12, 3));

            Assert.Throws<ArgumentException>(() =>
                movie.Duration = 0
            );
        }

        [Test]
        public void DurationSetterValidation_ShouldSetDurationSuccessfully()
        {
            var directors = new List<string> { "Christopher Nolan" };
            var genres = new List<GenreEnum> { GenreEnum.Sci_fi, GenreEnum.Thriller };
            var movie = new Movie("Inception", directors, genres, ScreeningEnum.IMAX, 148, new DateTime(2025, 12, 3));

            movie.Duration = 150;

            Assert.That(movie.Duration, Is.EqualTo(150));
        }

        [Test]
        public void Constructor_ValidData_ShouldCreateMovie()
        {
            var directors = new List<string> { "Christopher Nolan" };
            var genres = new List<GenreEnum> { GenreEnum.Sci_fi, GenreEnum.Thriller };
            var movie = new Movie("Inception", directors, genres, ScreeningEnum.IMAX, 148, new DateTime(2025, 12, 3));

            Assert.That(movie.Title, Is.EqualTo("Inception"));
            Assert.That(movie.Directors.Count, Is.EqualTo(1));
            Assert.That(movie.Genres.Count, Is.EqualTo(2));
            Assert.That(movie.Duration, Is.EqualTo(148));
            Assert.That(movie.ScreeningType, Is.EqualTo(ScreeningEnum.IMAX));
        }

        [Test]
        public void Constructor_EmptyTitle_ShouldThrowException()
        {
            var directors = new List<string> { "Unknown" };
            var genres = new List<GenreEnum> { GenreEnum.Drama };
            Assert.Throws<ArgumentException>(() =>
                new Movie("", directors, genres, ScreeningEnum.TwoD, 120, new DateTime(2025, 12, 3))
            );
        }

        [Test]
        public void Constructor_EmptyDirectors_ShouldThrowException()
        {
            var genres = new List<GenreEnum> { GenreEnum.Drama };
            Assert.Throws<ArgumentException>(() =>
                new Movie("No Director", null, genres, ScreeningEnum.TwoD, 120, new DateTime(2025, 12, 3))
            );
        }

        [Test]
        public void Constructor_NegativeDuration_ShouldThrowException()
        {
            var directors = new List<string> { "James Cameron" };
            var genres = new List<GenreEnum> { GenreEnum.Drama };
            Assert.Throws<ArgumentException>(() =>
                new Movie("Invalid", directors, genres, ScreeningEnum.TwoD, -120, new DateTime(2025, 12, 3))
            );
        }

        [Test]
        public void ToString_ShouldContainTitleAndDirectorAndGenre()
        {
            var directors = new List<string> { "Lana Wachowski", "Lilly Wachowski" };
            var genres = new List<GenreEnum> { GenreEnum.Sci_fi, GenreEnum.Thriller };
            var movie = new Movie("Matrix", directors, genres, ScreeningEnum.TwoD, 136, new DateTime(2025, 12, 3));

            string result = movie.ToString();

            Assert.That(result, Does.Contain("Matrix"));
            Assert.That(result, Does.Contain("Lana Wachowski"));
            Assert.That(result, Does.Contain(GenreEnum.Sci_fi.ToString()));
        }

        [Test]
        public void AddDirector_AfterReleaseDate_ShouldThrowException()
        {
            var directors = new List<string> { "Lana Wachowski" };
            var genres = new List<GenreEnum> { GenreEnum.Sci_fi };
            var oldDate = DateTime.Now.AddDays(-1);
            var movie = new Movie("Matrix", directors, genres, ScreeningEnum.TwoD, 136, oldDate);
            Assert.Throws<InvalidOperationException>(() =>
                movie.AddDirector("Lilly Wachowski")
            );
        }

     
     
        
        // REFLEXIVE ASSOCIATION TESTS 
        [Test]
        public void AddSequel_CreatesReversePrequel()
        {
            var a = new Movie("A", new List<string> { "D" }, new List<GenreEnum> { GenreEnum.Action }, ScreeningEnum.TwoD, 100, DateTime.Now.AddYears(-1));
            var b = new Movie("B", new List<string> { "D" }, new List<GenreEnum> { GenreEnum.Action }, ScreeningEnum.TwoD, 95, DateTime.Now.AddYears(-1));

            a.AddSequel(b);

            Assert.IsTrue(a.Sequels.Contains(b), "A should list B as sequel");
            Assert.IsTrue(b.Prequels.Contains(a), "B should list A as prequel (reverse connection)");
            Assert.IsTrue(a.SequelIds.Contains(b.Id));
            Assert.IsTrue(b.PrequelIds.Contains(a.Id));
        }

      
        [Test]
        public void RemoveSequel_RemovesReversePrequel()
        {
            var a = new Movie("A", new List<string> { "D" }, new List<GenreEnum> { GenreEnum.Comedy }, ScreeningEnum.TwoD, 100, DateTime.Now.AddYears(-1));
            var b = new Movie("B", new List<string> { "D" }, new List<GenreEnum> { GenreEnum.Comedy }, ScreeningEnum.TwoD, 95, DateTime.Now.AddYears(-1));
            a.AddSequel(b);

            a.RemoveSequel(b);

            Assert.IsFalse(a.Sequels.Contains(b));
            Assert.IsFalse(b.Prequels.Contains(a));
            Assert.IsFalse(a.SequelIds.Contains(b.Id));
            Assert.IsFalse(b.PrequelIds.Contains(a.Id));
        }

       
        [Test]
        public void AddSequel_ErrorCases_ThrowsAppropriateExceptions()
        {
            var a = new Movie("A", new List<string> { "D" }, new List<GenreEnum> { GenreEnum.Drama }, ScreeningEnum.TwoD, 100, DateTime.Now.AddYears(-1));
            var b = new Movie("B", new List<string> { "D" }, new List<GenreEnum> { GenreEnum.Drama }, ScreeningEnum.TwoD, 95, DateTime.Now.AddYears(-1));

            // null
            Assert.Throws<ArgumentException>(() => a.AddSequel(null));

            // self
            Assert.Throws<InvalidOperationException>(() => a.AddSequel(a));

            // duplicate
            a.AddSequel(b);
            Assert.Throws<InvalidOperationException>(() => a.AddSequel(b));
        }

       
        [Test]
        public void RemoveReflexiveAssociations_ClearsBothSides()
        {
            var a = new Movie("A", new List<string> { "D" }, new List<GenreEnum> { GenreEnum.Action }, ScreeningEnum.TwoD, 100, DateTime.Now.AddYears(-1));
            var b = new Movie("B", new List<string> { "D" }, new List<GenreEnum> { GenreEnum.Action }, ScreeningEnum.TwoD, 95, DateTime.Now.AddYears(-1));
            var c = new Movie("C", new List<string> { "D" }, new List<GenreEnum> { GenreEnum.Action }, ScreeningEnum.TwoD, 97, DateTime.Now.AddYears(-1));

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
            Assert.IsEmpty(a.SequelIds);
            Assert.IsEmpty(a.PrequelIds);
        }

       
        [Test]
        public void ReplaceSequel_RemovesOldReverseAndAddsNewReverse()
        {
            var a = new Movie("A", new List<string> { "D" }, new List<GenreEnum> { GenreEnum.Action }, ScreeningEnum.TwoD, 100, DateTime.Now.AddYears(-1));
            var b = new Movie("B", new List<string> { "D" }, new List<GenreEnum> { GenreEnum.Action }, ScreeningEnum.TwoD, 95, DateTime.Now.AddYears(-1));
            var c = new Movie("C", new List<string> { "D" }, new List<GenreEnum> { GenreEnum.Action }, ScreeningEnum.TwoD, 97, DateTime.Now.AddYears(-1));

            a.AddSequel(b);
            Assert.IsTrue(b.Prequels.Contains(a));

            // replace: önce eskiyi kaldır, sonra yeniyi ekle
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
            var a = new Movie("A", new List<string> { "D" }, new List<GenreEnum> { GenreEnum.Comedy }, ScreeningEnum.TwoD, 100, DateTime.Now.AddYears(-1));
            var b = new Movie("B", new List<string> { "D" }, new List<GenreEnum> { GenreEnum.Comedy }, ScreeningEnum.TwoD, 95, DateTime.Now.AddYears(-1));
            a.AddSequel(b);

            // call Delete() — expects implementation to clean reflexive associations
            a.Delete();

            Assert.IsFalse(b.Prequels.Contains(a), "After delete, other movies should no longer reference deleted movie as prequel");
            Assert.IsFalse(Movie.Movies.Contains(a), "Deleted movie should be removed from extent");
        }
        
        
        
        
        
        
        [Test]
        public void SaveAndLoad_ShouldPersistMovies()
        {
            if (File.Exists(filePath))
                File.Delete(filePath);

            var directors1 = new List<string> { "James Cameron" };
            var genres1 = new List<GenreEnum> { GenreEnum.Sci_fi, GenreEnum.Thriller };
            var m1 = new Movie("Avatar", directors1, genres1, ScreeningEnum.ThreeD, 162, new DateTime(2025, 12, 3));

            var directors2 = new List<string> { "James Cameron" };
            var genres2 = new List<GenreEnum> { GenreEnum.Sci_fi, GenreEnum.Thriller };
            var m2 = new Movie("Titanic", directors2, genres2, ScreeningEnum.TwoD, 195, new DateTime(2025, 12, 3));

            Movie.Save(filePath);
            Movie.ClearExtent();
            Movie.Load(filePath);

            Assert.That(Movie.Movies.Count, Is.EqualTo(2));
            Assert.That(Movie.Movies[0].Title, Is.EqualTo("Avatar"));
            Assert.That(Movie.Movies[1].Title, Is.EqualTo("Titanic"));
        }
    }
}

