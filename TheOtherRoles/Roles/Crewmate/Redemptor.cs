using TheOtherRoles.Objects;

namespace TheOtherRoles.Roles.Crewmate;

[CustomRpcHolder]
public class Redemptor : RoleBase
{
    public static Color color = new Color32(255, 216, 70, byte.MaxValue);

    public static readonly RoleInfo roleinfo = new(
        typeof(Redemptor),
        (p) => new Redemptor(p),
        RoleId.Redemptor,
        RoleType.Crewmate,
        "Redemptor",
        color,
        303900,
        AddOptions
    );
    public Redemptor(PlayerControl p) : base(p, roleinfo) { }

    public PlayerControl RevivedPlayer;
    public PlayerControl Target;
    public Arrow arrow;
    public bool Revelating;
    public bool Prayering;

    public static bool revelation;
    public static float revelationCooldown;
    public static float revelationDuration;
    public static bool prayer;
    public static float prayerCooldown;
    public static float prayerDuration;
    public static float reviveDuration;

    public CustomButton redemptorReviveButton;
    public CustomButton redemptorRevelationButton;
    public CustomButton redemptorPrayerButton;
    public static Sprite reviveButton = new ResourceSprite("Revive.png");
    public static TextMeshPro text;

    public static CustomOption redemptorRevelation;
    public static CustomOption redemptorRevelationCooldown;
    public static CustomOption redemptorRevelationDuration;
    public static CustomOption redemptorPrayer;
    public static CustomOption redemptorPrayerCooldown;
    public static CustomOption redemptorPrayerDuration;
    public static CustomOption redemptorReviveDuration;

