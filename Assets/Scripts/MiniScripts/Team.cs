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
        switch (team)
        {
            case Team.BLUE:
                if (ColorUtility.TryParseHtmlString("#4891a0", out Color blueColor))
                {
                    return blueColor;
                }
                else
                {
                    goto case Team.NONE;
                }
            case Team.PINK:
                if (ColorUtility.TryParseHtmlString("#a0485b", out Color pinkColor))
                {
                    return pinkColor;
                }
                else
                {
                    goto case Team.NONE;
                }
            case Team.NONE:
            default:
                return Color.magenta;
        }
    }
    
    public static Color GetTeamTextColor(this Team team)
    {
        switch (team)
        {
            case Team.BLUE:
                if (ColorUtility.TryParseHtmlString("#4bcfe9", out Color blueColor))
                {
                    return blueColor;
                }
                else
                {
                    goto case Team.NONE;
                }
            case Team.PINK:
                if (ColorUtility.TryParseHtmlString("#ff7190", out Color pinkColor))
                {
                    return pinkColor;
                }
                else
                {
                    goto case Team.NONE;
                }
            case Team.NONE:
            default:
                return Color.magenta;
        }
    }
}