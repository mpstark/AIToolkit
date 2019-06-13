using System.Collections.Generic;
using AIToolkit.Util;
using Harmony;

namespace AIToolkit.Selectors
{
    public abstract class Selector<T>
    {
        private static readonly Dictionary<string, Selector<T>> Selectors
            = new Dictionary<string, Selector<T>>();


        public abstract bool Select(string selectString, T obj);


        public static Selector<T> FindSelector(string name)
        {
            if (Selectors.ContainsKey(name))
                return Selectors[name];

            var type = TypeUtil.GetTypeByName(name, typeof(Selector<T>));
            if (type == null)
                return null;

            Selectors[name] = AccessTools.Constructor(type).Invoke(null) as Selector<T>;
            return Selectors[name];
        }
    }
}
