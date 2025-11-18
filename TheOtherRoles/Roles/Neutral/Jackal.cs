using AmongUs.GameOptions;
using TheOtherRoles.Attributes;
using TheOtherRoles.Mode;

namespace TheOtherRoles.Roles.Neutral;

[CustomRpcHolder]
public class Jackal : RoleBase, INeutral
{

    public static Color color = new Color32(0, 180, 235, byte.MaxValue);

    public static RoleInfo roleinfo = new(
        typeof(Jackal),
        (p) => new Jackal(p),
        RoleId.Jackal,
        RoleType.Neutral,
        "Jackal",
        color,
        206100,
        AddOptions,
        IsKiller: true
    );

    public Jackal(PlayerControl player) : base(player, roleinfo) { }

    public NeutralType NeutralType => NeutralType.Kill;
    public override bool? CanUseVent => jackalCanUseVents.GetBool();
    public override bool? HasImpVision => jackalAndSidekickHaveImpostorVision.GetBool();
    public override bool? IsKiller => true;

    public PlayerControl MySidekick;
    public PlayerControl currentTarget;
    public bool CanCreateSidekick;
    public static bool canCreateSidekick;

    public static float cooldown = 30f;
    public static float createSidekickCooldown = 30f;
    public static bool canSabotage;
    public static bool killFakeImpostor;

    public static float chanceSwoop;
    public static bool canSwoop;
    public static float swoopCooldown = 30f;
    public static float duration = 30f;

    public bool IsInvisable;
    public float swoopTimer;

    public static CustomOption jackalChanceSwoop;
    public static CustomOption jackalKillCooldown;
    public static CustomOption jackalSwooperCooldown;
    public static CustomOption jackalSwooperDuration;
    public static CustomOption jackalCanUseVents;
    public static CustomOption jackalCanUseSabo;
    public static CustomOption jackalAndSidekickHaveImpostorVision;
    public static CustomOption jackalCanCreateSidekick;
    public static CustomOption jackalCreateSidekickCooldown;
    public static CustomOption jackalkillFakeImpostor;
    public static CustomOption sidekickCanKill;
    public static CustomOption sidekickCanUseVents;
    public static CustomOption sidekickPromotesToJackal;
    public static CustomOption jackalPromotedFromSidekickCanCreateSidekick;

