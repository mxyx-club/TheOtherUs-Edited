namespace TheOtherRoles.Roles.Crewmate;

[CustomRpcHolder]
public class Deputy : RoleBase, IPowerCrew
{
    public static Color color = new Color32(248, 205, 70, byte.MaxValue);

    public static readonly RoleInfo roleinfo = new(
        typeof(Deputy),
        (p) => new Deputy(p),
        RoleId.Deputy,
        RoleType.Crewmate,
        "Deputy",
        color,
        301700,
        AddOptions
    );

    public Deputy(PlayerControl p) : base(p, roleinfo) { CustomRoleManager.OnFixedUpdateOthers.Add(OnFixedUpdate); }

    public static int promotesToSheriff; // No: 0, Immediately: 1, After Meeting: 2
    public static bool keepsHandcuffsOnPromotion;
    public static float handcuffDuration;
    public static float numberOfHandcuffs;
    public static float handcuffCooldown;
    public static bool knowsSheriff;

    public PlayerControl MySheriff;
    public PlayerControl currentTarget;
    public int remainingHandcuffs;

    public static CustomOption deputyNumberOfHandcuffs;
    public static CustomOption deputyHandcuffCooldown;
    public static CustomOption deputyHandcuffDuration;
    public static CustomOption deputyGetsPromoted;
    public static CustomOption deputyKnowsSheriff;
    public static CustomOption deputyKeepsHandcuffs;

    private CustomButton deputyHandcuffButton;

    public static ResourceSprite handcuffSprite = new("DeputyHandcuffButton.png");
    public static ResourceSprite handcuffedSprite = new("DeputyHandcuffed.png");
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
        deputyNumberOfHandcuffs = CustomOption.Create(configId++, CustomOptionType.Crewmate, "deputyNumberOfHandcuffs", 5f, 1f, 15f, 1f, roleinfo.RoleOption);
        deputyHandcuffCooldown = CustomOption.Create(configId++, CustomOptionType.Crewmate, "deputyHandcuffCooldown", 20f, 10f, 60f, 2.5f, roleinfo.RoleOption);
        deputyHandcuffDuration = CustomOption.Create(configId++, CustomOptionType.Crewmate, "deputyHandcuffDuration", 10f, 5f, 60f, 2.5f, roleinfo.RoleOption);
        deputyGetsPromoted = CustomOption.Create(configId++, CustomOptionType.Crewmate, "deputyGetsPromoted",
            ["optionOff", "deputyGetsPromoted2", "deputyGetsPromoted3"], roleinfo.RoleOption);
        deputyKnowsSheriff = CustomOption.Create(configId++, CustomOptionType.Crewmate, "deputyKnowsSheriff", true, roleinfo.RoleOption);
        deputyKeepsHandcuffs = CustomOption.Create(configId++, CustomOptionType.Crewmate, "deputyKeepsHandcuffs", true, deputyGetsPromoted);
    }

    public void deputyCheckPromotion(bool isMeeting = false)
    {
        if (promotesToSheriff == 0 || Player.IsDead() || (promotesToSheriff == 2 && !isMeeting)) return;
        if (MySheriff.IsDead())
        {
            var handcuffs = remainingHandcuffs;
            Destroy();
            var newRole = CustomRoleManager.CreateRoleById(Player.PlayerId, RoleId.Sheriff);
            var sheriff = newRole.As<Sheriff>();
            sheriff.CanUseHandcuffs = true;
            sheriff.remainingHandcuffs = handcuffs;
        }
    }

    public override void OnExiledWrapUp(GameData.PlayerInfo exiled)
    {
        // Deputy check Promotion, see if the sheriff still exists. The promotion will be after the meeting.
        deputyCheckPromotion(true);
    }

    public override void Initialize()
    {
        MySheriff = null;
        currentTarget = null;
        remainingHandcuffs = 0;
        promotesToSheriff = deputyGetsPromoted.GetSelection();
        numberOfHandcuffs = deputyNumberOfHandcuffs.GetFloat();
        handcuffCooldown = deputyHandcuffCooldown.GetFloat();
        keepsHandcuffsOnPromotion = deputyKeepsHandcuffs.GetBool();
        handcuffDuration = deputyHandcuffDuration.GetFloat();
        knowsSheriff = deputyKnowsSheriff.GetBool();
    }

    public override void OnFixedUpdate(PlayerControl player)
    {
        if (player == null || !CustomRoleManager.handcuffedKnows.ContainsKey(player.PlayerId)) return;

        if (CustomRoleManager.handcuffedKnows[player.PlayerId] <= 0)
        {
            CustomRoleManager.handcuffedKnows.Remove(player.PlayerId);
            // Resets the buttons
            CustomRoleManager.setHandcuffedKnows(false);

            // Ghost info
            var writer = StartRPC(CustomRPC.ShareGhostInfo);
            writer.Write(player.PlayerId);
            writer.Write((byte)RPCProcedure.GhostInfoTypes.HandcuffOver);
            writer.EndRPC();
        }
    }

    public override void CleanUp(HudManager __instance)
    {
        deputyHandcuffButton?.Destroy();
        deputyHandcuffButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        // Deputy Handcuff
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
                return Player == PlayerControl.LocalPlayer && Player.IsAlive();
            },
            () =>
            {
                currentTarget = SetTarget();
                SetPlayerOutline(currentTarget, Sheriff.color);

                deputyHandcuffButton.showTargetNameOnButton(currentTarget, GetString("HandcuffText"));
                if (deputyHandcuffButton.ButtonTitle != null) deputyHandcuffButton.ButtonTitle.text = $"{remainingHandcuffs}";
                return remainingHandcuffs > 0 && currentTarget && PlayerControl.LocalPlayer.CanMove;
            },
            () => { deputyHandcuffButton.Timer = deputyHandcuffButton.MaxTimer; },
            handcuffSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.abilityInput.keyCode,
            buttonText: GetString("HandcuffText")
        )
        {
            MaxTimer = handcuffCooldown
        };
    }
    public override void OnPlayerDisconnect(PlayerControl player)
    {
        if (player == MySheriff) deputyCheckPromotion();
    }
}
