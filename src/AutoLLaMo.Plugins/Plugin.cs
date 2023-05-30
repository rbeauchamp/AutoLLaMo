namespace AutoLLaMo.Plugins;

/// <summary>
/// Defines a plugin that executes a command.
/// <remarks>
/// A plugin enables an AI assistant to access real-time data or change the external world.
/// </remarks>
/// </summary>
public abstract class Plugin
{
    /// <summary>
    /// The signature that this plugin supports.
    /// </summary>
    public abstract IPluginSignature Signature { get; }

    /// <summary>
    /// Implements custom logic for executing a command
    /// </summary>
    /// <param name="command">The command to execute.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    public abstract Task<Response> ExecuteAsync(
        Command command,
        CancellationToken cancellationToken);
}
