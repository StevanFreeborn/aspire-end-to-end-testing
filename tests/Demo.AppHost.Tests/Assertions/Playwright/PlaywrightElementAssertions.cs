namespace Demo.AppHost.Tests.Assertions.Playwright;

internal sealed class PlaywrightElementAssertions : IElementAssertions
{
  private readonly ILocatorAssertions _locatorAssertions;

  private PlaywrightElementAssertions(ILocator locator)
  {
    _locatorAssertions = Microsoft.Playwright.Assertions.Expect(locator);
  }

  public static PlaywrightElementAssertions From(ILocator locator)
  {
    return new(locator);
  }

  public Task IsVisibleAsync()
  {
    return _locatorAssertions.ToBeVisibleAsync();
  }

  public Task HasTextAsync(string expectedText)
  {
    return _locatorAssertions.ToHaveTextAsync(expectedText);
  }
}