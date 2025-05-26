namespace TheOtherRoles.Roles.Crewmate;

public static class Spy
{
    public static PlayerControl spy;
    public static Color color = Palette.ImpostorRed;

    public static bool impostorsCanKillAnyone = true;
    public static bool canEnterVents;
    public static bool hasImpostorVision;

    public static void clearAndReload()
    {
        spy = null;
        impostorsCanKillAnyone = CustomOptionHolder.spyImpostorsCanKillAnyone.GetBool();
        canEnterVents = CustomOptionHolder.spyCanEnterVents.GetBool();
        hasImpostorVision = CustomOptionHolder.spyHasImpostorVision.GetBool();
    }
}
