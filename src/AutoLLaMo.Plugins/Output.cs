namespace AutoLLaMo.Plugins
{
    /// <summary>
    /// The result of executing a command.
    /// </summary>
    public abstract record Output
    {
        /// <summary>
        /// A one sentence summary of what was produced.
        /// </summary>
        public abstract string Summary { get; init; }
    }
}
