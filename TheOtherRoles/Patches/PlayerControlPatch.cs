using System;
using System.Collections.Generic;
using System.Linq;
using AmongUs.GameOptions;
using Assets.CoreScripts;
using Hazel;
using InnerNet;
using Reactor.Utilities.Extensions;
using TheOtherRoles.Buttons;
using TheOtherRoles.Objects;
using TheOtherRoles.Utilities;
using TMPro;
using UnityEngine;
using static TheOtherRoles.GameHistory;
using Object = UnityEngine.Object;

namespace TheOtherRoles.Patches;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
public static class PlayerControlFixedUpdatePatch
{
    private static bool mushroomSaboWasActive;
    // Helpers

    public static PlayerControl SetTarget(bool onlyCrewmates = false, bool targetPlayersInVents = false,
        List<PlayerControl> untargetablePlayers = null, PlayerControl targetingPlayer = null, float KillDistances = 0f)
    {
        PlayerControl result = null;
        var num = GameOptionsData.KillDistances[Mathf.Clamp(GameOptionsManager.Instance.currentNormalGameOptions.KillDistance, 0, 3)];
        if (!MapUtilities.CachedShipStatus) return result;
        if (targetingPlayer == null) targetingPlayer = PlayerControl.LocalPlayer;
        if (targetingPlayer.Data.IsDead) return result;
        num += KillDistances;

        var truePosition = targetingPlayer.GetTruePosition();
        foreach (var playerInfo in GameData.Instance.AllPlayers.GetFastEnumerator())
            if (!playerInfo.Disconnected && playerInfo.PlayerId != targetingPlayer.PlayerId && !playerInfo.IsDead &&
                (!onlyCrewmates || !playerInfo.Role.IsImpostor))
            {
                var @object = playerInfo.Object;
                if (untargetablePlayers != null && untargetablePlayers.Any(x => x == @object))
                    // if that player is not targetable: skip check
                    continue;

                if (@object && (!@object.inVent || targetPlayersInVents))
                {
                    var vector = @object.GetTruePosition() - truePosition;
                    var magnitude = vector.magnitude;
                    if (magnitude <= num && !PhysicsHelpers.AnyNonTriggersBetween(truePosition, vector.normalized,
                            magnitude, Constants.ShipAndObjectsMask))
                    {
                        result = @object;
                        num = magnitude;
                    }
                }
            }

        return result;
    }

    public static void SetPlayerOutline(PlayerControl target, Color color)
    {
        if (target == null || target.cosmetics?.currentBodySprite?.BodySprite == null) return;

        color = color.SetAlpha(Chameleon.visibility(target.PlayerId));

        target.cosmetics.currentBodySprite.BodySprite.material.SetFloat("_Outline", 1f);
        target.cosmetics.currentBodySprite.BodySprite.material.SetColor("_OutlineColor", color);
    }

    // Update functions

    private static void setBasePlayerOutlines()
    {
        var local = PlayerControl.LocalPlayer;
        foreach (PlayerControl target in PlayerControl.AllPlayerControls)
        {
            if (target == null || target.cosmetics?.currentBodySprite?.BodySprite == null) continue;

            var isMorphedMorphling = target == Morphling.morphling && Morphling.morphTarget != null && Morphling.morphTimer > 0f;
            var hasVisibleShield = false;
            var color = Medic.shieldedColor;
            if (!isCamoComms && Camouflager.camouflageTimer <= 0f && !MushroomSabotageActive &&
                Medic.shielded != null && ((target == Medic.shielded && !isMorphedMorphling) ||
                (isMorphedMorphling && Morphling.morphTarget == Medic.shielded)))
            {
                hasVisibleShield = Medic.showShielded == 0 || shouldShowGhostInfo() // Everyone or Ghost info
                    || (Medic.showShielded == 1 && (local == Medic.shielded || local == Medic.medic)) // Shielded + Medic
                    || (Medic.showShielded == 2 && local == Medic.medic); // Medic only

                // Make shield invisible till after the next meeting if the option is set (the medic can already see the shield)
                hasVisibleShield = hasVisibleShield && (Medic.meetingAfterShielding || !Medic.showShieldAfterMeeting ||
                    local == Medic.medic || shouldShowGhostInfo());
            }

            if (BodyGuard.guarded.IsAlive() && target == BodyGuard.guarded &&
                (shouldShowGhostInfo() || local == BodyGuard.bodyguard || (local == BodyGuard.guarded && BodyGuard.showShielded)))
            {
                hasVisibleShield = true;
                color = new Color32(205, 150, 100, byte.MaxValue);
            }

            if (!isCamoComms && Camouflager.camouflageTimer <= 0f && !MushroomSabotageActive &&
                ModOption.firstKillPlayer != null && ModOption.shieldFirstKill &&
                ((target == ModOption.firstKillPlayer && !isMorphedMorphling) ||
                 (isMorphedMorphling && Morphling.morphTarget == ModOption.firstKillPlayer)))
            {
                hasVisibleShield = true;
                color = Color.blue;
            }

            if (hasVisibleShield)
            {
                target.cosmetics.currentBodySprite.BodySprite.material.SetFloat("_Outline", 1f);
                target.cosmetics.currentBodySprite.BodySprite.material.SetColor("_OutlineColor", color);
            }
            else
            {
                target.cosmetics.currentBodySprite.BodySprite.material.SetFloat("_Outline", 0f);
            }
        }
    }

    public static void updatePlayerInfo()
    {
        if (!InGame) return;
        var local = PlayerControl.LocalPlayer;
        foreach (PlayerControl p in PlayerControl.AllPlayerControls)
        {
            var playerVoteArea = MeetingHud.Instance?.playerStates?.FirstOrDefault(x => x.TargetPlayerId == p.PlayerId);
            if (playerVoteArea != null && playerVoteArea.ColorBlindName.gameObject.active)
            {
                playerVoteArea.ColorBlindName.transform.localPosition = new Vector3(-0.93f, -0.2f, -0.1f);
                playerVoteArea.ColorBlindName.fontSize *= 1.75f;
            }
            p.cosmetics.nameText.transform.parent.SetLocalZ(-0.0001f);

            bool teamSeeRoles = (Lawyer.lawyerKnowsRole && local == Lawyer.lawyer && p == Lawyer.target) ||
                (PartTimer.knowsRole && local == PartTimer.partTimer && p == PartTimer.target) ||
                (local == PartTimer.target && p == PartTimer.partTimer) ||
                (Akujo.knowsRoles && local == Akujo.akujo &&
                    (p == Akujo.honmei || Akujo.keeps.Any(x => x.PlayerId == p.PlayerId))) ||
                (ModOption.impostorSeeRoles && Spy.spy == null && PlayerControl.LocalPlayer.isImpostor() &&
                    PlayerControl.LocalPlayer.IsAlive() && p.isImpostor() && p.IsAlive());

            bool reported = ((local == Slueth.slueth && Slueth.reported.Any(x => x.PlayerId == p.PlayerId)) ||
                             (local == Poucher.poucher && Poucher.killed.Any(x => x.PlayerId == p.PlayerId))) && p.IsDead();

            bool revealed = (Mayor.mayor == p && Mayor.Revealed) || (WolfLord.Player == p && WolfLord.Revealed);

            if (p == local || local.Data.IsDead || teamSeeRoles || reported || revealed)
            {
                var roleNames = RoleInfo.GetRolesString(p, true, false, false, false);
                var mainRole = RoleInfo.GetRolesString(p, true, false, false, false);
                var allRoleText = RoleInfo.GetRolesString(p, true, true, true, true);

                var playerInfoTransform = p.cosmetics.nameText.transform.parent.FindChild("Info");
                var playerInfo = playerInfoTransform != null ? playerInfoTransform.GetComponent<TextMeshPro>() : null;
                if (playerInfo == null)
                {
                    playerInfo = Object.Instantiate(p.cosmetics.nameText, p.cosmetics.nameText.transform.parent);
                    playerInfo.transform.localPosition += Vector3.up * 0.225f;
                    playerInfo.fontSize *= 0.8f;
                    playerInfo.gameObject.name = "Info";
                    playerInfo.color = playerInfo.color.SetAlpha(1f);
                }

                var meetingInfoTransform = playerVoteArea != null ? playerVoteArea.NameText.transform.parent.FindChild("Info") : null;
                var meetingInfo = meetingInfoTransform != null ? meetingInfoTransform.GetComponent<TextMeshPro>() : null;

                if (meetingInfo == null && playerVoteArea != null)
                {
                    meetingInfo = Object.Instantiate(playerVoteArea.NameText, playerVoteArea.NameText.transform.parent);
                    meetingInfo.transform.localPosition += Vector3.down * 0.2f;
                    meetingInfo.fontSize *= 0.64f;
                    meetingInfo.gameObject.name = "Info";
                }

                // Set player name higher to align in middle
                if (meetingInfo != null && playerVoteArea != null)
                {
                    var playerName = playerVoteArea.NameText;
                    playerName.transform.localPosition = new Vector3(0.3384f, 0.0311f, -0.1f);
                }

                var (tasksCompleted, tasksTotal) = TasksHandler.taskInfo(p.Data);
                var taskInfo = tasksTotal > 0 ? $"<color=#FAD934FF>({tasksCompleted}/{tasksTotal})</color>" : "";

                var playerInfoText = "";
                var meetingInfoText = "";
                if (p == local)
                {
                    if (p.Data.IsDead) roleNames = allRoleText;
                    playerInfoText = $"{roleNames}";
                    if (HudManager.Instance.TaskPanel != null)
                    {
                        var tabText = HudManager.Instance.TaskPanel.tab.transform.FindChild("TabText_TMP").GetComponent<TextMeshPro>();
                        tabText.SetText(string.Format("tasksNum".Translate(), taskInfo));
                    }
                    meetingInfoText = $"{allRoleText} {taskInfo}".Trim();
                }
                else if (local.IsAlive() && Mayor.mayor == p && Mayor.Revealed)
                {
                    meetingInfoText = cs(Mayor.color, "Mayor".Translate());
                }
                else if (local.IsAlive() && WolfLord.Player == p && WolfLord.Revealed)
                {
                    meetingInfoText = cs(WolfLord.color, "WolfLord".Translate());
                }
                else if (teamSeeRoles)
                {
                    meetingInfoText = playerInfoText = roleNames;
                }
                else if (reported)
                {
                    meetingInfoText = playerInfoText = mainRole;
                }
                else
                {
                    if (CanSeeRoleInfo)
                    {
                        playerInfoText = $"{allRoleText} {taskInfo}".Trim();
                        meetingInfoText = playerInfoText;
                    }
                }

                playerInfo.text = playerInfoText;
                playerInfo.gameObject.SetActive(p.Visible);
                if (meetingInfo != null)
                {
                    meetingInfo.text = MeetingHud.Instance.state == MeetingHud.VoteStates.Results ? "" : meetingInfoText;
                }
            }
            else
            {
                if (local != p && p != null)
                {
                    var playerInfoTransform = p.cosmetics.nameText.transform.parent.FindChild("Info");
                    var playerInfo = playerInfoTransform?.GetComponent<TextMeshPro>();
                    if (playerInfo != null) playerInfo.text = "";
                }
            }
        }
    }

