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
            var value = BehaviorVariableOverride.TryOverrideValue(__instance, name);
            if (value == null)
                return true;

            __result = value;
            return false;
        }
    }
}
