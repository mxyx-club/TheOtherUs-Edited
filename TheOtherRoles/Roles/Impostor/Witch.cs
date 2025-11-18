namespace TheOtherRoles.Roles.Impostor;

[CustomRpcHolder]
public class Witch : RoleBase
{
    public static Color color = Palette.ImpostorRed;

    public static RoleInfo roleInfo = new(
        typeof(Witch),
        (p) => new Witch(p),
        RoleId.Witch,
        RoleType.Impostor,
        "Witch",
        color,
        102400,
        AddOptions
    );

    public Witch(PlayerControl p) : base(p, roleInfo) { }

    public List<PlayerControl> futureSpelled = new();
    public PlayerControl currentTarget;
    public PlayerControl spellCastingTarget;
    public bool witchWasGuessed;

    public static float cooldown = 30f;
    public static float spellCastingDuration = 2f;
    public static float cooldownAddition = 10f;
    public static float currentCooldownAddition;
    public static bool canSpellAnyone;
    public static bool triggerBothCooldowns = true;
    public static bool voteSavesTargets = true;

    public static CustomOption witchCooldown;
    public static CustomOption witchAdditionalCooldown;
    public static CustomOption witchCanSpellAnyone;
    public static CustomOption witchSpellCastingDuration;
    public static CustomOption witchTriggerBothCooldowns;
    public static CustomOption witchVoteSavesTargets;

    public CustomButton witchSpellButton;
    public static ResourceSprite buttonSprite = new("SpellButton.png");
    public static ResourceSprite spelledOverlaySprite = new("SpellButtonMeeting.png", 225f);
    public static RemoteProcess<(PlayerControl player, PlayerControl target)> SetFutureSpelled = new("SetFutureSpelled", (data, _) =>
    {
        if (data.player.TryGetRole<Witch>(out var witch))
        {
            witch.futureSpelled ??= new List<PlayerControl>();
            if (data.target != null) witch.futureSpelled.Add(data.target);
        }
    });

    private static void AddOptions()
    {
        var configId = roleInfo.ConfigId + 2;
        witchCooldown = CustomOption.Create(configId++, CustomOptionType.Impostor, "witchCooldown", 20f, 10f, 60, 2.5f, roleInfo.RoleOption);
        witchAdditionalCooldown = CustomOption.Create(configId++, CustomOptionType.Impostor, "witchAdditionalCooldown", 5f, 0f, 60f, 2.5f, roleInfo.RoleOption);
        witchCanSpellAnyone = CustomOption.Create(configId++, CustomOptionType.Impostor, "witchCanSpellAnyone", false, roleInfo.RoleOption);
        witchSpellCastingDuration = CustomOption.Create(configId++, CustomOptionType.Impostor, "witchSpellCastingDuration", 0.5f, 0f, 10f, 0.25f, roleInfo.RoleOption);
        witchTriggerBothCooldowns = CustomOption.Create(configId++, CustomOptionType.Impostor, "witchTriggerBothCooldowns", false, roleInfo.RoleOption);
        witchVoteSavesTargets = CustomOption.Create(configId++, CustomOptionType.Impostor, "witchVoteSavesTargets", true, roleInfo.RoleOption);
    }

    public override void Initialize()
    {
        futureSpelled = new();
        witchWasGuessed = false;
        currentTarget = spellCastingTarget = null;
        cooldown = witchCooldown.GetFloat();
        cooldownAddition = witchAdditionalCooldown.GetFloat();
        currentCooldownAddition = 0f;
        canSpellAnyone = witchCanSpellAnyone.GetBool();
        spellCastingDuration = witchSpellCastingDuration.GetFloat();
        triggerBothCooldowns = witchTriggerBothCooldowns.GetBool();
        voteSavesTargets = witchVoteSavesTargets.GetBool();
    }

