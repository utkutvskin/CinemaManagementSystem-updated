using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using CinemaManagementSystem;

namespace CinemaManagementSystem.Tests
{
    [TestFixture]
    public class MovieTests
    {
        private string _filePath;

        [SetUp]
        public void Setup()
        {
            // Her test için benzersiz temp dosyası
            _filePath = Path.Combine(Path.GetTempPath(), $"movies_test_{Guid.NewGuid()}.xml");

            // Temiz statik durum
            Movie.ClearAllMovies();
        }

        [TearDown]
        public void Cleanup()
        {
            try
            {
                if (File.Exists(_filePath))
                    File.Delete(_filePath);
            }
            catch { /* ignore cleanup errors */ }

            Movie.ClearAllMovies();
        }

        [Test]
        public void Constructor_ValidData_ShouldCreateMovie()
        {
            var directors = new List<string> { "Christopher Nolan" };
            var genres = new List<string> { "Sci-Fi", "Thriller" };
            var movie = new Movie("Inception", directors, genres, "IMAX", 148, new DateTime(2010, 7, 16));

            Assert.That(movie.Title, Is.EqualTo("Inception"));
            Assert.That(movie.Directors.Count, Is.EqualTo(1));
            Assert.That(movie.Genres.Count, Is.EqualTo(2));
            Assert.That(movie.Duration, Is.EqualTo(148));
            Assert.That(movie.ScreeningType, Is.EqualTo("IMAX"));
            Assert.That(movie.AgeInYears, Is.GreaterThan(10));

            // constructor adds to static list
            Assert.That(Movie.Movies.Count, Is.EqualTo(1));
            Assert.That(Movie.Movies[0], Is.EqualTo(movie));
        }

        [Test]
        public void Constructor_EmptyTitle_ShouldThrowException()
        {
            var directors = new List<string> { "Unknown" };
            var genres = new List<string> { "Drama" };
            Assert.Throws<ArgumentException>(() =>
                new Movie("", directors, genres, "2D", 120, DateTime.Now.AddYears(-1))
            );
        }

        [Test]
        public void Constructor_EmptyDirectors_ShouldThrowException()
        {
            var genres = new List<string> { "Drama" };
            Assert.Throws<ArgumentException>(() =>
                new Movie("No Director", new List<string>(), genres, "2D", 120, DateTime.Now.AddYears(-1))
            );
        }

        [Test]
        public void Constructor_FutureReleaseDate_ShouldThrowException()
        {
            var directors = new List<string> { "Future Man" };
            var genres = new List<string> { "Action" };
            Assert.Throws<ArgumentException>(() =>
                new Movie("Future Film", directors, genres, "3D", 100, DateTime.Now.AddMonths(6))
            );
        }

        [Test]
        public void Constructor_NegativeDuration_ShouldThrowException()
        {
            var directors = new List<string> { "James Cameron" };
            var genres = new List<string> { "Adventure" };
            Assert.Throws<ArgumentException>(() =>
                new Movie("Invalid", directors, genres, "2D", -120, new DateTime(2010, 1, 1))
            );
        }

        [Test]
        public void ToString_ShouldContainTitleAndDirectorAndGenre()
        {
            var directors = new List<string> { "Lana Wachowski", "Lilly Wachowski" };
            var genres = new List<string> { "Sci-Fi", "Action" };
            var movie = new Movie("Matrix", directors, genres, "2D", 136, new DateTime(1999, 3, 31));

            string result = movie.ToString();

            Assert.That(result, Does.Contain("Matrix"));
            Assert.That(result, Does.Contain("Lana Wachowski"));
            Assert.That(result, Does.Contain("Sci-Fi"));
        }

        [Test]
        public void SaveAndLoad_ShouldPersistMovies()
        {
            Movie.ClearAllMovies();

            var directors1 = new List<string> { "James Cameron" };
            var genres1 = new List<string> { "Adventure", "Sci-Fi" };
            var m1 = new Movie("Avatar", directors1, genres1, "3D", 162, new DateTime(2009, 12, 18));

            var directors2 = new List<string> { "James Cameron" };
            var genres2 = new List<string> { "Romance", "Drama" };
            var m2 = new Movie("Titanic", directors2, genres2, "2D", 195, new DateTime(1997, 12, 19));

            Movie.Save(_filePath);

            // Clear in-memory list to ensure Load restores data
            Movie.ClearAllMovies();
            Assert.That(Movie.Movies.Count, Is.EqualTo(0));

            Movie.Load(_filePath);

            Assert.That(Movie.Movies.Count, Is.EqualTo(2));
            Assert.That(Movie.Movies[0].Title, Is.EqualTo("Avatar"));
            Assert.That(Movie.Movies[1].Title, Is.EqualTo("Titanic"));
        }
    }
}
