using System;
using System.Collections.Generic;
using System.Linq;
using Hazel;
using PowerTools;
using TheOtherRoles.Objects;
using TheOtherRoles.Utilities;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;
using Object = UnityEngine.Object;

namespace TheOtherRoles.Patches;

[HarmonyPatch(typeof(ExileController), nameof(ExileController.Begin))]
[HarmonyPriority(Priority.First)]
internal class ExileControllerBeginPatch
{
    public static GameData.PlayerInfo lastExiled;

    public static void Prefix(ExileController __instance, [HarmonyArgument(0)] ref GameData.PlayerInfo exiled,
        [HarmonyArgument(1)] bool tie)
    {
        lastExiled = exiled;

        // Medic shield
        if (Medic.medic != null && AmongUsClient.Instance.AmHost && Medic.futureShielded != null &&
            !Medic.medic.Data.IsDead)
        {
            // We need to send the RPC from the host here, to make sure that the order of shifting and setting the shield is correct(for that reason the futureShifted and futureShielded are being synced)
            var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                (byte)CustomRPC.MedicSetShielded, SendOption.Reliable);
            writer.Write(Medic.futureShielded.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.medicSetShielded(Medic.futureShielded.PlayerId);
        }

        if (Medic.usedShield) Medic.meetingAfterShielding = true; // Has to be after the setting of the shield

        // Shifter shift
        if (Shifter.shifter != null && AmongUsClient.Instance.AmHost && Shifter.futureShift != null)
        {
            // We need to send the RPC from the host here, to make sure that the order of shifting and erasing is correct (for that reason the futureShifted and futureErased are being synced)
            var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                (byte)CustomRPC.ShifterShift, SendOption.Reliable);
            writer.Write(Shifter.futureShift.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.shifterShift(Shifter.futureShift.PlayerId);
        }

        Shifter.futureShift = null;
        //末日预言家
        if (Doomsayer.doomsayer != null && AmongUsClient.Instance.AmHost && !Doomsayer.canGuess)
            Doomsayer.canGuess = true;

        // Eraser erase
        if (Eraser.eraser != null && AmongUsClient.Instance.AmHost && Eraser.futureErased != null)
            // We need to send the RPC from the host here, to make sure that the order of shifting and erasing is correct (for that reason the futureShifted and futureErased are being synced)
            foreach (var target in Eraser.futureErased)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                    (byte)CustomRPC.ErasePlayerRoles, SendOption.Reliable);
                writer.Write(target.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.erasePlayerRoles(target.PlayerId);
                Eraser.alreadyErased.Add(target.PlayerId);
            }

        Eraser.futureErased = new List<PlayerControl>();

        // Trickster boxes
        if (Trickster.trickster != null && JackInTheBox.hasJackInTheBoxLimitReached()) JackInTheBox.convertToVents();

        // Activate portals.
        Portal.meetingEndsUpdate();

