using UnityEngine;

public enum Team
{
    NONE,
    BLUE,
    PINK
}

public static class TeamExtensions
{
    public static Color GetTeamColor(this Team team)
    {
        return team switch
        {
            Team.BLUE => Color.cyan,
            Team.PINK => Color.magenta,
            _ => Color.white
        };
    }
}