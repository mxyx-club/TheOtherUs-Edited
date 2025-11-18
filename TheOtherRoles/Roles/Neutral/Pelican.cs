using TheOtherRoles.Patches;

namespace TheOtherRoles.Roles.Neutral;

[CustomRpcHolder]
public class Pelican : RoleBase, INeutral
{
    public static Color color = new Color32(240, 120, 200, byte.MaxValue);

    public static RoleInfo roleinfo = new(
        typeof(Pelican),
        (p) => new Pelican(p),
        RoleId.Pelican,
        RoleType.Neutral,
        "Pelican",
        color,
        206900,
        AddOptions,
        IsKiller: true
    );

    public Pelican(PlayerControl player) : base(player, roleinfo) { }


    private static readonly HashSet<byte> _eatenPlayerId = new();

    public NeutralType NeutralType => NeutralType.Kill;
    public override bool? CanUseVent => pelicanCanUseVents.GetBool();
    public override bool? HasImpVision => pelicanHasImpVision.GetBool();
    public override bool? IsKiller => true;

    public PlayerControl currentTarget;
    public List<PlayerControl> eatenPlayers = new();
    public static float cooldown = 25f;
    public static float reduceCooldown = 25f;
    public bool DieOnExile;

    public static CustomOption pelicanCooldown;
    public static CustomOption pelicanReduceCooldown;
    public static CustomOption pelicanHasImpVision;
    public static CustomOption pelicanCanUseVents;

    public CustomButton pelicanKillButton;

    public static bool HasEaten(byte playerId)
    {
        return _eatenPlayerId.Contains(playerId);
    }

