namespace TheOtherRoles.Roles.Impostor;

public static class Undertaker
{
    public static PlayerControl undertaker;
    public static Color color = Palette.ImpostorRed;

    public static float dragingDelaiAfterKill;

    public static DeadBody targetBody;
    public static DeadBody dragedBody;
    public static bool canDragAndVent;

    public static float velocity = 1;

    public static ResourceSprite buttonSprite = new("UndertakerDragButton.png");

    public static void DragBody(byte targetId)
    {
        if (targetId == byte.MaxValue)
        {
            dragedBody = null;
            return;
        }
        dragedBody = GetDeadBody(targetId);
        Message($"Rpc UndertakerDragBody target: {targetId}, body: {dragedBody?.ParentId.ToString() ?? "NULL"}");
    }

    public static void clearAndReload()
    {
        undertaker = null;
        targetBody = null;
        dragedBody = null;
        canDragAndVent = CustomOptionHolder.undertakerCanDragAndVent.GetBool();
        velocity = CustomOptionHolder.undertakerDragingAfterVelocity.GetFloat();
        dragingDelaiAfterKill = CustomOptionHolder.undertakerDragingDelaiAfterKill.GetFloat();
    }
}
