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

    public static void PelicanKill(byte targetId)
    {
        var target = playerById(targetId);
        if (Player.IsDead() || target == null) return;
        target.Die(DeathReason.Kill, false);
        MurderPlayerPatch.HandleMurderPostfix(Player, target);
        target.NetTransform.RpcSnapTo(new Vector3(-10f, 10f, 0f));
        GameHistory.OverrideDeathReasonAndKiller(target, CustomDeathReason.Eaten, Player);
        eatenPlayers.Add(target);
    }

    public static void PelicanDie()
    {
        if (Player == null)
        {
            if (eatenPlayers.Any(x => x == PlayerControl.LocalPlayer))
            {
                HudManager.Instance.PlayerCam.Target = PlayerControl.LocalPlayer;
                PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(Player.transform.position);
            }
            eatenPlayers = new();
        }

        if (Player?.Data.IsDead == true)
        {
            HudManager.Instance.PlayerCam.Target = PlayerControl.LocalPlayer;
            PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(Player.transform.position);
        };
    }

    public static void clearAndReload(bool clear = true)
    {
        Player = null;
        currentTarget = null;
        if (clear) eatenPlayers = new();
        cooldown = CustomOptionHolder.pelicanCooldown.GetFloat();
        reduceCooldown = CustomOptionHolder.pelicanReduceCooldown.GetFloat();
    }
}
