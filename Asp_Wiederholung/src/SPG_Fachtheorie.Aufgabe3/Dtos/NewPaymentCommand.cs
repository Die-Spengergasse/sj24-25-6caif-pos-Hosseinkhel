using System;
using System.ComponentModel.DataAnnotations;

namespace SPG_Fachtheorie.Aufgabe3.Dtos
{
    public class NewPaymentCommand
    {
        public int CashDeskNumber { get; set; }

        public DateTime PaymentDateTime { get; set; }

        [Required]
        public string PaymentType { get; set; } = null!;

        public int EmployeeRegistrationNumber { get; set; }

        public bool IsPaymentDateTimeValid()
        {
            return PaymentDateTime <= DateTime.Now.AddMinutes(1);
        }
    }
}