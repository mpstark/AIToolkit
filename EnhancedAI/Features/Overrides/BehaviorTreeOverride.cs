using EnhancedAI.Resources;
using EnhancedAI.Util;

namespace EnhancedAI.Features.Overrides
{
    public static class BehaviorTreeOverride
    {
        public static void TryReplaceTree(BehaviorTree tree, UnitAIOverride aiOverride)
        {
            if (aiOverride.NewBehaviorTreeRoot == null)
                return;

            Main.HBSLog?.Log($"TreeReplace from AIOverrideDef {aiOverride.Name}");
            tree.ReplaceRoot(aiOverride.NewBehaviorTreeRoot.ToNode(tree, tree.unit));
        }
    }
}
