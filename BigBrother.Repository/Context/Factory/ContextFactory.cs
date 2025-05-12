namespace BigBrother.Repository.Context.Factory;

public class ContextFactory : IContextFactory
{
    private readonly string _connectionString;

    public ContextFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public Context GetContext()
    {
        return new Context(_connectionString);
    }
}
