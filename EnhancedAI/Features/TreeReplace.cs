using EnhancedAI.Util;

namespace EnhancedAI.Features
{
    public static class TreeReplace
    {
        public static bool ShouldReplaceTree(BehaviorTree tree)
        {
            return Main.Settings.ReplaceTreeAlways.ContainsKey(tree.GetIDString());
        }

        public static void ReplaceTreeFromPath(BehaviorTree tree, string path)
        {
            var newRoot = BehaviorNodeJSONRepresentation.FromPath(path)?.ToNode(tree, tree.unit);

            if (newRoot != null)
                tree.ReplaceRoot(newRoot);
        }
    }
}
