using System.Collections.Generic;
using System.Reflection;
using Harmony;

namespace AIToolkit.Selectors
{
    public class Custom<T> : ISelector<T>
    {
        private readonly Dictionary<string, MethodInfo> _methodCache = new Dictionary<string, MethodInfo>();

        public bool Select(string selectString, T obj)
        {
            if (string.IsNullOrEmpty(selectString))
                return false;

            // find static method at the entry point in selector, cache it when found
            // method must return bool and takes (T)
            if (!_methodCache.ContainsKey(selectString))
            {
                var pos = selectString.LastIndexOf('.');
                if (pos == -1)
                    return false;

                var typeName = selectString.Substring(0, pos);
                var methodName = selectString.Substring(pos + 1);

                var type = AccessTools.TypeByName(typeName);
                if (type == null)
                    return false;

                var method = AccessTools.Method(type, methodName, new[] { typeof(T) });
                if (method == null)
                    return false;

                if (!method.IsStatic)
                    return false;

                if (method.ReturnType != typeof(bool))
                    return false;

                _methodCache.Add(selectString, method);
            }

            var returnValue = _methodCache[selectString].Invoke(null, new object[] {obj});
            if (returnValue == null)
                return false;

            return (bool) returnValue;
        }
    }
}
