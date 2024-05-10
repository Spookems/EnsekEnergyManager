using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnsekEnergyManager.Classes
{
    public class MeterReading
    {
        [Required]
        [Key]
        public Guid Id { get; set; }

        [Required]
        [ForeignKey("AccountId")]
        public Account? Account { get; set; }

        [Required]
        public DateTime? MeterReadingDateTime { get; set; }

        [StringLength(10)]
        public string? MeterReadValue { get; set; } = string.Empty;
    }
}
