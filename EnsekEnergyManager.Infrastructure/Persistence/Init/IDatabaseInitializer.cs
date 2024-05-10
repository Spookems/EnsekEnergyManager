namespace Infrastructure.Persistence.Init
{
    public interface IDatabaseInitializer
    {
        Task InitializeDatabasesAsync(CancellationToken cancellationToken);
    }
}
