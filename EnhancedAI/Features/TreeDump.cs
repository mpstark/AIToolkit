using System;
using System.Collections.Generic;
using System.IO;
using EnhancedAI.Util;
using Harmony;

namespace EnhancedAI.Features
{
    public static class TreeDump
    {
        public static readonly List<BehaviorTreeIDEnum> DumpedTrees = new List<BehaviorTreeIDEnum>();

        public enum DumpType
        {
            None,
            JSON,
            Text,
            Both
        }

        public static void DumpTree(BehaviorTree tree, DumpType type)
        {
            if (type == DumpType.None)
                return;

            var id = Traverse.Create(tree).Field("behaviorTreeIDEnum").GetValue<BehaviorTreeIDEnum>();
            var idString = Enum.GetName(typeof(BehaviorTreeIDEnum), id);

            if (DumpedTrees.Contains(id))
                return;

            DumpedTrees.Add(id);

            // dump to json
            if (type == DumpType.JSON || type == DumpType.Both)
            {
                BehaviorNodeJSONRepresentation.FromNode(tree.RootNode)
                    .ToJSONFile(Path.Combine(Main.Directory, $"{idString}.json"));
            }

            // dump to text
            if (type == DumpType.Text || type == DumpType.Both)
            {
                tree.RootNode.DumpTree(Path.Combine(Main.Directory, $"{idString}_dump.txt"));
            }
        }
    }
}
