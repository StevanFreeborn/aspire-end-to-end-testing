namespace Demo.AppHost.Tests.Affordances;

internal sealed class UiElement : ITypeable, IClickable, IReadable
{
  public string Selector { get; }

  private UiElement(string selector)
  {
    Selector = selector;
  }

  public static UiElement From(string selector)
  {
    return new(selector);
  }
}