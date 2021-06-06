using System;

namespace AIToolkit.Util
{
    public static class BehaviorTreeExtensions
    {
        public static BehaviorTreeIDEnum GetID(this BehaviorTree tree)
        {
            return tree.behaviorTreeIDEnum;
        }

        public static string GetIDString(this BehaviorTree tree)
        {
            return Enum.GetName(typeof(BehaviorTreeIDEnum), tree.GetID());
        }

        public static BehaviorVariableValue GetBVValue(this BehaviorTree tree, BehaviorVariableName name)
        {
            return tree.GetBehaviorVariableValue(name);
        }

        public static void ReplaceRoot(this BehaviorTree tree, BehaviorNode newRoot)
        {
            tree.RootNode = newRoot;
        }
    }
}
