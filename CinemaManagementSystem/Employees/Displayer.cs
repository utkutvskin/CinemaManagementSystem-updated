using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CinemaManagementSystem
{
    [Serializable]
    public class Displayer : Employee
    {
        // Attribute from Diagram: NumberOfScreensManaged
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

        // Empty Constructor for XML Serialization
        public Displayer() { }

        // Parameterized Constructor chaining to base (Employee)
        public Displayer(string name, string surname, DateTime birthDate,
            DateTime startDate, double salary)
            : base(name, surname, birthDate, startDate, salary)
        {
            NumberOfScreensManaged = 0;
        }

        // Method from Diagram: manageSelectedScreens
        public void ManageSelectedScreens(List<Hall> screens)
        {
            // Main logic will be implemented later
            // For now, we just update the managed count based on the list size
            if (screens != null)
            {
                NumberOfScreensManaged = screens.Count;
            }
        }
    }
}    
        public void ManageSelectedScreens(List<Hall> screens)
        {
            // Main logic will be implemented later
            // For now, we just update the managed count based on the list size
            if (screens != null)
            {
                NumberOfScreensManaged = screens.Count;
            }
        }
    }
}
