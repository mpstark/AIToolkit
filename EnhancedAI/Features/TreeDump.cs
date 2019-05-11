using System.Collections.Generic;
using System.IO;
using EnhancedAI.Util;

namespace EnhancedAI.Features
{
    public static class TreeDump
    {
        internal static readonly List<BehaviorTreeIDEnum> DumpedTrees = new List<BehaviorTreeIDEnum>();

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

            var id = tree.GetID();
            var idString = tree.GetIDString();

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
                tree.RootNode.DumpTree(Path.Combine(Main.Directory, $"{idString}.txt"));
            }
        }
    }
}
