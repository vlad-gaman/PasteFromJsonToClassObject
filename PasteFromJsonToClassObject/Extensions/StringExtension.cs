using System.Linq;

namespace PasteFromJsonToClassObject
{
    public static class StringExtension
    {
        public static string ToPascalCase(this string value)
        {
            return string.Concat(value.Select((c, i) => i == 0 ? $"{c}".ToUpper() : $"{c}"));
        }
    }
}
