namespace TheOtherRoles.Roles.Ghost;

public class Specter
{
    public static PlayerControl Player;
    public static Color color = new Color32(154, 147, 80, byte.MaxValue);

    public static float duration;
    public static bool resetRole;
    public static bool afterMeetingRevive;
    public static bool remember;

    public static bool exiledBeginRevive;
    public static bool revived;

    public static void ClearAndReload()
    {
        Player = null;
        exiledBeginRevive = false;
        revived = false;
        remember = !CustomOptionHolder.specterAfterMeetingTakeRole.GetBool();
        afterMeetingRevive = CustomOptionHolder.specterAfterMeetingRevived.GetBool();
        resetRole = CustomOptionHolder.specterResetRole.GetBool();
        duration = CustomOptionHolder.specterDuration.GetFloat();
    }

    public static void TakeRole(byte targetId)
    {
        var local = Player;
        var target = PlayerById(targetId);
        if (local == null || target == null) return;

        CustomRoleManager.RemoveRole(local);
        var role = target.GetRole();
        if (target.IsImpostor()) turnToImpostor(local);

        revived = true;

        DeadBody[] array = UObject.FindObjectsOfType<DeadBody>();
        for (var i = 0; i < array.Length; i++)
        {
            if (GameData.Instance.GetPlayerById(array[i].ParentId).PlayerId == targetId)
            {
                UObject.Destroy(array[i].gameObject);
                break;
            }
        }

        Player.GetRoleBase()?.Destroy();
        CustomRoleManager.CreateRole(Player, role);


        if (afterMeetingRevive)
        {
            exiledBeginRevive = true;
            return;
        }

        local.Revive();
    }
}