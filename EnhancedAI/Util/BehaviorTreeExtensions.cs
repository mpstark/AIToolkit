using System;
using Harmony;

namespace EnhancedAI.Util
{
    public static class BehaviorTreeExtensions
    {
        public static BehaviorTreeIDEnum GetID(this BehaviorTree tree)
        {
            return Traverse.Create(tree).Field("behaviorTreeIDEnum").GetValue<BehaviorTreeIDEnum>();
        }

        public static string GetIDString(this BehaviorTree tree)
        {
            return Enum.GetName(typeof(BehaviorTreeIDEnum), tree.GetID());
        }

        public static void ReplaceRoot(this BehaviorTree tree, BehaviorNode newRoot)
        {
            Traverse.Create(tree).Property("RootNode").SetValue(newRoot);
        }
    }
}
