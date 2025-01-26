using System.Collections.Generic;
using Hazel;
using UnityEngine;

namespace TheOtherRoles.Roles.Neutral;

public class Jackal
{
    public static List<PlayerControl> jackal = new();
    public static PlayerControl Sidekick;

    public static Color color = new Color32(0, 180, 235, byte.MaxValue);
    public static PlayerControl currentTarget;

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

    public static ResourceSprite SidekickButton = new("SidekickButton.png");

    public static void setSwoop()
    {
        var chance = canSwoop = rnd.NextDouble() < chanceSwoop;
        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.JackalCanSwooper, SendOption.Reliable);
        writer.Write(chance ? byte.MaxValue : 0);
        AmongUsClient.Instance.FinishRpcImmediately(writer);
        RPCProcedure.jackalCanSwooper(chance);
    }

    public static void clearAndReload()
    {
        jackal.Clear();
        Sidekick = null;
        currentTarget = null;
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