    private static void setPetVisibility()
    {
        var localalive = !PlayerControl.LocalPlayer.Data.IsDead;
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            var playeralive = !player.Data.IsDead;
            player.cosmetics.SetPetVisible((localalive && playeralive) || !localalive);
        }
    }

    public static void bendTimeUpdate()
    {
        if (TimeMaster.isRewinding)
        {
            if (localPlayerPositions.Count > 0)
            {
                // Set position
                var next = localPlayerPositions[0];
                if (next.Item2)
                {
                    // Exit current vent if necessary
                    if (PlayerControl.LocalPlayer.inVent)
                        foreach (var vent in MapUtilities.CachedShipStatus.AllVents)
                        {
                            vent.CanUse(PlayerControl.LocalPlayer.Data, out bool canUse, out bool couldUse);
                            if (canUse)
                            {
                                PlayerControl.LocalPlayer.MyPhysics.RpcExitVent(vent.Id);
                                vent.SetButtons(false);
                            }
                        }

                    // Set position
                    PlayerControl.LocalPlayer.transform.position = next.Item1;
                }
                else if (localPlayerPositions.Any(x => x.Item2))
                {
                    PlayerControl.LocalPlayer.transform.position = next.Item1;
                }

                if (SubmergedCompatibility.IsSubmerged) SubmergedCompatibility.ChangeFloor(next.Item1.y > -7);

                localPlayerPositions.RemoveAt(0);

                // Skip every second position to rewinde twice as fast, but never skip the last position
                if (localPlayerPositions.Count > 1)
                    localPlayerPositions.RemoveAt(0);
            }
            else
            {
                TimeMaster.isRewinding = false;
                PlayerControl.LocalPlayer.moveable = true;
            }
        }
        else
        {
            while (localPlayerPositions.Count >= Mathf.Round(TimeMaster.rewindTime / Time.fixedDeltaTime))
                localPlayerPositions.RemoveAt(localPlayerPositions.Count - 1);
            localPlayerPositions.Insert(0,
                new Tuple<Vector3, bool>(PlayerControl.LocalPlayer.transform.position,
                    PlayerControl.LocalPlayer.CanMove)); // CanMove = CanMove
        }
    }


    public static void deputyCheckPromotion(bool isMeeting = false)
    {
        // If LocalPlayer is Deputy, the Sheriff is disconnected and Deputy promotion is enabled, then trigger promotion
        if (Sheriff.Deputy == null || Sheriff.Deputy != PlayerControl.LocalPlayer) return;
        if (Sheriff.promotesToSheriff == 0 || Sheriff.Deputy.IsDead() || (Sheriff.promotesToSheriff == 2 && !isMeeting)) return;
        if (Sheriff.Player == null || Sheriff.Player.All(x => x.IsDead()))
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.DeputyPromotes, SendOption.Reliable);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            Sheriff.replaceCurrentSheriff();
        }
    }

    private static void detectiveUpdateFootPrints()
    {
        if (Detective.detective == null
            || Detective.detective != PlayerControl.LocalPlayer
            || InMeeting
            || Detective.detective.IsDead()) return;

        Detective.timer -= Time.fixedDeltaTime;
        if (Detective.timer <= 0f)
        {
            Detective.timer = Detective.footprintIntervall;
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                if (player != null && player != PlayerControl.LocalPlayer && !player.Data.IsDead && !player.inVent)
                    FootprintHolder.Instance.MakeFootprint(player);
        }
    }

    private static void sidekickCheckPromotion()
    {
        // If LocalPlayer is Sidekick, the Jackal is disconnected and Sidekick promotion is enabled, then trigger promotion
        if (Jackal.Sidekick.IsDead() || !Jackal.promotesToJackal || Jackal.Sidekick != PlayerControl.LocalPlayer) return;
        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
            (byte)CustomRPC.SidekickPromotes, SendOption.Reliable);
        writer.Write(Jackal.Sidekick.PlayerId);
        AmongUsClient.Instance.FinishRpcImmediately(writer);
        RPCProcedure.sidekickPromotes(Jackal.Sidekick.PlayerId);
    }

    private static void deputyUpdate()
    {
        if (PlayerControl.LocalPlayer == null ||
            !Sheriff.handcuffedKnows.ContainsKey(PlayerControl.LocalPlayer.PlayerId)) return;

        if (Sheriff.handcuffedKnows[PlayerControl.LocalPlayer.PlayerId] <= 0)
        {
            Sheriff.handcuffedKnows.Remove(PlayerControl.LocalPlayer.PlayerId);
            // Resets the buttons
            Sheriff.setHandcuffedKnows(false);

            // Ghost info
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.ShareGhostInfo, SendOption.Reliable);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            writer.Write((byte)RPCProcedure.GhostInfoTypes.HandcuffOver);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }
    }

    private static void engineerUpdate()
    {
        var jackalHighlight = Engineer.highlightForTeamJackal &&
                              (Jackal.jackal.Any(x => x == PlayerControl.LocalPlayer) ||
                               PlayerControl.LocalPlayer == Jackal.Sidekick);
        var impostorHighlight = Engineer.highlightForImpostors && PlayerControl.LocalPlayer.Data.Role.IsImpostor;
        if ((jackalHighlight || impostorHighlight) && MapUtilities.CachedShipStatus?.AllVents != null)
            foreach (var vent in MapUtilities.CachedShipStatus.AllVents)
                try
                {
                    if (vent?.myRend?.material != null)
                    {
                        if (Engineer.engineer != null && Engineer.engineer.inVent)
                        {
                            vent.myRend.material.SetFloat("_Outline", 1f);
                            vent.myRend.material.SetColor("_OutlineColor", Engineer.color);
                        }
                        else if (vent.myRend.material.GetColor("_AddColor") != Color.red)
                        {
                            vent.myRend.material.SetFloat("_Outline", 0);
                        }
                    }
                }
                catch
                {
                }
    }

    private static void swooperUpdate()
    {
        if (Swooper.isInvisable && Swooper.swoopTimer <= 0 && Swooper.swooper == PlayerControl.LocalPlayer)
        {
            var invisibleWriter = AmongUsClient.Instance.StartRpcImmediately(
                PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetSwoop, SendOption.Reliable);
            invisibleWriter.Write(Swooper.swooper.PlayerId);
            invisibleWriter.Write(byte.MaxValue);
            AmongUsClient.Instance.FinishRpcImmediately(invisibleWriter);
            RPCProcedure.setSwoop(Swooper.swooper.PlayerId, byte.MaxValue);
        }
        if (Jackal.isInvisable && Jackal.swoopTimer <= 0 && Jackal.jackal.Any(x => x == PlayerControl.LocalPlayer))
        {
            var invisibleWriter = AmongUsClient.Instance.StartRpcImmediately(
                PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetJackalSwoop, SendOption.Reliable);
            invisibleWriter.Write(PlayerControl.LocalPlayer.PlayerId);
            invisibleWriter.Write(byte.MaxValue);
            AmongUsClient.Instance.FinishRpcImmediately(invisibleWriter);
            RPCProcedure.setJackalSwoop(PlayerControl.LocalPlayer.PlayerId, byte.MaxValue);
        }
    }

    private static void ninjaUpdate()
    {
        if (Ninja.isInvisable && Ninja.invisibleTimer <= 0 && Ninja.ninja == PlayerControl.LocalPlayer)
        {
            var invisibleWriter = AmongUsClient.Instance.StartRpcImmediately(
                PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetInvisible, SendOption.Reliable);
            invisibleWriter.Write(Ninja.ninja.PlayerId);
            invisibleWriter.Write(byte.MaxValue);
            AmongUsClient.Instance.FinishRpcImmediately(invisibleWriter);
            RPCProcedure.setInvisible(Ninja.ninja.PlayerId, byte.MaxValue);
        }

        if (Ninja.arrow?.arrow != null)
        {
            if (Ninja.ninja == null || Ninja.ninja != PlayerControl.LocalPlayer ||
                !Ninja.knowsTargetLocation)
            {
                Ninja.arrow.arrow.SetActive(false);
                return;
            }

            if (Ninja.ninjaMarked != null && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                var trackedOnMap = !Ninja.ninjaMarked.Data.IsDead;
                var position = Ninja.ninjaMarked.transform.position;
                if (!trackedOnMap)
                {
                    // Check for dead body
                    var body = Object.FindObjectsOfType<DeadBody>()
                        .FirstOrDefault(b => b.ParentId == Ninja.ninjaMarked.PlayerId);
                    if (body != null)
                    {
                        trackedOnMap = true;
                        position = body.transform.position;
                    }
                }

                Ninja.arrow.Update(position);
                Ninja.arrow.arrow.SetActive(trackedOnMap);
            }
            else
            {
                Ninja.arrow.arrow.SetActive(false);
            }
        }
    }

    private static void prophetUpdate()
    {
        if (Prophet.arrows == null) return;

        foreach (var arrow in Prophet.arrows) arrow.arrow.SetActive(false);

        if (Prophet.prophet == null || Prophet.prophet.Data.IsDead) return;

        var local = PlayerControl.LocalPlayer;

        if (Prophet.isRevealed && (local.Data.Role.IsImpostor || isKillerNeutral(local)))
        {
            if (Prophet.arrows.Count == 0) Prophet.arrows.Add(new Arrow(Prophet.color));
            if (Prophet.arrows.Count != 0 && Prophet.arrows[0] != null)
            {
                Prophet.arrows[0].arrow.SetActive(true);
                Prophet.arrows[0].Update(Prophet.prophet.transform.position);
            }
        }
    }

    private static void trackerUpdate()
    {
        // Handle player tracking
        if (Tracker.arrow?.arrow != null)
        {
            if (Tracker.tracker == null || PlayerControl.LocalPlayer != Tracker.tracker)
            {
                Tracker.arrow.arrow.SetActive(false);
                if (Tracker.DangerMeterParent) Tracker.DangerMeterParent.SetActive(false);
                return;
            }

            if (Tracker.tracked != null && !Tracker.tracker.Data.IsDead)
            {
                Tracker.timeUntilUpdate -= Time.fixedDeltaTime;

                if (Tracker.tracked.Data.IsDead) Tracker.resetTracked();

                if (Tracker.timeUntilUpdate <= 0f)
                {
                    bool trackedOnMap = !Tracker.tracked.Data.IsDead;
                    Vector3 position = Tracker.tracked.transform.position;
                    if (!trackedOnMap)
                    {
                        // Check for dead body
                        DeadBody body = Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == Tracker.tracked.PlayerId);
                        if (body != null)
                        {
                            trackedOnMap = true;
                            position = body.transform.position;
                        }
                    }

                    if (Tracker.trackingMode is 1 or 2) Arrow.UpdateProximity(position);
                    if (Tracker.trackingMode is 0 or 2)
                    {
                        Tracker.arrow.Update(position, Tracker.tracked?.Data.Color);
                        Tracker.arrow.arrow.SetActive(trackedOnMap);
                    }
                    Tracker.timeUntilUpdate = Tracker.updateIntervall;
                }
                else
                {
                    if (Tracker.trackingMode is 0 or 2) Tracker.arrow.Update();
                }
            }
            else if (Tracker.tracker.Data.IsDead)
            {
                Tracker.DangerMeterParent?.SetActive(false);
                Tracker.Meter?.gameObject.SetActive(false);
            }
        }

        // Handle corpses tracking
        if (Tracker.tracker != null && Tracker.tracker == PlayerControl.LocalPlayer && Tracker.corpsesTrackingTimer >= 0f && !Tracker.tracker.Data.IsDead)
        {
            bool arrowsCountChanged = Tracker.localArrows.Count != Tracker.deadBodyPositions.Count;
            int index = 0;

            if (arrowsCountChanged)
            {
                foreach (Arrow arrow in Tracker.localArrows) Object.Destroy(arrow.arrow);
                Tracker.localArrows = new();
            }
            foreach (Vector3 position in Tracker.deadBodyPositions)
            {
                if (arrowsCountChanged)
                {
                    Tracker.localArrows.Add(new Arrow(Tracker.color));
                    Tracker.localArrows[index].arrow.SetActive(true);
                }
                if (Tracker.localArrows[index] != null) Tracker.localArrows[index].Update(position);
                index++;
            }
        }
        else if (Tracker.localArrows.Count > 0)
        {
            foreach (Arrow arrow in Tracker.localArrows) Object.Destroy(arrow.arrow);
            Tracker.localArrows = new();
        }
    }

    private static void MiniSizeUpdate(PlayerControl p)
    {
        if (Mini.mini == null) return;
        // Set default player size
        var collider = p.Collider.CastFast<CircleCollider2D>();

        p.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
        collider.radius = Mini.defaultColliderRadius;
        collider.offset = Mini.defaultColliderOffset * Vector2.down;

        // Set adapted player size to Mini and Morphling
        if (Mini.mini == null || isCamoComms || Camouflager.camouflageTimer > 0f ||
        MushroomSabotageActive || (Mini.mini == Morphling.morphling && Morphling.morphTimer > 0)) return;

        var growingProgress = Mini.growingProgress();
        var scale = (growingProgress * 0.35f) + 0.35f;
        var correctedColliderRadius = Mini.defaultColliderRadius * 0.7f / scale;
        // scale / 0.7f is the factor by which we decrease the player size, hence we need to increase the collider size by 0.7f / scale

        if (p == Mini.mini)
        {
            p.transform.localScale = new Vector3(scale, scale, 1f);
            collider.radius = correctedColliderRadius;
        }

        if (Morphling.morphling != null && p == Morphling.morphling && Morphling.morphTarget == Mini.mini &&
            Morphling.morphTimer > 0f)
        {
            p.transform.localScale = new Vector3(scale, scale, 1f);
            collider.radius = correctedColliderRadius;
        }
    }

    public static void GiantSizeUpdate(PlayerControl p)
    {
        if (Giant.giant == null) return;

        if (!isCamoComms && Camouflager.camouflageTimer == 0f && !MushroomSabotageActive &&
            ((Giant.giant == Morphling.morphling && Morphling.morphTimer == 0f) ||
             (p == Morphling.morphling && Giant.giant == Morphling.morphTarget && Morphling.morphTimer > 0f) ||
             Giant.giant == p))
        {
            var collider = p.Collider.CastFast<CircleCollider2D>();
            collider.offset = 0.3636057f * Vector2.down;

            p.transform.localScale = new Vector3(Giant.size, Giant.size, 1f);
            collider.radius = 0.2233912f * 0.85f;
        }
        else if (p != Mini.mini)
        {
            p.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
        }
    }

    public static void WitnessUpdate()
    {
        if (Witness.Player.IsDead() && !InMeeting) return;

        if (MeetingHud.Instance)
        {
            if (Witness.target != null)
            {
                setInfo(Witness.target.PlayerId, cs(Color.red, $"{Witness.target?.Data?.PlayerName} 疑似为本案的凶手"));
            }
            else if ((PlayerControl.LocalPlayer == Witness.Player || ModOption.DebugMode) && Witness.killerTarget != null)
            {
                setInfo(Witness.killerTarget.PlayerId, cs(Color.red, $"{Witness.killerTarget?.Data?.PlayerName} 为本案的真凶"));
            }
        }

        void setInfo(int targetPlayerId, string infoText)
        {
            var pva = MeetingHud.Instance?.playerStates?.FirstOrDefault(x => x.TargetPlayerId == targetPlayerId);
            if (pva == null) return;

            var meetingInfoTransform = pva.NameText.transform.parent.FindChild("WitnessInfo");
            var meetingInfo = meetingInfoTransform != null ? meetingInfoTransform.GetComponent<TextMeshPro>() : null;

            if (meetingInfo == null)
            {
                meetingInfo = Object.Instantiate(pva.NameText, pva.NameText.transform.parent);
                meetingInfo.transform.localPosition += Vector3.up * 0.2f;
                meetingInfo.fontSize *= 0.72f;
                meetingInfo.gameObject.name = "WitnessInfo";
            }

            if (meetingInfo != null)
            {
                meetingInfo.text = MeetingHud.Instance.state == MeetingHud.VoteStates.Results ? "" : infoText;
            }
        }
    }

    public static void securityGuardUpdate()
    {
        if (SecurityGuard.securityGuard == null ||
            PlayerControl.LocalPlayer != SecurityGuard.securityGuard ||
            SecurityGuard.securityGuard.Data.IsDead) return;
        var (playerCompleted, _) = TasksHandler.taskInfo(SecurityGuard.securityGuard.Data);
        if (playerCompleted == SecurityGuard.rechargedTasks)
        {
            SecurityGuard.rechargedTasks += SecurityGuard.rechargeTasksNumber;
            if (SecurityGuard.maxCharges > SecurityGuard.charges) SecurityGuard.charges++;
        }
    }

    private static void snitchUpdate()
    {
        if (Snitch.localArrows == null) return;

        foreach (var arrow in Snitch.localArrows) arrow.arrow.SetActive(false);

        if (Snitch.snitch == null || Snitch.snitch.Data.IsDead) return;

        var (playerCompleted, playerTotal) = TasksHandler.taskInfo(Snitch.snitch.Data);
        var numberOfTasks = playerTotal - playerCompleted;

        var snitchIsDead = Snitch.snitch.Data.IsDead;
        var local = PlayerControl.LocalPlayer;

        var forImpTeam = local.Data.Role.IsImpostor;
        var forKillerTeam = Snitch.Team == Snitch.includeNeutralTeam.KillNeutral && isKillerNeutral(local);
        var forEvilTeam = Snitch.Team == Snitch.includeNeutralTeam.EvilNeutral && isEvilNeutral(local);
        var forNeutraTeam = Snitch.Team == Snitch.includeNeutralTeam.AllNeutral && local.isNeutral();

        if (numberOfTasks <= Snitch.taskCountForReveal && (forImpTeam || forKillerTeam || forEvilTeam || forNeutraTeam))
        {
            if (Snitch.localArrows.Count == 0) Snitch.localArrows.Add(new Arrow(Snitch.color));
            if (Snitch.localArrows.Count != 0 && Snitch.localArrows[0] != null)
            {
                Snitch.localArrows[0].arrow.SetActive(true);
                Snitch.localArrows[0].Update(Snitch.snitch.transform.position);
            }
        }
        else if (local == Snitch.snitch && numberOfTasks == 0 && !snitchIsDead)
        {
            var arrowIndex = 0;
            foreach (PlayerControl p in PlayerControl.AllPlayerControls)
            {
                var arrowForImp = p.Data.Role.IsImpostor;
                if (Mimic.mimic == p) arrowForImp = true;
                var arrowForKillerTeam = Snitch.Team == Snitch.includeNeutralTeam.KillNeutral && isKillerNeutral(p);
                var arrowForEvilTeam = Snitch.Team == Snitch.includeNeutralTeam.EvilNeutral && isEvilNeutral(p);
                var arrowForNeutraTeam = Snitch.Team == Snitch.includeNeutralTeam.AllNeutral && p.isNeutral();
                var targetsRole = RoleInfo.getRoleInfoForPlayer(p, false).FirstOrDefault();

                if (!p.Data.IsDead && (arrowForImp || arrowForKillerTeam || arrowForEvilTeam || arrowForNeutraTeam))
                {
                    if (arrowIndex >= Snitch.localArrows.Count)
                    {
                        Snitch.localArrows.Add(new Arrow(Palette.ImpostorRed));
                    }
                    if (arrowIndex < Snitch.localArrows.Count && Snitch.localArrows[arrowIndex] != null)
                    {
                        Snitch.localArrows[arrowIndex].arrow.SetActive(true);
                        if (arrowForImp)
                        {
                            Snitch.localArrows[arrowIndex].Update(p.transform.position, Palette.ImpostorRed);
                        }
                        else if (arrowForKillerTeam || arrowForEvilTeam || arrowForNeutraTeam)
                        {
                            Snitch.localArrows[arrowIndex].Update(p.transform.position, Snitch.teamNeutraUseDifferentArrowColor ? targetsRole.color : Palette.ImpostorRed);
                        }
                    }
                    arrowIndex++;
                }
            }
        }
    }

    // Snitch Text
    private static void snitchTextUpdate()
    {
        if (Snitch.snitch == null) return;
        var (playerCompleted, playerTotal) = TasksHandler.taskInfo(Snitch.snitch.Data);
        var numberOfTasks = playerTotal - playerCompleted;

        var local = PlayerControl.LocalPlayer;

        var isDead = local == Snitch.snitch || local.Data.IsDead;
        var forImpTeam = local.isImpostor();
        var forKillerTeam = Snitch.Team == Snitch.includeNeutralTeam.KillNeutral && isKillerNeutral(local);
        var forEvilTeam = Snitch.Team == Snitch.includeNeutralTeam.EvilNeutral && isEvilNeutral(local);
        var forNeutraTeam = Snitch.Team == Snitch.includeNeutralTeam.AllNeutral && local.isNeutral();

        if (numberOfTasks <= Snitch.taskCountForReveal && (forImpTeam || forKillerTeam || forEvilTeam || forNeutraTeam || isDead))
        {
            if (Snitch.text == null && !Snitch.snitch.IsDead())
            {
                Snitch.text = Object.Instantiate(FastDestroyableSingleton<HudManager>.Instance.KillButton.cooldownTimerText, FastDestroyableSingleton<HudManager>.Instance.transform);
                Snitch.text.enableWordWrapping = false;
                Snitch.text.transform.localScale = Vector3.one * 0.75f;
                Snitch.text.transform.localPosition += new Vector3(0f, 1.8f, -69f);
                Snitch.text.gameObject.SetActive(true);
            }
            else if (!Snitch.snitch.IsDead())
            {
                Snitch.text.text = $"告密者还活着: {playerCompleted} / {playerTotal}";
            }
            else
            {
                if (MeetingHud.Instance == null) Snitch.needsUpdate = false;
                Snitch.text?.Destroy();
            }
        }
        else if (Snitch.text != null) Snitch.text.Destroy();
    }

    private static void partTimerUpdate()
    {
        if (PartTimer.partTimer == null
            || PlayerControl.LocalPlayer != PartTimer.partTimer
            || PartTimer.partTimer.IsDead()) return;

        if (PartTimer.target != null && PartTimer.target.IsDead())
        {
            var playerInfoTransform = PartTimer.target?.cosmetics.nameText.transform.parent.FindChild("Info");
            var playerInfo = playerInfoTransform?.GetComponent<TextMeshPro>();
            if (playerInfo != null) playerInfo.text = "";

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.PartTimerSet, SendOption.Reliable);
            writer.Write(byte.MaxValue);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.partTimerSet(byte.MaxValue);
        }
    }


    private static void undertakerDragBodyUpdate()
    {
        if (Undertaker.undertaker == null || Undertaker.undertaker.Data.IsDead) return;
        if (Undertaker.deadBodyDraged != null)
        {
            var currentPosition = Undertaker.undertaker.transform.position;
            Undertaker.deadBodyDraged.transform.position = currentPosition;
        }
    }

    private static void bountyHunterUpdate()
    {
        if (BountyHunter.bountyHunter == null || PlayerControl.LocalPlayer != BountyHunter.bountyHunter) return;

        if (BountyHunter.bountyHunter.Data.IsDead)
        {
            if (BountyHunter.arrow != null) Object.Destroy(BountyHunter.arrow.arrow);
            BountyHunter.arrow = null;
            if (BountyHunter.cooldownText != null && BountyHunter.cooldownText.gameObject != null) Object.Destroy(BountyHunter.cooldownText.gameObject);
            BountyHunter.cooldownText = null;
            BountyHunter.bounty = null;
            foreach (PoolablePlayer p in ModOption.playerIcons.Values)
            {
                if (p != null && p.gameObject != null) p.gameObject.SetActive(false);
            }
            return;
        }

        BountyHunter.arrowUpdateTimer -= Time.fixedDeltaTime;
        BountyHunter.bountyUpdateTimer -= Time.fixedDeltaTime;

        if (BountyHunter.bounty == null || BountyHunter.bountyUpdateTimer <= 0f)
        {
            // Set new bounty
            BountyHunter.bounty = null;
            BountyHunter.arrowUpdateTimer = 0f; // Force arrow to update
            BountyHunter.bountyUpdateTimer = BountyHunter.bountyDuration;
            var possibleTargets = new List<PlayerControl>();
            foreach (PlayerControl p in PlayerControl.AllPlayerControls.ToArray().Where(x => x.IsAlive()))
                if (p.IsAlive() && p != p.isImpostor(true) && (p != Mini.mini || Mini.isGrownUp()) && p != Lovers.otherLover(BountyHunter.bountyHunter))
                    possibleTargets.Add(p);
            if (possibleTargets.Count == 0) return;
            BountyHunter.bounty = possibleTargets[rnd.Next(0, possibleTargets.Count)];
            if (BountyHunter.bounty == null) return;

            // Ghost Info
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.ShareGhostInfo, SendOption.Reliable);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            writer.Write((byte)RPCProcedure.GhostInfoTypes.BountyTarget);
            writer.Write(BountyHunter.bounty.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            // Show poolable player
            if (FastDestroyableSingleton<HudManager>.Instance != null &&
                FastDestroyableSingleton<HudManager>.Instance.UseButton != null)
            {
                foreach (var pp in ModOption.playerIcons.Values) pp.gameObject.SetActive(false);
                if (ModOption.playerIcons.ContainsKey(BountyHunter.bounty.PlayerId) &&
                    ModOption.playerIcons[BountyHunter.bounty.PlayerId].gameObject != null)
                    ModOption.playerIcons[BountyHunter.bounty.PlayerId].gameObject.SetActive(true);
            }
        }

        // Hide in meeting
        if (MeetingHud.Instance && ModOption.playerIcons.ContainsKey(BountyHunter.bounty.PlayerId) &&
            ModOption.playerIcons[BountyHunter.bounty.PlayerId].gameObject != null)
            ModOption.playerIcons[BountyHunter.bounty.PlayerId].gameObject.SetActive(false);

        // Update Cooldown Text
        if (BountyHunter.cooldownText != null)
        {
            BountyHunter.cooldownText.text = Mathf
                .CeilToInt(Mathf.Clamp(BountyHunter.bountyUpdateTimer, 0, BountyHunter.bountyDuration)).ToString();
            BountyHunter.cooldownText.gameObject.SetActive(!MeetingHud.Instance); // Show if not in meeting
        }

        // Update Arrow
        if (BountyHunter.showArrow && BountyHunter.bounty.IsAlive())
        {
            BountyHunter.arrow ??= new Arrow(Color.red);
            if (BountyHunter.arrowUpdateTimer <= 0f)
            {
                BountyHunter.arrow.Update(BountyHunter.bounty.transform.position);
                BountyHunter.arrowUpdateTimer = BountyHunter.arrowUpdateIntervall;
            }

            BountyHunter.arrow.Update();
        }
    }

    private static void vultureUpdate()
    {
        if (Vulture.vulture == null || PlayerControl.LocalPlayer != Vulture.vulture ||
            Vulture.localArrows == null || !Vulture.showArrows) return;
        if (Vulture.vulture.Data.IsDead)
        {
            foreach (var arrow in Vulture.localArrows) Object.Destroy(arrow.arrow);
            Vulture.localArrows = new();
            return;
        }

        DeadBody[] deadBodies = Object.FindObjectsOfType<DeadBody>();
        var arrowUpdate = Vulture.localArrows.Count != deadBodies.Length;
        var index = 0;

        if (arrowUpdate)
        {
            foreach (var arrow in Vulture.localArrows) Object.Destroy(arrow.arrow);
            Vulture.localArrows = new();
        }

        foreach (var db in deadBodies)
        {
            if (arrowUpdate)
            {
                Vulture.localArrows.Add(new Arrow(Color.blue));
                Vulture.localArrows[index].arrow.SetActive(true);
            }

            if (Vulture.localArrows[index] != null) Vulture.localArrows[index].Update(db.transform.position);
            index++;
        }
    }

    private static void amnisiacUpdate()
    {
        if (Amnisiac.Player?.Count == 0 || Amnisiac.localArrows == null || !Amnisiac.showArrows || InMeeting) return;

        foreach (var p in Amnisiac.Player.ToList())
        {
            if (p.Data.IsDead)
            {
                foreach (var arrow in Amnisiac.localArrows)
                    Object.Destroy(arrow.arrow);
                Amnisiac.localArrows.Clear();
            }
        }
        if (Amnisiac.Player.Any(x => x.PlayerId == PlayerControl.LocalPlayer.PlayerId && x.IsAlive()))
        {
            DeadBody[] deadBodies = Object.FindObjectsOfType<DeadBody>();
            bool arrowUpdate = Amnisiac.localArrows.Count != deadBodies.Length;
            int index = 0;

            if (arrowUpdate)
            {
                foreach (var arrow in Amnisiac.localArrows)
                    Object.Destroy(arrow.arrow);

                Amnisiac.localArrows.Clear();
            }

            foreach (var db in deadBodies)
            {
                if (arrowUpdate)
                {
                    Amnisiac.localArrows.Add(new Arrow(Amnisiac.color));
                    Amnisiac.localArrows[index].arrow.SetActive(true);
                }

                Amnisiac.localArrows[index]?.Update(db.transform.position);
                index++;
            }
        }
    }

    public static void evilTrapperUpdate()
    {
        try
        {
            if (PlayerControl.LocalPlayer == EvilTrapper.evilTrapper && KillTrap.traps.Count != 0 && !KillTrap.hasTrappedPlayer() && !EvilTrapper.meetingFlag)
            {
                foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
                {
                    foreach (var trap in KillTrap.traps)
                    {
                        if (DateTime.UtcNow.Subtract(trap.Value.placedTime).TotalSeconds < EvilTrapper.extensionTime) continue;
                        if (trap.Value.isActive || p.Data.IsDead || p.inVent || EvilTrapper.meetingFlag) continue;
                        var p1 = p.transform.localPosition;
                        Dictionary<GameObject, byte> listActivate = new();
                        var p2 = trap.Value.killtrap.transform.localPosition;
                        var distance = Vector3.Distance(p1, p2);
                        if (distance < EvilTrapper.trapRange)
                        {
                            TMP_Text text;
                            RoomTracker roomTracker = FastDestroyableSingleton<HudManager>.Instance?.roomTracker;
                            GameObject gameObject = Object.Instantiate(roomTracker.gameObject);
                            Object.DestroyImmediate(gameObject.GetComponent<RoomTracker>());
                            gameObject.transform.SetParent(FastDestroyableSingleton<HudManager>.Instance.transform);
                            gameObject.transform.localPosition = new Vector3(0, -1.8f, gameObject.transform.localPosition.z);
                            gameObject.transform.localScale = Vector3.one * 2f;
                            text = gameObject.GetComponent<TMP_Text>();
                            text.text = string.Format(GetString("trapperGotTrapText"), p.Data.PlayerName);
                            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(3f, new Action<float>((p) =>
                            {
                                if (p == 1f && text != null && text.gameObject != null)
                                {
                                    Object.Destroy(text.gameObject);
                                }
                            })));
                            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ActivateTrap, SendOption.Reliable, -1);
                            writer.Write(trap.Key);
                            writer.Write(PlayerControl.LocalPlayer.PlayerId);
                            writer.Write(p.PlayerId);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            RPCProcedure.activateTrap(trap.Key, EvilTrapper.evilTrapper.PlayerId, p.PlayerId);
                            break;
                        }
                    }
                }
            }

            if (PlayerControl.LocalPlayer == EvilTrapper.evilTrapper && KillTrap.hasTrappedPlayer() && !EvilTrapper.meetingFlag)
            {
                // トラップにかかっているプレイヤーを救出する
                foreach (var trap in KillTrap.traps)
                {
                    if (trap.Value.killtrap == null || !trap.Value.isActive) return;
                    Vector3 p1 = trap.Value.killtrap.transform.position;
                    foreach (var player in PlayerControl.AllPlayerControls.GetFastEnumerator())
                    {
                        if (player.PlayerId == trap.Value.target.PlayerId || player.Data.IsDead || player.inVent || player == EvilTrapper.evilTrapper) continue;
                        Vector3 p2 = player.transform.position;
                        float distance = Vector3.Distance(p1, p2);
                        if (distance < 0.5)
                        {
                            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.DisableTrap, SendOption.Reliable, -1);
                            writer.Write(trap.Key);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            RPCProcedure.disableTrap(trap.Key);
                        }
                    }

                }
            }
        }
        catch (NullReferenceException e)
        {
            Warn(e.Message);
        }
    }

    private static void radarUpdate()
    {
        if (Radar.radar == null || PlayerControl.LocalPlayer != Radar.radar || Radar.localArrows == null || InMeeting) return;
        if (Radar.radar.Data.IsDead)
        {
            foreach (var arrow in Radar.localArrows) Object.Destroy(arrow.arrow);
            Radar.localArrows = new();
            return;
        }

        var arrowUpdate = true;
        var index = 0;

        if (arrowUpdate && !PlayerControl.LocalPlayer.Data.IsDead)
        {
            foreach (var arrow in Radar.localArrows) Object.Destroy(arrow.arrow);
            Radar.ClosestPlayer = GetClosestPlayer(PlayerControl.LocalPlayer,
                PlayerControl.AllPlayerControls.ToArray().ToList());
            Radar.localArrows = new();
        }


        foreach (PlayerControl player in PlayerControl.AllPlayerControls)
        {
            if (arrowUpdate && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                Radar.localArrows.Add(new Arrow(Radar.color));
                Radar.localArrows[index].arrow.SetActive(true);
            }

            Radar.localArrows[index]?.Update(Radar.ClosestPlayer.transform.position);
            index++;
        }
    }

    public static PlayerControl GetClosestPlayer(PlayerControl refPlayer, List<PlayerControl> AllPlayers)
    {
        var num = double.MaxValue;
        var refPosition = refPlayer.GetTruePosition();
        PlayerControl result = null;
        foreach (var player in AllPlayers)
        {
            if (player.Data.IsDead || player.PlayerId == refPlayer.PlayerId || !player.Collider.enabled) continue;
            var playerPosition = player.GetTruePosition();
            var distBetweenPlayers = Vector2.Distance(refPosition, playerPosition);
            var isClosest = distBetweenPlayers < num;
            if (!isClosest) continue;
            var vector = playerPosition - refPosition;
            //if (PhysicsHelpers.AnyNonTriggersBetween(
            //   refPosition, vector.normalized, vector.magnitude, Constants.ShipAndObjectsMask)) continue;
            num = distBetweenPlayers;
            result = player;
        }

        return result;
    }

    private static void morphlingAndCamouflagerUpdate()
    {
        var mushRoomSaboIsActive = MushroomSabotageActive;
        if (!mushroomSaboWasActive) mushroomSaboWasActive = mushRoomSaboIsActive;

        if (isCamoComms && !isActiveCamoComms)
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.CamouflagerCamouflage, SendOption.Reliable);
            writer.Write(0);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.camouflagerCamouflage(0);
        }

        var oldCamouflageTimer = Camouflager.camouflageTimer;
        var oldMorphTimer = Morphling.morphTimer;
        Camouflager.camouflageTimer = Mathf.Max(0f, Camouflager.camouflageTimer - Time.fixedDeltaTime);
        Morphling.morphTimer = Mathf.Max(0f, Morphling.morphTimer - Time.fixedDeltaTime);

        if (mushRoomSaboIsActive) return;
        if (isCamoComms) return;
        if (wasActiveCamoComms && Camouflager.camouflageTimer <= 0f) camoReset();

        // Camouflage reset and set Morphling look if necessary
        if (oldCamouflageTimer > 0f && Camouflager.camouflageTimer <= 0f)
        {
            Camouflager.resetCamouflage();
            camoReset();
            if (Morphling.morphTimer > 0f && Morphling.morphling != null && Morphling.morphTarget != null)
            {
                var target = Morphling.morphTarget;
                Morphling.morphling.setLook(target.Data.PlayerName, target.Data.DefaultOutfit.ColorId,
                    target.Data.DefaultOutfit.HatId, target.Data.DefaultOutfit.VisorId,
                    target.Data.DefaultOutfit.SkinId, target.Data.DefaultOutfit.PetId);
            }
        }

        // If the MushRoomSabotage ends while Morph is still active set the Morphlings look to the target's look
        if (mushroomSaboWasActive)
        {
            if (Morphling.morphTimer > 0f && Morphling.morphling != null && Morphling.morphTarget != null)
            {
                var target = Morphling.morphTarget;
                Morphling.morphling.setLook(target.Data.PlayerName, target.Data.DefaultOutfit.ColorId,
                    target.Data.DefaultOutfit.HatId, target.Data.DefaultOutfit.VisorId,
                    target.Data.DefaultOutfit.SkinId, target.Data.DefaultOutfit.PetId);
            }

            if (Camouflager.camouflageTimer > 0)
                foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                    player.setLook("", 6, "", "", "", "");
        }

        // Morphling reset (only if camouflage is inactive)
        if (Camouflager.camouflageTimer <= 0f && oldMorphTimer > 0f && Morphling.morphTimer <= 0f &&
            Morphling.morphling != null)
            Morphling.resetMorph();
        mushroomSaboWasActive = false;
    }

    public static void lawyerUpdate()
    {
        if (Lawyer.lawyer == null || Lawyer.lawyer != PlayerControl.LocalPlayer) return;

        // Promote to Pursuer
        if (Lawyer.target != null && Lawyer.target.Data.Disconnected && !Lawyer.lawyer.Data.IsDead)
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.LawyerPromotesToPursuer, SendOption.Reliable);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            Lawyer.PromotesToPursuer();
        }
    }

    public static void executionerUpdate()
    {
        if (Executioner.executioner == null || Executioner.executioner != PlayerControl.LocalPlayer) return;

        // Promote to Pursuer
        if (Executioner.target != null && Executioner.target.Data.Disconnected && !Executioner.executioner.Data.IsDead)
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.ExecutionerPromotesRole, SendOption.Reliable);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            Executioner.PromotesRole();
        }
    }

    public static void hackerUpdate()
    {
        if (Hacker.hacker == null || PlayerControl.LocalPlayer != Hacker.hacker ||
            Hacker.hacker.Data.IsDead) return;
        var (playerCompleted, _) = TasksHandler.taskInfo(Hacker.hacker.Data);
        if (playerCompleted == Hacker.rechargedTasks)
        {
            Hacker.rechargedTasks += Hacker.rechargeTasksNumber;
            if (Hacker.toolsNumber > Hacker.chargesVitals) Hacker.chargesVitals++;
            if (Hacker.toolsNumber > Hacker.chargesAdminTable) Hacker.chargesAdminTable++;
        }
    }

    // For swapper swap charges        
    public static void swapperUpdate()
    {
        if (Swapper.swapper == null || PlayerControl.LocalPlayer != Swapper.swapper ||
            PlayerControl.LocalPlayer.Data.IsDead) return;
        var (playerCompleted, _) = TasksHandler.taskInfo(PlayerControl.LocalPlayer.Data);
        if (playerCompleted == Swapper.rechargedTasks)
        {
            Swapper.rechargedTasks += Swapper.rechargeTasksNumber;
            Swapper.charges++;
        }
    }


    private static void baitUpdate()
    {
        if (!Bait.active.Any()) return;

        // Bait report
        foreach (var entry in new Dictionary<DeadPlayer, float>(Bait.active))
        {
            Bait.active[entry.Key] = entry.Value - Time.fixedDeltaTime;
            if (entry.Value <= 0)
            {
                Bait.active.Remove(entry.Key);
                if (entry.Key.KillerIfExisting != null &&
                    entry.Key.KillerIfExisting.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                {

                    handleVampireBiteOnBodyReport(); // Manually call Vampire handling, since the CmdReportDeadBody Prefix won't be called

                    handleBomberExplodeOnBodyReport(); // Manually call Vampire handling, since the CmdReportDeadBody Prefix won't be called
                    RPCProcedure.uncheckedCmdReportDeadBody(entry.Key.KillerIfExisting.PlayerId,
                        entry.Key.Player.PlayerId);

                    var writer = AmongUsClient.Instance.StartRpcImmediately(
                        PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.UncheckedCmdReportDeadBody,
                        SendOption.Reliable);
                    writer.Write(entry.Key.KillerIfExisting.PlayerId);
                    writer.Write(entry.Key.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }
        }
    }

    private static void bloodyUpdate()
    {
        if (!Bloody.active.Any()) return;
        foreach (var entry in new Dictionary<byte, float>(Bloody.active))
        {
            var player = playerById(entry.Key);
            var bloodyPlayer = playerById(Bloody.bloodyKillerMap[player.PlayerId]);

            Bloody.active[entry.Key] = entry.Value - Time.fixedDeltaTime;
            if (entry.Value <= 0 || player.Data.IsDead)
            {
                Bloody.active.Remove(entry.Key);
                continue; // Skip the creation of the next blood drop, if the killer is dead or the time is up
            }

            _ = new Bloodytrail(player, bloodyPlayer);
        }
    }

    // Mini set adapted button cooldown for Vampire, Sheriff, Jackal, Sidekick, Warlock, Cleaner
    public static void miniCooldownUpdate()
    {
        if (Mini.mini != null && PlayerControl.LocalPlayer == Mini.mini)
        {
            var multiplier = Mini.isGrownUp() ? 0.66f : 2f;
            HudManagerStartPatch.sheriffKillButton.MaxTimer = Sheriff.cooldown * multiplier;
            HudManagerStartPatch.vampireKillButton.MaxTimer = Vampire.cooldown * multiplier;
            HudManagerStartPatch.jackalKillButton.MaxTimer = Jackal.cooldown * multiplier;
            HudManagerStartPatch.warlockCurseButton.MaxTimer = Warlock.cooldown * multiplier;
            HudManagerStartPatch.pavlovsdogsKillButton.MaxTimer = Pavlovsdogs.cooldown * multiplier;
            HudManagerStartPatch.witchSpellButton.MaxTimer = (Witch.cooldown + Witch.currentCooldownAddition) * multiplier;
            HudManagerStartPatch.ninjaButton.MaxTimer = Ninja.cooldown * multiplier;
            HudManagerStartPatch.thiefKillButton.MaxTimer = Thief.cooldown * multiplier;
            HudManagerStartPatch.swooperKillButton.MaxTimer = Swooper.cooldown * multiplier;
            HudManagerStartPatch.werewolfRampageButton.MaxTimer = Thief.cooldown * multiplier;
            HudManagerStartPatch.juggernautKillButton.MaxTimer = Thief.cooldown * multiplier;
        }
    }

    public static void PelicanUpdate()
    {
        if (Pelican.Player == null) return;
        if (Pelican.Player.IsAlive() && Pelican.eatenPlayers.Any(x => x == PlayerControl.LocalPlayer) && !InMeeting)
        {
            HudManager.Instance.PlayerCam.Target = Pelican.Player;
            PlayerControl.LocalPlayer.transform.position = new(-10f, 10f, 0f);
        }
    }

    public static void trapperUpdate()
    {
        if (Trapper.trapper == null || PlayerControl.LocalPlayer != Trapper.trapper ||
            Trapper.trapper.Data.IsDead) return;
        var (playerCompleted, _) = TasksHandler.taskInfo(Trapper.trapper.Data);
        if (playerCompleted == Trapper.rechargedTasks)
        {
            Trapper.rechargedTasks += Trapper.rechargeTasksNumber;
            if (Trapper.maxCharges > Trapper.charges) Trapper.charges++;
        }
    }

    public static void akujoUpdate()
    {
        if (Akujo.akujo == null || Akujo.akujo.Data.IsDead || PlayerControl.LocalPlayer != Akujo.akujo) return;
        Akujo.timeLeft = (int)Math.Ceiling(Akujo.timeLimit - (DateTime.UtcNow - Akujo.startTime).TotalSeconds);
        if (Akujo.timeLeft > 0)
        {
            if (Akujo.honmei == null)
            {
                if (HudManagerStartPatch.akujoTimeRemainingText != null)
                {
                    HudManagerStartPatch.akujoTimeRemainingText.text = TimeSpan.FromSeconds(Akujo.timeLeft).ToString(@"mm\:ss");
                }
                HudManagerStartPatch.akujoTimeRemainingText.enabled = !(MapBehaviour.Instance && MapBehaviour.Instance.IsOpen) &&
                  !MeetingHud.Instance &&
                  !ExileController.Instance;
            }
            else HudManagerStartPatch.akujoTimeRemainingText.enabled = false;
        }
        else if (Akujo.timeLeft <= 0)
        {
            if (Akujo.honmei == null || (Akujo.keeps?.Count < 1 && Akujo.forceKeeps))
            {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AkujoSuicide, SendOption.Reliable, -1);
                writer.Write(Akujo.akujo.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.akujoSuicide(Akujo.akujo.PlayerId);
            }
        }
    }

    private static void pavlovsownerUpdate()
    {
        if (Pavlovsdogs.arrow == null) return;

        foreach (var arrow in Pavlovsdogs.arrow) arrow.arrow.SetActive(false);

        if (Pavlovsdogs.pavlovsowner == null || Pavlovsdogs.pavlovsowner.Data.IsDead || PlayerControl.LocalPlayer != Pavlovsdogs.pavlovsowner) return;

        var index = 0;
        foreach (PlayerControl p in PlayerControl.AllPlayerControls)
        {
            if (!p.Data.IsDead && Pavlovsdogs.pavlovsdogs.Any(x => x == p))
            {
                if (index >= Pavlovsdogs.arrow.Count)
                {
                    Pavlovsdogs.arrow.Add(new Arrow(Pavlovsdogs.color));
                }
                else if (index < Pavlovsdogs.arrow.Count && Pavlovsdogs.arrow[index] != null)
                {
                    Pavlovsdogs.arrow[index].arrow.SetActive(true);
                    Pavlovsdogs.arrow[index].Update(p.transform.position, Pavlovsdogs.color);
                }
                index++;
            }
        }

    }

    public static void Postfix(PlayerControl __instance)
    {
        if (AmongUsClient.Instance.GameState != InnerNetClient.GameStates.Started ||
            GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return;

        // Mini and Morphling shrink
        MiniSizeUpdate(__instance);
        GiantSizeUpdate(__instance);

        if (PlayerControl.LocalPlayer == __instance)
        {
            // Update player outlines
            setBasePlayerOutlines();

            // Update Role Description
            refreshRoleDescription(__instance);

            // Update Player Info
            updatePlayerInfo();

            //Update pet visibility
            setPetVisibility();

            if (!InGame) return;

            impostorSetTarget();
            jackalSetTarget();
            akujoSetTarget();

            // EvilTrapper
            evilTrapperUpdate();
            // Time Master
            bendTimeUpdate();
            // Swooper
            swooperUpdate();
            // Prophet
            prophetUpdate();
            // Deputy
            deputyUpdate();
            // Detective
            detectiveUpdateFootPrints();
            // Vampire
            Garlic.UpdateAll();
            Trap.Update();
            // Engineer
            engineerUpdate();
            // Tracker
            trackerUpdate();
            // Pavlovsdogs
            pavlovsownerUpdate();
            // Check for deputy promotion on Sheriff disconnect
            deputyCheckPromotion();
            // Check for sidekick promotion on Jackal disconnect
            sidekickCheckPromotion();
            // Witness
            WitnessUpdate();
            // SecurityGuard
            securityGuardUpdate();
            // Snitch
            snitchUpdate();
            snitchTextUpdate();
            // undertaker
            undertakerDragBodyUpdate();
            // Amnisiac
            amnisiacUpdate();
            // BountyHunter
            bountyHunterUpdate();
            // Vulture
            vultureUpdate();
            radarUpdate();
            // Morphling and Camouflager
            morphlingAndCamouflagerUpdate();
            // Lawyer
            lawyerUpdate();
            // Executioner
            executionerUpdate();
            // Ninja
            NinjaTrace.UpdateAll();
            ninjaUpdate();
            // yoyo
            Silhouette.UpdateAll();
            // PartTimer
            partTimerUpdate();
            //Balancer
            Balancer.FixedUpdate();
            PelicanUpdate();

            hackerUpdate();
            swapperUpdate();
            // Hacker
            hackerUpdate();
            // Trapper
            trapperUpdate();
            // Akojo
            akujoUpdate();
            // Bait
            baitUpdate();
            // Bloody
            bloodyUpdate();
            // mini (for the cooldowns)
            miniCooldownUpdate();
            // Chameleon (invis stuff, timers)
            Chameleon.update();
            Bomb.update();
        }
    }

    private static void jackalSetTarget()
    {
        if (Jackal.jackal.Any(x => x.IsAlive() && x.PlayerId == PlayerControl.LocalPlayer.PlayerId))
        {
            var untargetablePlayers = new List<PlayerControl>();
            untargetablePlayers.AddRange(Jackal.jackal);
            if (Jackal.Sidekick != null) untargetablePlayers.Add(Jackal.Sidekick);
            if (Mini.mini != null && !Mini.isGrownUp()) untargetablePlayers.Add(Mini.mini);
            Jackal.currentTarget = SetTarget(untargetablePlayers: untargetablePlayers);
            SetPlayerOutline(Jackal.currentTarget, Palette.ImpostorRed);
        }
    }

    public static void akujoSetTarget()
    {
        if (Akujo.akujo == null || Akujo.akujo.Data.IsDead || PlayerControl.LocalPlayer != Akujo.akujo) return;
        var untargetables = new List<PlayerControl>();
        if (Akujo.honmei != null) untargetables.Add(Akujo.honmei);
        if (Akujo.keeps != null) untargetables.AddRange(Akujo.keeps);
        Akujo.currentTarget = SetTarget(untargetablePlayers: untargetables);
        if (Akujo.honmei == null || Akujo.keepsLeft > 0) SetPlayerOutline(Akujo.currentTarget, Akujo.color);
    }

    private static void impostorSetTarget()
    {
        if (!PlayerControl.LocalPlayer.Data.Role.IsImpostor || !PlayerControl.LocalPlayer.CanMove ||
            PlayerControl.LocalPlayer.Data.IsDead)
        {
            FastDestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(null);
            return;
        }

        PlayerControl target;
        if (Spy.spy != null)
        {
            if (Spy.impostorsCanKillAnyone)
            {
                target = SetTarget(false, true);
            }
            else
            {
                target = SetTarget(true, true, [Spy.spy]);
            }
        }
        else
        {
            target = SetTarget(true, true);
        }

        FastDestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(target); // Includes setPlayerOutline(target, Palette.ImpstorRed);
    }
}

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.WalkPlayerTo))]
internal class PlayerPhysicsWalkPlayerToPatch
{
    private static Vector2 offset = Vector2.zero;

