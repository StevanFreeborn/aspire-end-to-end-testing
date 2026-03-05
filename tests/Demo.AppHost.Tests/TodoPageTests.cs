namespace Demo.AppHost.Tests;

public class TodoPageTests(TestAppHost appHost) : EndToEndTest(appHost)
{
  [Fact]
  public async Task GivenAUserIsOnTheTodoPage_AndTheUserHasNoTodos_WhenTheyViewThePage_TheyShouldSeeNoTodos()
  {
    var user = TestUser.WhoCan(BrowseWithPlaywright.Using(Page));
    await user.UsesAbility<IBrowseTheWeb>().To().NavigateToAsync(TodoManagerPage.Route);

    await user.UsesAbility<IBrowseTheWeb>().To().Check(TodoManagerPage.Header).IsVisibleAsync();
    await user.UsesAbility<IBrowseTheWeb>().To().Check(TodoManagerPage.EmptyStateMessage).IsVisibleAsync();
  }

  [Fact]
  public async Task GivenAUserIsOnTheTodoPage_WhenTheyAddATodo_ThenTheyShouldSeeTheTodoInTheList()
  {
    var user = TestUser.WhoCan(BrowseWithPlaywright.Using(Page));
    await user.UsesAbility<IBrowseTheWeb>().To().NavigateToAsync(TodoManagerPage.Route);

    await user.UsesAbility<IBrowseTheWeb>().To().TypeAsync(TodoManagerPage.TaskInput, "Buy groceries");
    await user.UsesAbility<IBrowseTheWeb>().To().ClickAsync(TodoManagerPage.AddTaskButton);

    await user.UsesAbility<IBrowseTheWeb>().To().Check(TodoManagerPage.TaskItem("Buy groceries")).IsVisibleAsync();
  }

  [Fact]
  public async Task GivenAUserHasAddedATodo_WhenTheyViewThePage_ThenTheyShouldBeAbleToDeleteTheTodo()
  {
    var todo = new Todo
    {
      Task = "Buy groceries",
      Completed = false
    };
    await Database.Todos.AddAsync(todo);
    await Database.SaveChangesAsync();

    var user = TestUser.WhoCan(BrowseWithPlaywright.Using(Page));
    await user.UsesAbility<IBrowseTheWeb>().To().NavigateToAsync(TodoManagerPage.Route);
    await user.UsesAbility<IBrowseTheWeb>().To().ClickAsync(TodoManagerPage.DeleteButton(todo.Task));

    await user.UsesAbility<IBrowseTheWeb>().To().Check(TodoManagerPage.EmptyStateMessage).IsVisibleAsync();
  }
}