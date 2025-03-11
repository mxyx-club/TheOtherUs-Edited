using UnityEngine;

namespace TheOtherRoles.Roles.Modifier;

public static class Lovers
{
    public static PlayerControl lover1;
    public static PlayerControl lover2;
    public static Color color = new Color32(232, 57, 185, byte.MaxValue);

    public static bool bothDie = true;

    public static bool enableChat = true;

    // Lovers save if next to be exiled is a lover, because RPC of ending game comes before RPC of exiled
    public static bool notAckedExiledIsLover;

    public static bool isLover(this PlayerControl player)
    {
        return player != null && (player == lover1 || player == lover2);
    }

    public static bool IsAlive()
    {
        // ADD NOT ACKED IS LOVER
        return lover1.IsAlive() && lover2.IsAlive() && !notAckedExiledIsLover;
    }

    public static bool isKillerLover()
    {
        return lover1.IsKiller() || lover2.IsKiller();
    }

    public static PlayerControl otherLover(PlayerControl player)
    {
        if (player == null) return null;
        if (player == lover1) return lover2;
        if (player == lover2) return lover1;
        return null;
    }

    public static bool hasAliveKillingLover(this PlayerControl player)
    {
        return player.isLover() && IsAlive() && isKillerLover();
    }

    public static void clearAndReload()
    {
        lover1 = null;
        lover2 = null;
        notAckedExiledIsLover = false;
        bothDie = CustomOptionHolder.modifierLoverBothDie.GetBool();
        enableChat = CustomOptionHolder.modifierLoverEnableChat.GetBool();
    }
}
