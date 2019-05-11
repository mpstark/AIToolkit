using BattleTech;
using Harmony;

namespace EnhancedAI
{
    public static class HotReload
    {
        public static void DoHotReload(GameInstance game)
        {
            Main.HBSLog?.Log("HotReload!");

            // reload behavior variables by forcing a new scope manager
            Traverse.Create(game).Property("BehaviorVariableScopeManager")
                .SetValue(new BehaviorVariableScopeManager(game));

            // TODO: reload behavior trees onto their units
        }
    }
}
