using EnhancedAI.Resources;
using EnhancedAI.Util;

namespace EnhancedAI.Features
{
    public static class BehaviorTreeReplace
    {
        public static void TryReplaceTree(BehaviorTree tree, AIOverrideDef aiOverride)
        {
            if (aiOverride.NewBehaviorTreeRoot == null)
                return;

            Main.HBSLog?.Log($"TreeReplace from AIOverrideDef {aiOverride.Name}");
            tree.ReplaceRoot(aiOverride.NewBehaviorTreeRoot.ToNode(tree, tree.unit));
        }
    }
}
