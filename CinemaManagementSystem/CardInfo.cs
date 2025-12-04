using System;
using System.Linq;
using System.Xml.Serialization;
using CinemaManagementSystem.PersistenceForAllClasses;

namespace CinemaManagementSystem
{
    [Serializable]
    public class CardInfo 
    {
        private string _name;
        private string _number;
        private DateTime _expiryDate;
        private string _pinCode;

        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Card holder name cannot be empty.");
                _name = value;
            }
        }

        public string Number
        {
            get => _number;
            set
            {
                if (value.Length != 16)
                    throw new ArgumentException("Card number must have 16 digits.");
                if(!value.All(char.IsDigit))
                    throw new ArgumentException("Card number must contain digits.");
                _number = value;
            }
        }

        public DateTime ExpiryDate
        {
            get => _expiryDate;
            set
            {
                if (value <= DateTime.Now)
                    throw new ArgumentException("Card expiry date must be in the future.");
                _expiryDate = value;
            }
        }

        public string PINcode
        {
            get => _pinCode;
            set
            {
                if(!value.All(char.IsDigit))
                    throw new ArgumentException("Pin code must contain digits.");
                if (value.Length != 3)
                    throw new ArgumentException("PIN code must have 3 characters.");
                _pinCode = value;
            }
        }

        //Constructors
        public CardInfo() { } 

        public CardInfo(string name, string number, DateTime expiryDate, string pinCode)
        {
            Name = name;
            Number = number;
            ExpiryDate = expiryDate;
            PINcode = pinCode;
        }

        public override string ToString()
        {
            return $"Card Holder: {Name}, Number: {Number}, Expires: {ExpiryDate:MM/yyyy}";
        }

    }
}
