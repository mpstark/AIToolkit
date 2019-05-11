using System.IO;
using BattleTech;
using EnhancedAI.Util;
using Harmony;

namespace EnhancedAI.Features
{
    public static class HotReload
    {
        public static void DoHotReload(GameInstance game)
        {
            Main.HBSLog?.Log("HotReload!");

            // reload behavior variables by forcing a new scope manager
            Traverse.Create(game).Property("BehaviorVariableScopeManager")
                .SetValue(new BehaviorVariableScopeManager(game));

            // reload behavior variables in TeamBasedBehaviorVariables
            foreach (var wrapper in TeamBasedBehaviorVariables.TeamToScope.Values)
                wrapper.Load(game);

            // reload behavior trees from ReplaceTreeAlways
            // TODO: find a more elegant solution lol
            var actorsWithReplaceAll = game.Combat.AllActors.FindAll(actor =>
                Main.Settings.ReplaceTreeAlways.ContainsKey(actor.BehaviorTree.GetIDString()));

            foreach (var actor in actorsWithReplaceAll)
            {
                var path = Path.Combine(Main.Directory, Main.Settings.ReplaceTreeAlways[actor.BehaviorTree.GetIDString()]);
                TreeReplace.ReplaceTreeFromPath(actor.BehaviorTree, path);
            }
        }
    }
}
