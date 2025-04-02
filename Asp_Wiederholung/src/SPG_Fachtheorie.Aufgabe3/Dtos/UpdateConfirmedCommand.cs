using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SPG_Fachtheorie.Aufgabe3.Dtos
{
    public class UpdateConfirmedCommand : IValidatableObject
    {
        public DateTime? Confirmed { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Confirmed.HasValue)
            {
                // Check if the confirmed date is within 1 minute in the future
                if (Confirmed.Value > DateTime.UtcNow.AddMinutes(1))
                {
                    yield return new ValidationResult(
                        "Confirmed date cannot be more than 1 minute in the future",
                        new[] { nameof(Confirmed) }
                    );
                }
            }
        }
    }
}