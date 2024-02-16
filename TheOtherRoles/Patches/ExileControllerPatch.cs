using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Hazel;
using PowerTools;
using TheOtherRoles.Helper;
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
        //???????
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
        TORMapOptions.camerasToAdd.ForEach(camera =>
        {
            camera.gameObject.SetActive(true);
            camera.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            allCameras.Add(camera);
        });
        MapUtilities.CachedShipStatus.AllCameras = allCameras.ToArray();
        TORMapOptions.camerasToAdd = new List<SurvCamera>();

        foreach (var vent in TORMapOptions.ventsToSeal)
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
            if (SubmergedCompatibility.IsSubmerged && vent.Id == 0)
                vent.myRend.sprite = SecurityGuard.getSubmergedCentralUpperSealedSprite();
            if (SubmergedCompatibility.IsSubmerged && vent.Id == 14)
                vent.myRend.sprite = SecurityGuard.getSubmergedCentralLowerSealedSprite();
            rend.color = Color.white;
            vent.name = "SealedVent_" + vent.name;
        }

        TORMapOptions.ventsToSeal = new List<Vent>();
        // 1 = reset per turn
        if (TORMapOptions.restrictDevices == 1)
            TORMapOptions.resetDeviceTimes();

        EventUtility.meetingEndsUpdate();
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
                if (!TORMapOptions.playerIcons.ContainsKey(p.PlayerId)) continue;
                if (p.Data.IsDead || p.Data.Disconnected)
                {
                    TORMapOptions.playerIcons[p.PlayerId].gameObject.SetActive(false);
                }
                else
                {
                    TORMapOptions.playerIcons[p.PlayerId].transform.localPosition =
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
                    s.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
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

            var skeldSpawn = new List<Vector3>
            {
                new(-2.2f, 2.2f, 0.0f), //cafeteria. botton. top left.
                new(0.7f, 2.2f, 0.0f), //caffeteria. button. top right.
                new(-2.2f, -0.2f, 0.0f), //caffeteria. button. bottom left.
                new(0.7f, -0.2f, 0.0f), //caffeteria. button. bottom right.
                new(10.0f, 3.0f, 0.0f), //weapons top
                new(9.0f, 1.0f, 0.0f), //weapons bottom
                new(6.5f, -3.5f, 0.0f), //O2
                new(11.5f, -3.5f, 0.0f), //O2-nav hall
                new(17.0f, -3.5f, 0.0f), //navigation top
                new(18.2f, -5.7f, 0.0f), //navigation bottom
                new(11.5f, -6.5f, 0.0f), //nav-shields top
                new(9.5f, -8.5f, 0.0f), //nav-shields bottom
                new(9.2f, -12.2f, 0.0f), //shields top
                new(8.0f, -14.3f, 0.0f), //shields bottom
                new(2.5f, -16f, 0.0f), //coms left
                new(4.2f, -16.4f, 0.0f), //coms middle
                new(5.5f, -16f, 0.0f), //coms right
                new(-1.5f, -10.0f, 0.0f), //storage top
                new(-1.5f, -15.5f, 0.0f), //storage bottom
                new(-4.5f, -12.5f, 0.0f), //storrage left
                new(0.3f, -12.5f, 0.0f), //storrage right
                new(4.5f, -7.5f, 0.0f), //admin top
                new(4.5f, -9.5f, 0.0f), //admin bottom
                new(-9.0f, -8.0f, 0.0f), //elec top left
                new(-6.0f, -8.0f, 0.0f), //elec top right
                new(-8.0f, -11.0f, 0.0f), //elec bottom
                new(-12.0f, -13.0f, 0.0f), //elec-lower hall
                new(-17f, -10f, 0.0f), //lower engine top
                new(-17.0f, -13.0f, 0.0f), //lower engine bottom
                new(-21.5f, -3.0f, 0.0f), //reactor top
                new(-21.5f, -8.0f, 0.0f), //reactor bottom
                new(-13.0f, -3.0f, 0.0f), //security top
                new(-12.6f, -5.6f, 0.0f), // security bottom
                new(-17.0f, 2.5f, 0.0f), //upper engibe top
                new(-17.0f, -1.0f, 0.0f), //upper engine bottom
                new(-10.5f, 1.0f, 0.0f), //upper-mad hall
                new(-10.5f, -2.0f, 0.0f), //medbay top
                new(-6.5f, -4.5f, 0.0f) //medbay bottom
            };

            var miraSpawn = new List<Vector3>
            {
                new(-4.5f, 3.5f, 0.0f), //launchpad top
                new(-4.5f, -1.4f, 0.0f), //launchpad bottom
                new(8.5f, -1f, 0.0f), //launchpad- med hall
                new(14f, -1.5f, 0.0f), //medbay
                new(16.5f, 3f, 0.0f), // comms
                new(10f, 5f, 0.0f), //lockers
                new(6f, 1.5f, 0.0f), //locker room
                new(2.5f, 13.6f, 0.0f), //reactor
                new(6f, 12f, 0.0f), //reactor middle
                new(9.5f, 13f, 0.0f), //lab
                new(15f, 9f, 0.0f), //bottom left cross
                new(17.9f, 11.5f, 0.0f), //middle cross
                new(14f, 17.3f, 0.0f), //office
                new(19.5f, 21f, 0.0f), //admin
                new(14f, 24f, 0.0f), //greenhouse left
                new(22f, 24f, 0.0f), //greenhouse right
                new(21f, 8.5f, 0.0f), //bottom right cross
                new(28f, 3f, 0.0f), //caf right
                new(22f, 3f, 0.0f), //caf left
                new(19f, 4f, 0.0f), //storage
                new(22f, -2f, 0.0f) //balcony
            };

            var polusSpawn = new List<Vector3>
            {
                new(16.6f, -1f, 0.0f), //dropship top
                new(16.6f, -5f, 0.0f), //dropship bottom
                new(20f, -9f, 0.0f), //above storrage
                new(22f, -7f, 0.0f), //right fuel
                new(25.5f, -6.9f, 0.0f), //drill
                new(29f, -9.5f, 0.0f), //lab lockers
                new(29.5f, -8f, 0.0f), //lab weather notes
                new(35f, -7.6f, 0.0f), //lab table
                new(40.4f, -8f, 0.0f), //lab scan
                new(33f, -10f, 0.0f), //lab toilet
                new(39f, -15f, 0.0f), //specimen hall top
                new(36.5f, -19.5f, 0.0f), //specimen top
                new(36.5f, -21f, 0.0f), //specimen bottom
                new(28f, -21f, 0.0f), //specimen hall bottom
                new(24f, -20.5f, 0.0f), //admin tv
                new(22f, -25f, 0.0f), //admin books
                new(16.6f, -17.5f, 0.0f), //office coffe
                new(22.5f, -16.5f, 0.0f), //office projector
                new(24f, -17f, 0.0f), //office figure
                new(27f, -16.5f, 0.0f), //office lifelines
                new(32.7f, -15.7f, 0.0f), //lavapool
                new(31.5f, -12f, 0.0f), //snowmad below lab
                new(10f, -14f, 0.0f), //below storrage
                new(21.5f, -12.5f, 0.0f), //storrage vent
                new(19f, -11f, 0.0f), //storrage toolrack
                new(12f, -7.2f, 0.0f), //left fuel
                new(5f, -7.5f, 0.0f), //above elec
                new(10f, -12f, 0.0f), //elec fence
                new(9f, -9f, 0.0f), //elec lockers
                new(5f, -9f, 0.0f), //elec window
                new(4f, -11.2f, 0.0f), //elec tapes
                new(5.5f, -16f, 0.0f), //elec-O2 hall
                new(1f, -17.5f, 0.0f), //O2 tree hayball
                new(3f, -21f, 0.0f), //O2 middle
                new(2f, -19f, 0.0f), //O2 gas
                new(1f, -24f, 0.0f), //O2 water
                new(7f, -24f, 0.0f), //under O2
                new(9f, -20f, 0.0f), //right outside of O2
                new(7f, -15.8f, 0.0f), //snowman under elec
                new(11f, -17f, 0.0f), //comms table
                new(12.7f, -15.5f, 0.0f), //coms antenna pult
                new(13f, -24.5f, 0.0f), //weapons window
                new(15f, -17f, 0.0f), //between coms-office
                new(17.5f, -25.7f, 0.0f) //snowman under office
            };

            var dleksSpawn = new List<Vector3>
            {
                new(2.2f, 2.2f, 0.0f), //cafeteria. botton. top left.
                new(-0.7f, 2.2f, 0.0f), //caffeteria. button. top right.
                new(2.2f, -0.2f, 0.0f), //caffeteria. button. bottom left.
                new(-0.7f, -0.2f, 0.0f), //caffeteria. button. bottom right.
                new(-10.0f, 3.0f, 0.0f), //weapons top
                new(-9.0f, 1.0f, 0.0f), //weapons bottom
                new(-6.5f, -3.5f, 0.0f), //O2
                new(-11.5f, -3.5f, 0.0f), //O2-nav hall
                new(-17.0f, -3.5f, 0.0f), //navigation top
                new(-18.2f, -5.7f, 0.0f), //navigation bottom
                new(-11.5f, -6.5f, 0.0f), //nav-shields top
                new(-9.5f, -8.5f, 0.0f), //nav-shields bottom
                new(-9.2f, -12.2f, 0.0f), //shields top
                new(-8.0f, -14.3f, 0.0f), //shields bottom
                new(-2.5f, -16f, 0.0f), //coms left
                new(-4.2f, -16.4f, 0.0f), //coms middle
                new(-5.5f, -16f, 0.0f), //coms right
                new(1.5f, -10.0f, 0.0f), //storage top
                new(1.5f, -15.5f, 0.0f), //storage bottom
                new(4.5f, -12.5f, 0.0f), //storrage left
                new(-0.3f, -12.5f, 0.0f), //storrage right
                new(-4.5f, -7.5f, 0.0f), //admin top
                new(-4.5f, -9.5f, 0.0f), //admin bottom
                new(9.0f, -8.0f, 0.0f), //elec top left
                new(6.0f, -8.0f, 0.0f), //elec top right
                new(8.0f, -11.0f, 0.0f), //elec bottom
                new(12.0f, -13.0f, 0.0f), //elec-lower hall
                new(17f, -10f, 0.0f), //lower engine top
                new(17.0f, -13.0f, 0.0f), //lower engine bottom
                new(21.5f, -3.0f, 0.0f), //reactor top
                new(21.5f, -8.0f, 0.0f), //reactor bottom
                new(13.0f, -3.0f, 0.0f), //security top
                new(12.6f, -5.6f, 0.0f), // security bottom
                new(17.0f, 2.5f, 0.0f), //upper engibe top
                new(17.0f, -1.0f, 0.0f), //upper engine bottom
                new(10.5f, 1.0f, 0.0f), //upper-mad hall
                new(10.5f, -2.0f, 0.0f), //medbay top
                new(6.5f, -4.5f, 0.0f) //medbay bottom
            };

            var fungleSpawn = new List<Vector3>
            {
                new(-10.0842f, 13.0026f, 0.013f),
                new(0.9815f, 6.7968f, 0.0068f),
                new(22.5621f, 3.2779f, 0.0033f),
                new(-1.8699f, -1.3406f, -0.0013f),
                new(12.0036f, 2.6763f, 0.0027f),
                new(21.705f, -7.8691f, -0.0079f),
                new(1.4485f, -1.6105f, -0.0016f),
                new(-4.0766f, -8.7178f, -0.0087f),
                new(2.9486f, 1.1347f, 0.0011f),
                new(-4.2181f, -8.6795f, -0.0087f),
                new(19.5553f, -12.5014f, -0.0125f),
                new(15.2497f, -16.5009f, -0.0165f),
                new(-22.7174f, -7.0523f, 0.0071f),
                new(-16.5819f, -2.1575f, 0.0022f),
                new(9.399f, -9.7127f, -0.0097f),
                new(7.3723f, 1.7373f, 0.0017f),
                new(22.0777f, -7.9315f, -0.0079f),
                new(-15.3916f, -9.3659f, -0.0094f),
                new(-16.1207f, -0.1746f, -0.0002f),
                new(-23.1353f, -7.2472f, -0.0072f),
                new(-20.0692f, -2.6245f, -0.0026f),
                new(-4.2181f, -8.6795f, -0.0087f),
                new(-9.9285f, 12.9848f, 0.013f),
                new(-8.3475f, 1.6215f, 0.0016f),
                new(-17.7614f, 6.9115f, 0.0069f),
                new(-0.5743f, -4.7235f, -0.0047f),
                new(-20.8897f, 2.7606f, 0.002f)
            };

            var airshipSpawn = new List<Vector3>(); //no spawns since it already has random spawns

            if (GameOptionsManager.Instance.currentNormalGameOptions.MapId == 0)
                CachedPlayer.LocalPlayer.PlayerControl.transform.position = skeldSpawn[rnd.Next(skeldSpawn.Count)];
            if (GameOptionsManager.Instance.currentNormalGameOptions.MapId == 1)
                CachedPlayer.LocalPlayer.PlayerControl.transform.position = miraSpawn[rnd.Next(miraSpawn.Count)];
            if (GameOptionsManager.Instance.currentNormalGameOptions.MapId == 2)
                CachedPlayer.LocalPlayer.PlayerControl.transform.position = polusSpawn[rnd.Next(polusSpawn.Count)];
            if (GameOptionsManager.Instance.currentNormalGameOptions.MapId == 3)
                CachedPlayer.LocalPlayer.PlayerControl.transform.position = dleksSpawn[rnd.Next(dleksSpawn.Count)];
            if (GameOptionsManager.Instance.currentNormalGameOptions.MapId == 4)
                CachedPlayer.LocalPlayer.PlayerControl.transform.position = airshipSpawn[rnd.Next(airshipSpawn.Count)];
            if (GameOptionsManager.Instance.currentNormalGameOptions.MapId == 5)
                CachedPlayer.LocalPlayer.PlayerControl.transform.position = fungleSpawn[rnd.Next(fungleSpawn.Count)];
        }

        // Invert add meeting
        if (Invert.meetings > 0) Invert.meetings--;

        Chameleon.lastMoved.Clear();

        foreach (var trap in Trap.traps) trap.triggerable = false;
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(
            (GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown / 2) + 2, new Action<float>(p =>
            {
                if (p == 1f)
                    foreach (var trap in Trap.traps)
                        trap.triggerable = true;
            })));
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
                if (Tiebreaker.isTiebreak) __result += " (???)";
                Tiebreaker.isTiebreak = false;
            }
        }
        catch
        {
            // pass - Hopefully prevent leaving while exiling to softlock game
        }
    }
}