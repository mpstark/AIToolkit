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

            Main.HBSLog.Log($"Dumping behavior tree to {path}");
            using (var writer = File.CreateText(path))
            {
                WriteNodeRecursive(writer, tree.RootNode);
            }
        }

        internal static BehaviorNode FindChildByName(this BehaviorTree tree, string lookingFor, out BehaviorNode parent)
        {
            BehaviorNode FindNodeRecursive(BehaviorNode node, BehaviorNode thisParent, out BehaviorNode outParent)
            {
                outParent = thisParent;

                var name = Traverse.Create(node).Field("name").GetValue<string>();
                if (name == lookingFor)
                    return node;

                BehaviorNode foundNode = null;
                switch (node)
                {
                    case CompositeBehaviorNode composite:
                    {
                        foreach (var child in composite.Children)
                        {
                            foundNode = FindNodeRecursive(child, node, out outParent);

                            if (foundNode != null)
                                break;
                        }
                        break;
                    }

                    case DecoratorBehaviorNode decorator:
                    {
                        foundNode = FindNodeRecursive(decorator.ChildNode, node, out outParent);
                        break;
                    }
                }

                if (foundNode == null)
                    outParent = null;

                return foundNode;
            }

            return FindNodeRecursive(tree.RootNode, null, out parent);
        }

        internal static BehaviorNode RemoveChildByName(this BehaviorTree tree, string name)
        {
            var child = tree.FindChildByName(name, out var parent);

            // didn't find child or child was root
            if (parent == null)
            {
                var rootName = Traverse.Create(tree.RootNode).Field("name").GetValue<string>();
                Main.HBSLog.Log($"Could not find child to remove named: {name} in tree with root {rootName}");
                return null;
            }

            var parentName = Traverse.Create(parent).Field("name").GetValue<string>();
            Main.HBSLog.Log($"Deleting child named: {name} from parent named: {parentName}");
            switch (parent)
            {
                case CompositeBehaviorNode composite:
                    composite.Children.Remove(child);
                    break;
                case DecoratorBehaviorNode decorator:
                    decorator.ChildNode = null;
                    break;
            }

            return child;
        }

        internal static bool AddNode(this BehaviorTree tree, BehaviorNode nodeToAdd, string parentName = null, string siblingName = null, bool insertBefore = false)
        {
            var parentNode = tree.RootNode;
            BehaviorNode sibling = null;

            if (siblingName != null)
            {
                sibling = tree.FindChildByName(siblingName, out parentNode);

                if (sibling == null)
                    return false;

                var foundParentName = Traverse.Create(parentNode).Field("name").GetValue<string>();
                if (foundParentName != parentName)
                    return false;
            }
            else if (parentName != null)
            {
                parentNode = tree.FindChildByName(parentName, out _);
            }

            if (parentNode == null)
                return false;

            switch (parentNode)
            {
                case CompositeBehaviorNode composite:
                {
                    if (sibling != null)
                    {
                        var index = composite.Children.IndexOf(sibling);
                        if (!insertBefore)
                            index++;

                        composite.Children.Insert(index, nodeToAdd);
                    }
                    else
                    {
                        composite.AddChild(nodeToAdd);
                    }

                    return true;
                }

                case DecoratorBehaviorNode decorator:
                {
                    if (decorator.ChildNode == null)
                    {
                        decorator.ChildNode = nodeToAdd;
                        return true;
                    }
                    break;
                }
            }

            return false;
        }
    }
}
