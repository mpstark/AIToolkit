using Newtonsoft.Json;
using System;
using EnhancedAI.Features;
using Newtonsoft.Json.Converters;

namespace EnhancedAI
{
    internal class ModSettings
    {
        public bool ShouldDump = false;

        [JsonConverter(typeof(StringEnumConverter))]
        public BehaviorTreeDump.DumpType DumpType = BehaviorTreeDump.DumpType.None;

        public bool ShouldPauseAI = false;
        public bool FocusOnPause = false;

        public static ModSettings Parse(string json)
        {
            ModSettings settings;

            try
            {
                settings = JsonConvert.DeserializeObject<ModSettings>(json);
            }
            catch (Exception e)
            {
                Main.HBSLog.Log($"Reading settings failed: {e.Message}");
                settings = new ModSettings();
            }

            return settings;
        }
    }
}
