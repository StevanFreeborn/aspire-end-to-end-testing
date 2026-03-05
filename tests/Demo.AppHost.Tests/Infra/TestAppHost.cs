namespace Demo.AppHost.Tests.Infra;

public class TestAppHost() : DistributedApplicationFactory(typeof(Projects.Demo_AppHost)), IAsyncLifetime
{
  public async ValueTask InitializeAsync()
  {
    await StartAsync();
  }

  public override async ValueTask DisposeAsync()
  {
    await base.DisposeAsync();
    GC.SuppressFinalize(this);
  }

  public string GetWebHttpUrl()
  {
    return GetEndpoint(ResourceNames.Web, "http").ToString();
  }

  public async Task<string> GetDatabaseConnectionStringAsync()
  {
    return await GetConnectionString(ResourceNames.Database) ?? throw new InvalidOperationException("Database connection string is not available.");
  }
}