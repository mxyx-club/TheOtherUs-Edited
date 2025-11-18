using AmongUs.GameOptions;
using TheOtherRoles.Mode;
using TheOtherRoles.Objects;

namespace TheOtherRoles.Roles.Neutral;

[CustomRpcHolder]
public class Pavlovsowner : RoleBase, INeutral
{
    public static Color color = new Color32(244, 169, 106, byte.MaxValue);

    public static RoleInfo roleinfo = new(
        typeof(Pavlovsowner),
        (p) => new Pavlovsowner(p),
        RoleId.Pavlovsowner,
        RoleType.Neutral,
        "Pavlovsowner",
        color,
        206300,
        AddOptions,
        IsKiller: true
    );

    public Pavlovsowner(PlayerControl player) : base(player, roleinfo) { }

    public NeutralType NeutralType => NeutralType.Kill;
    public override bool? CanUseVent => pavlovsownerCanUseVents.GetSelection() >= 2;
    public override bool? HasImpVision => pavlovsownerHasImpostorVision.GetBool();
    public override bool? IsKiller => false;

    public PlayerControl currentTarget;
    public List<Arrow> arrow;
    public int CreateDogNum;
    public List<PlayerControl> MyDogs = new();

    public static float createDogCooldown = 30f;
    public static int createDogNum;

    public static bool canSabotage;

    public static float deathTime;

    public static CustomOption pavlovsownerAndJackalAsWell;
    public static CustomOption pavlovsownerKillCooldown;
    public static CustomOption pavlovsownerCreateDogCooldown;
    public static CustomOption pavlovsownerCreateDogNum;
    public static CustomOption pavlovsownerCanUseSabo;
    public static CustomOption pavlovsownerHasImpostorVision;
    public static CustomOption pavlovsownerCanUseVents;
    public static CustomOption pavlovsownerRampage;
    public static CustomOption pavlovsownerRampageKillCooldown;
    public static CustomOption pavlovsownerRampageDeathTime;

