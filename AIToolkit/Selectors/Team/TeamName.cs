namespace AIToolkit.Selectors.Team
{
    public class TeamName : Selector<AITeam>
    {
        public override bool Select(string selectString, AITeam team)
        {
            return team.Name == selectString;
        }
    }
}
