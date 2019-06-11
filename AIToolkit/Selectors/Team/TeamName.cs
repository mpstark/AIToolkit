namespace AIToolkit.Selectors.Team
{
    public class TeamName : ISelector<AITeam>
    {
        public bool Select(string selectString, AITeam team)
        {
            return team.Name == selectString;
        }
    }
}
