using System;
using System.Linq;
using Harmony;

namespace EnhancedAI.Util
{
    public static class LeafFactory
    {
        public static LeafBehaviorNode CreateInternalLeaf(string name, params object[] parameters)
        {
            if (parameters.Contains(null))
                return null;

            var parameterTypes = parameters.Select(p => p.GetType()).ToArray();
            return CreateInternalLeaf(name, parameterTypes, parameters);
        }

        public static LeafBehaviorNode CreateInternalLeaf(string name, Type[] parameterTypes, params object[] parameters)
        {
            var type = AccessTools.TypeByName(name);

            if (type == null)
                return null;

            var constructor = AccessTools.Constructor(type, parameterTypes);
            return constructor?.Invoke(parameters) as LeafBehaviorNode;
        }
    }
}
