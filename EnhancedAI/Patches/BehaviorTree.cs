using EnhancedAI.Features;
using Harmony;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace EnhancedAI.Patches
{
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
