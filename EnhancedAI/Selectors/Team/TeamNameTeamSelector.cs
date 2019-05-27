namespace EnhancedAI.Selectors.Team
{
    public class TeamNameTeamSelector : ISelector<AITeam>
    {
        public bool Select(string selectString, AITeam team)
        {
            return team.Name == selectString;
        }
    }
}