    public override void OnMurderPlayer(MurderInfo Info)
    {
        if (triggerBothCooldowns && Info.Killer == Player && witchSpellButton != null)
            witchSpellButton.Timer = witchSpellButton.MaxTimer;

        if (Info.Target == Player) witchWasGuessed = true;
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        // Add overlay for spelled players
        if (futureSpelled != null)
        {
            foreach (PlayerVoteArea pva in __instance.playerStates)
            {
                if (futureSpelled.Any(x => x.PlayerId == pva.TargetPlayerId))
                {
                    var local = PlayerControl.LocalPlayer;
                    var rend = new GameObject().AddComponent<SpriteRenderer>();
                    rend.transform.SetParent(pva.transform);
                    rend.gameObject.layer = pva.Megaphone.gameObject.layer;
                    rend.transform.localPosition = new Vector3(-0.5f, -0.03f, -1f);
                    rend.sprite = spelledOverlaySprite;
                }
            }
        }
    }

    public override void OnExiledWrapUp(GameData.PlayerInfo exiled)
    {
        // Witch execute casted spells
        if (Player != null && futureSpelled.Count > 0)
        {
            var partner = exiled?.Object?.GetPartner();

            var exiledIsWitch = exiled?.PlayerId == Player.PlayerId;
            var witchDiesWithExiledLover = partner?.PlayerId == Player.PlayerId || exiled?.PlayerId == Player.PlayerId;

            if ((((witchDiesWithExiledLover || exiledIsWitch) && voteSavesTargets) || witchWasGuessed) && Player.IsDead())
                futureSpelled.Clear();

            foreach (var target in futureSpelled.Where(x => x.IsAlive()))
            {
                target.Exiled();
                PlayerData.SetDeathReason(target, CustomDeathReason.WitchExile, Player);
            }
        }
        futureSpelled.Clear();
    }

    public override void CleanUp(HudManager __instance)
    {
        witchSpellButton?.Destroy();
        witchSpellButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        // Witch Spell button
        witchSpellButton?.Destroy();
        witchSpellButton = new CustomButton(
            () =>
            {
                var target = currentTarget;
                if (target != null)
                {
                    if (CheckAndDoVetKill(PlayerControl.LocalPlayer, target)) return;
                    spellCastingTarget = target;
                    SoundEffectsManager.play("witchSpell");
                }
            },
            () =>
            {
                return Player.IsAlive() && PlayerControl.LocalPlayer == Player;
            },
            () =>
            {
                List<PlayerControl> untargetables;
                if (spellCastingTarget != null)
                {
                    untargetables = PlayerControl.AllPlayerControls.ToArray().Where(x => x.PlayerId != spellCastingTarget.PlayerId).ToList();
                }
                else
                {
                    untargetables = new();
                    //if (Spy.Player != null && !canSpellAnyone) untargetables.Add(Spy.Player);
                }

                currentTarget = SetTarget(untargetables, !canSpellAnyone);
                SetPlayerOutline(currentTarget, color);

                witchSpellButton.showTargetNameOnButton(currentTarget, GetString("WitchText"));
                if (witchSpellButton.isEffectActive && spellCastingTarget != currentTarget)
                {
                    spellCastingTarget = null;
                    witchSpellButton.Timer = 0f;
                    witchSpellButton.isEffectActive = false;
                }
                return PlayerControl.LocalPlayer.CanMove && currentTarget != null;
            },
            () =>
            {
                witchSpellButton.Timer = witchSpellButton.MaxTimer;
                witchSpellButton.isEffectActive = false;
                spellCastingTarget = null;
            },
            buttonSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.abilityInput.keyCode,
            true,
            spellCastingDuration,
            () =>
            {
                if (spellCastingTarget != null)
                {
                    SetFutureSpelled.Invoke((PlayerControl.LocalPlayer, currentTarget));
                }
                spellCastingTarget = null;
            },
            buttonText: "WitchText".Translate()
        )
        {
            MaxTimer = cooldown,
            EffectDuration = spellCastingDuration,
        };
    }
}
