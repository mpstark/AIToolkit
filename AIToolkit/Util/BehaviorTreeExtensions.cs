using System;
using Harmony;

namespace AIToolkit.Util
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

        public static BehaviorVariableValue GetBVValue(this BehaviorTree tree, BehaviorVariableName name)
        {
            return Traverse.Create(tree).Method("GetBehaviorVariableValue", name).GetValue<BehaviorVariableValue>();
        }

        public static void ReplaceRoot(this BehaviorTree tree, BehaviorNode newRoot)
        {
            Traverse.Create(tree).Property("RootNode").SetValue(newRoot);
        }
    }
}
