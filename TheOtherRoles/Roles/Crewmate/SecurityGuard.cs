using PowerTools;
using static TheOtherRoles.Options.ModOption;

namespace TheOtherRoles.Roles.Crewmate;

[CustomRpcHolder]
public class SecurityGuard : RoleBase
{
    public static Color color = new Color32(195, 178, 95, byte.MaxValue);

    public static readonly RoleInfo roleinfo = new(
        typeof(SecurityGuard),
        (p) => new SecurityGuard(p),
        RoleId.SecurityGuard,
        RoleType.Crewmate,
        "SecurityGuard",
        color,
        303000,
        AddOptions
    );

    public SecurityGuard(PlayerControl p) : base(p, roleinfo) { }

    public int charges = 1;
    public int rechargedTasks = 3;
    public Vent ventTarget;
    public Minigame minigame;

    public static float cooldown = 30f;
    public static int remainingScrews = 7;
    public static int totalScrews = 7;
    public static int ventPrice = 1;
    public static int camPrice = 2;
    public static int placedCameras;
    public static float duration = 10f;
    public static int rechargeTasksNumber = 3;
    public static bool cantMove = true;
    public static int maxCharges = 5;

    private static float lastPPU;
    private static Sprite animatedVentSealedSprite;
    private static Sprite camSprite;
    private static Sprite logSprite;
    public static Sprite closeVentButtonSprite = new ResourceSprite("CloseVentButton.png");
    public static Sprite placeCameraButtonSprite = new ResourceSprite("PlaceCameraButton.png");
    public static Sprite staticVentSealedSprite = new ResourceSprite("StaticVentSealed.png", 160);
    public static Sprite fungleVentSealedSprite = new ResourceSprite("FungleVentSealed.png", 160);
    public static Sprite submergedCentralUpperVentSealedSprite = new ResourceSprite("CentralUpperBlocked.png", 145);
    public static Sprite submergedCentralLowerVentSealedSprite = new ResourceSprite("CentralLowerBlocked.png", 145);
    public CustomButton securityGuardButton;
    public CustomButton securityGuardCamButton;

