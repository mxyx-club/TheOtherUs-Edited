using TheOtherRoles.Objects;

namespace TheOtherRoles.Roles.Impostor;

[CustomRpcHolder]
public class Ninja : RoleBase
{
    public static Color color = Palette.ImpostorRed;

    public static RoleInfo roleInfo = new(
        typeof(Ninja),
        (p) => new Ninja(p),
        RoleId.Ninja,
        RoleType.Impostor,
        "Ninja",
        color,
        102500,
        AddOptions
    );

    public Ninja(PlayerControl p) : base(p, roleInfo) { }

    public static float cooldown = 30f;
    public static float traceTime = 1f;
    public static bool knowsTargetLocation;
    public static float invisibleDuration = 5f;

    public PlayerControl ninjaMarked;
    public PlayerControl currentTarget;

    public float invisibleTimer;
    public bool isInvisable;
    public Arrow arrow = new(Color.black);

    public CustomButton ninjaButton;
    public static Sprite markButtonSprite = new ResourceSprite("NinjaMarkButton.png");
    public static Sprite killButtonSprite = new ResourceSprite("NinjaAssassinateButton.png");

    public static CustomOption ninjaCooldown;
    public static CustomOption ninjaKnowsTargetLocation;
    public static CustomOption ninjaTraceTime;
    public static CustomOption ninjaTraceColorTime;
    public static CustomOption ninjaInvisibleDuration;
    public static RemoteProcess<(PlayerControl player, byte flag)> SetNinjaInvisible = new("SetNinjaInvisible", (data, _) =>
    {
        if (data.player == null) return;
        if (data.player.TryGetRole<Ninja>(out var ninja))
        {
            if (data.flag == byte.MaxValue)
            {
                data.player.cosmetics.currentBodySprite.BodySprite.color = Color.white;
                data.player.cosmetics.colorBlindText.gameObject.SetActive(DataManager.Settings.Accessibility.ColorBlindMode);
                data.player.cosmetics.colorBlindText.color = data.player.cosmetics.colorBlindText.color.SetAlpha(1f);

                if (!MushroomSabotageActive && !isActiveCamoComms)
                    data.player.setDefaultLook();
                ninja.isInvisable = false;
                return;
            }

            data.player.setLook("", 6, "", "", "", "");
            var color = Color.clear;
            var canSee = PlayerControl.LocalPlayer.IsImpostor(AndCat: true) || CanSeeGhostInfo;
            if (canSee) color.a = 0.1f;
            data.player.cosmetics.currentBodySprite.BodySprite.color = color;
            data.player.cosmetics.colorBlindText.gameObject.SetActive(false);
            data.player.cosmetics.colorBlindText.color = data.player.cosmetics.colorBlindText.color.SetAlpha(canSee ? 0.1f : 0f);
            ninja.invisibleTimer = Ninja.invisibleDuration;
            ninja.isInvisable = true;
        }
    });
    public static RemoteProcess<Vector3> PlaceNinjaTrace = new("PlaceNinjaTrace", (position, _) =>
    {
        var ninjaTrace = new NinjaTrace(position, Ninja.traceTime);
    });

    public static void AddOptions()
    {
        var configId = roleInfo.ConfigId + 2;
        ninjaCooldown = CustomOption.Create(configId++, CustomOptionType.Impostor, "ninjaCooldown", 20f, 10f, 60f, 2.5f, roleInfo.RoleOption);
        ninjaKnowsTargetLocation = CustomOption.Create(configId++, CustomOptionType.Impostor, "ninjaKnowsTargetLocation", true, roleInfo.RoleOption);
        ninjaTraceTime = CustomOption.Create(configId++, CustomOptionType.Impostor, "ninjaTraceTime", 6f, 1f, 20f, 0.5f, roleInfo.RoleOption);
        ninjaTraceColorTime = CustomOption.Create(configId++, CustomOptionType.Impostor, "ninjaTraceColorTime", 3f, 0f, 20f, 0.5f, roleInfo.RoleOption);
        ninjaInvisibleDuration = CustomOption.Create(configId++, CustomOptionType.Impostor, "ninjaInvisibleDuration", 10f, 0f, 20f, 0.5f, roleInfo.RoleOption);
    }

