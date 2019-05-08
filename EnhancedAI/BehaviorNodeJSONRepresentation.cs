using System.Collections.Generic;
using System.IO;
using System.Linq;
using BattleTech;
using EnhancedAI.Util;
using Harmony;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace EnhancedAI
{
    public class BehaviorNodeJSONRepresentation
    {
        private static readonly Dictionary<string, string[]> ExtraParameterMapping = new Dictionary<string, string[]>
        {
            { "DebugLogNode", new []{ "string" } },
            { "DebugLogToContextNode", new []{ "string", "AIDebugContext" } },
            { "ExpectedDamageToMeLessThanNode", new []{ "BehaviorVariableName" } },
            { "IsBVTrueNode", new []{ "BehaviorVariableName" } },
            { "MoveTowardsHighestPriorityMoveCandidateNode", new []{ "bool" } },
            { "RandomPercentageLessThanBVNode", new []{ "BehaviorVariableName" } },
            { "SetMoodNode", new []{ "AIMood" } }
        };

        [JsonRequired]
        public string Name { get; set; }

        [JsonRequired]
        public string TypeName { get; set; }

        public List<BehaviorNodeJSONRepresentation> Children { get; set; }

        // Parameters
        public bool? Bool { get; set; }
        public string String { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public BehaviorVariableName? BehaviorVariableName { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public AIDebugContext? AIDebugContext { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public AIMood? AIMood { get; set; }


        public BehaviorNode ToNode(BehaviorTree tree, AbstractActor unit)
        {
            var node = BehaviorNodeFactory.CreateBehaviorNode(TypeName, GetNodeConstructorParameters(tree, unit));

            if (node == null)
                Main.HBSLog?.LogWarning($"BehaviorNode name: {Name} type: {TypeName} did not convert properly!");

            switch (node)
            {
                case CompositeBehaviorNode composite:
                    foreach (var child in Children)
                        composite.AddChild(child.ToNode(tree, unit));
                    break;

                case DecoratorBehaviorNode decorator:
                    decorator.ChildNode = Children.FirstOrDefault()?.ToNode(tree, unit);
                    break;
            }

            return node;
        }

        private object[] GetNodeConstructorParameters(BehaviorTree tree, AbstractActor unit)
        {
            var parameters = new List<object> { Name, tree, unit };

            if (!ExtraParameterMapping.ContainsKey(TypeName))
                return parameters.ToArray();

            var extraParameterTypes = ExtraParameterMapping[TypeName];

            foreach (var extraParameterType in extraParameterTypes)
            {
                switch (extraParameterType)
                {
                    case "bool":
                        parameters.Add(Bool);
                        break;

                    case "string":
                        parameters.Add(String);
                        break;

                    case "AIDebugContext":
                        parameters.Add(AIDebugContext);
                        break;

                    case "AIMood":
                        parameters.Add(AIMood);
                        break;

                    case "BehaviorVariableName":
                        parameters.Add(BehaviorVariableName);
                        break;
                }
            }

            return parameters.ToArray();
        }

        public void ToJSONFile(string path)
        {
            using (var sw = File.CreateText(path))
            using (var writer = new JsonTextWriter(sw))
            {
                var serializer = new JsonSerializer();
                serializer.NullValueHandling = NullValueHandling.Ignore;
                serializer.Formatting = Formatting.Indented;
                serializer.DefaultValueHandling = DefaultValueHandling.Ignore;

                serializer.Serialize(writer, this);
            }
        }

        public static BehaviorNodeJSONRepresentation FromNode(BehaviorNode node)
        {
            var rep = new BehaviorNodeJSONRepresentation();
            rep.Name = node.GetName();
            rep.TypeName = node.GetType().ToString();

            switch (node)
            {
                case CompositeBehaviorNode composite:
                    rep.Children = new List<BehaviorNodeJSONRepresentation>();
                    foreach (var child in composite.Children)
                        rep.Children.Add(FromNode(child));
                    break;

                case DecoratorBehaviorNode decorator:
                    rep.Children = new List<BehaviorNodeJSONRepresentation> { FromNode(decorator.ChildNode) };
                    break;

                default:
                    rep.Children = null;
                    break;
            }

            if (!ExtraParameterMapping.ContainsKey(rep.TypeName))
                return rep;

            var extraParameterTypes = ExtraParameterMapping[rep.TypeName];
            foreach (var extraParameterType in extraParameterTypes)
            {
                switch (extraParameterType)
                {
                    case "bool":
                        rep.Bool = GetParameterValueByType<bool>(node, rep.TypeName);
                        break;

                    case "string":
                        rep.String = GetParameterValueByType<string>(node, rep.TypeName);
                        break;

                    case "AIDebugContext":
                        rep.AIDebugContext = GetParameterValueByType<AIDebugContext>(node, rep.TypeName);
                        break;

                    case "AIMood":
                        rep.AIMood = GetParameterValueByType<AIMood>(node, rep.TypeName);
                        break;

                    case "BehaviorVariableName":
                        rep.BehaviorVariableName = GetParameterValueByType<BehaviorVariableName>(node, rep.TypeName);
                        break;
                }
            }

            return rep;
        }

        private static T GetParameterValueByType<T>(BehaviorNode node, string typeName)
        {
            var type = AccessTools.TypeByName(typeName);
            var fields = AccessTools.GetDeclaredFields(type).Except(AccessTools.GetDeclaredFields(typeof(BehaviorNode)));

            return (T) fields.FirstOrDefault(field => field.FieldType == typeof(T))?.GetValue(node);
        }

        public static BehaviorNodeJSONRepresentation FromJSON(string json)
        {
            return JsonConvert.DeserializeObject<BehaviorNodeJSONRepresentation>(json);
        }
    }
}
