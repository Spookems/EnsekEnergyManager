using System.ComponentModel.DataAnnotations;

namespace EnsekEnergyManager.Classes
{
    public class Account
    {
        [Key]
        public int AccountId { get; set; }

        [StringLength(20)]
        public string FirstName { get; set; } = string.Empty;

        [StringLength(20)]
        public string LastName { get; set; } = string.Empty;
    }
}
