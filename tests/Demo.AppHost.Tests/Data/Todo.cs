namespace Demo.AppHost.Tests.Data;

public sealed class Todo
{
  public int Id { get; set; }
  public string Task { get; set; } = string.Empty;
  public bool Completed { get; set; }
}