namespace AutoLLaMo.Plugins;

/// <summary>
/// Informs the DI container that a class is a settings class.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class SettingsAttribute : Attribute
{
}