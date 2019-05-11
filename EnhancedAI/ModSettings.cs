using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Converters;

namespace EnhancedAI
{
    internal class ModSettings
    {
        public bool ShouldDump = false;

        [JsonConverter(typeof(StringEnumConverter))]
        public TreeDump.DumpType DumpType = TreeDump.DumpType.None;

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
