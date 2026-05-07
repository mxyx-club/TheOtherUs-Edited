namespace TheOtherRoles.Roles.Impostor;

public static class Glitch
{
    public static PlayerControl Player;
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
        if (Player == null) return;
        Player.setDefaultLook();
    }

    public static void clearAndReload()
    {
        resetMorph();
        Player = null;
        currentTarget = null;
        sampledTarget = null;
        morphTarget = null;
        morphTimer = 0f;
        cooldown = CustomOptionHolder.glitchCooldown.GetFloat();
        duration = CustomOptionHolder.glitchDuration.GetFloat();
        ResetAfterMeeting = CustomOptionHolder.glitchResetAfterMeeting.GetBool();
    }
}
