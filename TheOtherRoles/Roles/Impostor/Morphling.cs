namespace TheOtherRoles.Roles.Impostor;

public static class Morphling
{
    public static PlayerControl morphling;
    public static Color color = Palette.ImpostorRed;
    public static ResourceSprite sampleSprite = new("SampleButton.png");
    public static ResourceSprite morphSprite = new("MorphButton.png");

    public static float cooldown = 30f;
    public static float duration = 10f;

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
    }
}
