using EnhancedAI.Resources;
using EnhancedAI.Util;

namespace EnhancedAI.Features
{
    public static class TreeReplace
    {
        public static void TryReplaceTreeFromAIOverrides(BehaviorTree tree)
        {
            var aiOverride = AIOverrideDef.SelectFrom(Main.AIOverrideDefs, tree.unit);

            if (aiOverride.NewBehaviorTreeRoot == null)
                return;

            Main.HBSLog?.Log($"TreeReplace from AIOverrideDef {aiOverride.Name}");
            tree.ReplaceRoot(aiOverride.NewBehaviorTreeRoot.ToNode(tree, tree.unit));
        }
    }
}
