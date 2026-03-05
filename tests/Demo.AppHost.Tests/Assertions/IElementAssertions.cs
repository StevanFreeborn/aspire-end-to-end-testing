namespace Demo.AppHost.Tests.Assertions;

internal interface IElementAssertions
{
  Task IsVisibleAsync();
  Task HasTextAsync(string expectedText);
}