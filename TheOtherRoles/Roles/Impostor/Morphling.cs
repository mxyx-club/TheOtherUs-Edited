namespace TheOtherRoles.Roles.Impostor;

public static class Morphling
{
    public static PlayerControl morphling;
    public static Color color = Palette.ImpostorRed;
    public static Sprite sampleSprite = new ResourceSprite("SampleButton.png");
    public static Sprite morphSprite = new ResourceSprite("MorphButton.png");

    public static float cooldown = 30f;
    public static float duration = 10f;
    public static bool ResetAfterMeeting;

    public static PlayerControl currentTarget;
    public static PlayerControl sampledTarget;
    public static PlayerControl morphTarget;
    public static float morphTimer;

    public static void resetMorph()
    {
        morphTarget = null;
        morphTimer = 0f;
        if (morphling == null) return;
        morphling.setDefaultLook();
    }

    public static void clearAndReload()
    {
        resetMorph();
        morphling = null;
        currentTarget = null;
        sampledTarget = null;
        morphTarget = null;
        morphTimer = 0f;
        cooldown = CustomOptionHolder.morphlingCooldown.GetFloat();
        duration = CustomOptionHolder.morphlingDuration.GetFloat();
        ResetAfterMeeting = CustomOptionHolder.morphlingResetAfterMeeting.GetBool();
    }
}
