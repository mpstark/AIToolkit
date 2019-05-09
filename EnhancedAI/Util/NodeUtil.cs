using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Harmony;

namespace EnhancedAI.Util
{
    public static class NodeUtil
    {
        private static bool _hasRunCacheFill;
        private static readonly Dictionary<string, Type> BehaviorNodeTypeCache = new Dictionary<string, Type>();

        private static void BehaviorNodeTypeCacheFill()
        {
            var types = typeof(BehaviorNode).Assembly.GetTypes();
            foreach (var type in types)
            {
                if (type.IsSubclassOf(typeof(BehaviorNode)) && !BehaviorNodeTypeCache.ContainsKey(type.Name))
                    BehaviorNodeTypeCache.Add(type.Name, type);
            }
        }

        public static Type GetBehaviorNodeTypeByName(string typeName)
        {
            if (!_hasRunCacheFill)
            {
                BehaviorNodeTypeCacheFill();
                _hasRunCacheFill = true;
            }

            if (BehaviorNodeTypeCache.ContainsKey(typeName))
                return BehaviorNodeTypeCache[typeName];

            var type = AccessTools.TypeByName(typeName);
            BehaviorNodeTypeCache.Add(typeName, type);

            return BehaviorNodeTypeCache[typeName];
        }

        public static ParameterInfo[] GetConstructorExtraParameterInfo(string typeName)
        {
            var type = GetBehaviorNodeTypeByName(typeName);
            if (type == null)
                return null;

            return GetConstructorExtraParameterInfo(type);
        }

        public static ParameterInfo[] GetConstructorExtraParameterInfo(Type type)
        {
            var constructors = AccessTools.GetDeclaredConstructors(type);
            if (constructors == null || constructors.Count == 0)
                return null;

            // the first 3 parameters are always the same, we only want the extra
            return constructors[0].GetParameters().Skip(3).ToArray();
        }

        public static object GetParameterValueByType(BehaviorNode node, Type type)
        {
            var fields = AccessTools.GetDeclaredFields(node.GetType()).Except(AccessTools.GetDeclaredFields(typeof(BehaviorNode)));
            return fields.FirstOrDefault(field => field.FieldType == type)?.GetValue(node);
        }
    }
}
