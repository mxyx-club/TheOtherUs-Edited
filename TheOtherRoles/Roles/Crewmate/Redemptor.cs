using Reactor.Utilities.Extensions;
using TheOtherRoles.Objects;
using TMPro;
using UnityEngine;

namespace TheOtherRoles.Roles.Crewmate;
public class Redemptor
{
    public static PlayerControl Player;
    public static Color color = new Color32(255, 216, 70, byte.MaxValue);
    public static PlayerControl target;
    public static Arrow arrow;
    public static bool Revelating;
    public static bool Reviving;
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
        var player = playerById(targetId);
        player?.ModRevive();
        RevivedPlayer = player;
        target = null;
    }

    ///<summary>
    /// off = 0, on > 0
    /// </summary>
    public static void RedemptorPrayer(byte status)
    {
        Reviving = status != 0;
    }

    public static void ClearAndReload(bool clear = true)
    {
        Player = null;
        target = null;
        if (clear) RevivedPlayer = null;
        arrow?.arrow?.Destroy();
        Reviving = false;
        Revelating = false;
        if (text != null) Object.Destroy(text);
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
    public static class Redemptor_Patch
    {
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start)), HarmonyPostfix]
        public static void MeetingStartPatch()
        {
            if (Reviving)
            {
                Reviving = false;
                target = null;
                RevivedPlayer = null;
            }
            if (Revelating)
            {
                target = null;
                Revelating = false;
            }
            RevivedPlayer = null;
        }
    }
}
