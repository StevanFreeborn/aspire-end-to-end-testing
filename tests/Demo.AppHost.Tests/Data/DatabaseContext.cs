namespace Demo.AppHost.Tests.Data;

public sealed class DatabaseContext : IAsyncDisposable
{
  public NpgsqlConnection Connection { get; }
  public NpgsqlTransaction Transaction { get; private set; }

  private DatabaseContext(string connectionString)
  {
    Connection = new NpgsqlConnection(connectionString);
    Connection.Open();
    Transaction = Connection.BeginTransaction();
  }

  public static DatabaseContext From(string connectionString)
  {
    return new(connectionString);
  }

  public async Task SaveChangesAsync()
  {
    try
    {
      await Transaction.CommitAsync();
    }
    catch
    {
      await Transaction.RollbackAsync();
      throw;
    }
    finally
    {
      await Transaction.DisposeAsync();
      Transaction = await Connection.BeginTransactionAsync();
    }
  }

  public async ValueTask DisposeAsync()
  {
    await Transaction.DisposeAsync();
    await Connection.DisposeAsync();
  }
}