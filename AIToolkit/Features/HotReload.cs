using System.Linq;
using BattleTech;
using AIToolkit.Features.Overrides;
using Harmony;

namespace AIToolkit.Features
{
    public static class HotReload
    {
        public static void DoHotReload(GameInstance game)
        {
            Main.HBSLog?.Log("HotReload!");

            Main.ReloadResources();

            // reload behavior variables by forcing a new scope manager
            // TODO: THIS CAUSES A GAME FREEZE IF HOT RELOAD DURING AI PAUSE
            // this is because of scope manager taking a little bit to get info
            // from dataManager, temp solution is to skip if paused
            if (!AIPause.IsPaused)
                Traverse.Create(game).Property("BehaviorVariableScopeManager")
                .SetValue(new BehaviorVariableScopeManager(game));

            var aiActors = game.Combat.AllActors.Where(unit => unit.team is AITeam);
            foreach (var unit in aiActors)
            {
                Main.ResetUnitAI(unit);
                Main.TryOverrideUnitAI(unit);
            }

            var aiTeams = game.Combat.Teams.Where(team => team is AITeam).Cast<AITeam>();
            foreach (var team in aiTeams)
            {
                Main.ResetTeamAI(team);
                Main.TryOverrideTeamAI(team);
            }

            if (AIPause.IsPaused)
            {
                AIPause.Reset();
                TurnOrderOverride.TryRecalculateCurrentUnit(AIPause.CurrentAITeam);
            }
        }
    }
}