    public CustomButton jackalKillButton;
    public CustomButton jackalSwoopButton;
    public CustomButton jackalCreateSidekickButton;
    public static Sprite SidekickButton = new ResourceSprite("SidekickButton.png");
    public static RemoteProcess<PlayerControl> JackalSidekickPromotes = new("JackalSidekickPromotes", (player, _) =>
    {
        if (player == null) return;
        CustomRoleManager.RemoveRole(player);
        var role = CustomRoleManager.CreateRoleById(player.PlayerId, RoleId.Jackal) as Jackal;
        role!.MySidekick = null;
        role.CanCreateSidekick = Sidekick.promotedFromSidekickCanCreateSidekick;
    });
    public static RemoteProcess<(PlayerControl player, PlayerControl target)> JackalCreatesSidekick = new("JackalCreatesSidekick", (data, _) =>
    {
        if (data.target == null) return;
        if (data.player.TryGetRole<Jackal>(out var jackal))
        {
            /*if (Executioner.Target == target && Executioner.executioner != null && !Executioner.executioner.Data.IsDead)
            {
                if (Lawyer.lawyer == null && Executioner.promotesToLawyer)
                {
                    Lawyer.lawyer = Executioner.executioner;
                    Lawyer.Target = Executioner.Target;
                    Executioner.clearAndReload();
                }
                else if (!Executioner.promotesToLawyer)
                {
                    Pursuer.Player.Add(Executioner.executioner);
                    Executioner.clearAndReload();
                }
            }*/

            FastDestroyableSingleton<RoleManager>.Instance.SetRole(data.target, RoleTypes.Crewmate);

            CustomRoleManager.RemoveRole(data.target);
            var sidekick = CustomRoleManager.CreateRoleById(data.target.PlayerId, RoleId.Sidekick) as Sidekick;
            sidekick!.MyJackal = data.player;
            jackal!.MySidekick = data.target;

            if (data.target == PlayerControl.LocalPlayer) SoundEffectsManager.play("jackalSidekick");
            if (HandleGuesser.isGuesserGm && GuesserGM.guesserGamemodeSidekickIsAlwaysGuesser.GetBool() && !HandleGuesser.isGuesser(data.target.PlayerId))
                setGuesserGm(data.target.PlayerId);

            Jackal.canCreateSidekick = false;
        }
    });
    public static RemoteProcess<(PlayerControl player, bool flag)> SetJackalSwoop = new("SetJackalSwoop", (data, _) =>
    // public static void setJackalSwoop(byte playerId, bool flag)
    {
        var target = data.player;
        if (data.player.TryGetRole<Jackal>(out var jackal))
        {
            if (target == null) return;
            if (!data.flag)
            {
                target.cosmetics.currentBodySprite.BodySprite.color = Color.white;
                target.cosmetics.colorBlindText.gameObject.SetActive(DataManager.Settings.Accessibility.ColorBlindMode);
                target.cosmetics.colorBlindText.color = target.cosmetics.colorBlindText.color.SetAlpha(1f);
                if (!isActiveCamoComms && !MushroomSabotageActive) target.setDefaultLook();
                jackal.IsInvisable = false;
                return;
            }

            target.setLook("", 6, "", "", "", "");
            var color = Color.clear;
            var canSee = (PlayerControl.LocalPlayer.GetRole() is RoleId.Jackal or RoleId.Sidekick) || CanSeeGhostInfo;
            if (canSee) color.a = 0.1f;
            target.cosmetics.currentBodySprite.BodySprite.color = color;
            target.cosmetics.colorBlindText.gameObject.SetActive(false);
            target.cosmetics.colorBlindText.color = target.cosmetics.colorBlindText.color.SetAlpha(canSee ? 0.1f : 0f);
            jackal.swoopTimer = Jackal.duration;
            jackal.IsInvisable = true;
        }
    });
    public static RemoteProcess<bool> JackalCanSwooper = new("JackalCanSwooper", (chance, _) =>
    {
        canSwoop = chance;
    });

    public static void setGuesserGm(byte playerId)
    {
        var target = PlayerById(playerId);
        if (target == null) return;
        _ = new GuesserGM(target);
    }
    public static void AddOptions()
    {

        var configId = roleinfo.ConfigId + 2;
        jackalChanceSwoop = CustomOption.Create(configId++, CustomOptionType.Neutral, Cs(Swooper.color, "jackalChanceSwoop"), CustomOptionHolder.rates, roleinfo.RoleOption);
        jackalKillCooldown = CustomOption.Create(configId++, CustomOptionType.Neutral, "killCooldown", 25f, 10f, 60f, 2.5f, roleinfo.RoleOption);
        jackalSwooperCooldown = CustomOption.Create(configId++, CustomOptionType.Neutral, "jackalSwooperCooldown", 25f, 10f, 60f, 2.5f, jackalChanceSwoop);
        jackalSwooperDuration = CustomOption.Create(configId++, CustomOptionType.Neutral, "jackalSwooperDuration", 12.5f, 1f, 20f, 0.5f, jackalChanceSwoop);
        jackalCanUseVents = CustomOption.Create(configId++, CustomOptionType.Neutral, "jackalCanUseVents", true, roleinfo.RoleOption);
        jackalCanUseSabo = CustomOption.Create(configId++, CustomOptionType.Neutral, "jackalCanUseSabo", false, roleinfo.RoleOption);
        jackalAndSidekickHaveImpostorVision = CustomOption.Create(configId++, CustomOptionType.Neutral, "jackalAndSidekickHaveImpostorVision", true, roleinfo.RoleOption);
        jackalCanCreateSidekick = CustomOption.Create(configId++, CustomOptionType.Neutral, Cs(color, "jackalCanCreateSidekick"), false, roleinfo.RoleOption);
        jackalCreateSidekickCooldown = CustomOption.Create(configId++, CustomOptionType.Neutral, "jackalCreateSidekickCooldown", 25f, 10f, 60f, 2.5f, jackalCanCreateSidekick);
        jackalkillFakeImpostor = CustomOption.Create(configId++, CustomOptionType.Neutral, Cs(Palette.ImpostorRed, "jackalkillFakeImpostor"), false, jackalCanCreateSidekick);
        sidekickCanKill = CustomOption.Create(configId++, CustomOptionType.Neutral, "sidekickCanKill", true, jackalCanCreateSidekick);
        sidekickCanUseVents = CustomOption.Create(configId++, CustomOptionType.Neutral, "sidekickCanUseVents", true, jackalCanCreateSidekick);
        sidekickPromotesToJackal = CustomOption.Create(configId++, CustomOptionType.Neutral, "sidekickPromotesToJackal", false, jackalCanCreateSidekick);
        jackalPromotedFromSidekickCanCreateSidekick = CustomOption.Create(configId++, CustomOptionType.Neutral, "jackalPromotedFromSidekickCanCreateSidekick", true, sidekickPromotesToJackal);

    }

