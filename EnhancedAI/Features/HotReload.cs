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

                // potentially reset current unit
                if (Main.TeamToAIOverride.ContainsKey(AIPause.CurrentAITeam))
                {
                    var teamAIOverride = Main.TeamToAIOverride[AIPause.CurrentAITeam];
                    if (teamAIOverride.TurnOrderFactorWeights.Count != 0)
                    {
                        // can't do simple thing and just reprocess the activation
                        // because reasons? locks AI after they act after pause
                        // AIPause.CurrentAITeam.TurnActorProcessActivation();

                        var teamTraverse = Traverse.Create(AIPause.CurrentAITeam);
                        var currentUnitTraverse = teamTraverse.Field("currentUnit");

                        var newUnit = teamTraverse.Method("selectCurrentUnit").GetValue<AbstractActor>();
                        currentUnitTraverse.SetValue(newUnit);

                        // side effects of TurnActorProcessActivation
                        teamTraverse.Method("UpdateAloneStatus", newUnit).GetValue();
                        AIRoleAssignment.AssignRoleToUnit(newUnit, AIPause.CurrentAITeam.units);
                    }
                }
            }
        }
    }
}
