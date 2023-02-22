using Newtonsoft.Json.Linq;

namespace PasteFromJsonToClassObject
{
    public class PropertyBuilder
    {
        public PropertyBuilder()
        {
        }

        public Property BuildProperty(JToken item)
        {
            return item.Type switch
            {
                JTokenType.Boolean => CreateProperty("bool", item),
                JTokenType.Float => CreateProperty("float", item),
                JTokenType.Date => CreateProperty("DateTime", item),
                JTokenType.Integer => CreateProperty("int", item),
                JTokenType.String => CreatePropertyFromString("string?", item),

                JTokenType.Guid => CreateProperty("Guid", item),
                JTokenType.Bytes => CreateProperty("byte[]", item),
                JTokenType.Uri => CreateProperty("Uri?", item),
                JTokenType.TimeSpan => CreateProperty("TimeSpan", item),

                JTokenType.Array => CreatePropertyArray(item),
                JTokenType.Object => CreatePropertyObject(item),

                JTokenType.Null => CreateProperty("object?", item),

                JTokenType.Raw => CreateProperty("raw", item),
                JTokenType.None => CreateProperty("none", item),
                JTokenType.Property => CreateProperty("property", item),
                JTokenType.Undefined => CreateProperty("undefined", item),
                JTokenType.Comment => CreateProperty("comment", item),
                JTokenType.Constructor => CreateProperty("constructor", item),

                _ => CreateProperty("null", item),
            };
        }

        private Property CreateProperty(string type, JToken item)
        {
            return new Property()
            {
                Type = type,
                Name = GetName(item)
            };
        }

        private static string GetName(JToken item)
        {
            return (item.Parent as JProperty).Name.ToPascalCase();
        }

        private Property CreatePropertyFromString(string type, JToken item)
        {
            return AttemptCreatePropertyFromString<Guid>(item,
                //() => AttemptCreatePropertyFromString<byte[]?>(item,
                () => AttemptCreatePropertyFromString<Uri>(item,
                () => AttemptCreatePropertyFromString<TimeSpan>(item,
                () => CreateProperty(type, item))))
                //)
                ;
        }

        private Property AttemptCreatePropertyFromString<T>(JToken item, Func<Property> func)
        {
            try
            {
                item.ToObject<T>();
                var type = typeof(T);
                return CreateProperty($"{type.Name}{(type.IsClass ? "?" : "")}", item);
            }
            catch (Exception)
            {
                return func();
            }
        }

        private Property CreatePropertyArray(JToken item)
        {
            return new Property()
            {
                Name = GetName(item),
                IsList = true,
                Source = (item as JArray)[0] as JObject
            };
        }

        private Property CreatePropertyObject(JToken item)
        {
            return new Property()
            {
                Name = GetName(item),
                IsObjectType = true,
                Source = item as JObject
            };
        }
    }

    public class Property
    {
        public string Type { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public bool IsList { get; set; } = false;
        public bool IsObjectType { get; set; } = false;
        public JObject Source { get; set; } = null!;

        public override int GetHashCode()
        {
            return Name.GetHashCode()
                + IsList.GetHashCode()
                + IsObjectType.GetHashCode()
                ;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Property))
            {
                return false;
            }
            var other = obj as Property;

            return this.Name == other.Name
                && this.IsList == other.IsList
                && this.IsObjectType == other.IsObjectType;
        }
    }
}
