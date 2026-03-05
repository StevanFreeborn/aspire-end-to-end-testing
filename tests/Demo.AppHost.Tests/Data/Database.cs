namespace Demo.AppHost.Tests.Data;

public sealed class Database : IAsyncDisposable
{
  private readonly DatabaseContext _ctx;
  public TodoRepository Todos { get; }

  private Database(string connectionString)
  {
    _ctx = DatabaseContext.From(connectionString);
    Todos = TodoRepository.From(_ctx);
  }

  public static Database From(string connectionString)
  {
    return new(connectionString);
  }

  public Task SaveChangesAsync()
  {
    return _ctx.SaveChangesAsync();
  }

  public async ValueTask DisposeAsync()
  {
    await _ctx.DisposeAsync();
  }
}
