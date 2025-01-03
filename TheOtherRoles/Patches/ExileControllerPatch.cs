using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AmongUs.GameOptions;
using Hazel;
using PowerTools;
using TheOtherRoles.Buttons;
using TheOtherRoles.Objects;
using TheOtherRoles.Utilities;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TheOtherRoles.Patches;

[HarmonyPatch(typeof(ExileController), nameof(ExileController.Begin))]
[HarmonyPriority(Priority.First)]
internal class ExileControllerBeginPatch
{
    public static NetworkedPlayerInfo lastExiled;
    public static TextMeshPro confirmImpostorSecondText;
    private static bool IsSec;
    public static bool Prefix(ExileController __instance, [HarmonyArgument(0)] ref ExileController.InitProperties init)
    {
        lastExiled = init?.networkedPlayer;
        var exiled = init?.networkedPlayer;
        Message($"开始放逐: {init?.networkedPlayer?.PlayerName ?? "null"}");
        if (Balancer.currentAbilityUser != null && Balancer.IsDoubleExile && !IsSec)
        {
            IsSec = true;
            __instance.initData.networkedPlayer = null;
            ExileController controller = Object.Instantiate(__instance, __instance.transform.parent);
            controller.initData.networkedPlayer = Balancer.targetplayerright.Data;
            controller.Begin(controller.initData);
            IsSec = false;
            controller.completeString = string.Empty;

            controller.Text.gameObject.SetActive(false);
            controller.Player.UpdateFromEitherPlayerDataOrCache(controller.initData.networkedPlayer, PlayerOutfitType.Default, PlayerMaterial.MaskType.Exile, includePet: false);
            controller.Player.ToggleName(active: false);
            SkinViewData skin = ShipStatus.Instance.CosmeticsCache.GetSkin(controller.initData.networkedPlayer.Outfits[PlayerOutfitType.Default].SkinId);
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
            init = GenerateExileInitProperties(Balancer.targetplayerleft.Data, false);

            if (isFungle)
            {
                Helpers.SetActiveAllObject(controller.gameObject.GetChildren(), "RaftAnimation", false);
                controller.transform.localPosition = new(-3.75f, -0.2f, -60f);
            }
            if (Lawyer.lawyer != null && exiled?.Object.PlayerId == Lawyer.target.PlayerId && Lawyer.target != Jester.jester)
            {
                var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.LawyerPromotesToPursuer);
                writer.Write(true);
                writer.EndRPC();
                Lawyer.PromotesToPursuer(true);
            }

            if (!IsSec) return true;
        }

