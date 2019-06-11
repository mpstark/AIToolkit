using Newtonsoft.Json;
using System;
using AIToolkit.Features;
using Newtonsoft.Json.Converters;

namespace AIToolkit
{
    internal class ModSettings
    {
        public bool ShouldDump = false;

        [JsonConverter(typeof(StringEnumConverter))]
        public BehaviorTreeDump.DumpType DumpType = BehaviorTreeDump.DumpType.Both;

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
