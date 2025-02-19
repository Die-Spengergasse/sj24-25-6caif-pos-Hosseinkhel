using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPG_Fachtheorie.Aufgabe1.Model
{
    public abstract class Employee
    {
        [Key]
        public string RegistrationNumber { get; set; }  // PK

        [Required]
        [MaxLength(255)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(255)]
        public string LastName { get; set; }

        public Address Address { get; set; }  // Value Object

        [Required]
        public string Type { get; protected set; }  // Discriminator (protected set)

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        protected Employee() { }  // EF Core benötigt einen parameterlosen Konstruktor
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public Employee(string registrationNumber, string firstName, string lastName, Address address)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        {
            RegistrationNumber = registrationNumber;
            FirstName = firstName;
            LastName = lastName;
            Address = address;
        }
    }
}