        if (Lawyer.lawyer != null && exiled?.Object.PlayerId == Lawyer.target.PlayerId && Lawyer.target != Jester.jester)
        {
            var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.LawyerPromotesToPursuer);
            writer.Write(true);
            writer.EndRPC();
            Lawyer.PromotesToPursuer(true);
        }

        // Medic shield
        if (Medic.medic != null && AmongUsClient.Instance.AmHost && Medic.futureShielded != null && !Medic.medic.Data.IsDead)
        {
            // We need to send the RPC from the host here, to make sure that the order of shifting and setting the shield is correct(for that reason the futureShifted and futureShielded are being synced)
            var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                (byte)CustomRPC.MedicSetShielded, SendOption.Reliable);
            writer.Write(Medic.futureShielded.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.medicSetShielded(Medic.futureShielded.PlayerId);
        }

        if (Medic.usedShield) Medic.meetingAfterShielding = true; // Has to be after the setting of the shield

        if (PartTimer.partTimer != null && PartTimer.partTimer.IsAlive())
        {
            if (PartTimer.deathTurn <= 0 && PartTimer.target == null) PartTimer.partTimer.Exiled();
        }

        if (Doomsayer.doomsayer != null && AmongUsClient.Instance.AmHost && !Doomsayer.canGuess) Doomsayer.canGuess = true;

        if (Butcher.butcher != null)
        {
            Butcher.dissected = null;
            Butcher.canDissection = true;
        }

        // Trickster boxes
        if (Trickster.trickster != null && JackInTheBox.hasJackInTheBoxLimitReached()) JackInTheBox.convertToVents();

        // Activate portals.
        Portal.meetingEndsUpdate();

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

    public static void Postfix(ExileController __instance)
    {
        confirmImpostorSecondText = Object.Instantiate(__instance.ImpostorText, __instance.Text.transform);
        StringBuilder changeStringBuilder = new();

        if (GameManager.Instance.LogicOptions.currentGameOptions.GetBool(BoolOptionNames.ConfirmImpostor))
            confirmImpostorSecondText.transform.localPosition += new Vector3(0f, -0.4f, 0f);
        else confirmImpostorSecondText.transform.localPosition += new Vector3(0f, -0.2f, 0f);

        confirmImpostorSecondText.text = changeStringBuilder.ToString();
        confirmImpostorSecondText.gameObject.SetActive(true);

        if (Balancer.currentAbilityUser != null && Balancer.IsDoubleExile && __instance?.initData?.networkedPlayer?.PlayerId == Balancer.targetplayerleft?.PlayerId)
        {
            __instance.completeString = GetString("二者一同放逐");
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
    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(GameObject))]
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
            AntiTeleport.setPosition();
            Chameleon.lastMoved.Clear();
        }
    }

    private static void WrapUpPostfix(NetworkedPlayerInfo exiled)
    {
        Message("WrapUp");
        if (CachedPlayer.LocalPlayer.IsDead) CanSeeRoleInfo = true;
        // Prosecutor win condition
        if (exiled != null && Executioner.executioner != null && Executioner.target != null &&
            Executioner.target.PlayerId == exiled.PlayerId && !Executioner.executioner.Data.IsDead)
        {
            Executioner.triggerExecutionerWin = true;
            return;
        }
        // Mini exile lose condition
        else if (exiled != null && Mini.mini != null && Mini.mini.PlayerId == exiled.PlayerId && !Mini.isGrownUp() &&
                 !Mini.mini.Data.Role.IsImpostor && !isNeutral(Mini.mini))
        {
            Mini.triggerMiniLose = true;
            return;
        }
        // Jester win condition
        else if (exiled != null && Jester.jester != null && Jester.jester.PlayerId == exiled.PlayerId)
        {
            Jester.triggerJesterWin = true;
            return;
        }
        else if (Executioner.executioner != null && Executioner.executioner == CachedPlayer.LocalPlayer.PlayerControl && Executioner.target.IsDead())
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                (byte)CustomRPC.ExecutionerPromotesRole, SendOption.Reliable);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            Executioner.PromotesRole();
        }
        if (Witness.target != null && Witness.killerTarget != null)
        {
            bool skip = exiled == null && Witness.skipMeeting;
            bool targetIsKillerAndNotExiled = Witness.target == Witness.killerTarget && (exiled?.Object == null || Witness.target != exiled?.Object);
            bool targetIsExiledAndNotKiller = Witness.target != Witness.killerTarget && (Witness.target == exiled?.Object ||
                                              (Witness.meetingDie && Witness.target.IsDead()));

            if ((!skip && targetIsKillerAndNotExiled) || targetIsExiledAndNotKiller)
            {
                Witness.exiledCount++;
            }

            if (Witness.exiledCount == Witness.exileToWin)
            {
                Witness.triggerWitnessWin = true;
            }
        }
        Witness.target = Witness.killerTarget = null;

        if (Vortox.Player.IsAlive() && exiled == null)
        {
            Vortox.skipCount++;
            if (Vortox.skipCount == Vortox.skipMeetingNum) Vortox.triggerImpWin = true;
        }

        // Reset custom button timers where necessary
        CustomButton.MeetingEndedUpdate();

        // Clear all traps
        KillTrap.clearAllTraps();
        EvilTrapper.meetingFlag = false;
        Balancer.WrapUp(exiled == null ? null : exiled.Object);
        // Mini set adapted cooldown
        if (Mini.mini != null && CachedPlayer.LocalPlayer.PlayerControl == Mini.mini && Mini.mini.Data.Role.IsImpostor)
        {
            var multiplier = Mini.isGrownUp() ? 0.66f : 2f;
            Mini.mini.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown * multiplier);
        }

        // Seer spawn souls
        if (Seer.deadBodyPositions != null && Seer.seer != null &&
            CachedPlayer.LocalPlayer.PlayerControl == Seer.seer && (Seer.mode == 0 || Seer.mode == 2))
        {
            foreach (var pos in Seer.deadBodyPositions)
            {
                var soul = new GameObject();
                //soul.transform.position = pos;
                soul.transform.position = new Vector3(pos.x, pos.y, (pos.y / 1000) - 1f);
                soul.layer = 5;
                var rend = soul.AddComponent<SpriteRenderer>();
                soul.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
                rend.sprite = Seer.soulSprite;

                if (Seer.limitSoulDuration)
                {
                    FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Seer.soulDuration,
                        new Action<float>(p =>
                        {
                            if (rend != null)
                            {
                                var tmp = rend.color;
                                tmp.a = Mathf.Clamp01(1 - p);
                                rend.color = tmp;
                            }

                            if (p == 1f && rend != null && rend.gameObject != null) Object.Destroy(rend.gameObject);
                        })));
                }
            }
            Seer.deadBodyPositions = new List<Vector3>();
        }

        // Tracker reset deadBodyPositions
        Tracker.deadBodyPositions = new List<Vector3>();

        if (Blackmailer.blackmailer != null && Blackmailer.blackmailed != null)
        {
            // Blackmailer reset blackmailed
            var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                (byte)CustomRPC.UnblackmailPlayer, SendOption.Reliable);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.unblackmailPlayer();
        }

        // Arsonist deactivate dead poolable players
        if (Arsonist.arsonist != null && Arsonist.arsonist == CachedPlayer.LocalPlayer.PlayerControl)
        {
            var visibleCounter = 0;
            var newBottomLeft = IntroCutsceneOnDestroyPatch.bottomLeft;
            var BottomLeft = newBottomLeft + new Vector3(-0.25f, -0.25f, 0);
            foreach (PlayerControl p in CachedPlayer.AllPlayers)
            {
                if (!ModOption.playerIcons.ContainsKey(p.PlayerId)) continue;
                if (p.Data.IsDead || p.Data.Disconnected)
                {
                    ModOption.playerIcons[p.PlayerId].gameObject.SetActive(false);
                }
                else
                {
                    ModOption.playerIcons[p.PlayerId].transform.localPosition =
                        newBottomLeft + (Vector3.right * visibleCounter * 0.35f);
                    visibleCounter++;
                }
            }
        }

        // Deputy check Promotion, see if the sheriff still exists. The promotion will be after the meeting.
        if (Deputy.deputy != null) PlayerControlFixedUpdatePatch.deputyCheckPromotion(true);

        // Force Bounty Hunter Bounty Update
        if (BountyHunter.bountyHunter != null && BountyHunter.bountyHunter == CachedPlayer.LocalPlayer.PlayerControl)
            BountyHunter.bountyUpdateTimer = 0f;

        // Eraser erase
        if (Eraser.eraser != null && AmongUsClient.Instance.AmHost && Eraser.futureErased != null)
        {
            var rasePlayerList = new List<PlayerControl>(Eraser.futureErased);
            foreach (var target in rasePlayerList)
            {
                var writer = StartRPC(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.ErasePlayerRoles);
                writer.Write(target.PlayerId);
                writer.EndRPC();
                RPCProcedure.erasePlayerRoles(target.PlayerId);
                Eraser.alreadyErased.Add(target.PlayerId);
            }
        }
        Eraser.futureErased = new List<PlayerControl>();

        // Shifter shift
        if (Shifter.shifter != null && AmongUsClient.Instance.AmHost && Shifter.futureShift != null)
        {
            var writer = StartRPC(CachedPlayer.LocalPlayer.PlayerControl, CustomRPC.ShifterShift);
            writer.Write(Shifter.futureShift.PlayerId);
            writer.EndRPC();
            RPCProcedure.shifterShift(Shifter.futureShift.PlayerId);
        }

        Shifter.futureShift = null;

        // Witch execute casted spells
        if (Witch.witch != null && Witch.futureSpelled != null && AmongUsClient.Instance.AmHost)
        {
            var partner = exiled?.Object?.getPartner();

            var exiledIsWitch = exiled?.PlayerId == Witch.witch.PlayerId;
            var witchDiesWithExiledLover = partner?.PlayerId == Witch.witch.PlayerId || exiled?.PlayerId == Witch.witch.PlayerId;

            if (((witchDiesWithExiledLover || exiledIsWitch) && Witch.witchVoteSavesTargets) || Witch.witchWasGuessed)
                Witch.futureSpelled = new List<PlayerControl>();

            foreach (var target in Witch.futureSpelled.Where(x => x.IsAlive()))
            {
                if (Lawyer.lawyer != null && target == Lawyer.target)
                {
                    var writer2 = StartRPC(CachedPlayer.LocalPlayer.PlayerControl, CustomRPC.LawyerPromotesToPursuer);
                    writer2.EndRPC();
                    Lawyer.PromotesToPursuer();
                }

                if (Executioner.executioner.IsAlive() && target == Executioner.target)
                {
                    var writer2 = StartRPC(CachedPlayer.LocalPlayer.PlayerControl, CustomRPC.ExecutionerPromotesRole);
                    writer2.EndRPC();
                    Executioner.PromotesRole();
                }

                var writer = StartRPC(CachedPlayer.LocalPlayer.PlayerControl, CustomRPC.UncheckedExilePlayer);
                writer.Write(target.PlayerId);
                writer.EndRPC();
                RPCProcedure.uncheckedExilePlayer(target.PlayerId);

                GameHistory.RpcOverrideDeathReasonAndKiller(target, CustomDeathReason.WitchExile, Witch.witch);
            }
        }

        Witch.futureSpelled = new List<PlayerControl>();

        // Medium spawn souls
        if (Medium.medium != null && CachedPlayer.LocalPlayer.PlayerControl == Medium.medium)
        {
            if (Medium.souls != null)
            {
                foreach (var sr in Medium.souls) Object.Destroy(sr.gameObject);
                Medium.souls = new List<SpriteRenderer>();
            }

            if (Medium.futureDeadBodies != null)
            {
                foreach (var (db, ps) in Medium.futureDeadBodies)
                {
                    var s = new GameObject();
                    //s.transform.position = ps;
                    s.transform.position = new Vector3(ps.x, ps.y, (ps.y / 1000) - 1f);
                    s.layer = 5;
                    var rend = s.AddComponent<SpriteRenderer>();
                    s.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
                    rend.sprite = Medium.soulSprite;
                    Medium.souls.Add(rend);
                }

                Medium.deadBodies = Medium.futureDeadBodies;
                Medium.futureDeadBodies = new List<Tuple<DeadPlayer, Vector3>>();
            }
        }

        // AntiTeleport set position
        AntiTeleport.setPosition();

        if (CustomOptionHolder.randomGameStartPosition.GetBool()) MapData.RandomSpawnPlayers();

        if (InfoSleuth.infoSleuth != null && InfoSleuth.target != null && InfoSleuth.infoSleuth == PlayerControl.LocalPlayer)
        {
            var isNotCrew = (isNeutral(InfoSleuth.target) || InfoSleuth.target.isImpostor()) ^ Vortox.Reversal;
            var team = "的阵营是 " + getTeam(InfoSleuth.target);
            var info = InfoSleuth.infoType switch
            {
                0 => isNotCrew ? "不是船员" : "是船员",
                1 => team,
                _ => rnd.Next(2) == 0 ? isNotCrew ? "不是船员" : "是船员" : team,
            };

            string msg = $"{InfoSleuth.target.Data.PlayerName} {info}";

            FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"{msg}");
            var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.ShareGhostInfo);
            writer.Write(InfoSleuth.infoSleuth.PlayerId);
            writer.Write((byte)RPCProcedure.GhostInfoTypes.GhostChat);
            writer.Write(msg);
            writer.EndRPC();

            var writer1 = StartRPC(PlayerControl.LocalPlayer, CustomRPC.InfoSleuthNoTarget);
            writer1.EndRPC();
            RPCProcedure.infoSleuthNoTarget();

            static string getTeam(PlayerControl player)
            {
                if (Vortox.Player.IsAlive())
                {
                    if (player.isCrew()) return rnd.Next(2) == 0 ? "NeutralRolesText".Translate() : "ImpostorRolesText".Translate();
                    if (isNeutral(player) || player.isImpostor()) return "CrewmateRolesText".Translate();
                }

                return isNeutral(player) ? "NeutralRolesText".Translate()
                    : player.isImpostor() ? "ImpostorRolesText".Translate()
                    : "CrewmateRolesText".Translate();
            }
        }

        // Invert add meeting
        if (Invert.meetings > 0) Invert.meetings--;

        Chameleon.lastMoved.Clear();

        foreach (var trap in Trap.traps) trap.triggerable = false;
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(
            (GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown / 2) + 2, new Action<float>(p =>
            { if (p == 1f) foreach (var trap in Trap.traps) trap.triggerable = true; })));

        if (!Yoyo.markStaysOverMeeting) Silhouette.clearSilhouettes();

        if (AmongUsClient.Instance.AmHost)
        {
            LastImpostor.promoteToLastImpostor();
        }
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    private class BaseExileControllerPatch
    {
        public static void Postfix(ExileController __instance)
        {
            Message("ExileController.WrapUp", "WrapUpPostfix");
            WrapUpPostfix(__instance.initData?.networkedPlayer);
        }
    }

    [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
    private class AirshipExileControllerPatch
    {
        public static void Postfix(AirshipExileController __instance)
        {
            Message("AirshipExileController.WrapUpAndSpawn", "WrapUpPostfix");
            WrapUpPostfix(__instance.initData?.networkedPlayer);
        }

        public static bool Prefix(AirshipExileController __instance)
        {

            if (Balancer.currentAbilityUser != null && Balancer.IsDoubleExile && __instance != ExileController.Instance)
            {
                if (__instance.initData?.networkedPlayer != null)
                {
                    PlayerControl @object = __instance.initData?.networkedPlayer.Object;
                    if (@object)
                    {
                        @object.Exiled();
                    }
                    __instance.initData.networkedPlayer.IsDead = true;
                }
                Object.Destroy(__instance.gameObject);
            }
            return true;
        }
    }
}

