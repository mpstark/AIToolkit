using System;
using System.IO;
using Newtonsoft.Json;

namespace AIToolkit.Util
{
    public static class SerializeUtil
    {
        public static T FromJSON<T>(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception e)
            {
                Main.HBSLog?.LogError($"SerializerHelper.FromJSON for class {typeof(T).Name} tossed exception");
                Main.HBSLog?.LogException(e);
                return default;
            }
        }

        public static T FromPath<T>(string path)
        {
            if (!File.Exists(path))
            {
                Main.HBSLog?.LogWarning($"Could not find file at: {path}");
                return default;
            }

            return FromJSON<T>(File.ReadAllText(path));
        }
    }
}
