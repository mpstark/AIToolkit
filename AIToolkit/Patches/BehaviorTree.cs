using AIToolkit.Features.Overrides;
using Harmony;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace AIToolkit.Patches
{
    /// <summary>
    /// Hook GetBehaviorVariableValue to replace the value if we're overriding
    /// </summary>
    [HarmonyPatch(typeof(BehaviorTree), "GetBehaviorVariableValue")]
    public static class BehaviorTree_GetBehaviorVariableValue_Patch
    {
        public static bool Prefix(BehaviorTree __instance, BehaviorVariableName name, ref BehaviorVariableValue __result)
        {
            if (!Main.UnitToAIOverride.ContainsKey(__instance.unit))
                return true;

            var aiOverride = Main.UnitToAIOverride[__instance.unit];

            var value = BehaviorVariableOverride.TryOverrideValue(__instance, name, aiOverride);
            if (value == null)
                return true;

            Main.HBSLog?.Log($"Using value (from override: {aiOverride.Name}) for behavior variable: {name} for {__instance.unit.UnitName}");
            __result = value;
            return false;
        }
    }
}
