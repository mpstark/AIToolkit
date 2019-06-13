using BattleTech;
using Harmony;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace AIToolkit.Patches
{
    /// <summary>
    /// OnCombatInit
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
    /// OnCombatInit
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