    public override void Initialize()
    {
        MySidekick = null;
        currentTarget = null;
        IsInvisable = false;
        CanCreateSidekick = jackalCanCreateSidekick.GetBool();

        cooldown = jackalKillCooldown.GetFloat();
        swoopCooldown = jackalSwooperCooldown.GetFloat();
        duration = jackalSwooperDuration.GetFloat();
        createSidekickCooldown = jackalCreateSidekickCooldown.GetFloat();
        canSabotage = jackalCanUseSabo.GetBool();
        canCreateSidekick = jackalCanCreateSidekick.GetBool();
        killFakeImpostor = jackalkillFakeImpostor.GetBool();
        chanceSwoop = jackalChanceSwoop.GetSelection() / 10f;
    }

    [OnGameStart]
    public static void setSwoop()
    {
        if (AmongUsClient.Instance?.AmHost == true)
        {
            var chance = canSwoop = rnd.NextDouble() < chanceSwoop;
            JackalCanSwooper.Invoke(chance);
        }
    }

    public override void OnFixedUpdate(PlayerControl player)
    {
        if (IsInvisable && swoopTimer <= 0 && player == Player)
        {
            SetJackalSwoop.Invoke((player, false));
        }

        if (Player.IsAlive())
        {
            var untargetablePlayers = new HashSet<PlayerControl>();
            var role = player.GetRole();
            if (role is RoleId.Jackal or RoleId.Sidekick) untargetablePlayers.Add(player);
            if (player.TryGetModifier<Mini>(out var mini) && !mini.isGrownUp()) untargetablePlayers.Add(player);
            currentTarget = SetTarget(untargetablePlayers);
            SetPlayerOutline(currentTarget, Palette.ImpostorRed);
        }
    }

    public override void OnHudUpdate(HudManager hudManager)
    {
        swoopTimer -= Time.deltaTime;
    }

    public override void OnExiledWrapUp(GameData.PlayerInfo exiled)
    {
        if (Sidekick.promotesToJackal && MySidekick.IsAlive() &&
            Player == PlayerControl.LocalPlayer)
        {
            JackalSidekickPromotes.Invoke(MySidekick);
        }

    }

