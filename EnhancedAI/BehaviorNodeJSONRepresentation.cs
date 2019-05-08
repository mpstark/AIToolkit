using System.Collections.Generic;
using System.ComponentModel;
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
        [JsonRequired]
        public string Name { get; set; }

        [JsonRequired]
        public string TypeName { get; set; }

        public List<BehaviorNodeJSONRepresentation> Children { get; set; }

        [DefaultValue(BehaviorVariableName.INVALID_UNSET)]
        [JsonConverter(typeof(StringEnumConverter))]
        public BehaviorVariableName BVName { get; set; } = BehaviorVariableName.INVALID_UNSET;

        public BehaviorNode ToNode(BehaviorTree tree, AbstractActor unit)
        {
            BehaviorNode node;

            if (BVName != BehaviorVariableName.INVALID_UNSET)
            {
                node = BehaviorNodeFactory.CreateBehaviorNode(TypeName,
                    new[] {typeof(string), typeof(BehaviorTree), typeof(AbstractActor), typeof(BehaviorVariableName)},
                    Name, tree, unit, BVName);
            }
            else
            {
                node = BehaviorNodeFactory.CreateBehaviorNode(TypeName, Name, tree, unit);
            }

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
            rep.BVName = Traverse.Create(node).Field("bvName").GetValue<BehaviorVariableName>();

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

            return rep;
        }

        public static BehaviorNodeJSONRepresentation FromJSON(string json)
        {
            return JsonConvert.DeserializeObject<BehaviorNodeJSONRepresentation>(json);
        }
    }
}
