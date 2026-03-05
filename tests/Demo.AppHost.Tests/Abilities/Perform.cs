namespace Demo.AppHost.Tests.Abilities;

internal sealed class Perform<TAbility> where TAbility : IAbility
{
  public TAbility Ability { get; }

  private Perform(TAbility ability)
  {
    Ability = ability;
  }

  public static Perform<TAbility> From(TAbility ability)
  {
    return new(ability);
  }

  public TAbility To()
  {
    return Ability;
  }
}