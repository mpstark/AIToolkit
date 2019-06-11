using System.Collections.Generic;
using BattleTech;

// ReSharper disable CollectionNeverUpdated.Global

namespace AIToolkit.Resources
{
    public class UnitAIOverrideDef : AIOverrideDef<AbstractActor>
    {
        public string TreeRootName;
        public Dictionary<string, BehaviorVariableValue> BehaviorVariableOverrides
            = new Dictionary<string, BehaviorVariableValue>();
        public List<string> AddInfluenceFactors = new List<string>();
        public List<string> RemoveInfluenceFactors = new List<string>();
        //public bool UseDifferentFactorNormalization;
    }
}
