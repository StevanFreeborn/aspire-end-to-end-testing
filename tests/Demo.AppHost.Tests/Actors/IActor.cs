namespace Demo.AppHost.Tests.Actors;

internal interface IActor
{
  Perform<TAbility> UsesAbility<TAbility>() where TAbility : IAbility;
}