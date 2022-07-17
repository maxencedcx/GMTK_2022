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
            Team.BLUE => new Color32(62, 135, 150, 255),
            Team.PINK => new Color32(160, 72, 91, 255),
            _ => Color.white
        };
    }
}