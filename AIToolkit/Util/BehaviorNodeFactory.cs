using System;
using System.Linq;
using BattleTech;
using Harmony;

namespace AIToolkit.Util
{
    public static class BehaviorNodeFactory
    {
        public static BehaviorNode CreateBehaviorNode(string typeName, string name, BehaviorTree tree, AbstractActor actor)
        {
            var parameterTypes = new[] {typeof(string), typeof(BehaviorTree), typeof(AbstractActor)};
            return CreateBehaviorNode(typeName, parameterTypes, name, tree, actor);
        }

        public static BehaviorNode CreateBehaviorNode(string typeName, params object[] parameters)
        {
            if (parameters.Contains(null))
                return null;

            var parameterTypes = parameters.Select(p => p.GetType()).ToArray();
            return CreateBehaviorNode(typeName, parameterTypes, parameters);
        }

        public static BehaviorNode CreateBehaviorNode(string typeName, Type[] parameterTypes, params object[] parameters)
        {
            var type = TypeUtil.GetTypeByName(typeName, typeof(BehaviorNode));

            if (type == null)
                return null;

            var constructor = AccessTools.Constructor(type, parameterTypes);
            return constructor?.Invoke(parameters) as BehaviorNode;
        }
    }
}
