using System;
using System.IO;
using NUnit.Framework;
using CinemaManagementSystem;

namespace CinemaManagementSystem.Tests
{
    [TestFixture]
    public class ActorTests
    {
        [SetUp]
        public void SetUp()
        {
            Actor.ClearAllActors();
        }

        [TearDown]
        public void TearDown()
        {
            Actor.ClearAllActors();
        }

        [Test]
        public void Name_SetEmpty_ThrowsArgumentException()
        {
            var actor = new Actor();
            Assert.Throws<ArgumentException>(() => actor.Name = "");
            Assert.Throws<ArgumentException>(() => actor.Name = "   ");
        }

        [Test]
        public void Surname_SetEmpty_ThrowsArgumentException()
        {
            var actor = new Actor();
            Assert.Throws<ArgumentException>(() => actor.Surname = "");
            Assert.Throws<ArgumentException>(() => actor.Surname = "   ");
        }

        [Test]
        public void Gender_SetEmpty_ThrowsArgumentException()
        {
            var actor = new Actor();
            Assert.Throws<ArgumentException>(() => actor.Gender = "");
            Assert.Throws<ArgumentException>(() => actor.Gender = "   ");
        }

        [Test]
        public void BirthDate_InFuture_ThrowsArgumentException()
        {
            var actor = new Actor();
            Assert.Throws<ArgumentException>(() => actor.BirthDate = DateTime.Now.AddDays(1));
        }

        [Test]
        public void Age_Computed_From_BirthDate_Correctly()
        {
            // Sabit bir doğum tarihi kullanarak testin sağlam kalmasını sağlıyoruz
            var birth = new DateTime(1990, 6, 15);
            var actor = new Actor("Test", "User", "M", birth);

            var today = DateTime.Today;
            int expectedAge = today.Year - birth.Year;
            if (birth > today.AddYears(-expectedAge)) expectedAge--;

            Assert.AreEqual(expectedAge, actor.Age);
        }

        [Test]
        public void SaveAndLoad_PreservesFields_And_Age_Computed_After_Load()
        {
            var birth = new DateTime(1988, 3, 20);
            var actor = new Actor("Alice", "Smith", "F", birth);

            var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".xml");
            try
            {
                Actor.Save(tempFile);

                // Belleği temizle ve dosyadan yükle
                Actor.ClearAllActors();
                Assert.IsEmpty(Actor.Actors);

                Actor.Load(tempFile);

                Assert.AreEqual(1, Actor.Actors.Count);
                var loaded = Actor.Actors[0];

                Assert.AreEqual("Alice", loaded.Name);
                Assert.AreEqual("Smith", loaded.Surname);
                Assert.AreEqual("F", loaded.Gender);
                Assert.AreEqual(birth, loaded.BirthDate);

                // Age dosyadan gelmez; BirthDate'e göre hesaplanır
                var today = DateTime.Today;
                int expectedAge = today.Year - birth.Year;
                if (birth > today.AddYears(-expectedAge)) expectedAge--;
                Assert.AreEqual(expectedAge, loaded.Age);
            }
            finally
            {
                if (File.Exists(tempFile)) File.Delete(tempFile);
            }
        }
    }
}
