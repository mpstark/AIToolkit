using System;
using BattleTech;

namespace EnhancedAI.Selectors
{
    public class RoleSelector : ISelector
    {
        public bool Select(string selector, AbstractActor unit)
        {
            if (unit == null)
                return false;

            var role = unit.DynamicUnitRole;
            if (role == UnitRole.Undefined)
                role = unit.StaticUnitRole;

            return Enum.GetName(typeof(UnitRole), role) == selector;
        }
    }
}
