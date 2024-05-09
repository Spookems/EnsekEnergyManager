namespace EnsekEnergyManager.Classes
{
    public class Account
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? Email { get; set; }
    }
}