    public override void CleanUp(HudManager __instance)
    {
        jackalKillButton?.Destroy();
        jackalSwoopButton?.Destroy();
        jackalCreateSidekickButton?.Destroy();
        jackalKillButton = null;
        jackalSwoopButton = null;
        jackalCreateSidekickButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        // Jackal Kill
        jackalKillButton?.Destroy();
        jackalKillButton = new CustomButton(
            () =>
            {
                if (CheckAndDoVetKill(PlayerControl.LocalPlayer, currentTarget)) return;
                RpcCustomMurderPlayer(PlayerControl.LocalPlayer, currentTarget, true, false, CustomDeathReason.Kill);

                jackalKillButton.Timer = jackalKillButton.MaxTimer;
            },
            () =>
            {
                return Player.IsAlive() && PlayerControl.LocalPlayer == Player;
            },
            () =>
            {
                jackalKillButton.showTargetNameOnButton(currentTarget, GetString("killButtonText"));
                return currentTarget && PlayerControl.LocalPlayer.CanMove;
            },
            () => { jackalKillButton.Timer = jackalKillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            __instance,
            __instance.KillButton,
            ModInputManager.modKillInput.keyCode
        )
        {
            MaxTimer = cooldown,
        };

        // Jackal Sidekick Button
        jackalCreateSidekickButton?.Destroy();
        jackalCreateSidekickButton = new CustomButton(
            () =>
            {
                var target = currentTarget;
                if (CheckAndDoVetKill(PlayerControl.LocalPlayer, target)) return;

                if (killFakeImpostor && target.Data.Role.IsImpostor)
                {
                    RpcCustomMurderPlayer(PlayerControl.LocalPlayer, target, true, false, CustomDeathReason.FakeSK);
                    jackalCreateSidekickButton.Timer = jackalCreateSidekickButton.MaxTimer;
                    return;
                }

                JackalCreatesSidekick.Invoke((PlayerControl.LocalPlayer, currentTarget));
                SoundEffectsManager.play("jackalSidekick");
                CanCreateSidekick = false;
                jackalCreateSidekickButton.Timer = jackalCreateSidekickButton.MaxTimer;
            },
            () =>
            {
                return canCreateSidekick && CanCreateSidekick && Player.IsAlive() && PlayerControl.LocalPlayer == Player;
            },
            () =>
            {
                var untargetablePlayers = new List<PlayerControl>();
                //untargetablePlayers.AddRange(Jackal.jackal);
                //if (Jackal.Sidekick != null) untargetablePlayers.Add(Jackal.Sidekick);
                //if (Mini.mini != null && !Mini.isGrownUp()) untargetablePlayers.Add(Mini.mini);
                currentTarget = SetTarget(untarget: untargetablePlayers);
                SetPlayerOutline(currentTarget, Palette.ImpostorRed);

                // Show now text since the button already says sidekick
                jackalCreateSidekickButton.showTargetNameOnButton(currentTarget, GetString("jackalSidekickText"));
                return currentTarget != null && PlayerControl.LocalPlayer.CanMove;
            },
            () => { jackalCreateSidekickButton.Timer = jackalCreateSidekickButton.MaxTimer; },
            SidekickButton,
            __instance,
            __instance.AbilityButton,
            ModInputManager.abilityInput.keyCode,
            buttonText: GetString("jackalSidekickText")
        )
        {
            MaxTimer = createSidekickCooldown,
        };

        jackalSwoopButton?.Destroy();
        jackalSwoopButton = new CustomButton(
            () =>
            {   // On Use 
                SetJackalSwoop.Invoke((PlayerControl.LocalPlayer, true));
            },
            () =>
            {   // Can See 
                return Player.IsAlive() && canSwoop && PlayerControl.LocalPlayer == Player;
            },
            () =>
            {   // On Click
                return canSwoop && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                // On Meeting End
                jackalSwoopButton.Timer = jackalSwoopButton.MaxTimer;
                jackalSwoopButton.isEffectActive = false;
                jackalSwoopButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
                IsInvisable = false;
            },
            Swooper.SwoopButtonSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.secondaryAbilityInput.keyCode,
            true,
            duration,
            () => { jackalSwoopButton.Timer = jackalSwoopButton.MaxTimer; },
            buttonText: GetString("SwoopText")
        )
        {
            EffectDuration = duration,
        };
    }
}
