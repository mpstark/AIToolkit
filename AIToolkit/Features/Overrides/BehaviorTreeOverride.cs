using System.Linq;
using AIToolkit.Resources;
using AIToolkit.Util;

namespace AIToolkit.Features.Overrides
{
    public static class BehaviorTreeOverride
    {
        public static void TryOverrideTree(BehaviorTree tree, UnitAIOverrideDef aiOverride)
        {
            if (string.IsNullOrEmpty(aiOverride.TreeRootName))
                return;

            var def = Main.BehaviorNodeDefs.FirstOrDefault(d => d.Name == aiOverride.TreeRootName);
            if (def == null)
            {
                Main.HBSLog?.LogWarning($"Tried to override tree with root named {aiOverride.TreeRootName}, but it wasn't specified as a resource");
                return;
            }

            Main.HBSLog?.Log($"Override Behavior Tree with ID {tree.GetIDString()} from UnitAIOverrideDef {aiOverride.Name} on unit {tree.unit.UnitName} with root name {def.Name}");
            tree.ReplaceRoot(def.ToNode(tree, tree.unit));
        }
    }
}
