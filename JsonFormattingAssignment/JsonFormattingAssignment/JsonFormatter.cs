using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace JsonFormattingAssignment
{
    public static class JsonFormatter
    {
        public static string Convert(object obj)
        {
            return Serialize(obj);
        }

        private static string Serialize(object obj)
        {
            if (obj == null) return "null";

            Type type = obj.GetType();

            // Handle string and char
            if (type == typeof(string) || type == typeof(char))
            {
                return $"\"{obj.ToString().Replace("\"", "\\\"")}\"";
            }

            // Handle bool
            if (type == typeof(bool))
            {
                return (bool)obj ? "true" : "false";
            }

            // Handle DateTime — match format: "10/3/2022 9:09:09 AM"
            if (type == typeof(DateTime))
            {
                return $"\"{((DateTime)obj).ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)}\"";
            }

            // Handle numeric primitives
            if (type == typeof(int) || type == typeof(long) || type == typeof(short) ||
                type == typeof(float) || type == typeof(double) || type == typeof(decimal))
            {
                return System.Convert.ToString(obj, CultureInfo.InvariantCulture);
            }

            // Handle arrays and lists (IEnumerable, but not string)
            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                var items = new List<string>();
                foreach (var element in (IEnumerable)obj)
                {
                    items.Add(Serialize(element));
                }
                return "[" + string.Join(",", items) + "]";
            }

            // Handle complex/nested objects using reflection
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var sb = new StringBuilder();
            sb.Append("{");

            var jsonProps = new List<string>();
            foreach (var prop in properties)
            {
                object value = prop.GetValue(obj, null);
                string serializedValue = Serialize(value);
                jsonProps.Add($"\"{prop.Name}\":{serializedValue}");
            }

            sb.Append(string.Join(",", jsonProps));
            sb.Append("}");
            return sb.ToString();
        }
    }
}