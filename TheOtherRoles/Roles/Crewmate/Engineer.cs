namespace TheOtherRoles.Roles.Crewmate;

[CustomRpcHolder]
public class Engineer : RoleBase
{
    public static Color color = new Color32(0, 40, 245, byte.MaxValue);

    public static readonly RoleInfo roleinfo = new(
        typeof(Engineer),
        (p) => new Engineer(p),
        RoleId.Engineer,
        RoleType.Crewmate,
        "Engineer",
        color,
        301200,
        AddOptions
    );

    public Engineer(PlayerControl p) : base(p, roleinfo)
    {
        CanUseVent = true;
        CustomRoleManager.OnFixedUpdateOthers.Add(OnFixedUpdate);
    }

    public override bool? CanUseVent => true;

    public int remainingFixes = 1;

    public static bool resetFixAfterMeeting;
    //public static bool expertRepairs = false;
    public static bool remoteFix = true;
    public static bool highlightForImpostors = true;
    public static bool highlightForTeamJackal = true;
    public static int NumberOfFixes = 1;

    public static CustomOption engineerRemoteFix;
    public static CustomOption engineerResetFixAfterMeeting;
    public static CustomOption engineerNumberOfFixes;
    public static CustomOption engineerHighlightForImpostors;
    public static CustomOption engineerHighlightForTeamJackal;


    public CustomButton engineerRepairButton;
    public static ResourceSprite buttonSprite = new("RepairButton.png");
    public static RemoteProcess FixLights = new("FixLights", (_) =>
    {
        var switchSystem = MapUtilities.Systems[SystemTypes.Electrical].CastFast<SwitchSystem>();
        switchSystem.ActualSwitches = switchSystem.ExpectedSwitches;
    });
    public static RemoteProcess FixSubmergedOxygen = new("FixSubmergedOxygen", (_) =>
    {
        SubmergedCompatibility.RepairOxygen();
    });
    private static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        engineerRemoteFix = CustomOption.Create(configId++, CustomOptionType.Crewmate, "engineerRemoteFix", true, roleinfo.RoleOption);
        engineerResetFixAfterMeeting = CustomOption.Create(configId++, CustomOptionType.Crewmate, "engineerResetFixAfterMeeting", true, engineerRemoteFix);
        engineerNumberOfFixes = CustomOption.Create(configId++, CustomOptionType.Crewmate, "engineerNumberOfFixes", 1f, 1f, 3f, 1f, engineerRemoteFix);
        //engineerExpertRepairs = CustomOption.Create(configId++, CustomOptionType.Crewmate, "engineerExpertRepairs", false, engineerSpawnRate);
        engineerHighlightForImpostors = CustomOption.Create(configId++, CustomOptionType.Crewmate, "engineerHighlightForImpostors", true, roleinfo.RoleOption);
        engineerHighlightForTeamJackal = CustomOption.Create(configId++, CustomOptionType.Crewmate, "engineerHighlightForTeamJackal", true, roleinfo.RoleOption);
    }

    public override void OnExiledWrapUp(GameData.PlayerInfo exiled)
    {
        if (!engineerResetFixAfterMeeting.GetBool()) return;
        remainingFixes = NumberOfFixes;
    }

    public override void Initialize()
    {
        remoteFix = engineerRemoteFix.GetBool();
        //expertRepairs = CustomOptionHolder.engineerExpertRepairs.getBool();
        resetFixAfterMeeting = engineerResetFixAfterMeeting.GetBool();
        remainingFixes = engineerNumberOfFixes.GetInt();
        highlightForImpostors = engineerHighlightForImpostors.GetBool();
        highlightForTeamJackal = engineerHighlightForTeamJackal.GetBool();
    }

    public override void OnFixedUpdate(PlayerControl player)
    {
        var jackalHighlight = highlightForTeamJackal && PlayerControl.LocalPlayer.Is(RoleId.Jackal);
        var impostorHighlight = highlightForImpostors && PlayerControl.LocalPlayer.IsImpostor();
        if ((jackalHighlight || impostorHighlight) && MapUtilities.CachedShipStatus?.AllVents != null)
        {
            foreach (var vent in MapUtilities.CachedShipStatus.AllVents)
                try
                {
                    if (vent?.myRend?.material != null)
                    {
                        if (Player.inVent)
                        {
                            vent.myRend.material.SetFloat("_Outline", 1f);
                            vent.myRend.material.SetColor("_OutlineColor", color);
                        }
                        else if (vent.myRend.material.GetColor("_AddColor") != Color.red)
                        {
                            vent.myRend.material.SetFloat("_Outline", 0);
                        }
                    }
                }
                catch
                {
                }
        }
    }

    public override void CleanUp(HudManager __instance)
    {
        engineerRepairButton?.Destroy();
        engineerRepairButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        // Engineer Repair
        engineerRepairButton?.Destroy();
        engineerRepairButton = new CustomButton(
            () =>
            {
                foreach (var task in PlayerControl.LocalPlayer.myTasks.GetFastEnumerator())
                    if (task.TaskType == TaskTypes.FixLights)
                    {
                        FixLights.Invoke();
                    }
                    else if (task.TaskType == TaskTypes.RestoreOxy)
                    {
                        MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.LifeSupp, 0 | 64);
                        MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.LifeSupp, 1 | 64);
                    }
                    else if (task.TaskType == TaskTypes.ResetReactor)
                    {
                        MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.Reactor, 16);
                    }
                    else if (task.TaskType == TaskTypes.ResetSeismic)
                    {
                        MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.Laboratory, 16);
                    }
                    else if (task.TaskType == TaskTypes.FixComms)
                    {
                        MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.Comms, 16 | 0);
                        MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.Comms, 16 | 1);
                    }
                    else if (task.TaskType == TaskTypes.StopCharles)
                    {
                        MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.Reactor, 0 | 16);
                        MapUtilities.CachedShipStatus.RpcRepairSystem(SystemTypes.Reactor, 1 | 16);
                    }
                    else if (SubmergedCompatibility.IsSubmerged && task.TaskType == SubmergedCompatibility.RetrieveOxygenMask)
                    {
                        FixSubmergedOxygen.Invoke();
                    }
                SoundEffectsManager.play("engineerRepair");
                remainingFixes--;
                engineerRepairButton.Timer = 0f;
            },
            () =>
            {
                return PlayerControl.LocalPlayer.IsAlive() && PlayerControl.LocalPlayer == Player && remainingFixes > 0 && remoteFix;
            },
            () =>
            {
                return isSabotageActive() && remainingFixes > 0 && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                //if (Engineer.resetFixAfterMeeting) Engineer.resetFixes();
            },
            buttonSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.abilityInput.keyCode,
            buttonText: GetString("RepairText")
        )
        {
            MaxTimer = 0f
        };
    }
}