        // Witch execute casted spells
        if (Witch.witch != null && Witch.futureSpelled != null && AmongUsClient.Instance.AmHost)
        {
            var exiledIsWitch = exiled != null && exiled.PlayerId == Witch.witch.PlayerId;
            var witchDiesWithExiledLover = exiled != null && Lovers.existing() && Lovers.bothDie &&
                                           (Lovers.lover1.PlayerId == Witch.witch.PlayerId ||
                                            Lovers.lover2.PlayerId == Witch.witch.PlayerId) &&
                                           (exiled.PlayerId == Lovers.lover1.PlayerId ||
                                            exiled.PlayerId == Lovers.lover2.PlayerId);

            if ((witchDiesWithExiledLover || exiledIsWitch) && Witch.witchVoteSavesTargets)
                Witch.futureSpelled = new List<PlayerControl>();
            foreach (var target in Witch.futureSpelled)
                if (target != null && !target.Data.IsDead && Helpers.checkMuderAttempt(Witch.witch, target, true) ==
                    MurderAttemptResult.PerformKill)
                {
                    if (exiled != null && Lawyer.lawyer != null &&
                        (target == Lawyer.lawyer || target == Lovers.otherLover(Lawyer.lawyer)) &&
                        Lawyer.target != null && Lawyer.isProsecutor && Lawyer.target.PlayerId == exiled.PlayerId)
                        continue;
                    if (target == Lawyer.target && Lawyer.lawyer != null)
                    {
                        var writer2 = AmongUsClient.Instance.StartRpcImmediately(
                            CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.LawyerPromotesToPursuer,
                            SendOption.Reliable);
                        AmongUsClient.Instance.FinishRpcImmediately(writer2);
                        RPCProcedure.lawyerPromotesToPursuer();
                    }

                    var writer = AmongUsClient.Instance.StartRpcImmediately(
                        CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.UncheckedExilePlayer,
                        SendOption.Reliable);
                    writer.Write(target.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.uncheckedExilePlayer(target.PlayerId);

                    var writer3 = AmongUsClient.Instance.StartRpcImmediately(
                        CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.ShareGhostInfo,
                        SendOption.Reliable);
                    writer3.Write(CachedPlayer.LocalPlayer.PlayerId);
                    writer3.Write((byte)RPCProcedure.GhostInfoTypes.DeathReasonAndKiller);
                    writer3.Write(target.PlayerId);
                    writer3.Write((byte)DeadPlayer.CustomDeathReason.WitchExile);
                    writer3.Write(Witch.witch.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer3);

                    GameHistory.overrideDeathReasonAndKiller(target, DeadPlayer.CustomDeathReason.WitchExile,
                        Witch.witch);
                }
        }

        Witch.futureSpelled = new List<PlayerControl>();

        // SecurityGuard vents and cameras
        var allCameras = MapUtilities.CachedShipStatus.AllCameras.ToList();
        MapOptions.camerasToAdd.ForEach(camera =>
        {
            camera.gameObject.SetActive(true);
            camera.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            allCameras.Add(camera);
        });
        MapUtilities.CachedShipStatus.AllCameras = allCameras.ToArray();
        MapOptions.camerasToAdd = new List<SurvCamera>();

        foreach (var vent in MapOptions.ventsToSeal)
        {
            var animator = vent.GetComponent<SpriteAnim>();
            vent.EnterVentAnim = vent.ExitVentAnim = null;
            var newSprite = animator == null
                ? SecurityGuard.getStaticVentSealedSprite()
                : SecurityGuard.getAnimatedVentSealedSprite();
            var rend = vent.myRend;
            if (Helpers.isFungle())
            {
                newSprite = SecurityGuard.getFungleVentSealedSprite();
                rend = vent.transform.GetChild(3).GetComponent<SpriteRenderer>();
                animator = vent.transform.GetChild(3).GetComponent<SpriteAnim>();
            }

            animator?.Stop();
            rend.sprite = newSprite;
            if (SubmergedCompatibility.IsSubmerged && vent.Id == 0) vent.myRend.sprite = SecurityGuard.getSubmergedCentralUpperSealedSprite();
            if (SubmergedCompatibility.IsSubmerged && vent.Id == 14) vent.myRend.sprite = SecurityGuard.getSubmergedCentralLowerSealedSprite();
            rend.color = Color.white;
            vent.name = "SealedVent_" + vent.name;
        }

        MapOptions.ventsToSeal = new List<Vent>();
        // 1 = reset per turn
        if (MapOptions.restrictDevices == 1)
            MapOptions.resetDeviceTimes();
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
        //if (!SubmergedCompatibility.IsSubmerged) return;
        if (obj.name.Contains("ExileCutscene"))
        {
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
        // Prosecutor win condition
        if (exiled != null && Lawyer.lawyer != null && Lawyer.target != null && Lawyer.isProsecutor &&
            Lawyer.target.PlayerId == exiled.PlayerId && !Lawyer.lawyer.Data.IsDead)
            Lawyer.triggerProsecutorWin = true;

        // Mini exile lose condition
        else if (exiled != null && Mini.mini != null && Mini.mini.PlayerId == exiled.PlayerId && !Mini.isGrownUp() &&
                 !Mini.mini.Data.Role.IsImpostor && !RoleInfo.getRoleInfoForPlayer(Mini.mini).Any(x => x.isNeutral))
            Mini.triggerMiniLose = true;
        // Jester win condition
        else if (exiled != null && Jester.jester != null && Jester.jester.PlayerId == exiled.PlayerId)
            Jester.triggerJesterWin = true;


        // Reset custom button timers where necessary
        CustomButton.MeetingEndedUpdate();

        // Clear all traps
        KillTrap.clearAllTraps();
        EvilTrapper.meetingFlag = false;

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
                //soul.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
                rend.sprite = Seer.getSoulSprite();

                if (Seer.limitSoulDuration)
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
                if (!MapOptions.playerIcons.ContainsKey(p.PlayerId)) continue;
                if (p.Data.IsDead || p.Data.Disconnected)
                {
                    MapOptions.playerIcons[p.PlayerId].gameObject.SetActive(false);
                }
                else
                {
                    MapOptions.playerIcons[p.PlayerId].transform.localPosition =
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
                    //s.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
                    rend.sprite = Medium.getSoulSprite();
                    Medium.souls.Add(rend);
                }

                Medium.deadBodies = Medium.futureDeadBodies;
                Medium.futureDeadBodies = new List<Tuple<DeadPlayer, Vector3>>();
            }
        }

