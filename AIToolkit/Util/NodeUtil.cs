using System;
using System.Linq;
using System.Reflection;
using Harmony;

namespace AIToolkit.Util
{
    public static class NodeUtil
    {
        public static ParameterInfo[] GetConstructorExtraParameterInfo(string typeName)
        {
            var type = TypeUtil.GetTypeByName(typeName, typeof(BehaviorNode));
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
