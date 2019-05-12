using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BattleTech;
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

        internal static readonly List<AIOverrideDef> AIOverrideDefs = new List<AIOverrideDef>();

        private static readonly List<string> AIOverridePaths = new List<string>();


        public static void Init(string modDir, string settings)
        {
            var harmony = HarmonyInstance.Create("io.github.mpstark.EnhancedAI");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            HBSLog = Logger.GetLogger("EnhancedAI");
            Logger.SetLoggerLevel("EnhancedAI", LogLevel.Log);

            Settings = ModSettings.Parse(settings);
            Directory = modDir;
        }

        public static void FinishedLoading(Dictionary<string, Dictionary<string, VersionManifestEntry>> customResources)
        {
            if (!customResources.ContainsKey(nameof(AIOverrideDef)))
                return;

            AIOverridePaths.AddRange(customResources[nameof(AIOverrideDef)].Values.Select(entry => entry.FilePath));
            ReloadAIOverrides();
        }

        internal static void ReloadAIOverrides()
        {
            AIOverrideDefs.Clear();

            foreach (var path in AIOverridePaths)
            {
                var overrideDef = AIOverrideDef.FromPath(path);
                if (overrideDef == null)
                {
                    HBSLog?.LogError($"AIOverrideDef Resource did not parse at {path}");
                    break;
                }

                HBSLog?.Log($"Parsed {overrideDef.Name} at {path}");
                AIOverrideDefs.Add(overrideDef);
            }
        }
    }
}
