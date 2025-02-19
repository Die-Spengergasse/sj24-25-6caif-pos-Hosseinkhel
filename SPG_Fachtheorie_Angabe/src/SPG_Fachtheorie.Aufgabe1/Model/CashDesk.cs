using System.ComponentModel.DataAnnotations;

namespace SPG_Fachtheorie.Aufgabe1.Model
{
    public class CashDesk
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Location { get; set; }

        public CashDesk(int id, string location)
        {
            Id = id;
            Location = location;
        }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        protected CashDesk() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    }
}