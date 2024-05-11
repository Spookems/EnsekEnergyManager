using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace EnsekEnergyManager.Classes
{
    public class Account /*: AuditableEntity, IAggregateRoot*/
    {
        [Key]
        public int AccountId { get; set; }

        [StringLength(20)]
        public string? FirstName { get; set; } = string.Empty;

        [StringLength(20)]
        public string? LastName { get; set; } = string.Empty;

        public Account Update(int? accountId, string? firstName, string? lastName)
        {
            if (accountId is not null && accountId.Equals(AccountId) is not true) AccountId = (int)accountId;
            if (firstName is not null && firstName.Equals(FirstName) is not true) FirstName = firstName;
            if (lastName is not null && lastName.Equals(LastName) is not true) LastName = lastName;

            return this;
        }
    }
}
