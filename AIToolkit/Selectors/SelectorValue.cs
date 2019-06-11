using System.Collections.Generic;

namespace AIToolkit.Selectors
{
    public class SelectorValue<T>
    {
        public string TypeName;
        public string SelectString;

        public bool Matches(T obj, Dictionary<string, ISelector<T>> selectors)
        {
            if (!selectors.ContainsKey(TypeName))
                return false;

            return selectors[TypeName].Select(SelectString, obj);
        }
    }
}