    public static void Prefix(PlayerPhysics __instance)
    {
        var correctOffset = !isCamoComms && Camouflager.camouflageTimer <= 0f &&
                            !MushroomSabotageActive && (__instance.myPlayer == Mini.mini ||
                                (Morphling.morphling != null &&
                                 __instance.myPlayer == Morphling.morphling &&
                                 Morphling.morphTarget == Mini.mini &&
                                 Morphling.morphTimer > 0f));
        correctOffset = correctOffset && !(Mini.mini == Morphling.morphling && Morphling.morphTimer > 0f);
        if (correctOffset)
        {
            var currentScaling = (Mini.growingProgress() + 1) * 0.5f;
            __instance.myPlayer.Collider.offset = currentScaling * Mini.defaultColliderOffset * Vector2.down;
        }
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Revive))]
internal class PlayerControlRevivePatch
{
    public static void Postfix(PlayerControl __instance)
    {
        if (PlayerControl.LocalPlayer == __instance) CanSeeRoleInfo = false;

        if (__instance == Specter.Player) Specter.Player.clearAllTasks();

        RPCProcedure.clearGhostRoles(__instance.PlayerId);
        DeadPlayers.RemoveAll(x => x.Player == __instance);

        if (__instance.isLover() && Lovers.otherLover(__instance)?.IsDead() == true)
        {
            Lovers.otherLover(__instance)?.Revive();
        }

        if (Akujo.isAkujoTeam(__instance) && Akujo.otherLover(__instance)?.IsDead() == true)
        {
            Akujo.otherLover(__instance)?.Revive();
        }

        DeadBody[] array = Object.FindObjectsOfType<DeadBody>();
        for (var i = 0; i < array.Length; i++)
        {
            if (GameData.Instance.GetPlayerById(array[i].ParentId).PlayerId == __instance.PlayerId)
            {
                Object.Destroy(array[i].gameObject);
                break;
            }
        }
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdReportDeadBody))]
internal class BodyReportPatch
{
    public static bool Prefix(PlayerControl __instance)
    {
        if (ModOption.DisableMeeting) return false;
        handleVampireBiteOnBodyReport();
        handleBomberExplodeOnBodyReport();
        handleTrapperTrapOnBodyReport();
        return true;
    }

