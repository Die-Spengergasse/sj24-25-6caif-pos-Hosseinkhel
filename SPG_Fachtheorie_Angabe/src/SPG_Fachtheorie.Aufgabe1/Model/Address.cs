using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SPG_Fachtheorie.Aufgabe1.Model
{
    [ComplexType]
    public class Address
    {
        [Required]
        [MaxLength(255)]
        public string Street { get; set; }

        [Required]
        [MaxLength(255)]
        public string City { get; set; }

        [Required]
        [MaxLength(10)]
        public string PostalCode { get; set; }

        public Address(string street, string city, string postalCode)
        {
            Street = street;
            City = city;
            PostalCode = postalCode;
        }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        protected Address() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    }
}