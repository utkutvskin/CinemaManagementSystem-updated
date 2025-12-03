using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CinemaManagementSystem
{
    [Serializable]
    public class Displayer : Employee
    {
        private int _numberOfScreensManaged;

        public int NumberOfScreensManaged
        {
            get => _numberOfScreensManaged;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Number of screens managed cannot be negative.");
                _numberOfScreensManaged = value;
            }
        }

        public Displayer() { }

        public Displayer(string name, string surname, DateTime birthDate,
            DateTime startDate, double salary)
            : base(name, surname, birthDate, startDate, salary)
        {
            NumberOfScreensManaged = 0;
        }

        public void ManageSelectedScreens(List<Hall> screens)
        {
            if (screens != null)
            {
              
                NumberOfScreensManaged = screens.Count;
            }
        }
    }
}
