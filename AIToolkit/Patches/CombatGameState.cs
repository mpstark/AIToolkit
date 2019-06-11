using BattleTech;
using Harmony;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace AIToolkit.Patches
{
    /// <summary>
    /// Clear AIOverrides if we're starting a new combat
    /// </summary>
    [HarmonyPatch(typeof(CombatGameState), "_Init")]
    public static class CombatGameState_Init_Patch
    {
        public static void Postfix()
        {
            Main.OnCombatInit();
        }
    }

    /// <summary>
    /// Clear AIOverrides when loading from a save
    /// </summary>
    [HarmonyPatch(typeof(CombatGameState), "_InitFromSave")]
    public static class CombatGameState_InitFromSave_Patch
    {
        public static void Postfix()
        {
            Main.OnCombatInit();
        }
    }
}
