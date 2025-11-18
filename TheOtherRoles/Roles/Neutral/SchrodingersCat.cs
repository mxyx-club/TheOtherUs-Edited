namespace TheOtherRoles.Roles.Neutral;

public class SchrodingersCat : RoleBase, INeutral
{
    public static Color color = Color.gray;

    public static RoleInfo roleinfo = new(
        typeof(SchrodingersCat),
        (p) => new SchrodingersCat(p),
        RoleId.SchrodingersCat,
        RoleType.Neutral,
        "SchrodingersCat",
        color,
        200600,
        AddOptions
    );

    public static List<SchrodingersCat> AllCat = new();

    public SchrodingersCat(PlayerControl player) : base(player, roleinfo) { AllCat.Add(this); }

    public NeutralType NeutralType => IsKiller.Value ? NeutralType.Kill : IsEvil ? NeutralType.Evil : NeutralType.Benign;
    public override bool? IsKiller => State is not CatState.Crewmate and not CatState.None;
    public override bool? HasImpVision => IsKiller.HasValue;


    public int remainingChange => TeamChanges ? MaxChangeCount - ChangeCount : 0;
    public bool IsEvil => State is not CatState.Crewmate and not CatState.None;
    public PlayerControl currentTarget;
    public int ChangeCount;
    public CatState State = CatState.None;

    public static float Cooldown;
    public static bool CanKill;
    public static bool hasImpVision;
    public static bool TeamChanges;
    public static int MaxChangeCount;
    public static bool IsGuessable;

    public static CustomOption schrodingersCatCanKill;
    public static CustomOption schrodingersCatCooldown;
    public static CustomOption schrodingersCatHasImpVision;
    public static CustomOption schrodingersCatTeamChanges;
    public static CustomOption schrodingersCatMaxChangeCount;
    public static CustomOption schrodingersCatIsGuessable;

    public static CustomButton schrodingersCatKillButton;

    public Color StateColor => State switch
    {
        CatState.None => Color.gray,
        CatState.Crewmate => Palette.CrewmateBlue,
        CatState.Jackal => Jackal.color,
        CatState.Pavlovsowner => Pavlovsdogs.color,
        CatState.Werewolf => Werewolf.color,
        CatState.Juggernaut => Juggernaut.color,
        CatState.Pelican => Pelican.color,
        CatState.Swooper => Swooper.color,
        CatState.Arsonist => Arsonist.color,
        CatState.Infected => Infected.color,
        CatState.Impostor => Palette.ImpostorRed,
        _ => Color.gray,
    };

