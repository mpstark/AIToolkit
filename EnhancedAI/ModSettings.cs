using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using EnhancedAI.Features;
using Newtonsoft.Json.Converters;

namespace EnhancedAI
{
    internal class ModSettings
    {
        public bool ShouldDump = false;

        [JsonConverter(typeof(StringEnumConverter))]
        public TreeDump.DumpType DumpType = TreeDump.DumpType.None;

        public Dictionary<string, string> ReplaceTreeAlways = new Dictionary<string, string>();

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
