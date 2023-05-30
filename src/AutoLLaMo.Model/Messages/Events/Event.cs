using AutoLLaMo.Plugins;

namespace AutoLLaMo.Model.Messages.Events;

/// <summary>
///     A significant change of state that occurred within the system.
///     Derived types should be named as verbs in the past tense.
/// </summary>
public abstract record Event : Message
{
    /// <summary>
    /// The message that initiated this event.
    /// </summary>
    protected Event(Message initiator)
    {
        Initiator = initiator;
    }

    /// <summary>The message that initiated this event.</summary>
    public Message Initiator { get; init; }
}
