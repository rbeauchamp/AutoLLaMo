namespace AutoLLaMo.Common;

public static class StringExtensions
{
    public static string? TrimToNull(this string? value)
    {
        return string.IsNullOrWhiteSpace(value?.Trim()) ? null : value;
    }
}
