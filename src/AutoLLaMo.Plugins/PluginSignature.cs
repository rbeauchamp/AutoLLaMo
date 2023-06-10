namespace AutoLLaMo.Plugins;

/// <summary>
///     Base implementation of a plugin's signature.
/// </summary>
/// <typeparam name="TCommand">The command type.</typeparam>
/// <typeparam name="TOutput">The output type.</typeparam>
public record PluginSignature<TCommand, TOutput> : IPluginSignature
    where TCommand : Command
    where TOutput : Output
{
    /// <summary>
    ///     Defines the command that the plugin will execute.
    /// </summary>
    public Type CommandType => typeof(TCommand);

    /// <summary>
    ///     Defines the output that executing the command will produce.
    /// </summary>
    public Type OutputType => typeof(TOutput);
}