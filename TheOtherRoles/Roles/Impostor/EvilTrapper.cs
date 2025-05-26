using TheOtherRoles.Objects;

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
    public static bool meetingFlag;

    public static ResourceSprite trapButtonSprite = new("TrapperButton.png");
    public static DateTime placedTime;

    public static void setTrap()
    {
        var pos = PlayerControl.LocalPlayer.transform.position;
        byte[] buff = new byte[sizeof(float) * 2];
        Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
        Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));
        var writer = StartRPC(CustomRPC.PlaceTrap);
        writer.WriteBytesAndSize(buff);
        writer.EndRPC();
        RPCProcedure.placeTrap(buff);
        placedTime = DateTime.UtcNow;
    }

    public static void clearAndReload()
    {
        evilTrapper = null;
        numTrap = (int)CustomOptionHolder.evilTrapperNumTrap.GetFloat();
        extensionTime = CustomOptionHolder.evilTrapperExtensionTime.GetFloat();
        killTimer = CustomOptionHolder.evilTrapperKillTimer.GetFloat();
        cooldown = CustomOptionHolder.evilTrapperCooldown.GetFloat();
        maxDistance = CustomOptionHolder.evilTrapperMaxDistance.GetFloat();
        trapRange = CustomOptionHolder.evilTrapperTrapRange.GetFloat();
        penaltyTime = CustomOptionHolder.evilTrapperPenaltyTime.GetFloat();
        bonusTime = CustomOptionHolder.evilTrapperBonusTime.GetFloat();
        meetingFlag = false;
        KillTrap.clearAllTraps();
    }
}