        // AntiTeleport set position
        AntiTeleport.setPosition();

        if (CustomOptionHolder.randomGameStartPosition.getBool() && AntiTeleport.antiTeleport
                .FindAll(x => x.PlayerId == CachedPlayer.LocalPlayer.PlayerControl.PlayerId).Count == 0)
        {
            //Random spawn on round start

            if (CustomOptionHolder.randomGameStartToVents.getBool())
            {
                CachedPlayer.LocalPlayer.PlayerControl.NetTransform.RpcSnapTo
                    (MapData.FindVentSpawnPositions()[rnd.Next(MapData.FindVentSpawnPositions().Count)]);
            }
            else
            {
                var SpawnPositions =
                    GameOptionsManager.Instance.currentNormalGameOptions.MapId switch
                    {
                        0 => MapData.SkeldSpawnPosition,
                        1 => MapData.MiraSpawnPosition,
                        2 => MapData.PolusSpawnPosition,
                        3 => MapData.DleksSpawnPosition,
                        4 => MapData.AirshipSpawnPosition,
                        5 => MapData.FungleSpawnPosition,
                        _ => MapData.FindVentSpawnPositions()
                    };
                CachedPlayer.LocalPlayer.PlayerControl.NetTransform.RpcSnapTo
                    (SpawnPositions[rnd.Next(SpawnPositions.Count)]);
            }
        }

        // Invert add meeting
        if (Invert.meetings > 0) Invert.meetings--;

        Chameleon.lastMoved.Clear();

        foreach (var trap in Trap.traps) trap.triggerable = false;
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(
            (GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown / 2) + 2, new Action<float>(p =>
            { if (p == 1f) foreach (var trap in Trap.traps) trap.triggerable = true; })));

        if (!Yoyo.markStaysOverMeeting)
            Silhouette.clearSilhouettes();
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    private class BaseExileControllerPatch
    {
        public static void Postfix(ExileController __instance)
        {
            WrapUpPostfix(__instance.exiled);
        }
    }

    [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
    private class AirshipExileControllerPatch
    {
        public static void Postfix(AirshipExileController __instance)
        {
            WrapUpPostfix(__instance.exiled);
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
            if (ExileController.Instance != null && ExileController.Instance.exiled != null)
            {
                var player = Helpers.playerById(ExileController.Instance.exiled.Object.PlayerId);
                if (player == null) return;
                // Exile role text
                if (id == StringNames.ExileTextPN || id == StringNames.ExileTextSN || id == StringNames.ExileTextPP ||
                    id == StringNames.ExileTextSP)
                    __result = player.Data.PlayerName + " was The " + string.Join(" ",
                        RoleInfo.getRoleInfoForPlayer(player, false).Select(x => x.name).ToArray());
                // Hide number of remaining impostors on Jester win
                if (id == StringNames.ImpostorsRemainP || id == StringNames.ImpostorsRemainS)
                    if (Jester.jester != null && player.PlayerId == Jester.jester.PlayerId)
                        __result = "";
                if (Tiebreaker.isTiebreak) __result += " (破平)";
                Tiebreaker.isTiebreak = false;
            }
        }
        catch
        {
            // pass - Hopefully prevent leaving while exiling to softlock game
        }
    }
}