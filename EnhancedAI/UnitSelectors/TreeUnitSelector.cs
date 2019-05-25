using System;
using BattleTech;
using EnhancedAI.Util;

namespace EnhancedAI.UnitSelectors
{
    public class TreeUnitSelector : IUnitSelector
    {
        public bool Select(string selectString, AbstractActor unit)
        {
            if (unit?.BehaviorTree == null)
                return false;

            return Enum.GetName(typeof(BehaviorTreeIDEnum), unit.BehaviorTree.GetID()) == selectString;
        }
    }
}
