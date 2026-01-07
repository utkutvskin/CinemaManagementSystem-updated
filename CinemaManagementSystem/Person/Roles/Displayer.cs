using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using CinemaManagementSystem.Area;
using CinemaManagementSystem.AssociationClasses;
using CinemaManagementSystem.Enums;
using CinemaManagementSystem.Person;

namespace CinemaManagementSystem
{
    [Serializable]
    public class Displayer : Employee
    {

        private int _numberOfScreensManaged;

        public int NumbersOfScreensManaged
        {
            get => _numberOfScreensManaged;
            private set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Numbers of screens cannot be negative.");
                }

                _numberOfScreensManaged = value;
            }
        }
        
        
        //attribute association Displayer - Hall
        [XmlIgnore]
        private readonly List<DisplayerAssigment> _assigments = new();

        [XmlIgnore]
        public IReadOnlyCollection<DisplayerAssigment> Assigments => _assigments;
        
        
        public void ManageHall(Hall hall, string? description = null)
        {
            if (hall == null)
                throw new ArgumentException("Hall cannot be null.");
        
            DisplayerAssigment.Create(this, hall, DateTime.Now, DateTime.Now.TimeOfDay, description);
            NumbersOfScreensManaged++;
        }
        
        internal void AddDisplayerAssignmentInternal(DisplayerAssigment assigment)
        {
            _assigments.Add(assigment);
        }
        
        internal void RemoveDisplayerAssignmentInternal(DisplayerAssigment assigment)
        {
            _assigments.Remove(assigment);
        }
       

        // Constructors
        public Displayer() { }

        public Displayer(string name, string surname, DateTime birthDate, GenderEnum gender)
            : base(name, surname, birthDate, gender, Role.Displayer)
        {
            _numberOfScreensManaged = 0;
        }

    }
}