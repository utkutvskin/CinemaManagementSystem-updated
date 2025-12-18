using System;

namespace CinemaManagementSystem
{
    [Serializable]
    public class Snack : Item
    {
        // Simple snack item (e.g. popcorn)
        public int Calories { get; }

        public Snack(string name, double price, int calories)
            : base(name, price)
        {
            if (calories <= 0)
                throw new ArgumentException("Calories must be positive.");

            Calories = calories;
        }
    }
}
