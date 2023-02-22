using System.Collections.Generic;
using System.Linq;

namespace PasteFromJsonToClassObject
{
    public class ClassesCache
    {
        private readonly Dictionary<int, Class> Classes = new();
        private readonly Dictionary<string, Class> ClassesByName = new();

        public bool TryGet(int key, out Class clas)
        {
            return Classes.TryGetValue(key, out clas!);
        }

        public bool TryGetByName(string name, out Class clas)
        {
            return ClassesByName.TryGetValue(name, out clas!);
        }

        public void Add(int key, Class clas)
        {
            Classes.Add(key, clas);
            ClassesByName.Add(clas.Name, clas);
        }

        public IEnumerable<Class> GetClasses()
        {
            return Classes.Values.Cast<Class>();
        }
    }
}