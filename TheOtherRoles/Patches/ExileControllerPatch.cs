using AmongUs.GameOptions;
using PowerTools;
using System.Text;

namespace TheOtherRoles.Patches;

[HarmonyPatch(typeof(ExileController), nameof(ExileController.Begin))]
[HarmonyPriority(Priority.First)]
internal class ExileControllerBeginPatch
{
    public static GameData.PlayerInfo lastExiled;
    public static TextMeshPro confirmImpostorSecondText;
    private static bool IsSec;
    public static bool ForceExile;
    public static bool Prefix(ExileController __instance, [HarmonyArgument(0)] ref GameData.PlayerInfo exiled, [HarmonyArgument(1)] bool tie)
    {
        lastExiled = exiled;
        Message($"开始放逐: {exiled?.PlayerName ?? "null"}");
        if (Balancer.currentAbilityUser != null && Balancer.IsDoubleExile && !IsSec)
        {
            IsSec = true;
            __instance.exiled = null;
            ExileController controller = UObject.Instantiate(__instance, __instance.transform.parent);
            controller.exiled = Balancer.targetplayerright.Data;
            controller.Begin(controller.exiled, false);
            IsSec = false;
            controller.completeString = string.Empty;

            controller.Text.gameObject.SetActive(false);
            controller.Player.UpdateFromEitherPlayerDataOrCache(controller.exiled, PlayerOutfitType.Default, PlayerMaterial.MaskType.Exile, includePet: false);
            controller.Player.ToggleName(active: false);
            SkinViewData skin = ShipStatus.Instance.CosmeticsCache.GetSkin(controller.exiled.Outfits[PlayerOutfitType.Default].SkinId);
            controller.Player.FixSkinSprite(skin.EjectFrame);
            AudioClip sound = null;
            if (controller.EjectSound != null)
            {
                sound = new(controller.EjectSound.Pointer);
            }
            controller.EjectSound = null;
            void createlate(int index)
            {
                _ = new LateTask(() => { controller.StopAllCoroutines(); controller.StartCoroutine(controller.Animate()); }, 0.025f + index * 0.025f);
            }
            _ = new LateTask(() => controller.StartCoroutine(controller.Animate()), 0f);
            for (int i = 0; i < 23; i++)
            {
                createlate(i);
            }
            _ = new LateTask(() => { controller.StopAllCoroutines(); controller.EjectSound = sound; controller.StartCoroutine(controller.Animate()); }, 0.6f);
            ExileController.Instance = __instance;
            __instance.exiled = Balancer.targetplayerleft.Data;
            exiled = __instance.exiled;
            if (isFungle)
            {
                Helpers.SetActiveAllObject(controller.gameObject.GetChildren(), "RaftAnimation", false);
                controller.transform.localPosition = new(-3.75f, -0.2f, -60f);
            }
            if (!IsSec) return true;
        }

        // SecurityGuard vents and cameras
        var allCameras = MapUtilities.CachedShipStatus.AllCameras.ToList();
        ModOption.camerasToAdd.ForEach(camera =>
        {
            camera.gameObject.SetActive(true);
            camera.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            allCameras.Add(camera);
        });
        MapUtilities.CachedShipStatus.AllCameras = allCameras.ToArray();
        ModOption.camerasToAdd = new List<SurvCamera>();

        foreach (var vent in ModOption.ventsToSeal)
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
            rend.color = Color.white;
            vent.name = "SealedVent_" + vent.name;
        }
        ModOption.ventsToSeal = new List<Vent>();

        // 1 = reset per turn
        if (ModOption.restrictDevices == 1) ModOption.resetDeviceTimes();

