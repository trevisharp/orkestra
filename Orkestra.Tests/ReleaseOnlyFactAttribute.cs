namespace Orkestra.Tests;

/// <summary>
/// A test that only means something with JIT optimizations on, such as an allocation check:
/// Debug builds turn constant <c>ReadOnlySpan&lt;int&gt;</c> data into a new array on every access.
/// Run it with <c>dotnet test -c Release</c>.
/// </summary>
public sealed class ReleaseOnlyFactAttribute : FactAttribute
{
    public ReleaseOnlyFactAttribute()
    {
#if DEBUG
        Skip = "Needs an optimized build: run with -c Release.";
#endif
    }
}
