using System;
using System.Collections.Generic;
using System.IO;
using EnhancedAI.Util;
using Harmony;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace EnhancedAI.Patches
{
    [HarmonyPatch(typeof(BehaviorTree), "InitRootNode")]
    public static class BehaviorTree_InitRootNode_Patch
    {
        private static readonly List<BehaviorTreeIDEnum> hasPrinted = new List<BehaviorTreeIDEnum>();

        public static void Postfix(BehaviorTree __instance)
        {
            var id = Traverse.Create(__instance).Field("behaviorTreeIDEnum").GetValue<BehaviorTreeIDEnum>();
            var idString = Enum.GetName(typeof(BehaviorTreeIDEnum), id);

            if (hasPrinted.Contains(id))
                return;

            __instance.RootNode.DumpTree(Path.Combine(Main.Directory, $"{idString}_dump.txt"));
            BehaviorNodeJSONRepresentation.FromNode(__instance.RootNode).ToJSONFile(Path.Combine(Main.Directory, $"{idString}.json"));
            hasPrinted.Add(id);
        }
    }
}
