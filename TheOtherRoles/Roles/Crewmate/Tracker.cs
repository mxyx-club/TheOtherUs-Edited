using TheOtherRoles.Objects;

namespace TheOtherRoles.Roles.Crewmate;

[CustomRpcHolder]
public class Tracker : RoleBase
{
    public static Color color = new Color32(100, 58, 220, byte.MaxValue);

    public static readonly RoleInfo roleinfo = new(
        typeof(Tracker),
        (p) => new Tracker(p),
        RoleId.Tracker,
        RoleType.Crewmate,
        "Tracker",
        color,
        302600,
        AddOptions
    );

    public Tracker(PlayerControl p) : base(p, roleinfo) { }

    public static float updateIntervall = 5f;
    public static bool resetTargetAfterMeeting;
    public static bool canTrackCorpses;
    public static float corpsesTrackingCooldown = 30f;
    public static float corpsesTrackingDuration = 5f;
    public static float corpsesTrackingTimer;
    public static List<Vector3> deadBodyPositions = new();

    public PlayerControl currentTarget;
    public PlayerControl tracked;
    public bool usedTracker;
    public float timeUntilUpdate;
    public Arrow arrow = new(Color.blue);
    public List<Arrow> localArrows = new();

    public static GameObject DangerMeterParent;
    public static DangerMeter Meter;

    public CustomButton trackerTrackPlayerButton;
    public CustomButton trackerTrackCorpsesButton;
    public static Sprite trackCorpsesButtonSprite = new ResourceSprite("PathfindButton.png");
    public static Sprite buttonSprite = new ResourceSprite("TrackerButton.png");

