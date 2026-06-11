namespace TheOtherRoles.Roles.Neutral;

public class Jackal
{
    public static List<PlayerControl> jackal = new();
    public static PlayerControl Sidekick;

    public static Color color = new Color32(0, 180, 235, byte.MaxValue);
    public static PlayerControl currentTarget;
    public static PlayerControl killTarget;

    public static float cooldown = 30f;
    public static float createSidekickCooldown = 30f;
    public static bool canUseVents = true;
    public static bool canCreateSidekick = true;
    public static bool jackalPromotedFromSidekickCanCreateSidekick = true;
    public static bool hasImpostorVision;
    public static bool canSabotage;
    public static bool killFakeImpostor;

    public static bool sidekickCanUseVents;
    public static bool sidekickCanKill;
    public static bool promotesToJackal;

    public static float chanceSwoop;
    public static bool canSwoop;
    public static float swoopTimer;
    public static float swoopCooldown = 30f;
    public static float duration = 30f;
    public static bool isInvisable;

    public static Sprite SidekickButton = new ResourceSprite("SidekickButton.png");

    public static void SetSwoop()
    {
        if (AmongUsClient.Instance?.AmHost == true)
        {
            var chance = rnd.NextDouble() < chanceSwoop;
            var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.JackalCanSwooper);
            writer.Write(chance);
            writer.EndRPC();
            canSwoop = chance;
        }
    }

    public static void clearAndReload()
    {
        if (isInvisable)
        {
            isInvisable = false;
            var invisibleWriter = StartRPC(CustomRPC.SetJackalSwoop);
            invisibleWriter.Write(PlayerControl.LocalPlayer.PlayerId);
            invisibleWriter.Write(false);
            invisibleWriter.EndRPC();
            RPCProcedure.setJackalSwoop(PlayerControl.LocalPlayer.PlayerId, false);
        }

        jackal.Clear();
        Sidekick = null;
        currentTarget = null;
        killTarget = null;
        isInvisable = false;
        cooldown = CustomOptionHolder.jackalKillCooldown.GetFloat();
        swoopCooldown = CustomOptionHolder.jackalSwooperCooldown.GetFloat();
        duration = CustomOptionHolder.jackalSwooperDuration.GetFloat();
        createSidekickCooldown = CustomOptionHolder.jackalCreateSidekickCooldown.GetFloat();
        canUseVents = CustomOptionHolder.jackalCanUseVents.GetBool();
        canSabotage = CustomOptionHolder.jackalCanUseSabo.GetBool();
        canCreateSidekick = CustomOptionHolder.jackalCanCreateSidekick.GetBool();
        jackalPromotedFromSidekickCanCreateSidekick = CustomOptionHolder.jackalPromotedFromSidekickCanCreateSidekick.GetBool();
        hasImpostorVision = CustomOptionHolder.jackalAndSidekickHaveImpostorVision.GetBool();
        killFakeImpostor = CustomOptionHolder.jackalkillFakeImpostor.GetBool();
        chanceSwoop = CustomOptionHolder.jackalChanceSwoop.GetSelection() / 10f;

        sidekickCanUseVents = CustomOptionHolder.sidekickCanUseVents.GetBool();
        sidekickCanKill = CustomOptionHolder.sidekickCanKill.GetBool();
        promotesToJackal = CustomOptionHolder.sidekickPromotesToJackal.GetBool();
    }
}
