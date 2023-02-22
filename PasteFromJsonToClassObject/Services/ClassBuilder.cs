using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace PasteFromJsonToClassObject
{
    public class ClassBuilder
    {
        private readonly PropertyBuilder _propertyBuilder;
        private readonly ClassesCache _classesCache;

        public ClassBuilder(PropertyBuilder propertyBuilder, ClassesCache classesCache)
        {
            _propertyBuilder = propertyBuilder;
            _classesCache = classesCache;
        }

        public Class Build(string className, JObject source)
        {
            Class clas;
            int it = 0;
            do
            {
                className = $"{className}{(it == 0 ? "" : it.ToString())}";
                clas = new Class()
                {
                    Name = className
                };
                it++;
            } while (_classesCache.TryGetByName(className, out _));

            foreach (var item in source)
            {
                clas.Properties.Add(_propertyBuilder.BuildProperty(item.Value!));
            }

            var classHashCode = clas.GetHashCode();
            if (_classesCache.TryGet(classHashCode, out var otherClas))
            {
                return otherClas;
            }
            else
            {
                _classesCache.Add(clas.GetHashCode(), clas);
            }

            clas.Properties.ForEach(p =>
            {
                if (!string.IsNullOrEmpty(p.Type))
                {
                    return;
                }

                if (p.IsList)
                {
                    var listClass = Build(p.Name, p.Source!);
                    p.Type = $"List<{listClass.Name}>?";
                }

                if (p.IsObjectType)
                {
                    var listClass = Build(p.Name, p.Source!);
                    p.Type = $"{listClass.Name}?";
                }
            });

            return clas;
        }
    }

    public class Class
    {
        public string Name { get; set; } = string.Empty;
        public List<Property> Properties { get; set; } = new();

        public override int GetHashCode()
        {
            return (int)(Properties.Sum(p => (decimal)p.GetHashCode()) / Properties.Count);
        }
        public override bool Equals(object obj)
        {
            if (!(obj is Class))
            {
                return false;
            }
            var other = obj as Class;

            return Properties.All(other.Properties.Contains);
        }
    }
}
