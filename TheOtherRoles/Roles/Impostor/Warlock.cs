namespace TheOtherRoles.Roles.Impostor;

public static class Warlock
{
    public static PlayerControl warlock;
    public static Color color = Palette.ImpostorRed;

    public static PlayerControl currentTarget;
    public static PlayerControl curseVictim;
    public static PlayerControl curseVictimTarget;

    public static float Cooldown = 30f;
    public static float RootTime = 5f;
    public static bool FriendlyFire;

    public static Sprite curseButtonSprite = new ResourceSprite("CurseButton.png");
    public static Sprite curseKillButtonSprite = new ResourceSprite("CurseKillButton.png");

    public static void clearAndReload()
    {
        warlock = null;
        currentTarget = null;
        curseVictim = null;
        curseVictimTarget = null;
        Cooldown = CustomOptionHolder.warlockCooldown.GetFloat();
        RootTime = CustomOptionHolder.warlockRootTime.GetFloat();
        FriendlyFire = CustomOptionHolder.warlockFriendlyFire.GetBool();
    }

    public static void resetCurse()
    {
        HudManagerStartPatch.warlockCurseButton.Timer = HudManagerStartPatch.warlockCurseButton.MaxTimer;
        HudManagerStartPatch.warlockCurseButton.Sprite = curseButtonSprite;
        HudManagerStartPatch.warlockCurseButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
        currentTarget = null;
        curseVictim = null;
        curseVictimTarget = null;
    }
}