    private static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        redemptorRevelation = CustomOption.Create(configId++, CustomOptionType.Crewmate, "redemptorRevelation", false, roleinfo.RoleOption);
        redemptorRevelationCooldown = CustomOption.Create(configId++, CustomOptionType.Crewmate, "redemptorRevelationCooldown", 25f, 10f, 60f, 2.5f, redemptorRevelation);
        redemptorRevelationDuration = CustomOption.Create(configId++, CustomOptionType.Crewmate, "redemptorRevelationDuration", 5f, 1f, 15f, 0.5f, redemptorRevelation);
        redemptorPrayer = CustomOption.Create(configId++, CustomOptionType.Crewmate, "redemptorPrayer", false, roleinfo.RoleOption);
        redemptorPrayerCooldown = CustomOption.Create(configId++, CustomOptionType.Crewmate, "redemptorPrayerCooldown", 25f, 10f, 60f, 2.5f, redemptorPrayer);
        redemptorPrayerDuration = CustomOption.Create(configId++, CustomOptionType.Crewmate, "redemptorPrayerDuration", 5f, 2f, 15f, 0.5f, redemptorPrayer);
        redemptorReviveDuration = CustomOption.Create(configId++, CustomOptionType.Crewmate, "redemptorReviveDuration", 1.5f, 0f, 15f, 0.5f, roleinfo.RoleOption);

    }

    public override void Initialize()
    {
        Target = null;
        RevivedPlayer = null;
        arrow?.arrow?.Destroy();
        Prayering = false;
        Revelating = false;
        if (text != null) UObject.Destroy(text);
        text = null;
        revelation = redemptorRevelation.GetBool();
        revelationCooldown = redemptorRevelationCooldown.GetFloat();
        revelationDuration = redemptorRevelationDuration.GetFloat();
        prayer = redemptorPrayer.GetBool();
        prayerCooldown = redemptorPrayerCooldown.GetFloat();
        prayerDuration = redemptorPrayerDuration.GetFloat();
        reviveDuration = redemptorReviveDuration.GetFloat();
    }

    public static RemoteProcess<(PlayerControl player, bool status)> RedemptorPrayer = new("RedemptorPrayer", (data, _) =>
    // public static void RedemptorPrayer(byte playerId, bool status)
    {
        if (data.player.TryGetRole<Redemptor>(out var role))
        {
            role.Prayering = data.status;
        }
    });

    public static RemoteProcess<(PlayerControl player, PlayerControl target)> RevivePlayer = new("RevivePlayer", (data, _) =>
    // public static void RevivePlayer(byte playerId, byte targetId)
    {
        if (data.player.TryGetRole<Redemptor>(out var role))
        {
            data.target?.ModRevive();
            role.RevivedPlayer = data.target;
            role.Target = null;
        }
    });

    public override void OnMeetingStart(MeetingHud __instance)
    {
        RevivedPlayer = null;
    }

    public override void OnExiledBegin(GameData.PlayerInfo exiled)
    {
        Target = null;
        Prayering = false;
        Revelating = false;
        RevivedPlayer = null;
    }

    public override void OnHudUpdate(HudManager hudManager)
    {
        if (Prayering && (Player.IsDead() || InMeeting))
        {
            Prayering = false;
            Target = null;
            RevivedPlayer = null;
        }
        if (Revelating && InMeeting)
        {
            Target = null;
            Revelating = false;
            RevivedPlayer = null;
        }

        //RedemptorUpdate
        var local = PlayerControl.LocalPlayer;
        if (Player.IsAlive() && Prayering && local.IsAlive() && local.IsKiller())
        {
            arrow ??= new Arrow(color);
            if (arrow != null)
            {
                arrow.arrow.SetActive(true);
                arrow.Update(Player.transform.position);
            }
        }
        else if (RevivedPlayer.IsAlive() && local.IsAlive() && local.IsKiller())
        {
            arrow ??= new Arrow(color);
            if (arrow != null)
            {
                arrow.arrow.SetActive(true);
                arrow.Update(RevivedPlayer.transform.position);
            }
        }
        else if (local == Player && Revelating)
        {
            var array = UObject.FindObjectsOfType<DeadBody>()?.FirstOrDefault(x => !(PlayerById(x.ParentId)?.Data?.Disconnected == true));
            if (array != null)
            {
                arrow ??= new Arrow(color);
                arrow.arrow.SetActive(true);
                arrow.Update(array.transform.position);
            }
        }
        else
        {
            arrow?.arrow?.Destroy();
        }

        //RedemptorTextUpdate
        if (Player == null && RevivedPlayer == null) return;

        if ((RevivedPlayer.IsAlive() || Prayering) && ((local.IsAlive() && local.IsKiller()) || local == Player || CanSeeGhostInfo))
        {
            if (text == null)
            {
                text = UObject.Instantiate(FastDestroyableSingleton<HudManager>.Instance.KillButton.cooldownTimerText, FastDestroyableSingleton<HudManager>.Instance.transform);
                text.enableWordWrapping = false;
                text.transform.localScale = Vector3.one * 0.7f;
                text.transform.localPosition += new Vector3(0f, 1.9f, -69f);
                text.gameObject.SetActive(true);
            }
            else if (Prayering && Player.IsAlive())
            {
                text.text = $"牧师正在祈祷！";
            }
            else if (RevivedPlayer.IsAlive())
            {
                text.text = $"有玩家已被复活！";
            }
            else
            {
                text?.Destroy();
                text = null;
            }
        }
        else if (text != null)
        {
            text.Destroy();
            text = null;
        }
    }

    public override void CleanUp(HudManager __instance)
    {
        redemptorRevelationButton?.Destroy();
        redemptorPrayerButton?.Destroy();
        redemptorReviveButton?.Destroy();
        redemptorRevelationButton = null;
        redemptorPrayerButton = null;
        redemptorReviveButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        redemptorRevelationButton = new CustomButton(
            () =>
            {
                Revelating = true;
            },
            () =>
            {
                return Player == PlayerControl.LocalPlayer && Player.IsAlive() &&
                    revelation;
            },
            () =>
            {
                return PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                Revelating = false;
                redemptorRevelationButton.Timer = redemptorRevelationButton.MaxTimer;
                redemptorRevelationButton.isEffectActive = false;
                redemptorRevelationButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            Tracker.trackCorpsesButtonSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.secondaryAbilityInput.keyCode,
            true,
            revelationDuration,
            () =>
            {
                Revelating = false;
                redemptorRevelationButton.Timer = redemptorRevelationButton.MaxTimer;
            },
            buttonText: GetString("RedemptorRevelation")
        )
        {
            MaxTimer = revelationCooldown,
            EffectDuration = revelationDuration
        };

        redemptorPrayerButton = new CustomButton(
            () =>
            {
                RedemptorPrayer.Invoke((Player, true));
            },
            () =>
            {
                return Player == PlayerControl.LocalPlayer && Player.IsAlive() && prayer && RevivedPlayer == null;
            },
            () =>
            {
                if (Target != null)
                {
                    redemptorPrayerButton.showTargetNameOnButton(Target, GetString("ReviveButton"));
                }
                return Target && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                redemptorPrayerButton.Timer = redemptorPrayerButton.MaxTimer;
                redemptorPrayerButton.isEffectActive = false;
                redemptorPrayerButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            reviveButton,
            __instance,
            __instance.AbilityButton,
            ModInputManager.modKillInput.keyCode,
            true,
            prayerDuration,
            () =>
            {
                RedemptorPrayer.Invoke((Player, false));

                if (Target != null)
                {
                    RevivePlayer.Invoke((Player, Target));

                    redemptorPrayerButton.Timer = redemptorPrayerButton.MaxTimer;
                }
            },
            buttonText: GetString("RedemptorPrayer")
        )
        {
            MaxTimer = prayerCooldown,
            EffectDuration = prayerDuration
        };

        redemptorReviveButton?.Destroy();
        redemptorReviveButton = new CustomButton(
            () =>
            {
                var target = Target;
                if (target == null) return;

                RpcCustomMurderPlayer(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer, false, true, CustomDeathReason.Suicide);

                _ = new LateTask(() =>
                {
                    RevivePlayer.Invoke((Player, Target));
                }, reviveDuration, "RedemptorRevive");
            },
            () =>
            {
                return Player == PlayerControl.LocalPlayer && Player.IsAlive();
            },
            () =>
            {
                var pos = PlayerControl.LocalPlayer.GetTruePosition();
                var maxDistance = PlayerControl.LocalPlayer.MaxReportDistance * 0.21f;

                var deadBody = Physics2D.OverlapCircleAll(pos, maxDistance, Constants.PlayersOnlyMask)
                    .Where(collider => collider.CompareTag("DeadBody"))
                    .Select(collider => collider.GetComponent<DeadBody>())
                    .FirstOrDefault(db => db != null && PlayerById(db.ParentId)?.Data?.IsDead == true && !(PlayerById(db.ParentId)?.Data?.Disconnected == true));

                if (Target != null)
                {
                    redemptorReviveButton.showTargetNameOnButton(Target, GetString("RedemptorRevive"));
                }
                Target = PlayerById(deadBody?.ParentId);
                return Target && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                redemptorReviveButton.Timer = 10f;
            },
            reviveButton,
            __instance,
            __instance.AbilityButton,
            ModInputManager.abilityInput.keyCode,
            buttonText: GetString("RedemptorRevive")
        )
        {
            MaxTimer = 10f
        };
    }
}

