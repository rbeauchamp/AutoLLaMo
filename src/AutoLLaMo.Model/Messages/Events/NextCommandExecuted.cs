using AutoLLaMo.Plugins;

namespace AutoLLaMo.Model.Messages.Events;

/// <summary>
///     The next command was executed.
/// </summary>
public record NextCommandExecuted : Event
{
    /// <summary>
    ///     The next command was executed.
    /// </summary>
    public NextCommandExecuted(Response response, NextCommandApproved nextCommandApproved) : base(nextCommandApproved)
    {
        Response = response;
        NextCommandApproved = nextCommandApproved;
    }

    public NextCommandApproved NextCommandApproved { get; set; }

    public Response Response { get; init; }
}
