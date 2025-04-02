using System;
using System.ComponentModel.DataAnnotations;

namespace SPG_Fachtheorie.Aufgabe3.Dtos
{
    public class UpdatePaymentItemCommand
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Id must be greater than 0")]
        public int Id { get; set; }

        [Required]
        [NotEmptyString(ErrorMessage = "ArticleName cannot be empty")]
        public string ArticleName { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public int Amount { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "PaymentId must be greater than 0")]
        public int PaymentId { get; set; }

        public DateTime? LastUpdated { get; set; }
    }

    // Custom validation attribute for non-empty string
    public class NotEmptyStringAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return value is string stringValue && !string.IsNullOrWhiteSpace(stringValue);
        }
    }
}