using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AIToolkit.Util;
using BattleTech;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AIToolkit.Resources
{
    public class BehaviorNodeDef
    {
        [JsonRequired]
        public string Name { get; set; }

        [JsonRequired]
        public string TypeName { get; set; }

        public List<BehaviorNodeDef> Children { get; set; }
        public Dictionary<string, object> ExtraParameters { get; set; }


        private object[] GetNodeConstructorParameters(BehaviorTree tree, AbstractActor unit, out Type[] parameterTypes)
        {
            var parameters = new List<object> { Name, tree, unit };
            var parameterTypeList = new List<Type> { typeof(string), typeof(BehaviorTree), typeof(AbstractActor) };

            if (ExtraParameters == null || ExtraParameters.Count == 0)
            {
                parameterTypes = parameterTypeList.ToArray();
                return parameters.ToArray();
            }

            var extraParameterInfo = NodeUtil.GetConstructorExtraParameterInfo(TypeName);
            parameterTypeList.AddRange(extraParameterInfo.Select(parameterInfo => parameterInfo.ParameterType));
            parameters.AddRange(extraParameterInfo.Select(parameterInfo => ExtraParameters[parameterInfo.Name]));

            // for some reason unity/newtonsoft json doesn't seem to like converting
            // string -> enum while the tests seem to work just fine
            // this causes hang on mission load without any sort of exception or anything
            for (var i = 0; i < parameterTypeList.Count; i++)
            {
                var parameterType = parameterTypeList[i];
                if (parameterType.IsEnum && parameters[i] is string parameter)
                {
                    parameters[i] = Enum.Parse(parameterTypeList[i], parameter);
                }
            }

            parameterTypes = parameterTypeList.ToArray();
            return parameters.ToArray();
        }

        public BehaviorNode ToNode(BehaviorTree tree, AbstractActor unit)
        {
            var parameters = GetNodeConstructorParameters(tree, unit, out var parameterTypes);
            var node = BehaviorNodeFactory.CreateBehaviorNode(TypeName, parameterTypes, parameters);

            if (node == null)
                Main.HBSLog?.LogError($"BehaviorNode name: {Name} type: {TypeName} did not convert properly!");

            switch (node)
            {
                case CompositeBehaviorNode composite:
                    foreach (var child in Children)
                        composite.AddChild(child.ToNode(tree, unit));
                    break;

                case DecoratorBehaviorNode decorator:
                    decorator.ChildNode = Children.Count != 0 ? Children[0].ToNode(tree, unit) : null;
                    break;
            }

            return node;
        }


        private void ToJSON(TextWriter textWriter, Formatting formatting)
        {
            using (var writer = new JsonTextWriter(textWriter))
            {
                var serializer = new JsonSerializer
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    Formatting = formatting
                };
                serializer.Converters.Add(new StringEnumConverter());

                serializer.Serialize(writer, this);
            }
        }

        public void ToJSONFile(string path)
        {
            using (var textWriter = File.CreateText(path))
            {
                ToJSON(textWriter, Formatting.Indented);
            }
        }

        public string ToJSONString()
        {
            using (var textWriter = new StringWriter())
            {
                ToJSON(textWriter, Formatting.Indented);
                return textWriter.ToString();
            }
        }


        public static BehaviorNodeDef FromNode(BehaviorNode node)
        {
            var rep = new BehaviorNodeDef();
            rep.Name = node.GetName();
            rep.TypeName = node.GetType().ToString();

            switch (node)
            {
                case CompositeBehaviorNode composite:
                    rep.Children = new List<BehaviorNodeDef>();
                    foreach (var child in composite.Children)
                        rep.Children.Add(FromNode(child));
                    break;

                case DecoratorBehaviorNode decorator:
                    rep.Children = new List<BehaviorNodeDef> { FromNode(decorator.ChildNode) };
                    break;

                default:
                    rep.Children = null;
                    break;
            }

            var extraParameterInfo = NodeUtil.GetConstructorExtraParameterInfo(node.GetType());
            if (extraParameterInfo == null || extraParameterInfo.Length == 0)
                return rep;

            rep.ExtraParameters = new Dictionary<string, object>();
            foreach (var parameterInfo in extraParameterInfo)
            {
                var value = NodeUtil.GetParameterValueByType(node, parameterInfo.ParameterType);
                rep.ExtraParameters.Add(parameterInfo.Name, value);
            }

            return rep;
        }
    }
}
