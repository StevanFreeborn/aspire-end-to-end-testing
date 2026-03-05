namespace Demo.AppHost.Tests.Actors;

internal sealed class TestUser : IActor
{
  private readonly Dictionary<Type, IAbility> _abilities = [];

  private TestUser(params IEnumerable<IAbility> abilities)
  {
    foreach (var ability in abilities)
    {
      var concreteType = ability.GetType();
      var interfaces = concreteType.GetInterfaces();
      var targetInterface = interfaces.FirstOrDefault(
        static i => typeof(IAbility).IsAssignableFrom(i) && i != typeof(IAbility)
      );
      var registrationType = targetInterface ?? concreteType;
      _abilities[registrationType] = ability;
    }
  }

  public static TestUser WhoCan(params IAbility[] abilities)
  {
    return new(abilities);
  }

  private T Ability<T>() where T : IAbility
  {
    if (_abilities.TryGetValue(typeof(T), out var ability))
    {
      return (T)ability;
    }

    throw new InvalidOperationException($"Actor does not have the ability of type {typeof(T).Name}");
  }

  public Perform<TAbility> UsesAbility<TAbility>() where TAbility : IAbility
  {
    return Perform<TAbility>.From(Ability<TAbility>());
  }
}