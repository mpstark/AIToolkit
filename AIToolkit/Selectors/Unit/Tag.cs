using BattleTech;

namespace AIToolkit.Selectors.Unit
{
    public class Tag : Selector<AbstractActor>
    {
        public override bool Select(string selectString, AbstractActor unit)
        {
            if (unit == null || unit.GetTags().Count == 0)
                return false;

            return unit.GetTags().Contains(selectString);
        }
    }
}
