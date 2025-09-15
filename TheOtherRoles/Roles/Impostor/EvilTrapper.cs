namespace TheOtherRoles.Roles.Impostor;

public static class EvilTrapper
{
    public static PlayerControl evilTrapper;
    public static Color color = Palette.ImpostorRed;

    public static float minDistance;
    public static float maxDistance;
    public static int numTrap;
    public static float extensionTime;
    public static float killTimer;
    public static float cooldown;
    public static float trapRange;
    public static float penaltyTime;
    public static float bonusTime;
    public static bool isTrapKill;

    public static Sprite trapButtonSprite = new ResourceSprite("TrapperButton.png");
    public static DateTime placedTime;

    public static void setTrap()
    {
        var pos = PlayerControl.LocalPlayer.transform.position;
        var writer = StartRPC(CustomRPC.PlaceTrap);
        writer.Write(PlayerControl.LocalPlayer.PlayerId);
        writer.Write(pos);
        writer.EndRPC();
        RPCProcedure.placeTrap(PlayerControl.LocalPlayer.PlayerId, pos);
        placedTime = DateTime.UtcNow;
    }

    public static void clearAndReload()
    {
        evilTrapper = null;
        numTrap = CustomOptionHolder.evilTrapperNumTrap.GetInt();
        extensionTime = CustomOptionHolder.evilTrapperExtensionTime.GetFloat();
        killTimer = CustomOptionHolder.evilTrapperKillTimer.GetFloat();
        cooldown = CustomOptionHolder.evilTrapperCooldown.GetFloat();
        maxDistance = CustomOptionHolder.evilTrapperMaxDistance.GetFloat();
        trapRange = CustomOptionHolder.evilTrapperTrapRange.GetFloat();
        penaltyTime = CustomOptionHolder.evilTrapperPenaltyTime.GetFloat();
        bonusTime = CustomOptionHolder.evilTrapperBonusTime.GetFloat();
    }
}