    public override void Initialize()
    {
        currentTarget = ninjaMarked = null;
        cooldown = ninjaCooldown.GetFloat();
        knowsTargetLocation = ninjaKnowsTargetLocation.GetBool();
        traceTime = ninjaTraceTime.GetFloat();
        invisibleDuration = ninjaInvisibleDuration.GetFloat();
        invisibleTimer = 0f;
        isInvisable = false;
        if (arrow?.arrow != null) UObject.Destroy(arrow.arrow);
        arrow = new Arrow(Color.black);
        arrow.arrow?.SetActive(false);
    }

    public override void OnHudUpdate(HudManager hudManager)
    {
        invisibleTimer -= Time.deltaTime;
    }

    public override void OnFixedUpdate(PlayerControl player)
    {
        CustomObject.UpdateAll();
        if (isInvisable && invisibleTimer <= 0)
        {
            SetNinjaInvisible.Invoke((PlayerControl.LocalPlayer, byte.MaxValue));
        }

        if (arrow?.arrow != null)
        {
            if (!knowsTargetLocation)
            {
                arrow.arrow.SetActive(false);
                return;
            }

            if (ninjaMarked != null && !player.Data.IsDead)
            {
                var trackedOnMap = !ninjaMarked.Data.IsDead;
                var position = ninjaMarked.transform.position;
                if (!trackedOnMap)
                {
                    // Check for dead body
                    var body = UObject.FindObjectsOfType<DeadBody>()
                        .FirstOrDefault(b => b.ParentId == ninjaMarked.PlayerId);
                    if (body != null)
                    {
                        trackedOnMap = true;
                        position = body.transform.position;
                    }
                }

                arrow.Update(position);
                arrow.arrow.SetActive(trackedOnMap);
            }
            else
            {
                arrow.arrow.SetActive(false);
            }
        }
    }

    public override void OnMurderPlayer(MurderInfo Info)
    {
        if (Player != null && PlayerControl.LocalPlayer == Player && Info.Killer == Player &&
            ninjaButton != null)
            ninjaButton.Timer = ninjaButton.MaxTimer;
    }

    public override void CleanUp(HudManager __instance)
    {
        ninjaButton?.Destroy();
        ninjaButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        // Ninja mark and assassinate button 
        ninjaButton?.Destroy();
        ninjaButton = new CustomButton(
            () =>
            {
                MessageWriter writer;
                if (ninjaMarked != null)
                {
                    // Murder attempt with teleport
                    // Create first trace before killing
                    var pos = PlayerControl.LocalPlayer.transform.position;

                    PlaceNinjaTrace.Invoke(pos);

                    SetNinjaInvisible.Invoke((PlayerControl.LocalPlayer, byte.MaxValue));

                    RpcCustomMurderPlayer(PlayerControl.LocalPlayer, ninjaMarked);

                    // Create Second trace after killing
                    pos = ninjaMarked.transform.position;

                    PlaceNinjaTrace.Invoke(pos);

                    ninjaMarked = null;
                    return;
                }

                else if (currentTarget != null)
                {
                    if (CheckAndDoVetKill(PlayerControl.LocalPlayer, currentTarget)) return;
                    ninjaMarked = currentTarget;
                    ninjaButton.Timer = 5f;
                    SoundEffectsManager.play("warlockCurse");
                }
            },
            () =>
            {
                return Player == PlayerControl.LocalPlayer && Player.IsAlive();
            },
            () =>
            {
                // CouldUse
                currentTarget = ImpostorSetTarget();
                SetPlayerOutline(currentTarget, color);

                ninjaButton.showTargetNameOnButton(currentTarget, GetString("NinjaText"));
                ninjaButton.Sprite = ninjaMarked != null
                    ? killButtonSprite
                    : markButtonSprite;
                return (currentTarget != null || (ninjaMarked != null
                        && !ninjaMarked.isUsingTransportation()))
                        && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                // on meeting ends
                ninjaButton.Timer = ninjaButton.MaxTimer;
                ninjaMarked = null;
            },
            markButtonSprite,
            __instance,
            __instance.KillButton,
            ModInputManager.abilityInput.keyCode,
            buttonText: GetString("NinjaText")
        )
        {
            MaxTimer = cooldown,
        };
    }
}
