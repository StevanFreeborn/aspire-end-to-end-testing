namespace Demo.AppHost.Tests.Abilities;

internal interface IBrowseTheWeb : IAbility
{
  Task ClickAsync(IClickable target);
  Task TypeAsync(ITypeable target, string text);
  Task NavigateToAsync(INavigable destination);
  IElementAssertions Check(IPerceptible target);
}