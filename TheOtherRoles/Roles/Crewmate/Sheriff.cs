namespace TheOtherRoles.Roles.Crewmate;

[CustomRpcHolder]
public class Sheriff : RoleBase, IPowerCrew
{
    public static Color color = new Color32(248, 205, 70, byte.MaxValue);

    public static readonly RoleInfo roleinfo = new(
        typeof(Sheriff),
        (p) => new Sheriff(p),
        RoleId.Sheriff,
        RoleType.Crewmate,
        "Sheriff",
        color,
        301400,
        AddOptions
    );

    public Sheriff(PlayerControl p) : base(p, roleinfo) { }

    public PlayerControl MyDeputy; // Needed for keeping handcuffs + shifting
    public PlayerControl currentTarget;
    public bool CanUseHandcuffs;
    public int remainingHandcuffs;

    public static float cooldown = 30f;
    public static bool canKillNeutrals;
    public static int misfireKills; // Self: 0, Target: 1, Both: 2

    public static CustomOption sheriffCooldown;
    public static CustomOption sheriffMisfireKills;
    public static CustomOption sheriffCanKillNeutrals;
    public static CustomOption sheriffCanKillSurvivor;
    public static CustomOption sheriffCanKillAmnesiac;
    public static CustomOption sheriffCanKillPursuer;
    public static CustomOption sheriffCanKillPartTimer;
    public static CustomOption sheriffCanKillJester;
    public static CustomOption sheriffCanKillLawyer;
    public static CustomOption sheriffCanKillExecutioner;
    public static CustomOption sheriffCanKillVulture;
    public static CustomOption sheriffCanKillDoomsayer;
    public static CustomOption sheriffCanKillThief;
    public static CustomOption sheriffCanKillWitness;
    public static CustomOption sheriffCanKillBandLeader;

    private CustomButton sheriffKillButton;
    private CustomButton deputyHandcuffButton;
    public static RemoteProcess<(PlayerControl player, PlayerControl target)> DeputyUsedHandcuffs = new("DeputyUsedHandcuffs", (data, _) =>
    {
        if (data.player.TryGetRole<Sheriff>(out var sheriff))
            sheriff.remainingHandcuffs--;
        else if (data.player.TryGetRole<Deputy>(out var deputy))
            deputy.remainingHandcuffs--;
        CustomRoleManager.handcuffedPlayers.Add(data.target.PlayerId);
    });
    public static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;

