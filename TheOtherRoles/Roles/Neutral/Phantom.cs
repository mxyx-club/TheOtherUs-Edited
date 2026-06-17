namespace TheOtherRoles.Roles.Neutral;

// Phantom
public static class Phantom
{
    public static PlayerControl Player;
    public static PlayerControl currentTarget;
    public static float cooldown = 30f;
    public static bool isInvisable;
    public static Color color = new Color32(224, 197, 219, byte.MaxValue);
    public static float duration = 5f;
    public static float swoopCooldown = 30f;
    public static float swoopTimer;
    public static float swoopSpeed;
    public static bool hasImpVision;
    public static bool canUseVents;

    public static Sprite SwoopButtonSprite = new ResourceSprite("Swoop.png");

    public static void clearAndReload()
    {
        if (isInvisable)
        {
            isInvisable = false;
            var writer = StartRPC(CustomRPC.SetSwoop);
            writer.Write(Player.PlayerId);
            writer.Write(false);
            writer.EndRPC();
            RPCProcedure.setSwoop(Player.PlayerId, false);
        }

        Player = null;
        isInvisable = false;
        cooldown = CustomOptionHolder.phantomKillCooldown.GetFloat();
        swoopCooldown = CustomOptionHolder.phantomCooldown.GetFloat();
        duration = CustomOptionHolder.phantomDuration.GetFloat();
        hasImpVision = CustomOptionHolder.phantomHasImpVision.GetBool();
        swoopSpeed = CustomOptionHolder.phantomSpeed.GetFloat();
        canUseVents = CustomOptionHolder.phantomCanUseVents.GetBool();
    }
}
