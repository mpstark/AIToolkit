using BattleTech;
using Harmony;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace EnhancedAI.Patches
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
}
