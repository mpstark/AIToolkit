using System.Linq;
using BattleTech;

namespace AIToolkit.Selectors.Unit
{
    public class VariantName : Selector<AbstractActor>
    {
        public override bool Select(string selectString, AbstractActor unit)
        {
            if (unit == null || string.IsNullOrEmpty(unit.VariantName))
                return false;

            var variants = selectString.Split(' ');
            return variants.Contains(unit.VariantName);
        }
    }
}
