using System.Text;

namespace AutoLLaMo.Common
{
    public static class TypeExtensions
    {
        public static string GetFullTypeName(this Type type)
        {
            if (!type.IsGenericType)
            {
                return type.Name;
            }

            var builder = new StringBuilder();
            var name = type.Name[..type.Name.IndexOf('`')];
            var types = type.GetGenericArguments();

            builder.Append(name);
            builder.Append('<');
            builder.AppendJoin(", ", types.Select(GetFullTypeName));
            builder.Append('>');

            return builder.ToString();
        }
    }
}
