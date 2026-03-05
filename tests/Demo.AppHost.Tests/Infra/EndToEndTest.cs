namespace Demo.AppHost.Tests.Infra;

public abstract class EndToEndTest(TestAppHost appHost) : PlaywrightTest, IAsyncDisposable, IClassFixture<TestAppHost>
{
  private readonly TestAppHost _appHost = appHost;
  private IBrowser Browser { get; set; } = null!;
  private IBrowserContext Context { get; set; } = null!;

  protected IPage Page { get; private set; } = null!;
  protected Database Database { get; private set; } = null!;

  public override async ValueTask InitializeAsync()
  {
    await base.InitializeAsync();

    Browser = await Playwright.Chromium.LaunchAsync(new()
    {
      Headless = false
    });

    Context = await Browser.NewContextAsync(new()
    {
      BaseURL = _appHost.GetWebHttpUrl()
    });

    Page = await Context.NewPageAsync();

    var connectionString = await _appHost.GetDatabaseConnectionStringAsync();
    Database = Database.From(connectionString);
  }

  public override async ValueTask DisposeAsync()
  {
    await Page.CloseAsync();
    await Context.CloseAsync();
    await Context.DisposeAsync();
    await Browser.CloseAsync();
    await Database.DisposeAsync();
    await base.DisposeAsync();

    GC.SuppressFinalize(this);
  }
}