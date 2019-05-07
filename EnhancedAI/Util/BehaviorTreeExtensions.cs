using System;
using System.IO;
using Harmony;

namespace EnhancedAI.Util
{
    internal static class BehaviorTreeExtensions
    {
        internal static void DumpTree(this BehaviorTree tree, string path)
        {
            void WriteNodeRecursive(TextWriter writer, BehaviorNode node, int indent = 1)
            {
                var traverse = Traverse.Create(node);
                var name = traverse.Field("name").GetValue<string>();
                var type = node.GetType().ToString();

                writer.Write($"{name}");

                if (!name.ToLower().StartsWith(type.Substring(0, type.Length - 4).ToLower()))
                    writer.Write($" ({type})");

                switch (type)
                {
                    case "SequenceNode":
                        writer.Write(" [&&]");
                        break;

                    case "SelectorNode":
                        writer.Write(" [||]");
                        break;

                    case "IsBVTrueNode":
                    case "RandomPercentageLessThanBVNode":
                        var bvName = traverse.Field("bvName").GetValue<BehaviorVariableName>();
                        writer.Write($" [{Enum.GetName(typeof(BehaviorVariableName), bvName)}]");
                        break;
                }

                switch (node)
                {
                    case CompositeBehaviorNode composite:
                    {
                        foreach (var child in composite.Children)
                        {
                            writer.Write("\n" + new string(' ', indent*2));
                            WriteNodeRecursive(writer, child, indent + 1);
                        }
                        break;
                    }

                    case DecoratorBehaviorNode decorator:
                    {
                        writer.Write(" <- ");
                        WriteNodeRecursive(writer, decorator.ChildNode, indent);
                        break;
                    }
                }
            }

            using (var writer = File.CreateText(path))
            {
                WriteNodeRecursive(writer, tree.RootNode);
            }
        }
    }
}
