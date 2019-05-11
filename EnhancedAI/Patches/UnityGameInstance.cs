using BattleTech;
using Harmony;
using UnityEngine;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace EnhancedAI.Patches
{
    [HarmonyPatch(typeof(UnityGameInstance), "Update")]
    public static class UnityGameInstance_Update_Patch
    {
        public static void Postfix(UnityGameInstance __instance)
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) &&
                Input.GetKeyDown(KeyCode.A) && __instance.Game.Combat != null)
            {
                AIHotReload.DoHotReload(__instance.Game);
            }
        }
    }
}
