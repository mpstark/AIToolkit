using System;
using BattleTech;
using AIToolkit.Util;

namespace AIToolkit.Selectors.Unit
{
    public class TreeID : ISelector<AbstractActor>
    {
        public bool Select(string selectString, AbstractActor unit)
        {
            if (unit?.BehaviorTree == null)
                return false;

            return Enum.GetName(typeof(BehaviorTreeIDEnum), unit.BehaviorTree.GetID()) == selectString;
        }
    }
}
