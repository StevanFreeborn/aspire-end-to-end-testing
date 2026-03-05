namespace Demo.AppHost.Tests.Affordances;

internal sealed class PageRoute : INavigable
{
  public string Url { get; }

  private PageRoute(string url)
  {
    Url = url;
  }

  public static PageRoute From(string url)
  {
    return new(url);
  }
}