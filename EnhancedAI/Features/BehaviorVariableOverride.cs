using BattleTech;
using EnhancedAI.Resources;
using Harmony;

namespace EnhancedAI.Features
{
    public static class BehaviorVariableOverride
    {
        public static BehaviorVariableValue TryGetValueFromAIOverrides(BehaviorTree tree, BehaviorVariableName name, out string overrideName)
        {
            overrideName = null;
            var aiOverride = AIOverrideDef.SelectFrom(Main.AIOverrideDefs, tree.unit);

            if (aiOverride == null)
                return null;

            overrideName = aiOverride.Name;

            // custom scope has value, and takes priority over everything else
            if (aiOverride.BehaviorVariableOverrides != null && aiOverride.BehaviorVariableOverrides.ContainsKey(name))
                return aiOverride.BehaviorVariableOverrides[name];

            if (string.IsNullOrEmpty(aiOverride.BehaviorScopesDirectory))
                return null;

            // if we don't have a custom scope and do have a scopeDirectory,
            // check the scopeManager in the same fashion that the vanilla game does
            // but for non scopeManger values, we'll return null to force the value
            // to come from the global scopeManager, so the logs don't say that
            // we overrode them

            if (aiOverride.ScopeWrapper == null)
                aiOverride.ScopeWrapper = new BehaviorVariableScopeManagerWrapper(tree.battleTechGame, aiOverride.BehaviorScopesDirectory);

            var scopeManager = aiOverride.ScopeWrapper.ScopeManager;

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