        sheriffCooldown = CustomOption.Create(configId++, CustomOptionType.Crewmate, "sheriffCooldown", 25f, 10f, 60f, 2.5f, roleinfo.RoleOption);
        sheriffMisfireKills = CustomOption.Create(configId++, CustomOptionType.Crewmate, "sheriffMisfireKills",
            ["sheriffMisfireKills1", "sheriffMisfireKills2", "sheriffMisfireKills3"], roleinfo.RoleOption);
        sheriffCanKillNeutrals = CustomOption.Create(configId++, CustomOptionType.Crewmate, "sheriffCanKillNeutrals", true, roleinfo.RoleOption);
        sheriffCanKillAmnesiac = CustomOption.Create(configId++, CustomOptionType.Crewmate, $"{"sheriffCanKill".Translate()}{Cs(Amnisiac.color, "Amnisiac".Translate())}", false, sheriffCanKillNeutrals);
        sheriffCanKillSurvivor = CustomOption.Create(configId++, CustomOptionType.Crewmate, $"{"sheriffCanKill".Translate()}{Cs(Survivor.color, "Survivor".Translate())}", false, sheriffCanKillNeutrals);
        sheriffCanKillPursuer = CustomOption.Create(configId++, CustomOptionType.Crewmate, $"{"sheriffCanKill".Translate()}{Cs(Pursuer.color, "Pursuer".Translate())}", true, sheriffCanKillNeutrals);
        sheriffCanKillPartTimer = CustomOption.Create(configId++, CustomOptionType.Crewmate, $"{"sheriffCanKill".Translate()}{Cs(PartTimer.color, "PartTimer".Translate())}", true, sheriffCanKillNeutrals);
        sheriffCanKillBandLeader = CustomOption.Create(configId++, CustomOptionType.Crewmate, $"{"sheriffCanKill".Translate()}{Cs(BandLeader.color, "BandLeader".Translate())}", true, sheriffCanKillNeutrals);
        sheriffCanKillJester = CustomOption.Create(configId++, CustomOptionType.Crewmate, $"{"sheriffCanKill".Translate()}{Cs(Jester.color, "Jester".Translate())}", true, sheriffCanKillNeutrals);
        sheriffCanKillLawyer = CustomOption.Create(configId++, CustomOptionType.Crewmate, $"{"sheriffCanKill".Translate()}{Cs(Lawyer.color, "Lawyer".Translate())}", true, sheriffCanKillNeutrals);
        sheriffCanKillExecutioner = CustomOption.Create(configId++, CustomOptionType.Crewmate, $"{"sheriffCanKill".Translate()}{Cs(Executioner.color, "Executioner".Translate())}", true, sheriffCanKillNeutrals);
        sheriffCanKillVulture = CustomOption.Create(configId++, CustomOptionType.Crewmate, $"{"sheriffCanKill".Translate()}{Cs(Vulture.color, "Vulture".Translate())}", true, sheriffCanKillNeutrals);
        sheriffCanKillDoomsayer = CustomOption.Create(configId++, CustomOptionType.Crewmate, $"{"sheriffCanKill".Translate()}{Cs(Doomsayer.color, "Doomsayer".Translate())}", true, sheriffCanKillNeutrals);
        sheriffCanKillWitness = CustomOption.Create(configId++, CustomOptionType.Crewmate, $"{"sheriffCanKill".Translate()}{Cs(Witness.color, "Witness".Translate())}", true, sheriffCanKillNeutrals);
        sheriffCanKillThief = CustomOption.Create(configId++, CustomOptionType.Crewmate, $"{"sheriffCanKill".Translate()}{Cs(Thief.color, "Thief".Translate())}", true, sheriffCanKillNeutrals);

    }

    public static bool sheriffCanKillNeutral(PlayerControl target)
    {
        var targetRole = target.GetRole();
        return (!target.TryGetModifier<Mini>(out var mini) || mini.isGrownUp()) &&
               (target.IsImpostor(Spy.spyCanDieToSheriff.GetBool()) ||
                (canKillNeutrals &&
                 (target.IsKillerNeutral() ||
                  targetRole == RoleId.Akujo ||
                  targetRole == RoleId.SchrodingersCat ||
                  (targetRole == RoleId.BandLeader == target && sheriffCanKillBandLeader.GetBool()) ||
                  (targetRole == RoleId.Witness && sheriffCanKillWitness.GetBool()) ||
                  (targetRole == RoleId.Amnisiac && sheriffCanKillAmnesiac.GetBool()) ||
                  (targetRole == RoleId.Survivor && sheriffCanKillSurvivor.GetBool()) ||
                  (targetRole == RoleId.Pursuer && sheriffCanKillPursuer.GetBool()) ||
                  (targetRole == RoleId.Jester && sheriffCanKillJester.GetBool()) ||
                  (targetRole == RoleId.Vulture && sheriffCanKillVulture.GetBool()) ||
                  (targetRole == RoleId.Thief && sheriffCanKillThief.GetBool()) ||
                  (targetRole == RoleId.PartTimer && sheriffCanKillPartTimer.GetBool()) ||
                  (targetRole == RoleId.Lawyer && sheriffCanKillLawyer.GetBool()) ||
                  (targetRole == RoleId.Executioner && sheriffCanKillExecutioner.GetBool()) ||
                  (targetRole == RoleId.Doomsayer && sheriffCanKillDoomsayer.GetBool()
                  ))));
    }

    public override void Initialize()
    {
        currentTarget = null;
        MyDeputy = null;
        CanUseHandcuffs = false;

        misfireKills = sheriffMisfireKills.GetSelection();
        cooldown = sheriffCooldown.GetFloat();
        canKillNeutrals = sheriffCanKillNeutrals.GetBool();
    }

    public override void CleanUp(HudManager __instance)
    {
        sheriffKillButton?.Destroy();
        deputyHandcuffButton?.Destroy();
        sheriffKillButton = null;
        deputyHandcuffButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        //Sheriff Kill
        sheriffKillButton?.Destroy();
        sheriffKillButton = new CustomButton(
            () =>
            {
                var target = currentTarget;
                if (target == null) return;
                if (CheckAndDoVetKill(PlayerControl.LocalPlayer, target)) return;
                if (!RoleHelpers.CheckMurderPlayer(PlayerControl.LocalPlayer, target)) return;
                else
                {
                    if (sheriffCanKillNeutral(target))
                    {
                        RpcCustomMurderPlayer(PlayerControl.LocalPlayer, target, true, true);
                    }
                    else
                    {
                        switch (misfireKills)
                        {
                            case 0:
                                RpcCustomMurderPlayer(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer, true, true, CustomDeathReason.SheriffMisfire);
                                break;
                            case 1:
                                RpcCustomMurderPlayer(PlayerControl.LocalPlayer, target, true, true, CustomDeathReason.SheriffMisadventure);
                                break;
                            case 2:
                                RpcCustomMurderPlayer(PlayerControl.LocalPlayer, target, true, true, CustomDeathReason.SheriffMisadventure);
                                RpcCustomMurderPlayer(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer, false, true, CustomDeathReason.SheriffMisfire);
                                break;
                        }
                    }
                }

                sheriffKillButton.Timer = sheriffKillButton.MaxTimer;
                currentTarget = null;
            },
            () =>
            {
                return Player == PlayerControl.LocalPlayer && Player.IsAlive();
            },
            () =>
            {
                currentTarget = SetTarget();
                SetPlayerOutline(currentTarget, color);

                sheriffKillButton.showTargetNameOnButton(currentTarget, GetString("killButtonText"));
                return currentTarget && PlayerControl.LocalPlayer.CanMove;
            },
            () => { sheriffKillButton.Timer = sheriffKillButton.MaxTimer; },
            __instance.KillButton.graphic.sprite,
            __instance,
            __instance.KillButton,
            ModInputManager.modKillInput.keyCode,
            buttonText: GetString("killButtonText")
        )
        {
            MaxTimer = cooldown
        };

        deputyHandcuffButton?.Destroy();
        deputyHandcuffButton = new CustomButton(
            () =>
            {
                if (currentTarget == null) return;
                if (CheckAndDoVetKill(PlayerControl.LocalPlayer, currentTarget)) return;
                DeputyUsedHandcuffs.Invoke((PlayerControl.LocalPlayer, currentTarget));
                currentTarget = null;
                deputyHandcuffButton.Timer = deputyHandcuffButton.MaxTimer;

                SoundEffectsManager.play("deputyHandcuff");
            },
            () =>
            {
                return Player == PlayerControl.LocalPlayer && Player.IsAlive() && CanUseHandcuffs;
            },
            () =>
            {
                currentTarget = SetTarget();
                SetPlayerOutline(currentTarget, color);

                deputyHandcuffButton.showTargetNameOnButton(currentTarget, GetString("HandcuffText"));
                if (deputyHandcuffButton.ButtonTitle != null) deputyHandcuffButton.ButtonTitle.text = $"{remainingHandcuffs}";
                return remainingHandcuffs > 0 && currentTarget && PlayerControl.LocalPlayer.CanMove;
            },
            () => { deputyHandcuffButton.Timer = deputyHandcuffButton.MaxTimer; },
            Deputy.handcuffSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.abilityInput.keyCode,
            buttonText: GetString("HandcuffText")
        )
        {
            MaxTimer = Deputy.handcuffCooldown
        };
    }
}
