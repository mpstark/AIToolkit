namespace AIToolkit.Selectors.Team
{
    public class IsPlayerEnemy : Selector<AITeam>
    {
        public override bool Select(string selectString, AITeam team)
        {
            var isTrue = team.IsEnemy(team.Combat.LocalPlayerTeam);
            switch (selectString)
            {
                case "true":
                    return isTrue;
                case "false":
                    return !isTrue;
                default:
                    return false;
            }
        }
    }
}
