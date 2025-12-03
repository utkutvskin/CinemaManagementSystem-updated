using System.Reflection.Metadata;

namespace CinemaManagementSystem.Employees
{
    [Serializable]
    public class Receptionist : Employee
    {
        private int _deskNumber;

        public int DeskNumber
        {
            get => _deskNumber;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Desk number must be positive.");
                _deskNumber = value;
            }
        }

        public Receptionist() { }

        public Receptionist(string name, string surname, DateTime birthDate,
            DateTime startDate, double salary, int deskNumber)
            : base(name, surname, birthDate, startDate, salary)
        {
            DeskNumber = deskNumber;
        }

        
    }
}