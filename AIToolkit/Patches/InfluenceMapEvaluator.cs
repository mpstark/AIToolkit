using BattleTech;
using AIToolkit.Features.Overrides;
using Harmony;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace AIToolkit.Patches
{
    /// <summary>
    /// Skip non-essential ValidateCoverage method, as it spams log if it detects
    /// that one of the enum values isn't hooked up
    /// </summary>
    [HarmonyPatch(typeof(InfluenceMapEvaluator), "ValidateCoverage")]
    public static class InfluenceMapEvaluator_ValidateCoverage_Patch
    {
        public static bool Prefix()
        {
            return false;
        }
    }

    /// <summary>
    /// Replace RunEvaluationForSeconds to use our own functions for evaluation
    /// </summary>
    [HarmonyPatch(typeof(InfluenceMapEvaluator), "RunEvaluationForSeconds")]
    public static class InfluenceMapEvaluator_RunEvaluationForSeconds_Patch
    {
        // ReSharper disable once RedundantAssignment
        public static bool Prefix(InfluenceMapEvaluator __instance, float seconds, ref bool __result)
        {
            __result = InfluenceMapEvaluatorOverride.RunEvaluationForSeconds(__instance, seconds);
            return false;
        }
    }
}
