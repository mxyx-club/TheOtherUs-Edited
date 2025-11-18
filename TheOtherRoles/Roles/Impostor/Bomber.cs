// 延期适配
using TheOtherRoles.Objects;
using static TheOtherRoles.Modules.ModInputManager;

namespace TheOtherRoles.Roles.Impostor;

[CustomRpcHolder]
public class Bomber : RoleBase
{
    public static Color color = Palette.ImpostorRed;

    public static readonly RoleInfo roleinfo = new(
        typeof(Bomber),
        (p) => new Bomber(p),
        RoleId.Bomber,
        RoleType.Impostor,
        "Bomber",
        color,
        101200,
        AddOptions
    );

    public Bomber(PlayerControl p) : base(p, roleinfo) { }

    public static Dictionary<Bomber, PlayerControl> hasBombPlayer = new();

    public static float cooldown = 30f;
    public static float bombDelay = 10f;
    public static float bombTimer = 10f;
    public static bool triggerBothCooldowns;
    public static bool canGiveToBomber;
    public static bool hotPotatoMode;


    public bool bombActive;
    public bool hasAlerted;
    public int timeLeft;

    public PlayerControl currentTarget;
    public static PlayerControl currentBombTarget;

    public static CustomOption bomberBombCooldown;
    public static CustomOption bomberDelay;
    public static CustomOption bomberTimer;
    public static CustomOption bomberTriggerBothCooldowns;
    public static CustomOption bomberCanGiveToBomber;
    public static CustomOption bomberHotPotatoMode;

    public static ResourceSprite buttonSprite = new("Bomber2.png");

    public static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        bomberBombCooldown = CustomOption.Create(configId++, CustomOptionType.Impostor, "bomberBombCooldown", 25f, 10f, 60f, 2.5f, roleinfo.RoleOption);
        bomberDelay = CustomOption.Create(configId++, CustomOptionType.Impostor, "bomberDelay", 5f, 0f, 20f, 0.5f, roleinfo.RoleOption);
        bomberTimer = CustomOption.Create(configId++, CustomOptionType.Impostor, "bomberTimer", 10f, 5f, 30f, 0.5f, roleinfo.RoleOption);
        bomberTriggerBothCooldowns = CustomOption.Create(configId++, CustomOptionType.Impostor, "bomberTriggerBothCooldowns", false, roleinfo.RoleOption);
        bomberCanGiveToBomber = CustomOption.Create(configId++, CustomOptionType.Impostor, "bomberCanGiveToBomber", false, roleinfo.RoleOption);
        bomberHotPotatoMode = CustomOption.Create(configId++, CustomOptionType.Impostor, "bomberHotPotatoMode", true, roleinfo.RoleOption);

    }

    public override void Initialize()
    {
        currentTarget = null;
        currentBombTarget = null;
        hasBombPlayer = new();
        bombActive = false;
        cooldown = bomberBombCooldown.GetFloat();
        bombDelay = bomberDelay.GetFloat();
        bombTimer = bomberTimer.GetFloat();
        triggerBothCooldowns = bomberTriggerBothCooldowns.GetBool();
        canGiveToBomber = bomberCanGiveToBomber.GetBool();
        hotPotatoMode = bomberHotPotatoMode.GetBool();
    }

    public override void OnReportDeadBody(PlayerControl reporter, GameData.PlayerInfo target)
    {
        CustomMurderPlayer(Player, hasBombPlayer[this], false);
        RpcGiveBomb.Invoke((Player.PlayerId, byte.MaxValue, false));
    }

    public override void OnMurderPlayer(MurderInfo Info)
    {
        if (triggerBothCooldowns && Player != null &&
            PlayerControl.LocalPlayer == Player && Info.Killer == Player &&
            bomberBombButton != null)
            bomberBombButton.Timer = bomberBombButton.MaxTimer;
    }

    public static CustomButton bomberBombButton;

    public override void CreateButton(HudManager __instance)
    {
        bomberBombButton.Destroy();
        bomberBombButton = new CustomButton(
            () =>
            {
                // On Use
                if (CheckAndDoVetKill(Player, currentTarget)) return;
                RpcGiveBomb.Invoke((PlayerControl.LocalPlayer.PlayerId, currentTarget.PlayerId, false));
                if (triggerBothCooldowns)
                {
                    Player.killTimer = bomberBombButton.MaxTimer * Mini.Multiplier;
                }
                bomberBombButton.Timer = bomberBombButton.MaxTimer;
            },
            () =>
            {
                // Can See
                return Player == PlayerControl.LocalPlayer && Player.IsAlive();
            },
            () =>
            {
                // On Click
                currentTarget = SetTarget();
                if (hasBombPlayer == null) SetPlayerOutline(currentTarget, color);
                return currentTarget && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                // On Meeting End
                bomberBombButton.Timer = bomberBombButton.MaxTimer;
                bomberBombButton.isEffectActive = false;
                bomberBombButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
                hasBombPlayer = null;
            },
            buttonSprite,
            __instance,
            __instance.AbilityButton,
            abilityInput.keyCode,
            buttonText: "giveBombText".Translate()
        )
        {
            MaxTimer = cooldown,
        };

    }


    public static RemoteProcess<(byte playerId, byte targetId, bool bomb)> RpcGiveBomb = new("RpcGiveBomb", (data, _) =>
    {
        var player = PlayerById(data.playerId);
        var target = PlayerById(data.targetId);
        if (!player.TryGetRole<Bomber>(out var bomber)) return;
        if (data.targetId == byte.MaxValue)
        {
            hasBombPlayer = null;
            bomber.bombActive = false;
            bomber.hasAlerted = false;
            bomber.timeLeft = 0;
            return;
        }

        if (data.bomb)
        {
            hasBombPlayer[bomber] = target;
            bomber.timeLeft += (int)0.5;
            return;
        }

        hasBombPlayer[bomber] = target;
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(bombDelay,
            new Action<float>(p =>
            {
                if (p == 1f) bomber.bombActive = true;
            })));
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(bombDelay + bombTimer,
            new Action<float>(p =>
            {
                // Delayed action
                if (hasBombPlayer[bomber].IsDead()) return;
                if (p == 1f && bomber.bombActive)
                {
                    // Perform kill if possible and reset bitten (regardless whether the kill was successful or not)
                    if (bomber.Player.IsAlive() && PlayerControl.LocalPlayer == bomber.Player)
                        RpcCustomMurderPlayer(bomber.Player, hasBombPlayer[bomber], false, false, CustomDeathReason.Kill);
                    hasBombPlayer = null;
                    bomber.bombActive = false;
                    bomber.hasAlerted = false;
                    bomber.timeLeft = 0;
                }

                if (PlayerControl.LocalPlayer == hasBombPlayer[bomber])
                {
                    var totalTime = (int)(bombDelay + bombTimer);
                    var timeLeft = (int)(totalTime - (totalTime * p));
                    if (timeLeft <= bombTimer)
                    {
                        if (bomber.timeLeft != timeLeft)
                        {
                            var message = new CustomMessage("你手中的炸弹将在 " + timeLeft + " 秒后引爆!", 1f);
                            bomber.timeLeft = timeLeft;
                        }

                        if (timeLeft % 5 == 0)
                        {
                            if (!bomber.hasAlerted)
                            {
                                Coroutines.Start(showFlashCoroutine(Palette.ImpostorRed, 0.75f));
                                bomber.hasAlerted = true;
                            }
                        }
                        else
                        {
                            bomber.hasAlerted = false;
                        }
                    }
                }
            })));
    });

}