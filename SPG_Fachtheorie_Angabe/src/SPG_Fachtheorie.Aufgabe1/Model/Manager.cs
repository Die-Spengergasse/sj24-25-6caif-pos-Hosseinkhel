using System.ComponentModel.DataAnnotations;

namespace SPG_Fachtheorie.Aufgabe1.Model
{
    public class Manager : Employee
    {
        [Required]
        [MaxLength(255)]
        public string Department { get; set; }

        public Manager(string registrationNumber, string firstName, string lastName, Address address, string department)
            : base(registrationNumber, firstName, lastName, address)
        {
            Department = department;
            Type = "Manager";
        }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        protected Manager() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    }
}