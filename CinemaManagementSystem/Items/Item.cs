using System;

namespace CinemaManagementSystem.Items
{
    [Serializable]
    public abstract class Item
    {
        //attributes
        private string _name;
        private double _price;
        private int _availableQuantity;

        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Name cannot be empty.");
                _name = value;
            }
        }

        public double Price
        {
            get => _price;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Price must be positive.");
                _price = value;
            }
        }

        public int AvailableQuantity
        {
            get => _availableQuantity;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Quantity must be positive.");
                _availableQuantity = value;
            }
        }

        protected Item(string name, double price, int availableQuantity)
        {
            Name = name;
            Price = price;
            AvailableQuantity = availableQuantity;
        }
        
        protected void AddQuantity(int quantity)
        {
            AvailableQuantity += quantity;
        }

        protected void SellItem()
        {
            AvailableQuantity -= 1;
        }
    }
}
