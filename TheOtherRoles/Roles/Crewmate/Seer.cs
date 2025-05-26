namespace TheOtherRoles.Roles.Crewmate;

public static class Seer
{
    public static PlayerControl seer;
    public static Color color = new Color32(97, 178, 108, byte.MaxValue);
    public static List<Vector3> deadBodyPositions = new();

    public static float soulDuration = 15f;
    public static bool limitSoulDuration;
    public static int mode;

    public static ResourceSprite soulSprite = new("Soul.png", 500f);

    public static void clearAndReload()
    {
        seer = null;
        deadBodyPositions.Clear();
        limitSoulDuration = CustomOptionHolder.seerLimitSoulDuration.GetBool();
        soulDuration = CustomOptionHolder.seerSoulDuration.GetFloat();
        mode = CustomOptionHolder.seerMode.GetSelection();
    }
}
