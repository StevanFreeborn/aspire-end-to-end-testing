namespace Demo.AppHost.Tests.Pages;

public static class TodoManagerPage
{
  public static INavigable Route => PageRoute.From("/");

  public static IReadable Header => UiElement.From("h1:has-text('Todo Manager')");
  public static ITypeable TaskInput => UiElement.From("input[placeholder='What needs to be done?']");
  public static IClickable AddTaskButton => UiElement.From("button:has-text('Add Task')");
  public static IPerceptible EmptyStateMessage => UiElement.From("text='No tasks yet. Add one above!'");

  public static IPerceptible TaskItem(string taskName)
  {
    return UiElement.From($"li:has-text('{taskName}')");
  }

  public static IClickable DeleteButton(string taskName)
  {
    return UiElement.From($"li:has-text('{taskName}') button:has-text('Delete')");
  }
}