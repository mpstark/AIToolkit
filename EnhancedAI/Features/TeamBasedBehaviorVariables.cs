using System.Collections.Generic;
using BattleTech;
using Harmony;

namespace EnhancedAI.Features
{
    public static class TeamBasedBehaviorVariables
    {
        public static readonly Dictionary<string, BehaviorVariableScopeManagerWrapper> TeamToScope
            = new Dictionary<string, BehaviorVariableScopeManagerWrapper>();

        public static bool ShouldOverrideBehaviorVariables(string teamName)
        {
            return Main.Settings.TeamBehaviorVariableDirectories.ContainsKey(teamName);
        }

        private static BehaviorVariableScopeManager GetBehaviorVariableScopeOverride(GameInstance game, string teamName)
        {
            if (!ShouldOverrideBehaviorVariables(teamName))
                return null;

            if (TeamToScope.ContainsKey(teamName))
                return TeamToScope[teamName].ScopeManager;

            var path = Main.Settings.TeamBehaviorVariableDirectories[teamName];
            var wrapper = new BehaviorVariableScopeManagerWrapper(game, path);
            TeamToScope.Add(teamName, wrapper);

            return wrapper.ScopeManager;
        }

        public static BehaviorVariableValue GetOverridenValue(BehaviorTree tree, BehaviorVariableName name)
        {
            // CODE IS LARGELY REWRITTEN FROM HBS CODE
            // LICENSE DOES NOT APPLY TO THIS FUNCTION

            var teamName = tree.unit.team.Name;
            var scopeManager = GetBehaviorVariableScopeOverride(tree.battleTechGame, teamName);
            var mood = tree.unit.BehaviorTree.mood;
            BehaviorVariableScope scope;

            // for non scopeManager values, we'll return null to force the value
            // from the actual Scope, so we have logs that make sense

            // internal variable storage
            var value = tree.unitBehaviorVariables.GetVariable(name);
            if (value != null)
                return null;

            // ai personality
            var pilot = tree.unit.GetPilot();
            if (pilot != null)
            {
                scope = scopeManager.GetScopeForAIPersonality(pilot.pilotDef.AIPersonality);
                if (scope != null)
                {
                    value = scope.GetVariableWithMood(name, mood);

                    if (value != null)
                        return value;
                }
            }

            // lance
            if (tree.unit.lance != null)
            {
                value = tree.unit.lance.BehaviorVariables.GetVariable(name);

                if (value != null)
                    return null;
            }

            // team
            if (tree.unit.team != null)
            {
                value = Traverse.Create(tree.unit.team).Field("BehaviorVariables")
                    .GetValue<BehaviorVariableScope>().GetVariable(name);

                if (value != null)
                    return null;
            }

            // role
            var role = tree.unit.DynamicUnitRole;
            if (role == UnitRole.Undefined)
                role = tree.unit.StaticUnitRole;

            scope = scopeManager.GetScopeForRole(role);
            if (scope != null)
            {
                value = scope.GetVariableWithMood(name, mood);
                if (value != null)
                    return value;
            }

            // "reckless movement" aka ace pilot
            if (tree.unit.CanMoveAfterShooting)
            {
                scope = scopeManager.GetScopeForAISkill(AISkillID.Reckless);

                if (scope != null)
                {
                    value = scope.GetVariableWithMood(name, mood);
                    if (value != null)
                        return value;
                }
            }

            // global scope
            scope = scopeManager.GetGlobalScope();
            if (scope != null)
            {
                value = scope.GetVariableWithMood(name, mood);
                if (value != null)
                    return value;
            }

            // if haven't gotten value by now, it's not in the overriden scope manager
            // so just return null, which will cause patch to try the default manager
            return null;
        }
    }
}
