using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SPG_Fachtheorie.Aufgabe1.Model
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        public List<PaymentItem> PaymentItems { get; set; } = new List<PaymentItem>();

        public Payment(int id, DateTime paymentDate, decimal totalAmount)
        {
            Id = id;
            PaymentDate = paymentDate;
            TotalAmount = totalAmount;
        }

        protected Payment() { }
    }
}