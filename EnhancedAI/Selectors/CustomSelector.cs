using System.Collections.Generic;
using System.Reflection;
using BattleTech;
using Harmony;

namespace EnhancedAI.Selectors
{
    public class CustomSelector : ISelector
    {
        private readonly Dictionary<string, MethodInfo> _methodCache = new Dictionary<string, MethodInfo>();

        public bool Select(string selector, AbstractActor unit)
        {
            if (string.IsNullOrEmpty(selector))
                return false;

            // find static method at the entry point in selector, cache it when found
            // method must return bool and takes (AbstractActor)
            if (!_methodCache.ContainsKey(selector))
            {
                var pos = selector.LastIndexOf('.');
                if (pos == -1)
                    return false;

                var typeName = selector.Substring(0, pos);
                var methodName = selector.Substring(pos + 1);

                var type = AccessTools.TypeByName(typeName);
                if (type == null)
                    return false;

                var method = AccessTools.Method(type, methodName, new[] { typeof(AbstractActor) });
                if (method == null)
                    return false;

                if (!method.IsStatic)
                    return false;

                if (method.ReturnType != typeof(bool))
                    return false;

                _methodCache.Add(selector, method);
            }

            var returnValue = _methodCache[selector].Invoke(null, new object[] {unit});
            if (returnValue == null)
                return false;

            return (bool) returnValue;
        }
    }
}