[HarmonyPatch(typeof(SpawnInMinigame),
    nameof(SpawnInMinigame.Close))] // Set position of AntiTp players AFTER they have selected a spawn.
internal class AirshipSpawnInPatch
{
    private static void Postfix()
    {
        AntiTeleport.setPosition();
        Chameleon.lastMoved.Clear();
    }
}

[HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), typeof(StringNames),
    typeof(Il2CppReferenceArray<Il2CppSystem.Object>))]
internal class ExileControllerMessagePatch
{
    private static void Postfix(ref string __result, [HarmonyArgument(0)] StringNames id)
    {
        try
        {
            if (ExileController.Instance != null && ExileController.Instance.initData.networkedPlayer != null)
            {
                var player = playerById(ExileController.Instance.initData.networkedPlayer.Object.PlayerId);
                if (player == null) return;
                // Exile role text
                if (id is StringNames.ExileTextPN or StringNames.ExileTextSN or StringNames.ExileTextPP or StringNames.ExileTextSP)
                    __result = $"{player.Data.PlayerName} 的职业是 {string.Join(" ", RoleInfo.getRoleInfoForPlayer(player, false).Select(x => x.Name).ToArray())}";
                // Hide number of remaining impostors on Jester win
                if (id is StringNames.ImpostorsRemainP or StringNames.ImpostorsRemainS)
                    if (Jester.jester != null && player.PlayerId == Jester.jester.PlayerId)
                        __result = "";
                if (Prosecutor.ProsecuteThisMeeting) __result += " (被起诉)";
            }
        }
        catch
        {
            // pass - Hopefully prevent leaving while exiling to softlock game
        }
    }
}