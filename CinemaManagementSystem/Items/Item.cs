using System;
using System.Xml.Serialization;

namespace CinemaManagementSystem.Items
{
    [Serializable]

    // INHERITANCE IMPLEMENTATION: Polymorphism Support for XML Serialization
    // These attributes register the derived classes (Snack, Glass3D) to the serializer.
    // This ensures that when we save a List<Item>, the system remembers whether
    // a specific item is a Snack or a Glass3D.
    [XmlInclude(typeof(Snack))]
    [XmlInclude(typeof(Glass3D))]
    
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
