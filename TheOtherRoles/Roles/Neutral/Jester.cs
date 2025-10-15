using TheOtherRoles.Attributes;

namespace TheOtherRoles.Roles.Neutral;

public static class Jester
{
    public static List<PlayerControl> Player = new();
    public static Dictionary<byte, DeadBody> dragedBodys = new();
    public static Color color = new Color32(236, 98, 165, byte.MaxValue);

    public static bool canCallEmergency = true;
    public static bool canUseVents;
    public static bool hasImpostorVision;
    public static bool canDragDeadBody;
    public static float velocity;

    public static bool triggerJesterWin;
    public static PlayerControl WinnerPlayer;
    public static DeadBody targetBody;

    public static void DragBody(PlayerControl player, byte targetId)
    {
        if (targetId == byte.MaxValue)
        {
            dragedBodys.Remove(player.PlayerId);
            return;
        }
        dragedBodys[player.PlayerId] = GetDeadBody(targetId);
    }

    public static void clearAndReload()
    {
        Player = new();
        triggerJesterWin = false;
        dragedBodys = new();
        targetBody = null;
        canCallEmergency = CustomOptionHolder.jesterCanCallEmergency.GetBool();
        canUseVents = CustomOptionHolder.jesterCanVent.GetBool();
        hasImpostorVision = CustomOptionHolder.jesterHasImpostorVision.GetBool();
        canDragDeadBody = CustomOptionHolder.jesterCanDragDeadBody.GetBool();
        velocity = CustomOptionHolder.jesterDragingVelocity.GetFloat();
    }

    [OnGameStart] public static void ClearWinner() => WinnerPlayer = null;
    [OnMeetingStart] public static void ClearDraggers() => dragedBodys = new();
}
