using System;
using System.Linq;
using BattleTech;
using Harmony;

namespace EnhancedAI.Util
{
    public static class LeafFactory
    {
        public static LeafBehaviorNode CreateInternalLeaf(string typeName, string name, BehaviorTree tree, AbstractActor actor)
        {
            var parameterTypes = new[] {typeof(string), typeof(BehaviorTree), typeof(AbstractActor)};
            return CreateInternalLeaf(typeName, parameterTypes, name, tree, actor);
        }

        public static LeafBehaviorNode CreateInternalLeaf(string typeName, params object[] parameters)
        {
            if (parameters.Contains(null))
                return null;

            var parameterTypes = parameters.Select(p => p.GetType()).ToArray();
            return CreateInternalLeaf(typeName, parameterTypes, parameters);
        }

        public static LeafBehaviorNode CreateInternalLeaf(string typeName, Type[] parameterTypes, params object[] parameters)
        {
            var type = AccessTools.TypeByName(typeName);

            if (type == null)
                return null;

            var constructor = AccessTools.Constructor(type, parameterTypes);
            return constructor?.Invoke(parameters) as LeafBehaviorNode;
        }
    }
}
