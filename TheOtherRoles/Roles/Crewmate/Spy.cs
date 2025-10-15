namespace TheOtherRoles.Roles.Crewmate;

public static class Spy
{
    public static PlayerControl spy;
    public static Color color = Palette.ImpostorRed;

    public static bool impostorsCanKillAnyone = true;
    public static bool canEnterVents;
    public static bool hasImpostorVision;
    public static bool EvilCanKillSpy;

    public static void clearAndReload()
    {
        spy = null;
        impostorsCanKillAnyone = CustomOptionHolder.spyImpostorsCanKillAnyone.GetBool();
        EvilCanKillSpy = CustomOptionHolder.spyEvilCanKillSpy.GetBool();
        canEnterVents = CustomOptionHolder.spyCanEnterVents.GetBool();
        hasImpostorVision = CustomOptionHolder.spyHasImpostorVision.GetBool();
    }
}
