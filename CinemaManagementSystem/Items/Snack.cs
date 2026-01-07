using System;

namespace CinemaManagementSystem.Items
{
    [Serializable]
    public class Snack : Item
    {
        // Simple snack item (e.g. popcorn)
        private int _calories;

        public int Calories
        {
            get => _calories;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Calories cannot be negative");
                _calories = value;
            }
        }

        public Snack(string name, double price, int calories, int quantity)
            : base(name, price, quantity)
        {
            Calories = calories;
        }
    }
}