    public static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        pelicanCooldown = CustomOption.Create(configId++, CustomOptionType.Neutral, "pelicanCooldown", 25f, 2.5f, 60f, 2.5f, roleinfo.RoleOption);
        pelicanReduceCooldown = CustomOption.Create(configId++, CustomOptionType.Neutral, "pelicanReduceCooldown", 20f, 2.5f, 60f, 2.5f, roleinfo.RoleOption);
        pelicanCanUseVents = CustomOption.Create(configId++, CustomOptionType.Neutral, "canUseVents", true, roleinfo.RoleOption);
        pelicanHasImpVision = CustomOption.Create(configId++, CustomOptionType.Neutral, "hasImpVision", true, roleinfo.RoleOption);
    }

    public static RemoteProcess<(PlayerControl player, PlayerControl target)> PelicanKill = new("PelicanKill", (data, _) =>
    // public static void PelicanKill(byte playerId, byte targetId)
    {
        if (data.player.TryGetRole<Pelican>(out var pelican))
        {
            if (data.player.IsDead() || data.target == null) return;
            data.target.Die(DeathReason.Kill, false);
            MurderPlayerPatch.HandleMurderPostfix(data.player, data.target);
            data.target.NetTransform.RpcSnapTo(new Vector3(-10f, 10f, 0f));
            PlayerData.SetDeathReason(data.target, CustomDeathReason.Eaten, data.player);
            pelican.eatenPlayers.Add(data.target);
            _eatenPlayerId.Add(data.target.PlayerId);
        }
    });

    public void PelicanDie()
    {
        if (Player?.Data.IsDead == true)
        {
            if (eatenPlayers.Any(x => x == PlayerControl.LocalPlayer))
            {
                HudManager.Instance.PlayerCam.Target = PlayerControl.LocalPlayer;
                PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(Player.transform.position);
            }
            Message($"Pelican Player {Player?.Data.PlayerName ?? "null"}", "Pelican");
        }
    }

    public override void Initialize()
    {
        currentTarget = null;
        eatenPlayers = new();
        _eatenPlayerId.Clear();
        DieOnExile = false;
        cooldown = pelicanCooldown.GetFloat();
        reduceCooldown = pelicanReduceCooldown.GetFloat();
        HasImpVision = pelicanHasImpVision.GetBool();
    }

    public override void OnHudUpdate(HudManager hudManager)
    {
        if (Player.IsAlive() && eatenPlayers.Any(x => x == PlayerControl.LocalPlayer) && !InMeeting)
        {
            HudManager.Instance.ShadowQuad?.gameObject?.SetActive(true);
            HudManager.Instance.PlayerCam.SetTargetWithLight(Player);
            PlayerControl.LocalPlayer.transform.position = new(-10f, 10f, 0f);
        }
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        foreach (var player in eatenPlayers)
        {
            if (player.AmOwner)
            {
                HudManager.Instance.PlayerCam.SetTargetWithLight(PlayerControl.LocalPlayer);
                PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(Player.transform.position);
            }
            player.Die(DeathReason.Kill, true);
            _eatenPlayerId.Remove(player.PlayerId);
        }
        eatenPlayers.Clear();
    }

    public override void OnMurderPlayer(MurderInfo Info)
    {
        if (Info.Target.PlayerId == Player.PlayerId && eatenPlayers.Count > 0)
        {
            foreach (var player in eatenPlayers.ToArray().Where(p => p != null))
            {
                player.Revive();

                if (PlayerControl.LocalPlayer == player)
                {
                    HudManager.Instance.PlayerCam.SetTargetWithLight(PlayerControl.LocalPlayer);
                    _ = new LateTask(() =>
                    {
                        HudManager.Instance.PlayerCam.SetTargetWithLight(PlayerControl.LocalPlayer);
                    }, 0.25f);
                }
                _eatenPlayerId.Remove(player.PlayerId);
                continue;
            }
            eatenPlayers = new();
            DieOnExile = true;
        }
    }

    public override void OnPlayerExlied(PlayerControl player)
    {
        if (player.PlayerId == Player?.PlayerId && eatenPlayers?.Count > 0)
        {
            foreach (var p in eatenPlayers.Where(p => p != null && p.Data.IsDead))
            {
                if (PlayerControl.LocalPlayer == p)
                {
                    HudManager.Instance.PlayerCam.SetTargetWithLight(PlayerControl.LocalPlayer);
                    PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(Player.transform.position);
                }
                _eatenPlayerId.Remove(player.PlayerId);
                continue;
            }
            foreach (var p in eatenPlayers) p.Die(DeathReason.Kill, true);
            eatenPlayers.Clear();

            PelicanDie();
        }
    }

    public override void OnPlayerDisconnect(PlayerControl player)
    {
        if (player == Player && eatenPlayers?.Count > 0)
        {
            foreach (var p in eatenPlayers.ToArray())
            {
                if (p != null && p.Data.IsDead)
                {
                    p.Revive();

                    if (p.AmOwner)
                    {
                        var pos = HudManager.Instance.PlayerCam.transform.position;
                        HudManager.Instance.PlayerCam.SetTargetWithLight(PlayerControl.LocalPlayer);
                        _ = new LateTask(() =>
                        {
                            HudManager.Instance.PlayerCam.SetTargetWithLight(PlayerControl.LocalPlayer);
                        }, 0.25f);
                        PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(pos);
                    }
                }
            }
            Destroy();
        }
    }

    public override void CleanUp(HudManager __instance)
    {
        pelicanKillButton?.Destroy();
        pelicanKillButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        pelicanKillButton?.Destroy();
        pelicanKillButton = new CustomButton(
            () =>
            {
                if (currentTarget == null) return;
                var target = currentTarget;
                if (CheckAndDoVetKill(PlayerControl.LocalPlayer, currentTarget)) return;
                if (!RoleHelpers.CheckMurderPlayer(PlayerControl.LocalPlayer, target))
                    return;

                PelicanKill.Invoke((PlayerControl.LocalPlayer, currentTarget));

                pelicanKillButton.Timer = reduceCooldown;
                currentTarget = null;
            },
            () =>
            {
                return Player == PlayerControl.LocalPlayer && Player.IsAlive();
            },
            () =>
            {
                var untargetablePlayers = new List<PlayerControl>();
                //if (Mini.mini != null && !Mini.isGrownUp()) untargetablePlayers.Add(Mini.mini);
                currentTarget = SetTarget(untarget: untargetablePlayers);
                SetPlayerOutline(currentTarget, Palette.ImpostorRed);

                pelicanKillButton.showTargetNameOnButton(currentTarget, GetString("VultureText"));
                return currentTarget && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                //pelicanKillButton.MaxTimer = Pelican.cooldown;
                pelicanKillButton.Timer = pelicanKillButton.MaxTimer;
            },
            __instance.KillButton.graphic.sprite,
            __instance,
            __instance.KillButton,
            ModInputManager.modKillInput.keyCode,
            buttonText: GetString("VultureText")
        )
        {
            MaxTimer = cooldown,
        };
    }
}
