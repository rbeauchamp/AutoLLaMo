namespace AutoLLaMo.Common;

public static class ListExtensions
{
    public static T? GetValueOrNull<T>(this IReadOnlyList<T> list, int index) where T : class
    {
        if (index >= 0 && index < list.Count)
        {
            return list[index];
        }

        return null;
    }
}