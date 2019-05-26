using EnhancedAI.Resources;
using EnhancedAI.Util;

namespace EnhancedAI.Features.Overrides
{
    public static class BehaviorTreeOverride
    {
        public static void TryOverrideTree(BehaviorTree tree, UnitAIOverrideDef aiOverride)
        {
            if (aiOverride.NewBehaviorTreeRoot == null)
                return;

            Main.HBSLog?.Log($"Override Behavior Tree with ID {tree.GetIDString()} from UnitAIOverrideDef {aiOverride.Name} on unit {tree.unit.UnitName}");
            tree.ReplaceRoot(aiOverride.NewBehaviorTreeRoot.ToNode(tree, tree.unit));
        }
    }
}
