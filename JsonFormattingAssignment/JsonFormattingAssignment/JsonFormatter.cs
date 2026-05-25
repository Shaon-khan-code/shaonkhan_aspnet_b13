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

            // Handle primitive types
            if (type == typeof(string) || type == typeof(char))
            {
                return $"\"{obj.ToString().Replace("\"", "\\\"")}\"";
            }
            if (type == typeof(bool))
            {
                return (bool)obj ? "true" : "false";
            }
            if (type == typeof(DateTime))
            {
                return $"\"{((DateTime)obj).ToString("o", CultureInfo.InvariantCulture)}\""; // ISO 8601 format
            }
            if (type.IsPrimitive || type == typeof(decimal) || type == typeof(double))
            {
                return System.Convert.ToString(obj, CultureInfo.InvariantCulture);
            }

            // Handle arrays and lists
            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                var items = new List<string>();
                foreach (var element in (IEnumerable)obj)
                {
                    items.Add(Serialize(element));
                }
                return "[" + string.Join(",", items) + "]";
            }

            // Handle complex objects
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