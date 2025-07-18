namespace TheOtherRoles.Roles.Crewmate;

public static class BodyGuard
{
    public static PlayerControl bodyguard;
    public static PlayerControl guarded;
    public static Color color = new Color32(145, 102, 64, byte.MaxValue);
    public static bool reset = true;
    public static bool usedGuard;
    public static bool guardFlash;
    public static bool showShielded;
    public static Sprite guardButtonSprite = new ResourceSprite("Shield.png");
    public static PlayerControl currentTarget;

    public static void resetGuarded()
    {
        currentTarget = guarded = null;
        usedGuard = false;
    }

    public static void clearAndReload()
    {
        bodyguard = null;
        showShielded = CustomOptionHolder.bodyGuardShowShielded.GetBool();
        guardFlash = CustomOptionHolder.bodyGuardFlash.GetBool();
        reset = CustomOptionHolder.bodyGuardResetTargetAfterMeeting.GetBool();
        guarded = null;
        usedGuard = false;
    }
}
