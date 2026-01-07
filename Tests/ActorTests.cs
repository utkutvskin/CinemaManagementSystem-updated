using System;
using System.IO;
using NUnit.Framework;
using CinemaManagementSystem;
using CinemaManagementSystem.Enums;
using CinemaManagementSystem.Person;

namespace CinemaManagementSystem.Tests
{
    [TestFixture]
    public class ActorTests
    {
        private string filePath = "actors_test.xml";

        [SetUp]
        public void SetUp()
        {
            
            Actor.ClearAllActors();
        }

        //Test Name/Surname setter 
        [Test]
        public void NameSetterValidation_ShouldThrowException()
        {
            Actor actor = new Actor("John", "Doe", GenderEnum.Men, new DateTime(1990, 5, 10));
            Assert.Throws<ArgumentException>(() =>
                actor.Name = " "
            );
        }
        [Test]
        public void SurnameSetterValidation_ShouldSetSurnameSuccessfully()
        {
            Actor actor = new Actor("John", "Doe", GenderEnum.Men, new DateTime(1990, 5, 10));
            actor.Surname = "Smith";
            Assert.That(actor.Surname, Is.EqualTo("Smith"));
        }
        
        //Test Gender setter
        
        [Test]
        public void GenderSetterValidation_ShouldSetGenderSuccessfully()
        {
            Actor actor = new Actor("John", "Doe", GenderEnum.Men, new DateTime(1990, 5, 10));
            actor.Gender = GenderEnum.Other;
            
            Assert.That(actor.Gender, Is.EqualTo(GenderEnum.Other));

        }
        
        //Test BirthDay setter
        [Test]
        public void BirthDaySetterValidation_ShouldThrowException()
        {
            Actor actor = new Actor("John", "Doe", GenderEnum.Men, new DateTime(1990, 5, 10));
            Assert.Throws<ArgumentException>(() =>
                actor.BirthDate = new DateTime(2030, 3,4)
            );

        }

        [Test]
        public void BirthDateSetterValidation_ShouldSetBirthDateSuccessfully()
        {
            Actor actor = new Actor("John", "Doe", GenderEnum.Men, new DateTime(1990, 5, 10));
            actor.BirthDate = new DateTime(2005, 3, 4);
            Assert.That(actor.BirthDate, Is.EqualTo(new DateTime(2005, 3, 4)));
        }

        //Test Constructor
        [Test]
        public void Constructor_ValidData_ShouldCreateActor()
        {
            var actor = new Actor("John", "Doe", GenderEnum.Men, new DateTime(1990, 5, 10));

            Assert.AreEqual("John", actor.Name);
            Assert.AreEqual("Doe", actor.Surname);
            Assert.AreEqual(GenderEnum.Men, actor.Gender);
            Assert.AreEqual(1990, actor.BirthDate.Year);
        }

        [Test]
        public void Constructor_EmptyName_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() =>
                new Actor("", "Doe", GenderEnum.Men, new DateTime(1990, 5, 10))
            );
        }

        [Test]
        public void Constructor_EmptySurname_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() =>
                new Actor("John", "", GenderEnum.Men, new DateTime(1990, 5, 10))
            );
        }

        [Test]
        public void Constructor_IncorectBirthDate_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() =>
                new Actor("John", "Doe", GenderEnum.Men, DateTime.Now.AddYears(2))
            );
        }

        [Test]
        public void Age_ShouldCalculateCorrectly()
        {
            var birthDate = new DateTime(2000, 1, 1);
            var actor = new Actor("Jane", "Smith", GenderEnum.Men, birthDate);

            int expectedAge = DateTime.Now.Year - 2000;
            if (DateTime.Now.DayOfYear < birthDate.DayOfYear)
                expectedAge--;

            Assert.AreEqual(expectedAge, actor.Age);
        }

        [Test]
        public void ToString_ShouldReturnFormattedString()
        {
            var actor = new Actor("John", "Doe", GenderEnum.Men, new DateTime(1990, 5, 10));
            string text = actor.ToString();
            StringAssert.Contains("John Doe", text);
            StringAssert.Contains(GenderEnum.Men.ToString(), text);
        }

        [Test]
        public void SaveAndLoad_ShouldPersistActors()
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
            
            new Actor("John", "Doe", GenderEnum.Men, new DateTime(1990, 5, 10));
            new Actor("Jane", "Smith", GenderEnum.Female, new DateTime(1985, 3, 20));

            Actor.Save(filePath);
            Assert.That(File.Exists(filePath));

            Actor.ClearAllActors();

            Actor.Load(filePath);

            Assert.That(Actor.Actors.Count, Is.EqualTo(2));
            Assert.That(Actor.Actors[0].Name, Is.EqualTo("John"));
            Assert.That(Actor.Actors[1].Name, Is.EqualTo("Jane"));
        }
    }
}