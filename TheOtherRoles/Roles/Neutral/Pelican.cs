using TheOtherRoles.Patches;

namespace TheOtherRoles.Roles.Neutral;

public class Pelican
{
    public static PlayerControl Player;
    public static PlayerControl currentTarget;
    public static List<PlayerControl> eatenPlayers = new();
    public static bool DieOnExile;
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
            target.Die(DeathReason.Kill, false);
            MurderPlayerPatch.HandleMurderPostfix(Player, target);

            if (target == PlayerControl.LocalPlayer)
            {
                HudManager.Instance.PlayerCam.SetTargetWithLight(Player);
                _ = new LateTask(() =>
                {
                    HudManager.Instance.PlayerCam.SetTargetWithLight(Player);
                }, 0.25f);
            }

            GameHistory.OverrideDeathReasonAndKiller(target, CustomDeathReason.Eaten, Player);
            eatenPlayers.Add(target);
            target.NetTransform.RpcSnapTo(new Vector2(-10f, 10f));
        }
    }

    public static void PelicanDie(bool clear = false, byte playerId = byte.MaxValue)
    {
        var player = PlayerById(playerId);
        player ??= Player;
        if (clear || player?.Data.IsDead == true)
        {
            if (eatenPlayers.Any(x => x == PlayerControl.LocalPlayer))
            {
                HudManager.Instance.PlayerCam.SetTargetWithLight(PlayerControl.LocalPlayer);
                _ = new LateTask(() =>
                {
                    HudManager.Instance.PlayerCam.SetTargetWithLight(PlayerControl.LocalPlayer);
                }, 0.25f);
                PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(player.transform.position);
            }
            Message($"Pelican Player {Player?.Data.PlayerName ?? "null"}", "Pelican");
            if (clear) clearAndReload(true);
        }
    }

    public static void clearAndReload(bool clear = true)
    {
        Player = null;
        currentTarget = null;
        DieOnExile = false;
        if (clear) eatenPlayers = new();
        cooldown = CustomOptionHolder.pelicanCooldown.GetFloat();
        reduceCooldown = CustomOptionHolder.pelicanReduceCooldown.GetFloat();
        CanUseVent = CustomOptionHolder.pelicanCanUseVents.GetBool();
        hasImpVision = CustomOptionHolder.pelicanHasImpVision.GetBool();
    }

    [HarmonyPatch]
    public class Pelican_Patch
    {

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update)), HarmonyPostfix]
        public static void HudUpdate()
        {
            if (Player == null) return;
            if (Player.IsAlive() && eatenPlayers.Any(x => x == PlayerControl.LocalPlayer) && !InMeeting)
            {
                HudManager.Instance.ShadowQuad?.gameObject?.SetActive(true);
                PlayerControl.LocalPlayer.transform.position = new(-10f, 10f, 0f);
            }
        }
    }
}
