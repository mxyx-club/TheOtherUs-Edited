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
    public static GameData.PlayerInfo lastExiled;
    public static TextMeshPro confirmImpostorSecondText;
    private static bool IsSec;
    public static bool Prefix(ExileController __instance, [HarmonyArgument(0)] ref GameData.PlayerInfo exiled, [HarmonyArgument(1)] bool tie)
    {
        lastExiled = exiled;
        Message($"开始放逐: {exiled?.PlayerName ?? "null"}");
        if (Balancer.currentAbilityUser != null && Balancer.IsDoubleExile && !IsSec)
        {
            IsSec = true;
            __instance.exiled = null;
            ExileController controller = Object.Instantiate(__instance, __instance.transform.parent);
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
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
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

    public static void Postfix(ExileController __instance, [HarmonyArgument(0)] ref GameData.PlayerInfo exiled)
    {
        var player = exiled?.Object ?? null;
        confirmImpostorSecondText = Object.Instantiate(__instance.ImpostorText, __instance.Text.transform);
        StringBuilder changeStringBuilder = new();

        if (GameManager.Instance.LogicOptions.currentGameOptions.GetBool(BoolOptionNames.ConfirmImpostor))
            confirmImpostorSecondText.transform.localPosition += new Vector3(0f, -0.4f, 0f);
        else confirmImpostorSecondText.transform.localPosition += new Vector3(0f, -0.2f, 0f);

        confirmImpostorSecondText.text = changeStringBuilder.ToString();
        confirmImpostorSecondText.gameObject.SetActive(true);

        if (Balancer.currentAbilityUser != null && Balancer.IsDoubleExile && __instance.exiled?.PlayerId == Balancer.targetplayerleft.PlayerId)
        {
            __instance.completeString = GetString("二者一同放逐");
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
                        __instance.completeString = $"{player.Data.PlayerName} 的职业是 {string.Join(" ", RoleInfo
                            .getRoleInfoForPlayer(player, false, false).Select(x => x.Name))}";
                        break;
                    case 3:
                        __instance.completeString = $"{player.Data.PlayerName} 是 {teamString(player)}";
                        break;
                    default:
                        break;
                }
            }

            if (CustomOptionHolder.exiledShowTeamNum.GetBool())
            {
                var Impostors = PlayerControl.AllPlayerControls.ToArray().Count(x => x.IsImpostor() && x.IsAlive() && x.PlayerId != player?.PlayerId);
                var Neutrals = PlayerControl.AllPlayerControls.ToArray().Count(x => x.IsNeutral() && x.IsAlive() && x.PlayerId != player?.PlayerId);
                __instance.ImpostorText.text =
                    $"\n{cs(getTeamColor(RoleType.Impostor), "伪装者阵营剩余 ") + Impostors}" +
                    $" | {cs(getTeamColor(RoleType.Neutral), "中立阵营剩余 ") + Neutrals}";

            }
        }

        if (Prosecutor.ProsecuteThisMeeting && player != null) __instance.completeString += " (被起诉)";
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

    private static void WrapUpPostfix(GameData.PlayerInfo exiled)
    {
        Message("WrapUp Postfix");
        if (PlayerControl.LocalPlayer.IsDead()) CanSeeRoleInfo = true;
        // Prosecutor win condition
        if (exiled != null && Executioner.executioner != null && Executioner.target != null &&
            Executioner.target.PlayerId == exiled.PlayerId && !Executioner.executioner.Data.IsDead)
        {
            Executioner.triggerExecutionerWin = true;
            return;
        }
        // Mini exile lose condition
        else if (exiled != null && Mini.mini != null && Mini.mini.PlayerId == exiled.PlayerId && !Mini.isGrownUp() &&
                 !Mini.mini.Data.Role.IsImpostor && !Mini.mini.IsNeutral())
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
        else if (Executioner.executioner != null && Executioner.executioner == PlayerControl.LocalPlayer && Executioner.target.IsDead())
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.ExecutionerPromotesRole, SendOption.Reliable);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            Executioner.PromotesRole();
        }
        if (Witness.target != null)
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

        if (Specter.Player == PlayerControl.LocalPlayer) Specter.remember = true;

        // Reset custom button timers where necessary
        CustomButton.MeetingEndedUpdate();

        // Clear all traps
        KillTrap.clearAllTraps();
        EvilTrapper.meetingFlag = false;
        Balancer.WrapUp(exiled == null ? null : exiled.Object);
        // Mini set adapted cooldown
        if (Mini.mini != null && PlayerControl.LocalPlayer == Mini.mini && Mini.mini.Data.Role.IsImpostor)
        {
            var multiplier = Mini.isGrownUp() ? 0.66f : 2f;
            Mini.mini.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown * multiplier);
        }

        // Seer spawn souls
        if (Seer.deadBodyPositions != null && Seer.seer != null &&
            PlayerControl.LocalPlayer == Seer.seer && (Seer.mode == 0 || Seer.mode == 2))
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
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.UnblackmailPlayer, SendOption.Reliable);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.unblackmailPlayer();
        }

        // Arsonist deactivate dead poolable players
        if (Arsonist.arsonist != null && Arsonist.arsonist == PlayerControl.LocalPlayer)
        {
            var visibleCounter = 0;
            var newBottomLeft = IntroCutsceneOnDestroyPatch.bottomLeft;
            var BottomLeft = newBottomLeft + new Vector3(-0.25f, -0.25f, 0);
            foreach (PlayerControl p in PlayerControl.AllPlayerControls)
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
        Sheriff.deputyCheckPromotion(true);

        // Force Bounty Hunter Bounty Update
        if (BountyHunter.bountyHunter != null && BountyHunter.bountyHunter == PlayerControl.LocalPlayer)
            BountyHunter.bountyUpdateTimer = 0f;

        if (Prosecutor.prosecutor != null && Prosecutor.ProsecuteThisMeeting)
        {
            if (exiled?.Object.IsCrew() == true && Prosecutor.diesOnIncorrectPros)
            {
                Prosecutor.prosecutor.Exiled();
            }

            if (exiled == null) Prosecutor.Prosecuted = false;
            Prosecutor.ProsecuteThisMeeting = false;
        }

        // Eraser erase
        if (Eraser.eraser != null)
        {
            var rasePlayerList = new List<PlayerControl>(Eraser.futureErased);
            foreach (var target in rasePlayerList)
            {
                if (target?.Data == null) continue;
                RPCProcedure.erasePlayerRoles(target.PlayerId);
                Eraser.alreadyErased.Add(target.PlayerId);
            }
        }

        if (AmongUsClient.Instance.AmHost)
        {
            // Shifter shift
            if (Shifter.shifter != null && Shifter.futureShift != null)
            {
                var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.ShifterShift);
                writer.Write(Shifter.futureShift.PlayerId);
                writer.EndRPC();
                RPCProcedure.shifterShift(Shifter.futureShift.PlayerId);
            }

            // Witch execute casted spells
            if (Witch.witch != null && Witch.futureSpelled != null)
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
                        var writer2 = StartRPC(PlayerControl.LocalPlayer, CustomRPC.LawyerPromotesToPursuer);
                        writer2.EndRPC();
                        Lawyer.PromotesToPursuer();
                    }

                    if (Executioner.executioner.IsAlive() && target == Executioner.target)
                    {
                        var writer2 = StartRPC(PlayerControl.LocalPlayer, CustomRPC.ExecutionerPromotesRole);
                        writer2.EndRPC();
                        Executioner.PromotesRole();
                    }

                    var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.UncheckedExilePlayer);
                    writer.Write(target.PlayerId);
                    writer.EndRPC();
                    RPCProcedure.uncheckedExilePlayer(target.PlayerId);

                    GameHistory.RpcOverrideDeathReasonAndKiller(target, CustomDeathReason.WitchExile, Witch.witch);
                }
            }
        }

        Eraser.futureErased = new List<PlayerControl>();
        Shifter.futureShift = null;
        Witch.futureSpelled = new List<PlayerControl>();

        if (Specter.Player != null && Specter.Player?.Data?.IsDead == true && Specter.exiledBeginRevive)
        {
            Specter.Player.Revive();
            Specter.exiledBeginRevive = false;
        }

        // Medium spawn souls
        if (Medium.medium != null && PlayerControl.LocalPlayer == Medium.medium)
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

        if (InfoSleuth.infoSleuth != null && InfoSleuth.target != null && InfoSleuth.infoSleuth == PlayerControl.LocalPlayer)
        {
            var isNotCrew = (InfoSleuth.target.IsNeutral() || InfoSleuth.target.IsImpostor()) ^ Vortox.Reversal;
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

            var writer1 = StartRPC(PlayerControl.LocalPlayer, CustomRPC.InfoSleuthSetTarget);
            writer1.Write(byte.MaxValue);
            writer1.EndRPC();
            RPCProcedure.infoSleuthSetTarget(byte.MaxValue);

            static string getTeam(PlayerControl player)
            {
                if (Vortox.Player.IsAlive() && Vortox.Reversal)
                {
                    if (player.IsCrew()) return rnd.Next(2) == 0 ? "NeutralRolesText".Translate() : "ImpostorRolesText".Translate();
                    if (player.IsNeutral() || player.IsImpostor()) return "CrewmateRolesText".Translate();
                }

                return player.IsNeutral() ? "NeutralRolesText".Translate()
                    : player.IsImpostor() ? "ImpostorRolesText".Translate()
                    : "CrewmateRolesText".Translate();
            }
        }

        Chameleon.lastMoved.Clear();

        foreach (var trap in Trap.traps) trap.triggerable = false;
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(
            (GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown / 2) + 2, new Action<float>(p =>
            { if (p == 1f) foreach (var trap in Trap.traps) trap.triggerable = true; })));

        if (!Yoyo.markStaysOverMeeting) Silhouette.clearSilhouettes();

        // AntiTeleport set position
        AntiTeleport.setPosition();

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
                Object.Destroy(__instance.gameObject);
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
        AntiTeleport.setPosition();
        Chameleon.lastMoved.Clear();
    }
}