using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CinemaManagementSystem
{
    [Serializable]
    public class Displayer : Employee
    {
        
              [XmlIgnore]
        private readonly List<Hall> _managedHalls = new List<Hall>();
        
        [XmlIgnore]
        public IReadOnlyList<Hall> ManagedHalls => _managedHalls.AsReadOnly();
        
        public int NumberOfScreensManaged => _managedHalls.Count;
        
        public void AddHall(Hall hall)
        {
            if (hall == null)
                throw new ArgumentException("Hall cannot be null.");
        
            if (_managedHalls.Contains(hall))
                return;
        
            if (hall.ManagedBy != null && hall.ManagedBy != this)
                throw new InvalidOperationException(
                    $"Hall {hall.Number} is already managed by another displayer.");
        
            _managedHalls.Add(hall);
        
            if (hall.ManagedBy != this)
                hall.SetDisplayerInternal(this);
        }
        
        public void RemoveHall(Hall hall)
        {
            if (hall == null)
                throw new ArgumentException("Hall cannot be null.");
        
            if (_managedHalls.Contains(hall))
            {
                _managedHalls.Remove(hall);
        
                if (hall.ManagedBy == this)
                    hall.RemoveDisplayerInternal();
            }
        }
        
        internal void AddHallInternal(Hall hall)
        {
            if (!_managedHalls.Contains(hall))
                _managedHalls.Add(hall);
        }
        
        internal void RemoveHallInternal(Hall hall)
        {
            if (_managedHalls.Contains(hall))
                _managedHalls.Remove(hall);
        }

        // Constructors
        public Displayer() { }

        public Displayer(string name, string surname, DateTime birthDate, DateTime startDate, double salary)
            : base(name, surname, birthDate, startDate, salary)
        {
        }

        public void ManageSelectedScreens(List<Hall> screens)
        {
            if (screens == null) return;

            foreach (var hall in screens)
            {
                if (!_managedHalls.Contains(hall))
                {
                    try 
                    {
                        AddHall(hall);
                    }
                    catch 
                    {
                        // Hata olursa (örn: başkası yönetiyorsa) bu salonu atla
                        continue; 
                    }
                }
            }
        }
    }
}
