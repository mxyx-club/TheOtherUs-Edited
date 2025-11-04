namespace TheOtherRoles.Roles.Ghost;

public class GhostEngineer
{
    public static PlayerControl Player;
    public static Color color = new Color32(25, 68, 142, byte.MaxValue);
    public static bool Fixes;

    public static void ClearAndReload()
    {
        Player = null;
        Fixes = false;
    }
}