    private static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] GameData.PlayerInfo target)
    {
        Message($"报告玩家 {__instance.Data.PlayerName} 被报告尸体 {target?.PlayerName ?? "null"}", "CmdReportDeadBody");

        // Medic or Detective report
        var isMedicReport = Medic.medic != null && Medic.medic == PlayerControl.LocalPlayer &&
                            __instance.PlayerId == Medic.medic.PlayerId;
        var isDetectiveReport = Detective.detective != null &&
                                Detective.detective == PlayerControl.LocalPlayer &&
                                __instance.PlayerId == Detective.detective.PlayerId;
        var isSluethReport = Slueth.slueth != null && Slueth.slueth == PlayerControl.LocalPlayer &&
                             __instance.PlayerId == Slueth.slueth.PlayerId;
        if (isMedicReport || isDetectiveReport)
        {
            var deadPlayer = DeadPlayers?.Where(x => x.Player?.PlayerId == target?.PlayerId)?.FirstOrDefault();
            if (deadPlayer != null && deadPlayer.KillerIfExisting != null)
            {
                var timeSinceDeath = (float)(DateTime.UtcNow - deadPlayer.TimeOfDeath).TotalMilliseconds;
                var msg = "";
                var killer = deadPlayer.KillerIfExisting;
                float timer = (float)Math.Round(timeSinceDeath / 1000);
                if (Vortox.Reversal)
                {
                    timer += rnd.Next(-2, 3);
                    if (timer < 0) timer = 1;
                }
                if (isMedicReport)
                {
                    if (timer <= Medic.ReportNameDuration)
                    {
                        msg = $"尸检报告: 凶手似乎是 {killer.Data.PlayerName}!\n尸体在 {timer} 秒前死亡";
                    }
                    else if (timer <= Medic.ReportColorDuration)
                    {
                        var typeOfColor = isLighterColor(killer) ? "浅" : "深";
                        msg = $"尸检报告: 凶手的颜色似乎是 {typeOfColor} 色的!\n尸体在{timer}秒前死亡";
                    }
                    else
                    {
                        msg = $"尸检报告: 死亡时间太久，无法获取信息! \n尸体在{timer}秒前死亡";
                    }
                }
                else if (isDetectiveReport)
                {
                    if (timer <= Detective.reportNameDuration)
                    {
                        msg = $"尸检报告: 凶手的职业似乎是 {RoleInfo.getRoleInfoForPlayer(killer, false, false).First().Name} !\n尸体在 {timer} 秒前死亡";
                    }
                    else if (timer <= Detective.reportColorDuration)
                    {
                        msg = $"尸检报告: 凶手的阵营似乎是 {teamString(killer)} !\n尸体在{timer}秒前死亡";
                    }
                    else
                    {
                        msg = $"尸检报告: 死亡时间太久，无法获取信息\n尸体在 {timer} 秒前死亡";
                    }
                }

                if (!string.IsNullOrWhiteSpace(msg))
                {
                    if (AmongUsClient.Instance.AmClient && FastDestroyableSingleton<HudManager>.Instance)
                    {
                        FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, msg);

                        // Ghost Info
                        var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.ShareGhostInfo);
                        writer.Write(PlayerControl.LocalPlayer.PlayerId);
                        writer.Write((byte)RPCProcedure.GhostInfoTypes.GhostChat);
                        writer.Write(msg);
                        writer.EndRPC();
                    }

                    if (msg.Contains("who", StringComparison.OrdinalIgnoreCase))
                        FastDestroyableSingleton<UnityTelemetry>.Instance.SendWho();
                }
            }
        }

        if (Witness.Player.IsAlive())
        {
            var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.WitnessReport);
            writer.Write(target?.PlayerId ?? byte.MaxValue);
            writer.EndRPC();
            Witness.WitnessReport(target?.PlayerId ?? byte.MaxValue);
        }

        if (isSluethReport)
        {
            var reported = playerById(target?.PlayerId);
            Slueth.reported.TryAdd(reported);
        }
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Die))]
public static class PlayerDiePatch
{
    public static void Postfix(PlayerControl __instance)
    {
        if (!InGame || PlayerControl.LocalPlayer != __instance) return;
        if (ModOption.gameMode is CustomGamemodes.Classic or CustomGamemodes.Guesser) return;
        _ = new LateTask(() =>
        {
            CanSeeRoleInfo = true;
        }, 1f, "CanSeeRoleInfo");
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
public static class MurderPlayerPatch
{
    public static bool resetToCrewmate;
    public static bool resetToDead;

    public static void Prefix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
    {
        // Allow everyone to murder players
        resetToCrewmate = !__instance.Data.Role.IsImpostor;
        resetToDead = __instance.Data.IsDead;
        __instance.Data.Role.TeamType = RoleTeamTypes.Impostor;
        __instance.Data.IsDead = false;
    }

    public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
    {
        HandleMurderPostfix(__instance, target);
    }

    public static void HandleMurderPostfix(PlayerControl __instance, PlayerControl target)
    {
        // Collect dead player info
        var deathReason = __instance == target ? CustomDeathReason.Suicide : CustomDeathReason.Kill;
        var deadPlayer = new DeadPlayer(target, DateTime.UtcNow, deathReason, __instance);
        DeadPlayers.Add(deadPlayer);

        // Reset killer to crewmate if resetToCrewmate
        if (resetToCrewmate) __instance.Data.Role.TeamType = RoleTeamTypes.Crewmate;
        if (resetToDead) __instance.Data.IsDead = true;

        // Remove fake tasks when player dies
        if (target.hasFakeTasks() || target == Lawyer.lawyer || Pursuer.Player.Contains(target) || target == Thief.thief)
            target.clearAllTasks();

        // First kill (set before lover suicide)
        if (ModOption.firstKillName == "") ModOption.firstKillName = target.Data.PlayerName;

        // Lover suicide trigger on murder
        if ((Lovers.lover1 != null && target == Lovers.lover1) || (Lovers.lover2 != null && target == Lovers.lover2))
        {
            var otherLover = target == Lovers.lover1 ? Lovers.lover2 : Lovers.lover1;
            if (otherLover != null && !otherLover.Data.IsDead && Lovers.bothDie)
            {
                otherLover.MurderPlayer(otherLover);
                OverrideDeathReasonAndKiller(otherLover, CustomDeathReason.LoverSuicide);
            }
        }

        // Bait
        if (Bait.bait.FindAll(x => x.PlayerId == target.PlayerId).Count > 0)
        {
            float reportDelay = rnd.Next((int)Bait.reportDelayMin, (int)Bait.reportDelayMax + 1);
            Bait.active.Add(deadPlayer, reportDelay);

            if (Bait.showKillFlash && __instance == PlayerControl.LocalPlayer)
                showFlash(new Color(204f / 255f, 102f / 255f, 0f / 255f));
        }

        if (Aftermath.aftermath != null && Aftermath.aftermath == target && PlayerControl.LocalPlayer == __instance)
        {
            _ = new LateTask(() =>
            {
                Aftermath.afterTrigger(target.PlayerId, __instance.PlayerId);

            }, 0.2f, "Aftermath Is Die");
        }

        if (target.Data.Role.IsImpostor && AmongUsClient.Instance.AmHost)
        {
            LastImpostor.promoteToLastImpostor();
        }

        // Sidekick promotion trigger on exile
        if (Jackal.promotesToJackal && Jackal.Sidekick.IsAlive() &&
            Jackal.jackal.Any(x => x == __instance && x == PlayerControl.LocalPlayer))
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.SidekickPromotes, SendOption.Reliable);
            writer.Write(Jackal.Sidekick.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.sidekickPromotes(Jackal.Sidekick.PlayerId);
        }

        // Pursuer promotion trigger on murder (the host sends the call such that everyone recieves the update before a possible game End)
        if (target == Lawyer.target && AmongUsClient.Instance.AmHost && Lawyer.lawyer != null)
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.LawyerPromotesToPursuer, SendOption.Reliable);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            Lawyer.PromotesToPursuer();
        }

