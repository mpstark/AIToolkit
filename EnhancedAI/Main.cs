using System.Reflection;
using Harmony;
using HBS.Logging;

namespace EnhancedAI
{
    public static class Main
    {
        internal static ILog HBSLog;
        internal static ModSettings Settings;
        internal static string Directory;

        public static void Init(string modDir, string settings)
        {
            var harmony = HarmonyInstance.Create("io.github.mpstark.EnhancedAI");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            HBSLog = Logger.GetLogger("EnhancedAI");
            Logger.SetLoggerLevel("EnhancedAI", LogLevel.Log);

            Settings = ModSettings.Parse(settings);
            Directory = modDir;
        }
    }
}
