using System;
using AIToolkit.Util;
using BattleTech;

namespace AIToolkit.Selectors.Unit
{
    public class Tree : Selector<AbstractActor>
    {
        public override bool Select(string selectString, AbstractActor unit)
        {
            if (unit?.BehaviorTree == null)
                return false;

            return Enum.GetName(typeof(BehaviorTreeIDEnum), unit.BehaviorTree.GetID()) == selectString;
        }
    }
}
