using BattleTech;
using Harmony;
using UnityEngine;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace EnhancedAI.Patches
{
    /// <summary>
    /// Check for key-presses to invoke hot reload
    /// </summary>
    [HarmonyPatch(typeof(UnityGameInstance), "Update")]
    public static class UnityGameInstance_Update_Patch
    {
        public static void Postfix(UnityGameInstance __instance)
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) &&
                Input.GetKeyDown(KeyCode.A) && __instance.Game.Combat != null)
            {
                HotReload.DoHotReload(__instance.Game);
            }
        }
    }
}
