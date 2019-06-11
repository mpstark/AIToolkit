using System;
using BattleTech;

namespace AIToolkit.Selectors.Unit
{
    public class Role : ISelector<AbstractActor>
    {
        public bool Select(string selectString, AbstractActor unit)
        {
            if (unit == null)
                return false;

            var role = unit.DynamicUnitRole;
            if (role == UnitRole.Undefined)
                role = unit.StaticUnitRole;

            return Enum.GetName(typeof(Role), role) == selectString;
        }
    }
}
