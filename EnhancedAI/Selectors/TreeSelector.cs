using System;
using BattleTech;
using EnhancedAI.Util;

namespace EnhancedAI.Selectors
{
    public class TreeSelector : ISelector
    {
        public bool Select(string selector, AbstractActor unit)
        {
            if (unit?.BehaviorTree == null)
                return false;

            return Enum.GetName(typeof(BehaviorTreeIDEnum), unit.BehaviorTree.GetID()) == selector;
        }
    }
}
