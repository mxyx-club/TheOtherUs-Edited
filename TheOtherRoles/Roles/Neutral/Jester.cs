namespace TheOtherRoles.Roles.Neutral;

public static class Jester
{
    public static PlayerControl jester;
    public static Color color = new Color32(236, 98, 165, byte.MaxValue);

    public static bool canCallEmergency = true;
    public static bool canUseVents;
    public static bool hasImpostorVision;
    public static bool canDragDeadBody;
    public static float velocity;

    public static bool triggerJesterWin;
    public static DeadBody targetBody;
    public static DeadBody dragedBody;

    public static void DragBody(byte targetId)
    {
        if (targetId == byte.MaxValue)
        {
            dragedBody = null;
            return;
        }
        dragedBody = GetDeadBody(targetId);
        Message($"Rpc JesterDragBody target: {targetId}, body: {dragedBody?.ParentId.ToString() ?? "NULL"}");
    }

    public static void clearAndReload()
    {
        jester = null;
        triggerJesterWin = false;
        dragedBody = null;
        targetBody = null;
        canCallEmergency = CustomOptionHolder.jesterCanCallEmergency.GetBool();
        canUseVents = CustomOptionHolder.jesterCanVent.GetBool();
        hasImpostorVision = CustomOptionHolder.jesterHasImpostorVision.GetBool();
        canDragDeadBody = CustomOptionHolder.jesterCanDragDeadBody.GetBool();
        velocity = CustomOptionHolder.jesterDragingVelocity.GetFloat();
    }
}