    public CustomButton pavlovsownerCreateDogButton;
    public static ResourceSprite CreateDogButton = new("SidekickButton.png");
    public static RemoteProcess<(PlayerControl player, PlayerControl target)> PavlovsCreateDog = new("PavlovsCreateDog", (data, _) =>
    // public static void pavlovsCreateDog(byte playerId, byte targetId)
    {
        if (data.player == null || data.target == null) return;
        if (data.player.TryGetRole<Pavlovsowner>(out var pavlovsowner))
        {
            FastDestroyableSingleton<RoleManager>.Instance.SetRole(data.target, RoleTypes.Crewmate);

            CustomRoleManager.RemoveRole(data.target);
            var role = CustomRoleManager.CreateRoleById(data.target.PlayerId, RoleId.Pavlovsdogs) as Pavlovsdogs;
            pavlovsowner.MyDogs.Add(data.target);

            if (data.target == PlayerControl.LocalPlayer) SoundEffectsManager.play("jackalSidekick");
            if (HandleGuesser.isGuesserGm && GuesserGM.guesserGamemodePavlovsdogIsAlwaysGuesser.GetBool() && !HandleGuesser.isGuesser(data.target.PlayerId))
                setGuesserGm(data.target.PlayerId);
            pavlovsowner.CreateDogNum++;
        }
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
        pavlovsownerAndJackalAsWell = CustomOption.Create(configId++, CustomOptionType.Neutral, "pavlovsownerAndJackalAsWell", true, roleinfo.RoleOption);
        pavlovsownerKillCooldown = CustomOption.Create(configId++, CustomOptionType.Neutral, "killCooldown", 25f, 10f, 60f, 2.5f, roleinfo.RoleOption);
        pavlovsownerCreateDogCooldown = CustomOption.Create(configId++, CustomOptionType.Neutral, "pavlovsownerCreateDogCooldown", 25f, 10f, 60f, 2.5f, roleinfo.RoleOption);
        pavlovsownerCreateDogNum = CustomOption.Create(configId++, CustomOptionType.Neutral, "pavlovsownerCreateDogNum", 3f, 1f, 15f, 1f, roleinfo.RoleOption);
        pavlovsownerCanUseSabo = CustomOption.Create(configId++, CustomOptionType.Neutral, "pavlovsownerCanUseSabo", true, roleinfo.RoleOption);
        pavlovsownerHasImpostorVision = CustomOption.Create(configId++, CustomOptionType.Neutral, "hasImpVision", true, roleinfo.RoleOption);
        pavlovsownerCanUseVents = CustomOption.Create(configId++, CustomOptionType.Neutral, "pavlovsownerCanUseVents",
            ["Pavlovsdogs", "Pavlovsowner", "pavlovsownerCanUseVents3"], roleinfo.RoleOption);
        pavlovsownerRampage = CustomOption.Create(configId++, CustomOptionType.Neutral, "pavlovsownerRampage", true, roleinfo.RoleOption);
        pavlovsownerRampageKillCooldown = CustomOption.Create(configId++, CustomOptionType.Neutral, "pavlovsownerRampageKillCooldown", 15f, 5f, 60f, 2.5f, pavlovsownerRampage);
        pavlovsownerRampageDeathTime = CustomOption.Create(configId++, CustomOptionType.Neutral, "pavlovsownerRampageDeathTime", 60f, 30f, 180f, 2.5f, pavlovsownerRampageKillCooldown);
    }

    public override void Initialize()
    {
        if (arrow != null)
        {
            foreach (var arrow in arrow)
                if (arrow?.arrow != null) UObject.Destroy(arrow.arrow);
        }
        arrow = new();
        MyDogs = new();
        CreateDogNum = 0;
        deathTime = pavlovsownerRampageDeathTime.GetInt();
        createDogCooldown = pavlovsownerCreateDogCooldown.GetFloat();
        createDogNum = pavlovsownerCreateDogNum.GetInt();
        canSabotage = pavlovsownerCanUseSabo.GetBool();
    }

    public override void OnFixedUpdate(PlayerControl player)
    {
        if (arrow == null) return;

        foreach (var arrow in arrow) arrow.arrow.SetActive(false);

        if (Player.IsDead()) return;

        var index = 0;
        foreach (PlayerControl p in MyDogs)
        {
            if (p.IsAlive() && p.Is(RoleId.Pavlovsdogs))
            {
                if (index >= arrow.Count)
                {
                    arrow.Add(new Arrow(Pavlovsdogs.color));
                }
                else if (index < arrow.Count && arrow[index] != null)
                {
                    arrow[index].arrow.SetActive(true);
                    arrow[index].Update(p.transform.position, Pavlovsdogs.color);
                }
                index++;
            }
        }
    }

    public override void CleanUp(HudManager __instance)
    {
        pavlovsownerCreateDogButton?.Destroy();
        pavlovsownerCreateDogButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        pavlovsownerCreateDogButton?.Destroy();
        pavlovsownerCreateDogButton = new CustomButton(
            () =>
            {
                if (CheckAndDoVetKill(PlayerControl.LocalPlayer, currentTarget)) return;
                PavlovsCreateDog.Invoke((PlayerControl.LocalPlayer, currentTarget));
                SoundEffectsManager.play("jackalSidekick");

                pavlovsownerCreateDogButton.Timer = pavlovsownerCreateDogButton.MaxTimer;
            },
            () =>
            {
                return Player == PlayerControl.LocalPlayer && Player.IsAlive();
                // && canCreateDog;
            },
            () =>
            {
                if (pavlovsownerCreateDogButton.ButtonTitle != null)
                    pavlovsownerCreateDogButton.ButtonTitle.text = $"{createDogNum}";

                var untargetablePlayers = new List<PlayerControl>();
                untargetablePlayers.AddRange(GetRoles(RoleId.Pavlovsowner)?.Select(x => x.Player));
                untargetablePlayers.AddRange(GetRoles(RoleId.Pavlovsdogs)?.Select(x => x.Player));
                untargetablePlayers.AddRange(Mini.UngrownMinis);
                currentTarget = SetTarget(untarget: untargetablePlayers);
                SetPlayerOutline(currentTarget, Palette.ImpostorRed);

                // Show now text since the button already says sidekick
                pavlovsownerCreateDogButton.showTargetNameOnButton(currentTarget, GetString("pavlovsCreateDogText"));
                return currentTarget != null && PlayerControl.LocalPlayer.CanMove;
            },
            () => { pavlovsownerCreateDogButton.Timer = pavlovsownerCreateDogButton.MaxTimer; },
            CreateDogButton,
            __instance,
            __instance.AbilityButton,
            ModInputManager.abilityInput.keyCode,
            buttonText: GetString("pavlovsCreateDogText")
        )
        {
            MaxTimer = createDogCooldown,
        };

        // PavlovsdogCreateNumText -> pavlovsownerCreateDogButton
    }
}
