namespace TheOtherRoles.Roles.Modifier;

public static class Lovers
{
    public static PlayerControl lover1;
    public static PlayerControl lover2;
    public static Color color = new Color32(232, 57, 185, byte.MaxValue);

    public static bool notAckedExiledIsLover;
    public static bool enableChat = true;
    public static bool neutraValid;
    public static bool IsAvengerLover;

    public static bool isLover(this PlayerControl player) => player != null && (player == lover1 || player == lover2 || player == Avenger.Player || player == Avenger.Lover);
    public static bool IsAlive() => lover1.IsAlive() && lover2.IsAlive() && !notAckedExiledIsLover;
    public static bool isKillerLover() => lover1.IsKiller() || lover2.IsKiller();
    public static bool isCrewLover() => lover1.IsCrew() && lover2.IsCrew();
    public static bool hasAliveKillingLover(this PlayerControl player) => player.isLover() && IsAlive() && isKillerLover();

    public static PlayerControl otherLover(PlayerControl player)
    {
        if (player == null) return null;
        if (player == lover1) return lover2;
        if (player == lover2) return lover1;
        return null;
    }

    public static void SetAvengerLover()
    {
        if (AmongUsClient.Instance?.AmHost == true)
        {
            var flag = rnd.Chance(CustomOptionHolder.modifierLoverAvengerChance.GetSelection() * 10);
            var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.SetAvengerLover);
            writer.Write(flag);
            writer.EndRPC();
            IsAvengerLover = flag;
        }
    }

    public static void clearAndReload()
    {
        lover1 = null;
        lover2 = null;
        notAckedExiledIsLover = false;
        enableChat = CustomOptionHolder.modifierLoverEnableChat.GetBool();
        neutraValid = CustomOptionHolder.modifierLoverNeutraValid.GetBool();
    }
}
