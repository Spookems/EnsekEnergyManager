using System.ComponentModel.DataAnnotations;

namespace EnsekEnergyManager.Classes
{
    public class Account /*: AuditableEntity, IAggregateRoot*/
    {
        public Account(int accountId, string firstName, string lastName)
        {
            AccountId = accountId;
            FirstName = firstName;
            LastName = lastName;
        }

        [Key]
        public int AccountId { get; set; }

        [StringLength(20)]
        public string FirstName { get; set; } = string.Empty;

        [StringLength(20)]
        public string LastName { get; set; } = string.Empty;
    }
}
