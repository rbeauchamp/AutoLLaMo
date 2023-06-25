namespace AutoLLaMo.Model.Messages.Chats;

/// <summary>
///     Provide the user's desire that will be fulfilled by an assistant.
/// </summary>
public record ProvideUserDesire : ChatResponse
{
    public ProvideUserDesire(GetUserDesire ResponseTo) : base(ResponseTo)
    {
    }

    /// <summary>
    ///     The user's desire.
    /// </summary>
    public string? UserDesire { get; init; }
}
