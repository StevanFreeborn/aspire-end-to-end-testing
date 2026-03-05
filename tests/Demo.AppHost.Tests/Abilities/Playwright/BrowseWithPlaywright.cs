namespace Demo.AppHost.Tests.Abilities.Playwright;

internal sealed class BrowseWithPlaywright : IBrowseTheWeb
{
  private readonly IPage _page;

  private BrowseWithPlaywright(IPage page)
  {
    _page = page;
  }

  public static BrowseWithPlaywright Using(IPage page)
  {
    return new(page);
  }

  public Task ClickAsync(IClickable target)
  {
    return _page.ClickAsync(target.Selector);
  }

  public IElementAssertions Check(IPerceptible target)
  {
    var locator = _page.Locator(target.Selector);
    return PlaywrightElementAssertions.From(locator);
  }

  public Task NavigateToAsync(INavigable destination)
  {
    return _page.GotoAsync(destination.Url);
  }

  public Task TypeAsync(ITypeable target, string text)
  {
    return _page.FillAsync(target.Selector, text);
  }
}