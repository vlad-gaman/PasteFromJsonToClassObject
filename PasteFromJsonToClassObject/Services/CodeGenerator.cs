using Newtonsoft.Json.Linq;
using System.Text;

namespace PasteFromJsonToClassObject
{
    public class CodeGenerator
    {
        private readonly ClassBuilder _classBuilder;
        private readonly ClassesCache _classesCache;

        public CodeGenerator(ClassBuilder classBuilder, ClassesCache classesCache)
        {
            _classBuilder = classBuilder;
            _classesCache = classesCache;
        }

        public StringBuilder GenerateCode(string value)
        {
            var stringBuilder = new StringBuilder();
            var source = JObject.Parse(value);

            _classBuilder.Build("RootObject", source);

            foreach (var clas in _classesCache.GetClasses())
            {
                stringBuilder.AppendLine($"public class {clas.Name}");
                stringBuilder.AppendLine("{");
                foreach (var prop in clas.Properties)
                {
                    stringBuilder.AppendLine($"\tpublic {prop.Type} {prop.Name} {{ get; set; }}");
                }
                stringBuilder.AppendLine("}");
                stringBuilder.AppendLine();
            }

            return stringBuilder;
        }
    }
}
