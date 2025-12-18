using System;

namespace CinemaManagementSystem
{
    [Serializable]
    public abstract class Item
    {
        // Base class for sellable items
        public string Name { get; protected set; }
        public double Price { get; protected set; }

        protected Item(string name, double price)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty.");

            if (price <= 0)
                throw new ArgumentException("Price must be positive.");

            Name = name;
            Price = price;
        }
    }
}