        if (target == Executioner.target && AmongUsClient.Instance.AmHost && Executioner.executioner != null)
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.ExecutionerPromotesRole, SendOption.Reliable);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            Executioner.PromotesRole();
        }

        if (target.PlayerId == Pelican.Player?.PlayerId && Pelican.eatenPlayers?.Count > 0)
        {
            foreach (var player in Pelican.eatenPlayers.ToArray().Where(p => p != null && p.Data.IsDead))
            {
                player.Revive();

                DeadPlayers.RemoveAll(x => x.Player.PlayerId == player.PlayerId);
                if (PlayerControl.LocalPlayer == player)
                {
                    HudManager.Instance.PlayerCam.Target = PlayerControl.LocalPlayer;
                    PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(Pelican.Player.transform.position);
                }
                continue;
            }
            Pelican.eatenPlayers = new();

            if (Pelican.Player == PlayerControl.LocalPlayer)
            {
                _ = new LateTask(() => { Pelican.PelicanDie(); }, 0.5f);
            }
        }

        // Undertaker Button Sync
        if (Undertaker.undertaker != null && PlayerControl.LocalPlayer == Undertaker.undertaker &&
            __instance == Undertaker.undertaker && HudManagerStartPatch.undertakerDragButton != null)
            HudManagerStartPatch.undertakerDragButton.Timer = Undertaker.dragingDelaiAfterKill;

        // Seer show flash and add dead player position
        if (Seer.seer != null &&
            (PlayerControl.LocalPlayer == Seer.seer || shouldShowGhostInfo()) &&
            !Seer.seer.Data.IsDead && Seer.seer != target && Seer.mode <= 1)
            showFlash(new Color(42f / 255f, 187f / 255f, 245f / 255f), message: GetString("seerShowInfoText"));
        Seer.deadBodyPositions?.Add(target.transform.position);

        // Tracker store body positions
        Tracker.deadBodyPositions?.Add(target.transform.position);

        // Medium add body
        if (Medium.deadBodies != null)
        {
            Medium.futureDeadBodies.Add(new Tuple<DeadPlayer, Vector3>(deadPlayer, target.transform.position));
        }

        // LastImpostor cooldown
        if (LastImpostor.lastImpostor != null && __instance == LastImpostor.lastImpostor && PlayerControl.LocalPlayer == __instance)
        {
            LastImpostor.lastImpostor.SetKillTimer(Mathf.Max(0f, ModOption.KillCooddown - LastImpostor.deduce));

            if (Vampire.vampire != null && Vampire.vampire.PlayerId == LastImpostor.lastImpostor.PlayerId)
                HudManagerStartPatch.vampireKillButton.MaxTimer = Vampire.cooldown - LastImpostor.deduce;
        }

        // Set Gambler cooldown
        if (Gambler.gambler != null && __instance == Gambler.gambler && PlayerControl.LocalPlayer == __instance)
        {
            var cooldown = Gambler.GetSuc() ? Gambler.minCooldown : Gambler.maxCooldown;
            Gambler.gambler.SetKillTimer(cooldown);
        }

        // Set bountyHunter cooldown
        if (BountyHunter.bountyHunter != null && PlayerControl.LocalPlayer == BountyHunter.bountyHunter &&
            __instance == BountyHunter.bountyHunter)
        {
            if (target == BountyHunter.bounty)
            {
                BountyHunter.bountyHunter.SetKillTimer(BountyHunter.bountyKillCooldown);
                BountyHunter.bountyUpdateTimer = 0f; // Force bounty update
            }
            else
            {
                BountyHunter.bountyHunter.SetKillTimer(ModOption.KillCooddown + BountyHunter.punishmentTime);
            }
        }

        // Mini Set Impostor Mini kill timer (Due to mini being a modifier, all "SetKillTimers" must have happened before this!)
        if (Mini.mini != null && __instance == Mini.mini && __instance == PlayerControl.LocalPlayer)
        {
            var multiplier = 1f;
            if (Mini.mini != null && PlayerControl.LocalPlayer == Mini.mini)
                multiplier = Mini.isGrownUp() ? 0.66f : 2f;
            Mini.mini.SetKillTimer(__instance.killTimer * multiplier);
        }

        // Cleaner Button Sync
        if (Cleaner.cleaner != null && PlayerControl.LocalPlayer == Cleaner.cleaner &&
            __instance == Cleaner.cleaner && HudManagerStartPatch.cleanerCleanButton != null)
            HudManagerStartPatch.cleanerCleanButton.Timer = Cleaner.cleaner.killTimer;

        // Witch Button Sync
        if (Witch.triggerBothCooldowns && Witch.witch != null &&
            PlayerControl.LocalPlayer == Witch.witch && __instance == Witch.witch &&
            HudManagerStartPatch.witchSpellButton != null)
            HudManagerStartPatch.witchSpellButton.Timer = HudManagerStartPatch.witchSpellButton.MaxTimer;

        // Bomber Button Sync
        if (Bomber.triggerBothCooldowns && Bomber.bomber != null &&
            PlayerControl.LocalPlayer == Bomber.bomber && __instance == Bomber.bomber &&
            HudManagerStartPatch.bomberBombButton != null)
            HudManagerStartPatch.bomberBombButton.Timer = HudManagerStartPatch.bomberBombButton.MaxTimer;

        // Warlock Button Sync
        if (Warlock.warlock != null && PlayerControl.LocalPlayer == Warlock.warlock &&
            __instance == Warlock.warlock && HudManagerStartPatch.warlockCurseButton != null)
            if (Warlock.warlock.killTimer > HudManagerStartPatch.warlockCurseButton.Timer)
                HudManagerStartPatch.warlockCurseButton.Timer = Warlock.warlock.killTimer;
        // Ninja Button Sync
        if (Ninja.ninja != null && PlayerControl.LocalPlayer == Ninja.ninja && __instance == Ninja.ninja &&
            HudManagerStartPatch.ninjaButton != null)
            HudManagerStartPatch.ninjaButton.Timer = HudManagerStartPatch.ninjaButton.MaxTimer;

        // EvilTrapper peforms normal kills
        if (EvilTrapper.evilTrapper != null && PlayerControl.LocalPlayer == EvilTrapper.evilTrapper && __instance == EvilTrapper.evilTrapper)
        {
            if (KillTrap.isTrapped(target) && !EvilTrapper.isTrapKill)  // トラップにかかっている対象をキルした場合のボーナス
            {
                EvilTrapper.evilTrapper.killTimer = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown - EvilTrapper.bonusTime;
                HudManagerStartPatch.evilTrapperSetTrapButton.Timer = EvilTrapper.cooldown - EvilTrapper.bonusTime;
            }
            else if (KillTrap.isTrapped(target) && EvilTrapper.isTrapKill)  // トラップキルした場合のペナルティ
            {
                EvilTrapper.evilTrapper.killTimer = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
                HudManagerStartPatch.evilTrapperSetTrapButton.Timer = EvilTrapper.cooldown;
            }
            else // トラップにかかっていない対象を通常キルした場合はペナルティーを受ける
            {
                EvilTrapper.evilTrapper.killTimer = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + EvilTrapper.penaltyTime;
                HudManagerStartPatch.evilTrapperSetTrapButton.Timer = EvilTrapper.cooldown + EvilTrapper.penaltyTime;
            }
            if (!EvilTrapper.isTrapKill)
            {
                MessageWriter writer;
                writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.ClearTrap, SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.clearTrap();
            }
            EvilTrapper.isTrapKill = false;
        }

        // Add Bloody Modifier
        if (Bloody.bloody.FindAll(x => x.PlayerId == target.PlayerId).Count > 0)
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.Bloody, SendOption.Reliable);
            writer.Write(__instance.PlayerId);
            writer.Write(target.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.bloody(__instance.PlayerId, target.PlayerId);
        }

        // VIP Modifier
        if (Vip.vip.FindAll(x => x.PlayerId == target.PlayerId).Count > 0)
        {
            var color = Color.yellow;
            if (Vip.showColor)
            {
                color = Color.white;
                if (target.Data.Role.IsImpostor) color = Color.red;
                else if (RoleInfo.getRoleInfoForPlayer(target, false).FirstOrDefault().roleType == RoleType.Neutral) color = Color.blue;
            }

            showFlash(color, 1.75f);
        }

        // Snitch

        // Akujo Lovers trigger suicide
        if ((Akujo.akujo != null && target == Akujo.akujo) || (Akujo.honmei != null && target == Akujo.honmei))
        {
            PlayerControl akujoPartner = target == Akujo.akujo ? Akujo.honmei : Akujo.akujo;
            if (akujoPartner != null && !akujoPartner.Data.IsDead)
            {
                akujoPartner.MurderPlayer(akujoPartner, MurderResultFlags.Succeeded);
                OverrideDeathReasonAndKiller(akujoPartner, CustomDeathReason.LoverSuicide);
            }
        }
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetKillTimer))]
internal class PlayerControlSetCoolDownPatch
{
    public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] float time)
    {
        if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return true;
        if (GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown <= 0f) return false;
        var multiplier = 1f;
        var addition = 0f;
        if (Mini.mini != null && PlayerControl.LocalPlayer == Mini.mini)
            multiplier = Mini.isGrownUp() ? 0.66f : 2f;
        if (BountyHunter.bountyHunter != null && PlayerControl.LocalPlayer == BountyHunter.bountyHunter)
            addition = BountyHunter.punishmentTime;
        if (Gambler.gambler != null && PlayerControl.LocalPlayer == Gambler.gambler)
            addition = Gambler.maxCooldown - GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
        if (LastImpostor.lastImpostor != null && PlayerControl.LocalPlayer == LastImpostor.lastImpostor)
            addition -= LastImpostor.deduce;

        __instance.killTimer = Mathf.Clamp(time, 0f, (ModOption.KillCooddown * multiplier) + addition);
        FastDestroyableSingleton<HudManager>.Instance.KillButton.SetCoolDown(__instance.killTimer, (ModOption.KillCooddown * multiplier) + addition);
        return false;
    }
}

