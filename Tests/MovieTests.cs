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
        private string filePath ="movies_test.xml";

        [SetUp]
        public void Setup()
        {
           
            Movie.ClearExtent();
        }

        [Test]
        public void TitleSetterValidation_ShouldThrowException()
        {
            var directors = new List<string> { "Christopher Nolan" };
            var genres = new List<string> { "Sci-Fi", "Thriller" };
            var movie = new Movie("Inception", directors, genres, ScreeningEnum.IMAX, 148);

            Assert.Throws<ArgumentException>(() =>
                movie.Title = " "
            );
        }

        [Test]
        public void TitleSetterValidation_ShouldSetTitleSuccessfully()
        {
            var directors = new List<string> { "Christopher Nolan" };
            var genres = new List<string> { "Sci-Fi", "Thriller" };
            var movie = new Movie("Inception", directors, genres, ScreeningEnum.IMAX, 148);

            movie.Title = "Title";

            Assert.That(movie.Title, Is.EqualTo("Title"));
        }
        
        public void DirectorsSetterValidation_ShouldThrowException()
        {
            var directors = new List<string> { "Christopher Nolan" };
            var genres = new List<string> { "Sci-Fi", "Thriller" };
            var movie = new Movie("Inception", directors, genres, ScreeningEnum.IMAX, 148);

            Assert.Throws<ArgumentException>(() =>
                movie.Directors = null
            );
        }

        [Test]
        public void DirectorsSetterValidation_ShouldSetDirectorsSuccessfully()
        {
            var directors = new List<string> { "Christopher Nolan" };
            var genres = new List<string> { "Sci-Fi", "Thriller" };
            var movie = new Movie("Inception", directors, genres, ScreeningEnum.IMAX, 148);
            var d = new List<string> { "Director" };
            
            movie.Directors = d;

            Assert.That(movie.Directors, Is.EqualTo(d));
        }

        [Test]
        public void GenresSetterValidation_ShouldThrowException()
        {
            var directors = new List<string> { "Christopher Nolan" };
            var genres = new List<string> { "Sci-Fi", "Thriller" };
            var movie = new Movie("Inception", directors, genres, ScreeningEnum.IMAX, 148);

            Assert.Throws<ArgumentException>(() =>
                movie.Genres = null
            );
        }

        [Test]
        public void GenresSetterValidation_ShouldSetGenresSuccessfully()
        {
            var directors = new List<string> { "Christopher Nolan" };
            var genres = new List<string> { "Sci-Fi", "Thriller" };
            var movie = new Movie("Inception", directors, genres, ScreeningEnum.IMAX, 148);
            
            var gen = new List<string> { "Sci-Fi", "Action" };

            movie.Genres = gen;

            Assert.That(movie.Genres, Is.EqualTo(gen));
        }
        
        [Test]
        public void DurationSetterValidation_ShouldThrowException()
        {
            var directors = new List<string> { "Christopher Nolan" };
            var genres = new List<string> { "Sci-Fi", "Thriller" };
            var movie = new Movie("Inception", directors, genres, ScreeningEnum.IMAX, 148);

            Assert.Throws<ArgumentException>(() =>
                movie.Duration = 0
            );

        }

        [Test]
        public void DurationSetterValidation_ShouldSetDurationSuccessfully()
        {
            var directors = new List<string> { "Christopher Nolan" };
            var genres = new List<string> { "Sci-Fi", "Thriller" };
            var movie = new Movie("Inception", directors, genres, ScreeningEnum.IMAX, 148);

            movie.Duration = 150;

            Assert.That(movie.Duration, Is.EqualTo(150));
        }
        
        [Test]
        public void Constructor_ValidData_ShouldCreateMovie()
        {
            var directors = new List<string> { "Christopher Nolan" };
            var genres = new List<string> { "Sci-Fi", "Thriller" };
            var movie = new Movie("Inception", directors, genres, ScreeningEnum.IMAX, 148);

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
            var genres = new List<string> { "Drama" };
            Assert.Throws<ArgumentException>(() =>
                new Movie("", directors, genres, ScreeningEnum.TwoD, 120)
            );
        }

        [Test]
        public void Constructor_EmptyDirectors_ShouldThrowException()
        {
            var genres = new List<string> { "Drama" };
            Assert.Throws<ArgumentException>(() =>
                new Movie("No Director", null, genres, ScreeningEnum.TwoD, 120)
            );
        }

        [Test]
        public void Constructor_NegativeDuration_ShouldThrowException()
        {
            var directors = new List<string> { "James Cameron" };
            var genres = new List<string> { "Adventure" };
            Assert.Throws<ArgumentException>(() =>
                new Movie("Invalid", directors, genres, ScreeningEnum.TwoD, -120)
            );
        }

        [Test]
        public void ToString_ShouldContainTitleAndDirectorAndGenre()
        {
            var directors = new List<string> { "Lana Wachowski", "Lilly Wachowski" };
            var genres = new List<string> { "Sci-Fi", "Action" };
            var movie = new Movie("Matrix", directors, genres, ScreeningEnum.TwoD, 136);

            string result = movie.ToString();

            Assert.That(result, Does.Contain("Matrix"));
            Assert.That(result, Does.Contain("Lana Wachowski"));
            Assert.That(result, Does.Contain("Sci-Fi"));
        }

        [Test]
        public void SaveAndLoad_ShouldPersistMovies()
        {
            if (File.Exists(filePath))
                File.Delete(filePath); 
            
            var directors1 = new List<string> { "James Cameron" };
            var genres1 = new List<string> { "Adventure", "Sci-Fi" };
            var m1 = new Movie("Avatar", directors1, genres1, ScreeningEnum.ThreeD, 162);

            var directors2 = new List<string> { "James Cameron" };
            var genres2 = new List<string> { "Romance", "Drama" };
            var m2 = new Movie("Titanic", directors2, genres2, ScreeningEnum.TwoD, 195);

            Movie.Save(filePath);
            Movie.ClearExtent();
            Movie.Load(filePath);

            Assert.That(Movie.Movies.Count, Is.EqualTo(2));
            Assert.That(Movie.Movies[0].Title, Is.EqualTo("Avatar"));
            Assert.That(Movie.Movies[1].Title, Is.EqualTo("Titanic"));
        }
    }
}
