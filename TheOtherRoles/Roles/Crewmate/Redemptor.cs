using TheOtherRoles.Objects;

namespace TheOtherRoles.Roles.Crewmate;
public class Redemptor
{
    public static PlayerControl Player;
    public static Color color = new Color32(255, 216, 70, byte.MaxValue);
    public static PlayerControl target;
    public static Arrow arrow;
    public static bool Revelating;
    public static bool Prayering;
    public static PlayerControl RevivedPlayer;

    public static bool revelation;
    public static float revelationCooldown;
    public static float revelationDuration;
    public static bool prayer;
    public static float prayerCooldown;
    public static float prayerDuration;
    public static float reviveDuration;

    public static Sprite reviveButton = new ResourceSprite("Revive.png");
    public static TextMeshPro text;

    public static void RevivePlayer(byte targetId)
    {
        var player = PlayerById(targetId);
        player?.ModRevive();
        RevivedPlayer = player;
        target = null;


        DeadBody[] array = UObject.FindObjectsOfType<DeadBody>();
        for (var i = 0; i < array.Length; i++)
        {
            if (GameData.Instance.GetPlayerById(array[i].ParentId).PlayerId == Player.PlayerId)
            {
                UObject.Destroy(array[i].gameObject);
                break;
            }
        }
    }

    public static void RedemptorPrayer(bool status) => Prayering = status;

    public static void ClearAndReload(bool clear = true)
    {
        Player = null;
        target = null;
        if (clear) RevivedPlayer = null;
        arrow?.arrow?.Destroy();
        Prayering = false;
        Revelating = false;
        if (text != null) UObject.Destroy(text);
        text = null;
        revelation = CustomOptionHolder.redemptorRevelation.GetBool();
        revelationCooldown = CustomOptionHolder.redemptorRevelationCooldown.GetFloat();
        revelationDuration = CustomOptionHolder.redemptorRevelationDuration.GetFloat();
        prayer = CustomOptionHolder.redemptorPrayer.GetBool();
        prayerCooldown = CustomOptionHolder.redemptorPrayerCooldown.GetFloat();
        prayerDuration = CustomOptionHolder.redemptorPrayerDuration.GetFloat();
        reviveDuration = CustomOptionHolder.redemptorReviveDuration.GetFloat();
    }

    [HarmonyPatch]
    private static class Redemptor_Patch
    {
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update)), HarmonyPostfix]
        private static void HudUpdatePatch()
        {
            if (Prayering && (Player.IsDead() || InMeeting))
            {
                Prayering = false;
                target = null;
                RevivedPlayer = null;
            }
            if (Revelating && InMeeting)
            {
                target = null;
                Revelating = false;
                RevivedPlayer = null;
            }
        }
    }
}
