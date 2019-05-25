using System;
using BattleTech;

namespace EnhancedAI.UnitSelectors
{
    public class RoleUnitSelector : IUnitSelector
    {
        public bool Select(string selectString, AbstractActor unit)
        {
            if (unit == null)
                return false;

            var role = unit.DynamicUnitRole;
            if (role == UnitRole.Undefined)
                role = unit.StaticUnitRole;

            return Enum.GetName(typeof(UnitRole), role) == selectString;
        }
    }
}
