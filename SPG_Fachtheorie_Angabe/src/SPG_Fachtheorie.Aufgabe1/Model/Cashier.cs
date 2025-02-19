using System.ComponentModel.DataAnnotations;
using System;

namespace SPG_Fachtheorie.Aufgabe1.Model
{
    public class Cashier : Employee
    {
        [Required]
        public DateTime HireDate { get; set; }

        public Cashier(string registrationNumber, string firstName, string lastName, Address address, DateTime hireDate)
            : base(registrationNumber, firstName, lastName, address)
        {
            HireDate = hireDate;
            Type = "Cashier";
        }

        protected Cashier() { }
    }
}