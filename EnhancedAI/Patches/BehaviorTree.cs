using System.IO;
using EnhancedAI.Features;
using EnhancedAI.Util;
using Harmony;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace EnhancedAI.Patches
{
    /// <summary>
    /// Hook BehaviorTree InitRootNode to:
    /// * Dump the tree to JSON/Text
    /// * Replace the root node of the tree from a provided JSON file
    /// </summary>
    [HarmonyPatch(typeof(BehaviorTree), "InitRootNode")]
    public static class BehaviorTree_InitRootNode_Patch
    {
        public static void Postfix(BehaviorTree __instance)
        {
            if (TreeReplace.ShouldReplaceTree(__instance))
            {
                var path = Path.Combine(Main.Directory, Main.Settings.ReplaceTreeAlways[__instance.GetIDString()]);
                TreeReplace.ReplaceTreeFromPath(__instance, path);
            }

            if (Main.Settings.ShouldDump)
                TreeDump.DumpTree(__instance, Main.Settings.DumpType);
        }
    }

    /// <summary>
    /// Hook GetBehaviorVariableValue to replace the value if we're overriding
    /// the team's behaviorVariables
    /// </summary>
    [HarmonyPatch(typeof(BehaviorTree), "GetBehaviorVariableValue")]
    public static class BehaviorTree_GetBehaviorVariableValue_Patch
    {
        public static bool Prefix(BehaviorTree __instance, BehaviorVariableName name, ref BehaviorVariableValue __result)
        {
            var teamName = __instance.unit.team.Name;
            if (!TeamBasedBehaviorVariables.ShouldOverrideBehaviorVariables(teamName))
                return true;

            var value = TeamBasedBehaviorVariables.GetOverridenValue(__instance, name);
            if (value == null)
                return true;

            Main.HBSLog?.Log($"Using overridden value (from team: {teamName}) for behavior variable: {name} for {__instance.unit.UnitName}");
            __result = value;
            return false;
        }
    }
}