    public static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        schrodingersCatIsGuessable = CustomOption.Create(configId++, CustomOptionType.Neutral, "schrodingersCatIsGuessable", true, roleinfo.RoleOption);
        schrodingersCatCanKill = CustomOption.Create(configId++, CustomOptionType.Neutral, "schrodingersCatCanKill", true, roleinfo.RoleOption);
        schrodingersCatCooldown = CustomOption.Create(configId++, CustomOptionType.Neutral, "killCooldown", 20f, 2.5f, 60f, 2.5f, schrodingersCatCanKill);
        schrodingersCatHasImpVision = CustomOption.Create(configId++, CustomOptionType.Neutral, "hasImpVision", true, roleinfo.RoleOption);
        schrodingersCatTeamChanges = CustomOption.Create(configId++, CustomOptionType.Neutral, "schrodingersCatTeamChanges", true, roleinfo.RoleOption);
        schrodingersCatMaxChangeCount = CustomOption.Create(configId++, CustomOptionType.Neutral, "schrodingersCatMaxChangeCount", 3, 1, 15, 1, schrodingersCatTeamChanges);
    }

    public override void OnDestroy()
    {
        AllCat.Remove(this);
        base.OnDestroy();
    }

    public override void Initialize()
    {
        State = CatState.None;
        ChangeCount = 0;
        CanKill = schrodingersCatCanKill.GetBool();
        Cooldown = schrodingersCatCooldown.GetFloat();
        hasImpVision = schrodingersCatHasImpVision.GetBool();
        TeamChanges = schrodingersCatTeamChanges.GetBool();
        MaxChangeCount = schrodingersCatMaxChangeCount.GetInt();
        IsGuessable = schrodingersCatIsGuessable.GetBool();
    }

    public override void OnMurderPlayer(MurderInfo Info)
    {
        if (Info.Target == Player && remainingChange > 0)
        {
            if (Info.Killer.IsCrew()) State = CatState.Crewmate;
            if (Info.Killer.IsImpostor()) State = CatState.Impostor;
            if (Info.Killer.TryGetRole<Jackal>(out var j) || j.MySidekick) State = CatState.Jackal;
            if (Info.Killer.Is(RoleId.Pavlovsdogs)) State = CatState.Pavlovsowner;
            if (Info.Killer.Is(RoleId.Werewolf)) State = CatState.Werewolf;
            if (Info.Killer.Is(RoleId.Juggernaut)) State = CatState.Juggernaut;
            if (Info.Killer.Is(RoleId.Swooper)) State = CatState.Swooper;
            if (Info.Killer.Is(RoleId.Arsonist)) State = CatState.Arsonist;
            if (Info.Killer.Is(RoleId.Pelican)) State = CatState.Pelican;
            if (Info.Killer.Is(RoleId.Infected)) State = CatState.Infected;

            if (Info.Target == PlayerControl.LocalPlayer)
            {
                _ = new LateTask(() =>
                {
                    var wrirer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.RevivePlayer);
                    wrirer.Write(PlayerControl.LocalPlayer.PlayerId);
                    wrirer.Write(true);
                    wrirer.Write(true);
                    wrirer.EndRPC();
                    Player.ModRevive(true, true);
                }, 0.1f, "Revived SchrodingersCat");
            }

            Message($"SchrodingersCat.State: {State}");
        }
    }

    public bool InTeam(PlayerControl player, out Color color)
    {
        color = StateColor;

        return State switch
        {
            CatState.Impostor => player.IsImpostor(true),
            CatState.Jackal => player.Is(RoleId.Jackal) || player.Is(RoleId.Sidekick),
            CatState.Pavlovsowner => player.Is(RoleId.Pavlovsdogs) || player.Is(RoleId.Pavlovsowner),
            CatState.Werewolf => player.Is(RoleId.Werewolf),
            CatState.Juggernaut => player.Is(RoleId.Juggernaut),
            CatState.Swooper => player.Is(RoleId.Swooper),
            CatState.Arsonist => player.Is(RoleId.Arsonist),
            CatState.Pelican => player.Is(RoleId.Pelican),
            CatState.Infected => player.Is(RoleId.Infected),
            _ => false,
        };
    }

    public enum CatState
    {
        None,
        Crewmate,
        Impostor,
        Jackal,
        Pavlovsowner,
        Werewolf,
        Juggernaut,
        Swooper,
        Arsonist,
        Pelican,
        Infected,
    }

    public override void CleanUp(HudManager __instance)
    {
        schrodingersCatKillButton?.Destroy();
        schrodingersCatKillButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        schrodingersCatKillButton?.Destroy();
        schrodingersCatKillButton = new CustomButton(
            () =>
            {
                if (!RpcCustomMurderPlayer(PlayerControl.LocalPlayer, currentTarget)) return;

                schrodingersCatKillButton.Timer = schrodingersCatKillButton.MaxTimer;
                currentTarget = null;
            },
            () =>
            {
                return Player.IsAlive() && Player == PlayerControl.LocalPlayer && CanKill &&
                       State is not CatState.None and not CatState.Crewmate;
            },
            () =>
            {
                currentTarget = SetTarget();
                SetPlayerOutline(currentTarget, StateColor);

                schrodingersCatKillButton.showTargetNameOnButton(currentTarget, GetString("killButtonText"));
                return PlayerControl.LocalPlayer.CanMove && currentTarget != null;
            },
            () =>
            {
                schrodingersCatKillButton.Timer = schrodingersCatKillButton.MaxTimer;
            },
            __instance.KillButton.graphic.sprite,
            __instance,
            __instance.KillButton,
            ModInputManager.modKillInput.keyCode,
            buttonText: GetString("killButtonText")
        )
        {
            MaxTimer = Cooldown,
        };
    }
}