    public static CustomOption securityGuardCooldown;
    public static CustomOption securityGuardTotalScrews;
    public static CustomOption securityGuardCamPrice;
    public static CustomOption securityGuardVentPrice;
    public static CustomOption securityGuardCamDuration;
    public static CustomOption securityGuardCamMaxCharges;
    public static CustomOption securityGuardCamRechargeTasksNumber;
    public static CustomOption securityGuardNoMove;
    public static RemoteProcess<int> SealVent = new("SealVent", (ventId, _) =>
    {
        var vent = MapUtilities.CachedShipStatus.AllVents.FirstOrDefault(x => x != null && x.Id == ventId);
        if (vent == null) return;

        SecurityGuard.remainingScrews -= SecurityGuard.ventPrice;
        if (PlayerControl.LocalPlayer.Is(RoleId.SecurityGuard))
        {
            var animator = vent.GetComponent<SpriteAnim>();

            vent.EnterVentAnim = vent.ExitVentAnim = null;
            var newSprite = animator == null
                ? SecurityGuard.staticVentSealedSprite
                : SecurityGuard.getAnimatedVentSealedSprite();
            var rend = vent.myRend;
            if (isFungle)
            {
                newSprite = SecurityGuard.fungleVentSealedSprite;
                rend = vent.transform.GetChild(3).GetComponent<SpriteRenderer>();
                animator = vent.transform.GetChild(3).GetComponent<SpriteAnim>();
            }

            animator?.Stop();
            rend.sprite = newSprite;
            if (SubmergedCompatibility.IsSubmerged && vent.Id == 0) vent.myRend.sprite = SecurityGuard.submergedCentralUpperVentSealedSprite;
            if (SubmergedCompatibility.IsSubmerged && vent.Id == 14) vent.myRend.sprite = SecurityGuard.submergedCentralLowerVentSealedSprite;
            rend.color = new Color(1f, 1f, 1f, 0.5f);
            vent.name = "FutureSealedVent_" + vent.name;
        }

        ventsToSeal.Add(vent);
    });
    public static RemoteProcess<byte[]> PlaceCamera = new("PlaceCamera", (buff, _) =>
    {
        var referenceCamera = UObject.FindObjectOfType<SurvCamera>();
        if (referenceCamera == null) return; // Mira HQ

        SecurityGuard.remainingScrews -= SecurityGuard.camPrice;
        SecurityGuard.placedCameras++;

        var position = Vector3.zero;
        position.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
        position.y = BitConverter.ToSingle(buff, 1 * sizeof(float));

        var camera = UObject.Instantiate(referenceCamera);
        camera.transform.position = new Vector3(position.x, position.y, referenceCamera.transform.position.z - 1f);
        camera.CamName = $"Security Camera {SecurityGuard.placedCameras}";
        camera.Offset = new Vector3(0f, 0f, camera.Offset.z);
        if (GameOptionsManager.Instance.currentNormalGameOptions.MapId is 2 or 4)
            camera.transform.localRotation = new Quaternion(0, 0, 1, 1); // Polus and Airship 

        if (SubmergedCompatibility.IsSubmerged)
        {
            // remove 2d box collider of console, so that no barrier can be created. (irrelevant for now, but who knows... maybe we need it later)
            var fixConsole = camera.transform.FindChild("FixConsole");
            if (fixConsole != null)
            {
                var boxCollider = fixConsole.GetComponent<BoxCollider2D>();
                if (boxCollider != null) UObject.Destroy(boxCollider);
            }
        }


        if (PlayerControl.LocalPlayer.Is(RoleId.SecurityGuard))
        {
            camera.gameObject.SetActive(true);
            camera.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
        }
        else
        {
            camera.gameObject.SetActive(false);
        }

        camerasToAdd.Add(camera);
    });
    private static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        securityGuardCooldown = CustomOption.Create(configId++, CustomOptionType.Crewmate, "securityGuardCooldown", 15f, 10f, 60f, 2.5f, roleinfo.RoleOption);
        securityGuardTotalScrews = CustomOption.Create(configId++, CustomOptionType.Crewmate, "securityGuardTotalScrews", 6f, 1f, 15f, 1f, roleinfo.RoleOption);
        securityGuardCamPrice = CustomOption.Create(configId++, CustomOptionType.Crewmate, "securityGuardCamPrice", 2f, 1f, 15f, 1f, roleinfo.RoleOption);
        securityGuardVentPrice = CustomOption.Create(configId++, CustomOptionType.Crewmate, "securityGuardVentPrice", 1f, 1f, 15f, 1f, roleinfo.RoleOption);
        securityGuardCamDuration = CustomOption.Create(configId++, CustomOptionType.Crewmate, "securityGuardCamDuration", 10f, 2.5f, 60f, 2.5f, roleinfo.RoleOption);
        securityGuardCamMaxCharges = CustomOption.Create(configId++, CustomOptionType.Crewmate, "securityGuardCamMaxCharges", 5f, 1f, 30f, 1f, roleinfo.RoleOption);
        securityGuardCamRechargeTasksNumber = CustomOption.Create(configId++, CustomOptionType.Crewmate, "securityGuardCamRechargeTasksNumber", 3f, 1f, 10f, 1f, roleinfo.RoleOption);
        securityGuardNoMove = CustomOption.Create(configId++, CustomOptionType.Crewmate, "securityGuardNoMove", true, roleinfo.RoleOption);
    }
    public static Sprite getAnimatedVentSealedSprite()
    {
        var ppu = 185f;
        if (SubmergedCompatibility.IsSubmerged) ppu = 120f;
        if (lastPPU != ppu)
        {
            animatedVentSealedSprite = null;
            lastPPU = ppu;
        }

        if (animatedVentSealedSprite) return animatedVentSealedSprite;
        animatedVentSealedSprite = UnityHelper.loadSpriteFromResources("TheOtherRoles.Resources.AnimatedVentSealed.png", ppu);
        return animatedVentSealedSprite;
    }

    public static Sprite getCamSprite()
    {
        if (camSprite) return camSprite;
        camSprite = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.CamsButton].Image;
        return camSprite;
    }

    public static Sprite getLogSprite()
    {
        if (logSprite) return logSprite;
        logSprite = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.DoorLogsButton].Image;
        return logSprite;
    }

    public override void Initialize()
    {
        ventTarget = null;
        minigame = null;
        placedCameras = 0;
        duration = securityGuardCamDuration.GetFloat();
        maxCharges = securityGuardCamMaxCharges.GetInt();
        rechargeTasksNumber = securityGuardCamRechargeTasksNumber.GetInt();
        rechargedTasks = securityGuardCamRechargeTasksNumber.GetInt();
        charges = securityGuardCamMaxCharges.GetInt() / 2;
        cooldown = securityGuardCooldown.GetFloat();
        totalScrews = remainingScrews = securityGuardTotalScrews.GetInt();
        camPrice = securityGuardCamPrice.GetInt();
        ventPrice = securityGuardVentPrice.GetInt();
        cantMove = securityGuardNoMove.GetBool();
    }

    public override void OnHudUpdate(HudManager hudManager)
    {
        if (Player.Data.IsDead) return;
        var (playerCompleted, _) = TasksHandler.taskInfo(Player.Data);
        if (playerCompleted == rechargedTasks)
        {
            rechargedTasks += rechargeTasksNumber;
            if (maxCharges > charges) charges++;
        }
    }

    public override void CleanUp(HudManager __instance)
    {
        securityGuardButton?.Destroy();
        securityGuardCamButton?.Destroy();
        securityGuardButton = null;
        securityGuardCamButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        // Security Guard button
        securityGuardButton?.Destroy();
        securityGuardButton = new CustomButton(
            () =>
            {
                if (ventTarget != null)
                {
                    // Seal vent
                    SealVent.Invoke(ventTarget.Id);
                    ventTarget = null;
                }
                else if (!isMira && !isFungle && !SubmergedCompatibility.IsSubmerged)
                {
                    // Place camera if there's no vent and it's not MiraHQ or Submerged
                    var pos = PlayerControl.LocalPlayer.transform.position;
                    var buff = new byte[sizeof(float) * 2];
                    Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                    Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

                    PlaceCamera.Invoke(buff);
                }

                SoundEffectsManager.play("securityGuardPlaceCam"); // Same sound used for both types (cam or vent)!
                securityGuardButton.Timer = securityGuardButton.MaxTimer;
            },
            () =>
            {
                return Player == PlayerControl.LocalPlayer && Player.IsAlive() &&
                       remainingScrews >= Mathf.Min(ventPrice, camPrice);
            },
            () =>
            {
                securityGuardButton.actionButton.graphic.sprite =
                    ventTarget == null && !isMira && !isFungle &&
                    !SubmergedCompatibility.IsSubmerged
                        ? placeCameraButtonSprite
                        : closeVentButtonSprite;
                if (securityGuardButton.ButtonTitle != null)
                    securityGuardButton.ButtonTitle.text = $"{remainingScrews}/{totalScrews}";


                Vent target = null;
                var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
                var closestDistance = float.MaxValue;
                for (var i = 0; i < MapUtilities.CachedShipStatus.AllVents.Length; i++)
                {
                    var vent = MapUtilities.CachedShipStatus.AllVents[i];
                    if (vent.gameObject.name.StartsWith("JackInTheBoxVent_") ||
                        vent.gameObject.name.StartsWith("SealedVent_") ||
                        vent.gameObject.name.StartsWith("FutureSealedVent_")) continue;
                    if (SubmergedCompatibility.IsSubmerged && vent.Id == 9) continue; // cannot seal submergeds exit only vent!
                    var distance = Vector2.Distance(vent.transform.position, truePosition);
                    if (distance <= vent.UsableDistance && distance < closestDistance)
                    {
                        closestDistance = distance;
                        target = vent;
                    }
                }

                ventTarget = target;

                if (ventTarget != null)
                    return remainingScrews >= ventPrice &&
                           PlayerControl.LocalPlayer.CanMove;
                return !isMira && !isFungle && !SubmergedCompatibility.IsSubmerged &&
                       remainingScrews >= camPrice &&
                       PlayerControl.LocalPlayer.CanMove;
            },
            () => { securityGuardButton.Timer = securityGuardButton.MaxTimer; },
            placeCameraButtonSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.abilityInput.keyCode
        )
        {
            MaxTimer = cooldown,
        };

        securityGuardCamButton?.Destroy();
        securityGuardCamButton = new CustomButton(
            () =>
            {
                if (!isMira)
                {
                    if (minigame == null)
                    {
                        var mapId = GameOptionsManager.Instance.currentNormalGameOptions.MapId;
                        var e = UObject.FindObjectsOfType<SystemConsole>().FirstOrDefault(x =>
                            x.gameObject.name.Contains("Surv_Panel") || x.name.Contains("Cam") ||
                            x.name.Contains("BinocularsSecurityConsole"));
                        if (isSkeld || mapId == 3)
                            e = UObject.FindObjectsOfType<SystemConsole>()
                                .FirstOrDefault(x => x.gameObject.name.Contains("SurvConsole"));
                        else if (isAirship)
                            e = UObject.FindObjectsOfType<SystemConsole>()
                                .FirstOrDefault(x => x.gameObject.name.Contains("task_cams"));
                        if (e == null || Camera.main == null) return;
                        minigame = UObject.Instantiate(e.MinigamePrefab, Camera.main.transform, false);
                    }

                    minigame.transform.SetParent(Camera.main.transform, false);
                    minigame.transform.localPosition = new Vector3(0.0f, 0.0f, -50f);
                    minigame.Begin(null);
                }
                else
                {
                    if (minigame == null)
                    {
                        var e = UObject.FindObjectsOfType<SystemConsole>()
                            .FirstOrDefault(x => x.gameObject.name.Contains("SurvLogConsole"));
                        if (e == null || Camera.main == null) return;
                        minigame = UObject.Instantiate(e.MinigamePrefab, Camera.main.transform, false);
                    }

                    minigame.transform.SetParent(Camera.main.transform, false);
                    minigame.transform.localPosition = new Vector3(0.0f, 0.0f, -50f);
                    minigame.Begin(null);
                }

                charges--;

                if (cantMove) PlayerControl.LocalPlayer.moveable = false;
                PlayerControl.LocalPlayer.NetTransform.Halt(); // Stop current movement 
            },
            () =>
            {
                return Player == PlayerControl.LocalPlayer && Player.IsAlive() &&
                       !PlayerControl.LocalPlayer.Data.IsDead && remainingScrews <
                       Mathf.Min(ventPrice, camPrice)
                       && !SubmergedCompatibility.IsSubmerged;
            },
            () =>
            {
                if (securityGuardCamButton.ButtonTitle != null)
                    securityGuardCamButton.ButtonTitle.text = $"{charges} / {maxCharges}";
                securityGuardCamButton.actionButton.graphic.sprite =
                    isMira ? getLogSprite() : getCamSprite();
                securityGuardCamButton.actionButton.OverrideText(isMira ?
                    GetString("hackerDoorLogText") : GetString("CamButtonText"));
                return PlayerControl.LocalPlayer.CanMove && charges > 0;
            },
            () =>
            {
                securityGuardCamButton.Timer = securityGuardCamButton.MaxTimer;
                securityGuardCamButton.isEffectActive = false;
                securityGuardCamButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            getCamSprite(),
            __instance,
            __instance.AbilityButton,
            ModInputManager.abilityInput.keyCode,
            true,
            0f,
            () =>
            {
                securityGuardCamButton.Timer = securityGuardCamButton.MaxTimer;
                if (Minigame.Instance) minigame.ForceClose();
                PlayerControl.LocalPlayer.moveable = true;
            },
            false,
            isMira ? GetString("hackerDoorLogText") : GetString("CamButtonText")
        )
        {
            MaxTimer = cooldown,
            EffectDuration = duration,
        };

        // securityGuardChargesText -> securityGuardCamButton
        // securityGuardButtonScrewsText -> securityGuardButton
    }
}