    public static CustomOption trackerUpdateIntervall;
    public static CustomOption trackerResetTargetAfterMeeting;
    public static CustomOption trackerCanTrackCorpses;
    public static CustomOption trackerCorpsesTrackingCooldown;
    public static CustomOption trackerCorpsesTrackingDuration;
    public static RemoteProcess<(PlayerControl player, PlayerControl target)> TrackerUsedTracker = new("TrackerUsedTracker", (data, _) =>
    {
        if (data.player.TryGetRole<Tracker>(out var tracker))
        {
            tracker.usedTracker = true;
            tracker.tracked = data.target;
        }
    });
    private static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        trackerUpdateIntervall = CustomOption.Create(configId++, CustomOptionType.Crewmate, "trackerUpdateIntervall", 0.5f, 0f, 30f, 0.5f, roleinfo.RoleOption);
        trackerResetTargetAfterMeeting = CustomOption.Create(configId++, CustomOptionType.Crewmate, "trackerResetTargetAfterMeeting ", false, roleinfo.RoleOption);
        trackerCanTrackCorpses = CustomOption.Create(configId++, CustomOptionType.Crewmate, "trackerCanTrackCorpses", true, roleinfo.RoleOption);
        trackerCorpsesTrackingCooldown = CustomOption.Create(configId++, CustomOptionType.Crewmate, "trackerCorpsesTrackingCooldown", 17.5f, 5f, 60f, 2.5f, trackerCanTrackCorpses);
        trackerCorpsesTrackingDuration = CustomOption.Create(configId++, CustomOptionType.Crewmate, "trackerCorpsesTrackingDuration", 7.5f, 2.5f, 30f, 2.5f, trackerCanTrackCorpses);
    }

    public override void Initialize()
    {
        currentTarget = tracked = null;
        usedTracker = false;
        if (arrow?.arrow != null) UObject.Destroy(arrow.arrow);
        arrow = new Arrow(Color.blue);
        arrow.arrow?.SetActive(false);
        timeUntilUpdate = 0f;
        updateIntervall = trackerUpdateIntervall.GetFloat();
        resetTargetAfterMeeting = trackerResetTargetAfterMeeting.GetBool();
        if (localArrows != null)
        {
            foreach (Arrow arrow in localArrows)
                if (arrow?.arrow != null)
                    UObject.Destroy(arrow.arrow);
        }
        deadBodyPositions.Clear();
        corpsesTrackingTimer = 0f;
        corpsesTrackingCooldown = trackerCorpsesTrackingCooldown.GetFloat();
        corpsesTrackingDuration = trackerCorpsesTrackingDuration.GetFloat();
        canTrackCorpses = trackerCanTrackCorpses.GetBool();
        if (DangerMeterParent)
        {
            Meter.gameObject.Destroy();
            DangerMeterParent.Destroy();
        }
    }

    public override void OnFixedUpdate(PlayerControl player)
    {
        // Handle player tracking
        if (arrow?.arrow != null)
        {
            if (Player == null || player != Player)
            {
                arrow.arrow.SetActive(false);
                if (DangerMeterParent) DangerMeterParent.SetActive(false);
                return;
            }

            if (tracked != null && Player.IsAlive())
            {
                timeUntilUpdate -= Time.fixedDeltaTime;

                if (tracked.Data.IsDead)
                {
                    currentTarget = tracked = null;
                    usedTracker = false;
                    if (arrow?.arrow != null) UObject.Destroy(arrow.arrow);
                    arrow = new Arrow(Color.blue);
                    arrow.arrow?.SetActive(false);
                }

                if (timeUntilUpdate <= 0f)
                {
                    bool trackedOnMap = tracked.IsAlive();
                    Vector3 position = tracked.transform.position;
                    if (!trackedOnMap)
                    {
                        // Check for dead body
                        DeadBody body = UObject.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == tracked.PlayerId);
                        if (body != null)
                        {
                            trackedOnMap = true;
                            position = body.transform.position;
                        }
                    }

                    arrow.Update(position, tracked?.Data.Color);
                    arrow.arrow.SetActive(trackedOnMap);

                    timeUntilUpdate = updateIntervall;
                }
                else
                {
                    arrow.Update();
                }
            }
            else if (Player.Data.IsDead)
            {
                DangerMeterParent?.SetActive(false);
                Meter?.gameObject.SetActive(false);
            }
        }

        // Handle corpses tracking
        if (Player != null && Player == player && corpsesTrackingTimer >= 0f && !Player.Data.IsDead)
        {
            bool arrowsCountChanged = localArrows.Count != deadBodyPositions.Count;
            int index = 0;

            if (arrowsCountChanged)
            {
                foreach (Arrow arrow in localArrows) UObject.Destroy(arrow.arrow);
                localArrows = new();
            }
            foreach (Vector3 position in deadBodyPositions)
            {
                if (arrowsCountChanged)
                {
                    localArrows.Add(new Arrow(color));
                    localArrows[index].arrow.SetActive(true);
                }
                if (localArrows[index] != null) localArrows[index].Update(position);
                index++;
            }
        }
        else if (localArrows.Count > 0)
        {
            foreach (Arrow arrow in localArrows) UObject.Destroy(arrow.arrow);
            localArrows = new();
        }
    }

    public override void OnExiledWrapUp(GameData.PlayerInfo exiled)
    {
        currentTarget = tracked = null;
        usedTracker = false;
        if (arrow?.arrow != null) UObject.Destroy(arrow.arrow);
        arrow = new Arrow(Color.blue);
        arrow.arrow?.SetActive(false);

        // Tracker reset deadBodyPositions
        deadBodyPositions = new List<Vector3>();

    }

    public override void OnMurderPlayer(MurderInfo Info)
    {
        deadBodyPositions?.Add(Info.Target.transform.position);
        if (Info.Target == tracked)
        {
            currentTarget = tracked = null;
            usedTracker = false;
            if (arrow?.arrow != null) UObject.Destroy(arrow.arrow);
            arrow = new Arrow(Color.blue);
            arrow.arrow?.SetActive(false);
        }
    }

    public override void CleanUp(HudManager __instance)
    {
        trackerTrackPlayerButton?.Destroy();
        trackerTrackCorpsesButton?.Destroy();
        trackerTrackPlayerButton = null;
        trackerTrackCorpsesButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        // Tracker button
        trackerTrackPlayerButton?.Destroy();
        trackerTrackPlayerButton = new CustomButton(
            () =>
            {
                if (CheckAndDoVetKill(PlayerControl.LocalPlayer, currentTarget)) return;

                TrackerUsedTracker.Invoke((PlayerControl.LocalPlayer, currentTarget));
                SoundEffectsManager.play("trackerTrackPlayer");
            },
            () =>
            {
                return Player == PlayerControl.LocalPlayer && Player.IsAlive();
            },
            () =>
            {
                currentTarget = SetTarget();
                if (!usedTracker)
                {
                    SetPlayerOutline(currentTarget, color);
                    trackerTrackPlayerButton.showTargetNameOnButton(currentTarget, GetString("TrackerText"));
                }

                return PlayerControl.LocalPlayer.CanMove && currentTarget != null && !usedTracker;
            },
            () =>
            {
                //if (Tracker.resetTargetAfterMeeting) Tracker.resetTracked();
            },
            buttonSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.abilityInput.keyCode,
            buttonText: GetString("TrackerText")
        )
        {
            MaxTimer = 0f,
        }; ;

        trackerTrackCorpsesButton = new CustomButton(
            () =>
            {
                corpsesTrackingTimer = corpsesTrackingDuration;
                SoundEffectsManager.play("trackerTrackCorpses");
            },
            () =>
            {
                return Player == PlayerControl.LocalPlayer && Player.IsAlive() && canTrackCorpses;
            },
            () =>
            {
                return PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                trackerTrackCorpsesButton.Timer = trackerTrackCorpsesButton.MaxTimer;
                trackerTrackCorpsesButton.isEffectActive = false;
                trackerTrackCorpsesButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            trackCorpsesButtonSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.secondaryAbilityInput.keyCode,
            true,
            corpsesTrackingDuration,
            () => { trackerTrackCorpsesButton.Timer = trackerTrackCorpsesButton.MaxTimer; },
            buttonText: GetString("TrackerCorpsesText")
        )
        {
            MaxTimer = corpsesTrackingCooldown,
            EffectDuration = corpsesTrackingDuration,
        };
    }
}