[HarmonyPatch(typeof(KillAnimation), nameof(KillAnimation.CoPerformKill))]
internal class KillAnimationCoPerformKillPatch
{
    public static bool hideNextAnimation;

    public static void Prefix(KillAnimation __instance, [HarmonyArgument(0)] ref PlayerControl source,
        [HarmonyArgument(1)] ref PlayerControl target)
    {
        if (hideNextAnimation)
            source = target;
        hideNextAnimation = false;
    }
}

[HarmonyPatch(typeof(KillAnimation), nameof(KillAnimation.SetMovement))]
internal class KillAnimationSetMovementPatch
{
    private static int? colorId;

    public static void Prefix(PlayerControl source, bool canMove)
    {
        var color = source.cosmetics.currentBodySprite.BodySprite.material.GetColor("_BodyColor");
        if (Morphling.morphling != null && source.Data.PlayerId == Morphling.morphling.PlayerId)
        {
            var index = Palette.PlayerColors.IndexOf(color);
            if (index != -1) colorId = index;
        }
    }

    public static void Postfix(PlayerControl source, bool canMove)
    {
        if (colorId.HasValue) source.RawSetColor(colorId.Value);
        colorId = null;
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Exiled))]
public static class ExilePlayerPatch
{
    public static void Postfix(PlayerControl __instance)
    {
        // Collect dead player info
        var deadPlayer = new DeadPlayer(__instance, DateTime.UtcNow, CustomDeathReason.Exile, null);
        DeadPlayers.Add(deadPlayer);

        if (MeetingHud.Instance)
        {
            foreach (var p in MeetingHud.Instance.playerStates)
            {
                if (p.TargetPlayerId == __instance.PlayerId)
                {
                    p.SetDead(p.DidReport, true);
                    p.Overlay.gameObject.SetActive(true);
                    MeetingHudPatch.swapperCheckAndReturnSwap(MeetingHud.Instance, p.TargetPlayerId);
                }
            }
        }

        _ = new LateTask(() => { if (__instance == PlayerControl.LocalPlayer) CanSeeRoleInfo = true; }, 0.5f, "CanSeeRoleInfo");

        // Remove fake tasks when player dies
        if (__instance.hasFakeTasks() || __instance == Pursuer.Player.Contains(__instance) || __instance == Thief.thief)
            __instance.clearAllTasks();

        // Lover suicide trigger on exile
        if (__instance.isLover() && Lovers.otherLover(__instance) != null)
        {
            var otherLover = Lovers.otherLover(__instance);
            if (otherLover != null && !otherLover.Data.IsDead && Lovers.bothDie)
            {
                otherLover.Exiled();
                OverrideDeathReasonAndKiller(otherLover, CustomDeathReason.LoverSuicide);
            }
        }

        if (__instance.PlayerId == Pelican.Player?.PlayerId && Pelican.eatenPlayers?.Count > 0)
        {
            foreach (var player in Pelican.eatenPlayers.Where(p => p != null && p.Data.IsDead))
            {
                if (PlayerControl.LocalPlayer == player)
                {
                    HudManager.Instance.PlayerCam.Target = PlayerControl.LocalPlayer;
                    PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(Pelican.Player.transform.position);
                }
                continue;
            }
            Pelican.eatenPlayers = new();

            Pelican.PelicanDie();
        }

        if (AmongUsClient.Instance.AmHost)
        {
            LastImpostor.promoteToLastImpostor();
        }

        // Sidekick promotion trigger on exile
        if (Jackal.promotesToJackal && Jackal.Sidekick.IsAlive() &&
            Jackal.jackal.Any(x => x == __instance && x == PlayerControl.LocalPlayer))
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.SidekickPromotes, SendOption.Reliable);
            writer.Write(Jackal.Sidekick.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.sidekickPromotes(Jackal.Sidekick.PlayerId);
        }
        if (Lawyer.lawyer != null && __instance == Lawyer.target)
        {
            if (AmongUsClient.Instance.AmHost && ((Lawyer.target != Jester.jester) || Lawyer.targetWasGuessed))
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.LawyerPromotesToPursuer, SendOption.Reliable);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Lawyer.PromotesToPursuer();
            }
        }
        if (Executioner.executioner != null && __instance == Executioner.target)
        {
            if (AmongUsClient.Instance.AmHost && Executioner.targetWasGuessed)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.ExecutionerPromotesRole, SendOption.Reliable);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Executioner.PromotesRole();
            }
        }

        // Akujo Partner suicide
        if ((Akujo.akujo != null && Akujo.akujo == __instance) || (Akujo.honmei != null && Akujo.honmei == __instance))
        {
            PlayerControl akujoPartner = __instance == Akujo.akujo ? Akujo.honmei : Akujo.akujo;
            if (akujoPartner != null && !akujoPartner.Data.IsDead)
            {
                akujoPartner.Exiled();
                OverrideDeathReasonAndKiller(akujoPartner, CustomDeathReason.LoverSuicide);
            }

            if (MeetingHud.Instance && akujoPartner != null)
            {
                foreach (PlayerVoteArea pva in MeetingHud.Instance.playerStates)
                {
                    if (pva.VotedFor != akujoPartner.PlayerId) continue;
                    pva.UnsetVote();
                    var voteAreaPlayer = playerById(pva.TargetPlayerId);
                    if (!voteAreaPlayer.AmOwner) continue;
                    MeetingHud.Instance.ClearVote();
                }

                if (AmongUsClient.Instance.AmHost)
                    MeetingHud.Instance.CheckForEndVoting();
            }
        }
    }
}

[HarmonyPatch]
public static class DisconnectPatch
{
    [HarmonyPatch(typeof(GameData), nameof(GameData.HandleDisconnect), [typeof(PlayerControl), typeof(DisconnectReasons)]), HarmonyPostfix]
    public static void DisconnectPostfix(PlayerControl player, DisconnectReasons reason)
    {
        Message($"玩家 {player?.Data?.PlayerName ?? "null"} 断开连接 {reason}", "HandleDisconnect");
        if (InGame)
        {
            if (player.isLover())
            {
                Lovers.clearAndReload();
            }

            if (Lawyer.lawyer != null && Lawyer.target == player)
            {
                Lawyer.PromotesToPursuer();
            }

            if (Executioner.executioner != null && Executioner.target == player)
            {
                Executioner.PromotesRole();
            }
        }
    }


    [HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.DisconnectInternal)), HarmonyPrefix]
    public static void InnerNetPrefix(InnerNetClient __instance, DisconnectReasons reason, string stringReason)
    {
        Info($"断开连接 {reason}:{stringReason}, Ping:{__instance.Ping}", "InnerNet");
    }
}
