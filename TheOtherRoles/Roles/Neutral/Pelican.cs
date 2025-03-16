using System.Collections.Generic;
using TheOtherRoles.Patches;
using UnityEngine;

namespace TheOtherRoles.Roles.Neutral;

public class Pelican
{
    public static PlayerControl Player;
    public static PlayerControl currentTarget;
    public static List<PlayerControl> eatenPlayers = new();
    public static Color color = new Color32(240, 120, 200, byte.MaxValue);
    public static float cooldown = 25f;
    public static float reduceCooldown = 25f;
    public static bool CanUseVent;
    public static bool hasImpVision;

    public static void PelicanKill(byte playerId, byte targetId)
    {
        var player = playerById(playerId);
        var target = playerById(targetId);
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
            GameHistory.OverrideDeathReasonAndKiller(target, CustomDeathReason.Eaten, Player);
            eatenPlayers.Add(target);
            target.NetTransform.RpcSnapTo(new Vector2(-10f, 10f));
        }
    }

    public static void PelicanDie(bool clear = false, byte playerId = byte.MaxValue)
    {
        var player = playerById(playerId);
        player ??= Player;
        if (clear || player?.Data.IsDead == true)
        {
            if (eatenPlayers.Any(x => x == PlayerControl.LocalPlayer))
            {
                HudManager.Instance.PlayerCam.Target = PlayerControl.LocalPlayer;
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
        if (clear) eatenPlayers = new();
        cooldown = CustomOptionHolder.pelicanCooldown.GetFloat();
        reduceCooldown = CustomOptionHolder.pelicanReduceCooldown.GetFloat();
        CanUseVent = CustomOptionHolder.pelicanCanUseVents.GetBool();
        hasImpVision = CustomOptionHolder.pelicanHasImpVision.GetBool();
    }
}
