using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BattleTech;
using EnhancedAI.Features;
using EnhancedAI.Features.Overrides;
using EnhancedAI.Resources;
using Harmony;
using HBS.Logging;

// ReSharper disable UnusedMember.Global

namespace EnhancedAI
{
    public static class Main
    {
        internal static ILog HBSLog;
        internal static ModSettings Settings;
        internal static string Directory;

        private static readonly List<UnitAIOverride> UnitAIOverrides
            = new List<UnitAIOverride>();

        internal static readonly Dictionary<AbstractActor, UnitAIOverride> UnitToAIOverride
            = new Dictionary<AbstractActor, UnitAIOverride>();

        private static readonly List<string> UnitAIOverridePaths = new List<string>();


        public static void Init(string modDir, string settings)
        {
            var harmony = HarmonyInstance.Create("io.github.mpstark.EnhancedAI");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            HBSLog = Logger.GetLogger("EnhancedAI");
            Logger.SetLoggerLevel("EnhancedAI", LogLevel.Log);

            Settings = ModSettings.Parse(settings);
            Directory = modDir;

            if (Settings.ShouldDump)
                BehaviorTreeDump.DumpTrees(Settings.DumpType);
        }

        public static void FinishedLoading(Dictionary<string, Dictionary<string, VersionManifestEntry>> customResources)
        {
            if (!customResources.ContainsKey(UnitAIOverride.ModTekResourceName))
                return;

            UnitAIOverridePaths.AddRange(customResources[UnitAIOverride.ModTekResourceName]
                .Values.Select(entry => entry.FilePath));
        }


        internal static void ReloadAIOverrides()
        {
            UnitToAIOverride.Clear();
            UnitAIOverrides.Clear();

            foreach (var path in UnitAIOverridePaths)
            {
                var overrideDef = UnitAIOverride.FromPath(path);
                if (overrideDef == null)
                {
                    HBSLog?.LogError($"AIOverrideDef Resource did not parse at {path}");
                    break;
                }

                HBSLog?.Log($"Parsed {overrideDef.Name} at {path}");
                UnitAIOverrides.Add(overrideDef);
            }
        }

        internal static void TryOverrideAI(AbstractActor unit)
        {
            var aiOverride = UnitAIOverride.MatchToUnitFrom(UnitAIOverrides, unit);

            // unit has already been overriden and has the same override then we just got
            if (UnitToAIOverride.ContainsKey(unit) && UnitToAIOverride[unit] == aiOverride)
                return;

            // unit has already been overridden but has a different override
            if (UnitToAIOverride.ContainsKey(unit) && UnitToAIOverride[unit] != aiOverride)
                ResetAI(unit);

            if (aiOverride == null)
                return;

            HBSLog?.Log($"Overriding AI on {unit.UnitName} with {aiOverride.Name}");

            UnitToAIOverride[unit] = aiOverride;
            BehaviorTreeOverride.TryReplaceTree(unit.BehaviorTree, aiOverride);
            InfluenceFactorOverride.TryOverrideInfluenceFactors(unit.BehaviorTree, aiOverride);
        }

        internal static void ResetAI(AbstractActor unit)
        {
            HBSLog?.Log($"Resetting AI for {unit.UnitName}");

            Traverse.Create(unit.BehaviorTree).Method("InitRootNode").GetValue();
            unit.BehaviorTree.influenceMapEvaluator = new InfluenceMapEvaluator();
            unit.BehaviorTree.Reset();
        }
    }
}
