namespace AutoLLaMo.Model.Thoughts;

public class ThoughtProcess
{
    public string Thoughts { get; set; } = string.Empty;

    public string Reasoning { get; set; } = string.Empty;

    /// <summary>
    /// a short, numbered list that conveys your plan of action to achieve your goals
    /// </summary>
    public IList<string> Plan { get; init; } = new List<string>();

    public string CritiqueOfPlan { get; set; } = string.Empty;
}
