using System.Linq;
using BattleTech;
using Harmony;

namespace EnhancedAI.Features
{
    public static class HotReload
    {
        public static void DoHotReload(GameInstance game)
        {
            Main.HBSLog?.Log("HotReload!");

            // reload all overrides from their path, this has side effect of
            // clearing all ScopeManagerWrappers as well
            Main.ReloadAIOverrides();

            // reload behavior variables by forcing a new scope manager
            Traverse.Create(game).Property("BehaviorVariableScopeManager")
                .SetValue(new BehaviorVariableScopeManager(game));

            // try to replace the root from all AI active actors
            var aiActors = game.Combat.AllActors.Where(unit => unit.team is AITeam);
            foreach (var unit in aiActors)
                TreeReplace.TryReplaceTreeFromAIOverrides(unit.BehaviorTree);
        }
    }
}
