namespace Demo.AppHost.Tests.Data;

public sealed class TodoRepository
{
  private readonly DatabaseContext _ctx;

  private TodoRepository(DatabaseContext ctx)
  {
    _ctx = ctx;
  }

  public static TodoRepository From(DatabaseContext ctx)
  {
    return new(ctx);
  }

  public async Task<int> AddAsync(Todo todo)
  {
    var sql = @"
      INSERT INTO todos (task, completed)
      VALUES (@Task, @Completed)
      RETURNING id;
    ";

    return await _ctx.Connection.QuerySingleAsync<int>(sql, todo, _ctx.Transaction);
  }
}