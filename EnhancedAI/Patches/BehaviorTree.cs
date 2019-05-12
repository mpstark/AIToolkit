using EnhancedAI.Features;
using Harmony;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace EnhancedAI.Patches
{
    /// <summary>
    /// Hook BehaviorTree InitRootNode to:
    /// * Dump the vanilla tree to JSON/Text
    /// * Replace the root node of the tree from an AIOverrideDef
    /// </summary>
    [HarmonyPatch(typeof(BehaviorTree), "InitRootNode")]
    public static class BehaviorTree_InitRootNode_Patch
    {
        public static void Postfix(BehaviorTree __instance)
        {
            if (Main.Settings.ShouldDump)
                TreeDump.DumpTree(__instance, Main.Settings.DumpType);

            if (__instance.unit.team is AITeam)
                TreeReplace.TryReplaceTreeFromAIOverrides(__instance);
        }
    }

    /// <summary>
    /// Hook GetBehaviorVariableValue to replace the value if we're overriding
    /// </summary>
    [HarmonyPatch(typeof(BehaviorTree), "GetBehaviorVariableValue")]
    public static class BehaviorTree_GetBehaviorVariableValue_Patch
    {
        public static bool Prefix(BehaviorTree __instance, BehaviorVariableName name, ref BehaviorVariableValue __result)
        {
            var value = BehaviorVariableOverride.TryGetValueFromAIOverrides(__instance, name, out var overrideName);

            if (value == null)
                return true;

            Main.HBSLog?.Log($"Using value (from override: {overrideName}) for behavior variable: {name} for {__instance.unit.UnitName}");
            __result = value;
            return false;
        }
    }
}