        return true;
    }

    public static void Postfix(ExileController __instance, [HarmonyArgument(0)] ref GameData.PlayerInfo exiled)
    {
        var player = exiled?.Object ?? null;
        confirmImpostorSecondText = UObject.Instantiate(__instance.ImpostorText, __instance.Text.transform);
        StringBuilder changeStringBuilder = new();
        if (GameManager.Instance.LogicOptions.currentGameOptions.GetBool(BoolOptionNames.ConfirmImpostor))
            confirmImpostorSecondText.transform.localPosition += new Vector3(0f, -0.4f, 0f);
        else confirmImpostorSecondText.transform.localPosition += new Vector3(0f, -0.2f, 0f);

        confirmImpostorSecondText.text = changeStringBuilder.ToString();
        confirmImpostorSecondText.gameObject.SetActive(true);

        GetRoles<Pelican>().Do(pelican =>
        {
            if (pelican.DieOnExile)
            {
                pelican.Player.Exiled();
                pelican.DieOnExile = false;
            }
        });

        CustomRoleManager.OnExiledBegin(exiled);

        if (ForceExile)
        {
            __instance.completeString = string.Format(GetString("ExileController.ForceExile"), exiled?.PlayerName ?? "NULL");
            ForceExile = false;
        }

        if (Balancer.currentAbilityUser != null && Balancer.IsDoubleExile && __instance.exiled?.PlayerId == Balancer.targetplayerleft.PlayerId)
        {
            __instance.completeString = GetString("ExileController.Balancer");
            return;
        }

        if (CustomOptionHolder.exiledController.GetBool())
        {
            if (player != null)
            {
                switch (CustomOptionHolder.exiledReviveRole.GetQuantity())
                {
                    case 1:
                        __instance.completeString = TranslationController.Instance.GetString(StringNames.ExileTextNonConfirm, player?.Data.PlayerName);
                        break;
                    case 2:
                        var roleName = player.GetRoleInfo().Name;
                        __instance.completeString = string.Format(GetString("ExileController.PlayerRole"), player.Data.PlayerName, roleName);
                        break;
                    case 3:
                        __instance.completeString = string.Format(GetString("ExileController.PlayerTeam"), player.Data.PlayerName, teamString(player));
                        break;
                    default:
                        break;
                }
            }

            if (Prosecutor.IsProsecuteMeeting && player != null) __instance.completeString += $" {GetString("ExileController.Prosecute")}";

            if (CustomOptionHolder.exiledShowTeamNum.GetBool())
            {
                var Impostors = PlayerControl.AllPlayerControls.ToArray().Count(x => x.IsImpostor() && x.IsAlive() && x.PlayerId != player?.PlayerId);
                var Neutrals = PlayerControl.AllPlayerControls.ToArray().Count(x => x.IsNeutral() && x.IsAlive() && x.PlayerId != player?.PlayerId);
                __instance.ImpostorText.text =
                    $"\n{Cs(getTeamColor(RoleType.Impostor), GetString("ExileController.ImpNum")) + Impostors}" +
                    $" | {Cs(getTeamColor(RoleType.Neutral), GetString("ExileController.NeutralNum")) + Neutrals}";

            }
        }

    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.ReEnableGameplay))]
    public class BalancerChatDisable
    {
        private static void Postfix()
        {
            if (confirmImpostorSecondText != null) confirmImpostorSecondText.gameObject?.SetActive(false);
        }
    }
}

[HarmonyPatch]
internal class ExileControllerWrapUpPatch
{
    // Workaround to add a "postfix" to the destroying of the exile controller (i.e. cutscene) and SpwanInMinigame of submerged
    [HarmonyPatch(typeof(UObject), nameof(UObject.Destroy), typeof(GameObject))]
    public static void Prefix(GameObject obj)
    {
        // Nightvision:
        if (obj != null && obj.name != null && obj.name.Contains("FungleSecurity"))
        {
            SurveillanceMinigamePatch.resetNightVision();
            return;
        }

        // submerged
        if (!SubmergedCompatibility.IsSubmerged) return;
        if (obj.name.Contains("ExileCutscene"))
        {
            Message("Object.Destroy", "WrapUpPostfix");
            WrapUpPostfix(ExileControllerBeginPatch.lastExiled);
        }
        else if (obj.name.Contains("SpawnInMinigame"))
        {
            //AntiTeleport.setPosition();
            Chameleon.lastMoved.Clear();
        }
    }

    private static void WrapUpPostfix(GameData.PlayerInfo exiled)
    {
        Message("WrapUp Postfix");
        if (PlayerControl.LocalPlayer.IsDead()) CanSeeGhostInfo = true;

        CustomRoleManager.AllActiveRoles.Values.Do(x => x.OnExiledWrapUp(exiled));
        CustomRoleManager.AllActiveModifier.Values.SelectMany(x => x).Do(x => x.OnExiledWrapUp(exiled));


        if (Specter.Player == PlayerControl.LocalPlayer) Specter.remember = true;

        // Reset custom button timers where necessary
        CustomButton.OnMeetingEnd();

        if (Specter.Player != null && Specter.Player?.Data?.IsDead == true && Specter.exiledBeginRevive)
        {
            Specter.Player.Revive();
            Specter.exiledBeginRevive = false;
        }

        if (CustomOptionHolder.randomGameStartPosition.GetBool()) MapData.RandomSpawnPlayers();
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    private class BaseExileControllerPatch
    {
        public static void Postfix(ExileController __instance)
        {
            Message("ExileController.WrapUp", "WrapUpPostfix");
            WrapUpPostfix(__instance.exiled);
        }
    }

    [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
    private class AirshipExileControllerPatch
    {
        public static void Postfix(AirshipExileController __instance)
        {
            Message("AirshipExileController.WrapUpAndSpawn", "WrapUpPostfix");
            WrapUpPostfix(__instance.exiled);
        }

        public static bool Prefix(AirshipExileController __instance)
        {

            if (Balancer.currentAbilityUser != null && Balancer.IsDoubleExile && __instance != ExileController.Instance)
            {
                if (__instance.exiled != null)
                {
                    PlayerControl @object = __instance.exiled.Object;
                    if (@object)
                    {
                        @object.Exiled();
                    }
                    __instance.exiled.IsDead = true;
                }
                UObject.Destroy(__instance.gameObject);
            }
            return true;
        }
    }
}

// Set position of AntiTp players AFTER they have selected a spawn.
[HarmonyPatch(typeof(SpawnInMinigame), nameof(SpawnInMinigame.Close))]
internal class AirshipSpawnInPatch
{
    private static void Postfix()
    {
        //AntiTeleport.setPosition();
        Chameleon.lastMoved.Clear();
    }
}