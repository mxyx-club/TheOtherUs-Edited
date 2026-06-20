using TheOtherRoles.Patches;

namespace TheOtherRoles.Roles.Neutral;

public class Pelican
{
    public static PlayerControl Player;
    public static PlayerControl currentTarget;
    public static Dictionary<PlayerControl, Vector2> eatenPlayers = new();
    public static Color color = new Color32(240, 120, 200, byte.MaxValue);
    public static float cooldown = 25f;
    public static float reduceCooldown = 25f;
    public static bool CanUseVent;
    public static bool hasImpVision;

    public static void PelicanKill(byte playerId, byte targetId)
    {
        var player = PlayerById(playerId);
        var target = PlayerById(targetId);
        if (Player.IsDead() || player != Player || target == null) return;
        if (SchrodingersCat.Player != null && target == SchrodingersCat.Player && SchrodingersCat.remainingChange > 0)
        {
            SchrodingersCat.State = SchrodingersCat.CatState.Pelican;
            SchrodingersCat.ChangeCount++;
            Message($"SchrodingersCat.State: {SchrodingersCat.State}");
        }
        else
        {
            var tpos = target.GetTruePosition();
            target.Die(DeathReason.Kill, false);
            MurderPlayerPatch.HandleMurderPostfix(Player, target);
            if (target == SoulSight.Player)
            {
                SoulSight.CanRevive = false;
                SoulSight.Reviveing = false;
            }

            if (target == PlayerControl.LocalPlayer)
            {
                HudManager.Instance.PlayerCam.SetTargetWithLight(Player);
                _ = new LateTask(() =>
                {
                    HudManager.Instance.PlayerCam.SetTargetWithLight(Player);
                }, 0.25f);
            }

            PlayerData.SetDeathReason(target, CustomDeathReason.Eaten, Player);
            eatenPlayers.Add(target, tpos);
            target.NetTransform.RpcSnapTo(new Vector2(-10f, 10f));
        }
    }

    public static void PelicanDie(bool clear = false, byte playerId = byte.MaxValue)
    {
        var player = PlayerById(playerId);
        player ??= Player;
        if (clear || player?.Data.IsDead == true)
        {
            foreach (var p in eatenPlayers.Keys)
            {
                if (p == PlayerControl.LocalPlayer)
                {
                    HudManager.Instance.PlayerCam.SetTargetWithLight(PlayerControl.LocalPlayer);
                    PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(player.transform.position);
                }
            }
            eatenPlayers = new();
            if (clear) clearAndReload(true);
        }
    }

    public static void clearAndReload(bool clear = true)
    {
        Player = null;
        currentTarget = null;
        if (clear) eatenPlayers = new();
        cooldown = CustomOptionHolder.pelicanCooldown.GetFloat();
        reduceCooldown = CustomOptionHolder.pelicanReduceCooldown.GetFloat();
        CanUseVent = CustomOptionHolder.pelicanCanUseVents.GetBool();
        hasImpVision = CustomOptionHolder.pelicanHasImpVision.GetBool();
    }

    [HarmonyPatch]
    public static class Pelican_Patch
    {
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update)), HarmonyPostfix]
        public static void HudUpdate(HudManager __instance)
        {
            if (Player.IsDead() || InMeeting) return;

            foreach (var p in eatenPlayers.Keys)
            {
                if (p == PlayerControl.LocalPlayer)
                {
                    HudManager.Instance.ShadowQuad?.gameObject?.SetActive(true);
                    PlayerControl.LocalPlayer.transform.position = new(-10f, 10f, 0f);
                }
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start)), HarmonyPostfix]
        public static void MeetingStart(MeetingHud __instance)
        {
            if (Player.IsDead()) return;
            foreach (var p in eatenPlayers.Keys)
            {
                if (p == PlayerControl.LocalPlayer)
                {
                    _ = new LateTask(() =>
                    {
                        HudManager.Instance.ShadowQuad?.gameObject?.SetActive(false);
                        PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(Player.GetTruePosition());
                        HudManager.Instance.PlayerCam.SetTargetWithLight(PlayerControl.LocalPlayer);
                        p.Exiled();
                    }, 0.25f);
                }
            }
            eatenPlayers = new();
        }
    }
}
