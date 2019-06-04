namespace EnhancedAI.Selectors.Team
{
    public class IsInterleaved : ISelector<AITeam>
    {
        public bool Select(string selectString, AITeam team)
        {
            if (selectString == "true")
                return team.Combat.TurnDirector.IsInterleaved;

            return !team.Combat.TurnDirector.IsInterleaved;
        }
    }
}
