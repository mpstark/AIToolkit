using System;
using BattleTech;
using AIToolkit.Util;

namespace AIToolkit.Features.Overrides
{
    public static class BehaviorVariableOverride
    {
        public static BehaviorVariableValue TryOverrideValue(BehaviorTree tree, BehaviorVariableName name)
        {
            if (Main.UnitToAIOverride.ContainsKey(tree.unit))
            {
                var unitOverride = Main.UnitToAIOverride[tree.unit];
                var variableName = Enum.GetName(typeof(BehaviorVariableName), name);

                if (variableName != null && unitOverride.BehaviorVariableOverrides.ContainsKey(variableName))
                {
                    Main.HBSLog?.Log( $"Using value (from unit override: {unitOverride.Name}) for behavior variable: {name} for {tree.unit.UnitName}");
                    return unitOverride.BehaviorVariableOverrides[variableName];
                }
            }

            var aiTeam = tree.unit.team as AITeam;
            if (aiTeam == null || !Main.TeamToAIOverride.ContainsKey(aiTeam))
                return null;

            var teamOverride = Main.TeamToAIOverride[aiTeam];
            if (string.IsNullOrEmpty(teamOverride.BehaviorScopesDirectory))
                return null;

            // if we don't have a custom scope and do have a scopeDirectory,
            // check the scopeManager in the same fashion that the vanilla game does
            // but for non scopeManger values, we'll return null to force the value
            // to come from the global scopeManager, so the logs don't say that
            // we overrode them
            // TODO: move this to a place that makes more sense?
            if (teamOverride.ScopeWrapper == null)
            {
                teamOverride.ScopeWrapper = new BVScopeManagerWrapper(
                    tree.battleTechGame, teamOverride.BehaviorScopesDirectory);
            }

            var scopeManager = teamOverride.ScopeWrapper.ScopeManager;

            // CODE IS LARGELY REWRITTEN FROM HBS CODE
            // LICENSE DOES NOT APPLY TO THIS FUNCTION
            BehaviorVariableScope scope;
            var mood = tree.unit.BehaviorTree.mood;

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
                    {
                        Main.HBSLog?.Log( $"Using value (from team override: {teamOverride.Name}) for behavior variable: {name} for {tree.unit.UnitName}");
                        return value;
                    }
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
                value = FieldRefs.BehaviorVariableRef(tree.unit.team).GetVariable(name);

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
                {
                    Main.HBSLog?.Log( $"Using value (from team override: {teamOverride.Name}) for behavior variable: {name} for {tree.unit.UnitName}");
                    return value;
                }
            }

            // "reckless movement" aka ace pilot
            if (tree.unit.CanMoveAfterShooting)
            {
                scope = scopeManager.GetScopeForAISkill(AISkillID.Reckless);

                if (scope != null)
                {
                    value = scope.GetVariableWithMood(name, mood);
                    if (value != null)
                    {
                        Main.HBSLog?.Log( $"Using value (from team override: {teamOverride.Name}) for behavior variable: {name} for {tree.unit.UnitName}");
                        return value;
                    }
                }
            }

            // global scope
            scope = scopeManager.GetGlobalScope();
            if (scope != null)
            {
                value = scope.GetVariableWithMood(name, mood);
                if (value != null)
                {
                    Main.HBSLog?.Log( $"Using value (from team override: {teamOverride.Name}) for behavior variable: {name} for {tree.unit.UnitName}");
                    return value;
                }
            }

            // if haven't gotten value by now, it's not in the overriden scope manager
            // so just return null, which will cause patch to try the default manager
            return null;
        }
    }
}
