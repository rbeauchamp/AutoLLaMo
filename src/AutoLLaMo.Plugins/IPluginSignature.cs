namespace AutoLLaMo.Plugins;

/// <summary>
///     Defines the signature of a plugin.
/// </summary>
public interface IPluginSignature
{
    /// <summary>
    /// Defines the command that the plugin will execute.
    /// </summary>
    Type CommandType { get; }

    /// <summary>
    /// Defines the output that executing the command will produce.
    /// </summary>
    Type OutputType { get; }
}