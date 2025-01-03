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

    private static PlayerControl setTarget(bool onlyCrewmates = false, bool targetPlayersInVents = false,
        List<PlayerControl> untargetablePlayers = null, PlayerControl targetingPlayer = null)
    {
        PlayerControl result = null;
        var num = GameOptionsData.KillDistances[Mathf.Clamp(GameOptionsManager.Instance.currentNormalGameOptions.KillDistance, 0, 3)];
        if (!MapUtilities.CachedShipStatus) return result;
        if (targetingPlayer == null) targetingPlayer = CachedPlayer.LocalPlayer.PlayerControl;
        if (targetingPlayer.Data.IsDead) return result;
        if (PlayerControl.LocalPlayer == Arsonist.arsonist) num += 0.5f;

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

    private static void setPlayerOutline(PlayerControl target, Color color)
    {
        if (target == null || target.cosmetics?.currentBodySprite?.BodySprite == null) return;

        color = color.SetAlpha(Chameleon.visibility(target.PlayerId));

        target.cosmetics.currentBodySprite.BodySprite.material.SetFloat("_Outline", 1f);
        target.cosmetics.currentBodySprite.BodySprite.material.SetColor("_OutlineColor", color);
    }

    // Update functions

    private static void setBasePlayerOutlines()
    {
        var local = CachedPlayer.LocalPlayer.PlayerControl;
        foreach (PlayerControl target in CachedPlayer.AllPlayers)
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
        var local = CachedPlayer.LocalPlayer.PlayerControl;
        foreach (PlayerControl p in CachedPlayer.AllPlayers)
        {
            var playerVoteArea = MeetingHud.Instance?.playerStates?.FirstOrDefault(x => x.TargetPlayerId == p.PlayerId);
            if (playerVoteArea != null && playerVoteArea.ColorBlindName.gameObject.active)
            {
                playerVoteArea.ColorBlindName.transform.localPosition = new Vector3(-0.93f, -0.2f, -0.1f);
                playerVoteArea.ColorBlindName.fontSize *= 1.75f;
            }
            p.cosmetics.nameText.transform.parent.SetLocalZ(-0.0001f);

            bool canSeeRole = (Lawyer.lawyerKnowsRole && local == Lawyer.lawyer && p == Lawyer.target) ||
                (PartTimer.knowsRole && local == PartTimer.partTimer && p == PartTimer.target) ||
                (local == PartTimer.target && p == PartTimer.partTimer) ||
                (Akujo.knowsRoles && local == Akujo.akujo &&
                    (p == Akujo.honmei || Akujo.keeps.Any(x => x.PlayerId == p.PlayerId))) ||
                (ModOption.impostorSeeRoles && Spy.spy == null && PlayerControl.LocalPlayer.isImpostor() &&
                    PlayerControl.LocalPlayer.IsAlive() && p.isImpostor() && p.IsAlive());

            bool reported = ((local == Slueth.slueth && Slueth.reported.Any(x => x.PlayerId == p.PlayerId)) ||
                             (local == Poucher.poucher && Poucher.killed.Any(x => x.PlayerId == p.PlayerId))) && p.IsDead();

            bool revealed = (Mayor.mayor == p && Mayor.Revealed) || (WolfLord.Player == p && WolfLord.Revealed);

            if (p == local || local.Data.IsDead || canSeeRole || reported || revealed)
            {
                var roleNames = RoleInfo.GetRolesString(p, true, false, false, true);
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
                else if (reported)
                {
                    meetingInfoText = playerInfoText = mainRole;
                }
                else if (local.IsAlive() && Mayor.mayor == p && Mayor.Revealed)
                {
                    meetingInfoText = cs(Mayor.color, "Mayor".Translate());
                }
                else if (local.IsAlive() && WolfLord.Player == p && WolfLord.Revealed)
                {
                    meetingInfoText = cs(WolfLord.color, "WolfLord".Translate());
                }
                else if (canSeeRole)
                {
                    meetingInfoText = playerInfoText = roleNames;
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
        var localalive = !CachedPlayer.LocalPlayer.Data.IsDead;
        foreach (var player in CachedPlayer.AllPlayers)
        {
            var playeralive = !player.Data.IsDead;
            player.PlayerControl.cosmetics.SetPetVisible((localalive && playeralive) || !localalive);
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
                    if (CachedPlayer.LocalPlayer.PlayerControl.inVent)
                        foreach (var vent in MapUtilities.CachedShipStatus.AllVents)
                        {
                            vent.CanUse(CachedPlayer.LocalPlayer.Data, out bool canUse, out bool couldUse);
                            if (canUse)
                            {
                                CachedPlayer.LocalPlayer.PlayerPhysics.RpcExitVent(vent.Id);
                                vent.SetButtons(false);
                            }
                        }

                    // Set position
                    CachedPlayer.LocalPlayer.transform.position = next.Item1;
                }
                else if (localPlayerPositions.Any(x => x.Item2))
                {
                    CachedPlayer.LocalPlayer.transform.position = next.Item1;
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
                CachedPlayer.LocalPlayer.PlayerControl.moveable = true;
            }
        }
        else
        {
            while (localPlayerPositions.Count >= Mathf.Round(TimeMaster.rewindTime / Time.fixedDeltaTime))
                localPlayerPositions.RemoveAt(localPlayerPositions.Count - 1);
            localPlayerPositions.Insert(0,
                new Tuple<Vector3, bool>(CachedPlayer.LocalPlayer.transform.position,
                    CachedPlayer.LocalPlayer.PlayerControl.CanMove)); // CanMove = CanMove
        }
    }


    public static void deputyCheckPromotion(bool isMeeting = false)
    {
        // If LocalPlayer is Deputy, the Sheriff is disconnected and Deputy promotion is enabled, then trigger promotion
        if (Deputy.deputy == null || Deputy.deputy != CachedPlayer.LocalPlayer.PlayerControl) return;
        if (Deputy.promotesToSheriff == 0 || Deputy.deputy.Data.IsDead ||
            (Deputy.promotesToSheriff == 2 && !isMeeting)) return;
        if (Sheriff.sheriff == null || Sheriff.sheriff?.Data?.Disconnected == true || Sheriff.sheriff.Data.IsDead)
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                (byte)CustomRPC.DeputyPromotes, SendOption.Reliable);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.deputyPromotes();
        }
    }

    private static void detectiveUpdateFootPrints()
    {
        if (Detective.detective == null
            || Detective.detective != CachedPlayer.LocalPlayer.PlayerControl
            || InMeeting
            || Detective.detective.IsDead()) return;

        Detective.timer -= Time.fixedDeltaTime;
        if (Detective.timer <= 0f)
        {
            Detective.timer = Detective.footprintIntervall;
            foreach (PlayerControl player in CachedPlayer.AllPlayers)
                if (player != null && player != CachedPlayer.LocalPlayer.PlayerControl && !player.Data.IsDead && !player.inVent)
                    FootprintHolder.Instance.MakeFootprint(player);
        }
    }

    private static void sidekickCheckPromotion()
    {
        // If LocalPlayer is Sidekick, the Jackal is disconnected and Sidekick promotion is enabled, then trigger promotion
        if (Jackal.sidekick.IsDead() || !Jackal.promotesToJackal || Jackal.sidekick != CachedPlayer.LocalPlayer.PlayerControl) return;
        var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
            (byte)CustomRPC.SidekickPromotes, SendOption.Reliable);
        writer.Write(Jackal.sidekick.PlayerId);
        AmongUsClient.Instance.FinishRpcImmediately(writer);
        RPCProcedure.sidekickPromotes(Jackal.sidekick.PlayerId);
    }

    private static void deputyUpdate()
    {
        if (CachedPlayer.LocalPlayer.PlayerControl == null ||
            !Deputy.handcuffedKnows.ContainsKey(CachedPlayer.LocalPlayer.PlayerId)) return;

        if (Deputy.handcuffedKnows[CachedPlayer.LocalPlayer.PlayerId] <= 0)
        {
            Deputy.handcuffedKnows.Remove(CachedPlayer.LocalPlayer.PlayerId);
            // Resets the buttons
            Deputy.setHandcuffedKnows(false);

            // Ghost info
            var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                (byte)CustomRPC.ShareGhostInfo, SendOption.Reliable);
            writer.Write(CachedPlayer.LocalPlayer.PlayerId);
            writer.Write((byte)RPCProcedure.GhostInfoTypes.HandcuffOver);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }
    }

    private static void engineerUpdate()
    {
        var jackalHighlight = Engineer.highlightForTeamJackal &&
                              (Jackal.jackal.Any(x => x == CachedPlayer.LocalPlayer.PlayerControl) ||
                               CachedPlayer.LocalPlayer.PlayerControl == Jackal.sidekick);
        var impostorHighlight = Engineer.highlightForImpostors && CachedPlayer.LocalPlayer.Data.Role.IsImpostor;
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
        if (Swooper.isInvisable && Swooper.swoopTimer <= 0 && Swooper.swooper == CachedPlayer.LocalPlayer.PlayerControl)
        {
            var invisibleWriter = AmongUsClient.Instance.StartRpcImmediately(
                CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.SetSwoop, SendOption.Reliable);
            invisibleWriter.Write(Swooper.swooper.PlayerId);
            invisibleWriter.Write(byte.MaxValue);
            AmongUsClient.Instance.FinishRpcImmediately(invisibleWriter);
            RPCProcedure.setSwoop(Swooper.swooper.PlayerId, byte.MaxValue);
        }
        if (Jackal.isInvisable && Jackal.swoopTimer <= 0 && Jackal.jackal.Any(x => x == CachedPlayer.LocalPlayer.PlayerControl))
        {
            var invisibleWriter = AmongUsClient.Instance.StartRpcImmediately(
                CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.SetJackalSwoop, SendOption.Reliable);
            invisibleWriter.Write(CachedPlayer.LocalPlayer.PlayerId);
            invisibleWriter.Write(byte.MaxValue);
            AmongUsClient.Instance.FinishRpcImmediately(invisibleWriter);
            RPCProcedure.setJackalSwoop(CachedPlayer.LocalPlayer.PlayerId, byte.MaxValue);
        }
    }

    private static void ninjaUpdate()
    {
        if (Ninja.isInvisable && Ninja.invisibleTimer <= 0 && Ninja.ninja == CachedPlayer.LocalPlayer.PlayerControl)
        {
            var invisibleWriter = AmongUsClient.Instance.StartRpcImmediately(
                CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.SetInvisible, SendOption.Reliable);
            invisibleWriter.Write(Ninja.ninja.PlayerId);
            invisibleWriter.Write(byte.MaxValue);
            AmongUsClient.Instance.FinishRpcImmediately(invisibleWriter);
            RPCProcedure.setInvisible(Ninja.ninja.PlayerId, byte.MaxValue);
        }

        if (Ninja.arrow?.arrow != null)
        {
            if (Ninja.ninja == null || Ninja.ninja != CachedPlayer.LocalPlayer.PlayerControl ||
                !Ninja.knowsTargetLocation)
            {
                Ninja.arrow.arrow.SetActive(false);
                return;
            }

            if (Ninja.ninjaMarked != null && !CachedPlayer.LocalPlayer.Data.IsDead)
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

        var local = CachedPlayer.LocalPlayer.PlayerControl;

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
            if (Tracker.tracker == null || CachedPlayer.LocalPlayer.PlayerControl != Tracker.tracker)
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
        if (Tracker.tracker != null && Tracker.tracker == CachedPlayer.LocalPlayer.PlayerControl && Tracker.corpsesTrackingTimer >= 0f && !Tracker.tracker.Data.IsDead)
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
            p.transform.localScale = new Vector3(Giant.size, Giant.size, 1f);
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
            CachedPlayer.LocalPlayer.PlayerControl != SecurityGuard.securityGuard ||
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
        var local = CachedPlayer.LocalPlayer.PlayerControl;

        var forImpTeam = local.Data.Role.IsImpostor;
        var forKillerTeam = Snitch.Team == Snitch.includeNeutralTeam.KillNeutral && isKillerNeutral(local);
        var forEvilTeam = Snitch.Team == Snitch.includeNeutralTeam.EvilNeutral && isEvilNeutral(local);
        var forNeutraTeam = Snitch.Team == Snitch.includeNeutralTeam.AllNeutral && isNeutral(local);

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
            foreach (PlayerControl p in CachedPlayer.AllPlayers)
            {
                var arrowForImp = p.Data.Role.IsImpostor;
                if (Mimic.mimic == p) arrowForImp = true;
                var arrowForKillerTeam = Snitch.Team == Snitch.includeNeutralTeam.KillNeutral && isKillerNeutral(p);
                var arrowForEvilTeam = Snitch.Team == Snitch.includeNeutralTeam.EvilNeutral && isEvilNeutral(p);
                var arrowForNeutraTeam = Snitch.Team == Snitch.includeNeutralTeam.AllNeutral && isNeutral(p);
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

        var snitchIsDead = Snitch.snitch.Data.IsDead;
        var local = CachedPlayer.LocalPlayer.PlayerControl;

        var isDead = local == Snitch.snitch || local.Data.IsDead;
        var forImpTeam = local.Data.Role.IsImpostor;
        var forKillerTeam = Snitch.Team == Snitch.includeNeutralTeam.KillNeutral && isKillerNeutral(local);
        var forEvilTeam = Snitch.Team == Snitch.includeNeutralTeam.EvilNeutral && isEvilNeutral(local);
        var forNeutraTeam = Snitch.Team == Snitch.includeNeutralTeam.AllNeutral && isNeutral(local);

        if (numberOfTasks <= Snitch.taskCountForReveal && (forImpTeam || forKillerTeam || forEvilTeam || forNeutraTeam || isDead))
        {
            if (Snitch.text == null && !snitchIsDead)
            {
                Snitch.text = Object.Instantiate(FastDestroyableSingleton<HudManager>.Instance.KillButton.cooldownTimerText, FastDestroyableSingleton<HudManager>.Instance.transform);
                Snitch.text.enableWordWrapping = false;
                Snitch.text.transform.localScale = Vector3.one * 0.75f;
                Snitch.text.transform.localPosition += new Vector3(0f, 1.8f, -69f);
                Snitch.text.gameObject.SetActive(true);
            }
            else if (!snitchIsDead)
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
            || CachedPlayer.LocalPlayer.PlayerControl != PartTimer.partTimer
            || PartTimer.partTimer.IsDead()) return;

        if (PartTimer.target != null && PartTimer.target.IsDead())
        {
            var playerInfoTransform = PartTimer.target?.cosmetics.nameText.transform.parent.FindChild("Info");
            var playerInfo = playerInfoTransform?.GetComponent<TextMeshPro>();
            if (playerInfo != null) playerInfo.text = "";

            var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
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
        if (BountyHunter.bountyHunter == null || CachedPlayer.LocalPlayer.PlayerControl != BountyHunter.bountyHunter) return;

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
            foreach (PlayerControl p in CachedPlayer.AllPlayers)
                if (!p.Data.IsDead && !p.Data.Disconnected && p != p.Data.Role.IsImpostor && p != Spy.spy &&
                    (p != Mini.mini || Mini.isGrownUp()) && (Lovers.otherLover(BountyHunter.bountyHunter) == null || p != Lovers.otherLover(BountyHunter.bountyHunter)))
                    possibleTargets.Add(p);
            if (possibleTargets.Count == 0) return;
            BountyHunter.bounty = possibleTargets[rnd.Next(0, possibleTargets.Count)];
            if (BountyHunter.bounty == null) return;

            // Ghost Info
            var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                (byte)CustomRPC.ShareGhostInfo, SendOption.Reliable);
            writer.Write(CachedPlayer.LocalPlayer.PlayerId);
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
        if (BountyHunter.showArrow && BountyHunter.bounty != null)
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
        if (Vulture.vulture == null || CachedPlayer.LocalPlayer.PlayerControl != Vulture.vulture ||
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
        if (Amnisiac.Player.Any(x => x.PlayerId == CachedPlayer.LocalId && x.IsAlive()))
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
            if (CachedPlayer.LocalPlayer.PlayerControl == EvilTrapper.evilTrapper && KillTrap.traps.Count != 0 && !KillTrap.hasTrappedPlayer() && !EvilTrapper.meetingFlag)
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
                            var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.ActivateTrap, SendOption.Reliable, -1);
                            writer.Write(trap.Key);
                            writer.Write(CachedPlayer.LocalPlayer.PlayerControl.PlayerId);
                            writer.Write(p.PlayerId);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            RPCProcedure.activateTrap(trap.Key, EvilTrapper.evilTrapper.PlayerId, p.PlayerId);
                            break;
                        }
                    }
                }
            }

            if (CachedPlayer.LocalPlayer.PlayerControl == EvilTrapper.evilTrapper && KillTrap.hasTrappedPlayer() && !EvilTrapper.meetingFlag)
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
                            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.DisableTrap, SendOption.Reliable, -1);
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
        if (Radar.radar == null || CachedPlayer.LocalPlayer.PlayerControl != Radar.radar || Radar.localArrows == null || InMeeting) return;
        if (Radar.radar.Data.IsDead)
        {
            foreach (var arrow in Radar.localArrows) Object.Destroy(arrow.arrow);
            Radar.localArrows = new();
            return;
        }

        var arrowUpdate = true;
        var index = 0;

        if (arrowUpdate && !CachedPlayer.LocalPlayer.Data.IsDead)
        {
            foreach (var arrow in Radar.localArrows) Object.Destroy(arrow.arrow);
            Radar.ClosestPlayer = GetClosestPlayer(PlayerControl.LocalPlayer,
                PlayerControl.AllPlayerControls.ToArray().ToList());
            Radar.localArrows = new();
        }


        foreach (PlayerControl player in CachedPlayer.AllPlayers)
        {
            if (arrowUpdate && !CachedPlayer.LocalPlayer.Data.IsDead)
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
            var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
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
                foreach (PlayerControl player in CachedPlayer.AllPlayers)
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
        if (Lawyer.lawyer == null || Lawyer.lawyer != CachedPlayer.LocalPlayer.PlayerControl) return;

        // Promote to Pursuer
        if (Lawyer.target != null && Lawyer.target.Data.Disconnected && !Lawyer.lawyer.Data.IsDead)
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                (byte)CustomRPC.LawyerPromotesToPursuer, SendOption.Reliable);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            Lawyer.PromotesToPursuer();
        }
    }

    public static void executionerUpdate()
    {
        if (Executioner.executioner == null || Executioner.executioner != CachedPlayer.LocalPlayer.PlayerControl) return;

        // Promote to Pursuer
        if (Executioner.target != null && Executioner.target.Data.Disconnected && !Executioner.executioner.Data.IsDead)
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                (byte)CustomRPC.ExecutionerPromotesRole, SendOption.Reliable);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            Executioner.PromotesRole();
        }
    }

    public static void hackerUpdate()
    {
        if (Hacker.hacker == null || CachedPlayer.LocalPlayer.PlayerControl != Hacker.hacker ||
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
        if (Swapper.swapper == null || CachedPlayer.LocalPlayer.PlayerControl != Swapper.swapper ||
            CachedPlayer.LocalPlayer.Data.IsDead) return;
        var (playerCompleted, _) = TasksHandler.taskInfo(CachedPlayer.LocalPlayer.Data);
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
                    entry.Key.KillerIfExisting.PlayerId == CachedPlayer.LocalPlayer.PlayerId)
                {

                    handleVampireBiteOnBodyReport(); // Manually call Vampire handling, since the CmdReportDeadBody Prefix won't be called

                    handleBomberExplodeOnBodyReport(); // Manually call Vampire handling, since the CmdReportDeadBody Prefix won't be called
                    RPCProcedure.uncheckedCmdReportDeadBody(entry.Key.KillerIfExisting.PlayerId,
                        entry.Key.Player.PlayerId);

                    var writer = AmongUsClient.Instance.StartRpcImmediately(
                        CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.UncheckedCmdReportDeadBody,
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
        if (Mini.mini != null && CachedPlayer.LocalPlayer.PlayerControl == Mini.mini)
        {
            var multiplier = Mini.isGrownUp() ? 0.66f : 2f;
            HudManagerStartPatch.sheriffKillButton.MaxTimer = Sheriff.cooldown * multiplier;
            HudManagerStartPatch.vampireKillButton.MaxTimer = Vampire.cooldown * multiplier;
            HudManagerStartPatch.jackalKillButton.MaxTimer = Jackal.cooldown * multiplier;
            HudManagerStartPatch.sidekickKillButton.MaxTimer = Jackal.cooldown * multiplier;
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

    public static void trapperUpdate()
    {
        if (Trapper.trapper == null || CachedPlayer.LocalPlayer.PlayerControl != Trapper.trapper ||
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
        if (Akujo.akujo == null || Akujo.akujo.Data.IsDead || CachedPlayer.LocalPlayer.PlayerControl != Akujo.akujo) return;
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
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.AkujoSuicide, SendOption.Reliable, -1);
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

        if (Pavlovsdogs.pavlovsowner == null || Pavlovsdogs.pavlovsowner.Data.IsDead || CachedPlayer.LocalPlayer.PlayerControl != Pavlovsdogs.pavlovsowner) return;

        var index = 0;
        foreach (PlayerControl p in CachedPlayer.AllPlayers)
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

        if (CachedPlayer.LocalPlayer.PlayerControl == __instance)
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

            UpdateSetTarget();
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

    public static void UpdateSetTarget()
    {
        if (InMeeting) return;

        impostorSetTarget();
        morphlingSetTarget();
        vampireSetTarget();
        eraserSetTarget();
        blackMailerSetTarget();
        ninjaSetTarget();
        witchSetTarget();
        warlockSetTarget();
        bomberSetTarget();

        jackalSetTarget();
        sidekickSetTarget();
        pavlovsownerSetTarget();
        pavlovsdogsSetTarget();
        arsonistSetTarget();
        werewolfSetTarget();
        juggernautSetTarget();
        doomsayerSetTarget();
        swooperSetTarget();
        pursuerSetTarget();
        survivorSetTarget();
        thiefSetTarget();
        partTimerSetTarget();
        akujoSetTarget();

        securityGuardSetTarget();
        bodyGuardSetTarget();
        mediumSetTarget();
        trackerSetTarget();
        deputySetTarget();
        sheriffSetTarget();
        prophetSetTarget();
        medicSetTarget();

        shifterSetTarget();
    }


    private static void medicSetTarget()
    {
        if (Medic.medic == null || Medic.medic != CachedPlayer.LocalPlayer.PlayerControl) return;
        Medic.currentTarget = setTarget();
        if (!Medic.usedShield) setPlayerOutline(Medic.currentTarget, Medic.shieldedColor);
    }

    private static void prophetSetTarget()
    {
        if (Prophet.prophet == null || CachedPlayer.LocalPlayer.PlayerControl != Prophet.prophet) return;
        Prophet.currentTarget = setTarget();
        if (Prophet.examinesLeft > 0) setPlayerOutline(Prophet.currentTarget, Prophet.color);
    }

    private static void partTimerSetTarget()
    {
        if (PartTimer.partTimer == null || PartTimer.partTimer != CachedPlayer.LocalPlayer.PlayerControl) return;
        PartTimer.currentTarget = setTarget();
        if (PartTimer.target != null) setPlayerOutline(PartTimer.currentTarget, PartTimer.color);
    }

    private static void bomberSetTarget()
    {
        setBomberBombTarget();
        if (Bomber.bomber == null || Bomber.bomber != CachedPlayer.LocalPlayer.PlayerControl) return;
        Bomber.currentTarget = setTarget();
        if (Bomber.hasBombPlayer == null) setPlayerOutline(Bomber.currentTarget, Bomber.color);
    }

    private static void trackerSetTarget()
    {
        if (Tracker.tracker == null || Tracker.tracker != CachedPlayer.LocalPlayer.PlayerControl) return;
        Tracker.currentTarget = setTarget();
        if (!Tracker.usedTracker) setPlayerOutline(Tracker.currentTarget, Tracker.color);
    }

    private static void vampireSetTarget()
    {
        if (Vampire.vampire == null || Vampire.vampire != CachedPlayer.LocalPlayer.PlayerControl) return;

        PlayerControl target = null;

        var untargetablePlayers = new List<PlayerControl>();

        if (Spy.spy != null)
        {
            if (Spy.impostorsCanKillAnyone)
            {
                target = setTarget(false, true);
            }
            else
            {
                target = setTarget(true, true);
            }
        }
        else
        {
            target = setTarget(true, true);
        }

        bool targetNearGarlic = false;
        if (target != null)
            foreach (var garlic in Garlic.garlics)
                if (Vector2.Distance(garlic.garlic.transform.position, target.transform.position) <= 1.91f)
                    targetNearGarlic = true;
        Vampire.targetNearGarlic = targetNearGarlic;
        Vampire.currentTarget = target;
        setPlayerOutline(Vampire.currentTarget, Vampire.color);
    }

    private static void jackalSetTarget()
    {
        if (Jackal.jackal.Any(x => x.IsAlive() && x.PlayerId == CachedPlayer.LocalId))
        {
            var untargetablePlayers = new List<PlayerControl>();
            foreach (var p in Jackal.jackal)
            {
                untargetablePlayers.Add(p);
            }
            if (Jackal.sidekick != null) untargetablePlayers.Add(Jackal.sidekick);
            if (Mini.mini != null && !Mini.isGrownUp()) untargetablePlayers.Add(Mini.mini);
            Jackal.currentTarget = setTarget(untargetablePlayers: untargetablePlayers);
            setPlayerOutline(Jackal.currentTarget, Palette.ImpostorRed);
        }
    }

    private static void sidekickSetTarget()
    {
        if (Jackal.sidekick == null || Jackal.sidekick != CachedPlayer.LocalPlayer.PlayerControl) return;
        var untargetablePlayers = new List<PlayerControl>();
        foreach (var p in Jackal.jackal)
        {
            untargetablePlayers.Add(p);
        }
        if (Mini.mini != null && !Mini.isGrownUp()) untargetablePlayers.Add(Mini.mini);
        Jackal.currentTarget2 = setTarget(untargetablePlayers: untargetablePlayers);
        setPlayerOutline(Jackal.currentTarget2, Palette.ImpostorRed);
    }

    private static void setBomberBombTarget()
    {
        if (Bomber.bomber == null || Bomber.hasBombPlayer != CachedPlayer.LocalPlayer.PlayerControl) return;
        Bomber.currentBombTarget = setTarget();
        //if (Bomber.hasBomb != null) setPlayerOutline(Bomber.currentBombTarget, Bomber.color);
    }

    private static void bodyGuardSetTarget()
    {
        if (BodyGuard.bodyguard == null || BodyGuard.bodyguard != CachedPlayer.LocalPlayer.PlayerControl) return;
        BodyGuard.currentTarget = setTarget();
        if (!BodyGuard.usedGuard) setPlayerOutline(Medic.currentTarget, Medic.shieldedColor);
    }

    public static void akujoSetTarget()
    {
        if (Akujo.akujo == null || Akujo.akujo.Data.IsDead || CachedPlayer.LocalPlayer.PlayerControl != Akujo.akujo) return;
        var untargetables = new List<PlayerControl>();
        if (Akujo.honmei != null) untargetables.Add(Akujo.honmei);
        if (Akujo.keeps != null) untargetables.AddRange(Akujo.keeps);
        Akujo.currentTarget = setTarget(untargetablePlayers: untargetables);
        if (Akujo.honmei == null || Akujo.keepsLeft > 0) setPlayerOutline(Akujo.currentTarget, Akujo.color);
    }

    private static void swooperSetTarget()
    {
        if (Swooper.swooper == null || Swooper.swooper != CachedPlayer.LocalPlayer.PlayerControl) return;
        var untargetablePlayers = new List<PlayerControl>();
        if (Mini.mini != null && !Mini.isGrownUp()) untargetablePlayers.Add(Mini.mini); // Exclude Jackal from targeting the Mini unless it has grown up
        Swooper.currentTarget = setTarget(untargetablePlayers: untargetablePlayers);
        setPlayerOutline(Swooper.currentTarget, Palette.ImpostorRed);
    }

    private static void eraserSetTarget()
    {
        if (Eraser.eraser == null || Eraser.eraser != CachedPlayer.LocalPlayer.PlayerControl) return;

        var untargetables = new List<PlayerControl>();
        if (Spy.spy != null) untargetables.Add(Spy.spy);
        Eraser.currentTarget = setTarget(!Eraser.canEraseAnyone,
            untargetablePlayers: Eraser.canEraseAnyone ? [] : untargetables);
        setPlayerOutline(Eraser.currentTarget, Eraser.color);
    }

    private static void impostorSetTarget()
    {
        if (!CachedPlayer.LocalPlayer.Data.Role.IsImpostor || !CachedPlayer.LocalPlayer.PlayerControl.CanMove ||
            CachedPlayer.LocalPlayer.Data.IsDead)
        {
            // !isImpostor || !canMove || isDead
            FastDestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(null);
            return;
        }

        PlayerControl target;
        if (Spy.spy != null)
        {
            if (Spy.impostorsCanKillAnyone)
            {
                target = setTarget(false, true);
            }
            else
            {
                target = setTarget(true, true, [Spy.spy]);
            }
        }
        else
        {
            target = setTarget(true, true);
        }

        FastDestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(target); // Includes setPlayerOutline(target, Palette.ImpstorRed);
    }

    private static void warlockSetTarget()
    {
        if (Warlock.warlock == null || Warlock.warlock != CachedPlayer.LocalPlayer.PlayerControl) return;
        if (Warlock.curseVictim != null && (Warlock.curseVictim.Data.Disconnected || Warlock.curseVictim.Data.IsDead))
            // If the cursed victim is disconnected or dead reset the curse so a new curse can be applied
            Warlock.resetCurse();
        if (Warlock.curseVictim == null)
        {
            Warlock.currentTarget = setTarget();
            setPlayerOutline(Warlock.currentTarget, Warlock.color);
        }
        else
        {
            Warlock.curseVictimTarget = setTarget(targetingPlayer: Warlock.curseVictim);
            setPlayerOutline(Warlock.curseVictimTarget, Warlock.color);
        }
    }

    public static void securityGuardSetTarget()
    {
        if (SecurityGuard.securityGuard == null || SecurityGuard.securityGuard != CachedPlayer.LocalPlayer.PlayerControl ||
            MapUtilities.CachedShipStatus == null || MapUtilities.CachedShipStatus.AllVents == null) return;

        Vent target = null;
        var truePosition = CachedPlayer.LocalPlayer.PlayerControl.GetTruePosition();
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

        SecurityGuard.ventTarget = target;
    }

    private static void pavlovsownerSetTarget()
    {
        if (Pavlovsdogs.pavlovsowner == null || Pavlovsdogs.pavlovsowner != CachedPlayer.LocalPlayer.PlayerControl) return;
        var untargetablePlayers = new List<PlayerControl>();
        if (Mini.mini != null && !Mini.isGrownUp()) untargetablePlayers.Add(Mini.mini);
        Pavlovsdogs.currentTarget = setTarget(untargetablePlayers: untargetablePlayers);
        setPlayerOutline(Pavlovsdogs.currentTarget, Palette.ImpostorRed);
    }

    public static void mediumSetTarget()
    {
        if (Medium.medium == null || Medium.medium != CachedPlayer.LocalPlayer.PlayerControl ||
            Medium.medium.Data.IsDead || Medium.deadBodies == null ||
            MapUtilities.CachedShipStatus?.AllVents == null) return;

        DeadPlayer target = null;
        var truePosition = CachedPlayer.LocalPlayer.PlayerControl.GetTruePosition();
        var closestDistance = float.MaxValue;
        var usableDistance = MapUtilities.CachedShipStatus.AllVents.FirstOrDefault().UsableDistance;
        foreach (var (dp, ps) in Medium.deadBodies)
        {
            var distance = Vector2.Distance(ps, truePosition);
            if (distance <= usableDistance && distance < closestDistance)
            {
                closestDistance = distance;
                target = dp;
            }
        }

        Medium.target = target;
    }

    private static void pavlovsdogsSetTarget()
    {
        if (Pavlovsdogs.pavlovsdogs == null || !Pavlovsdogs.pavlovsdogs.Any(p => p == CachedPlayer.LocalPlayer.PlayerControl)) return;
        var untargetablePlayers = new List<PlayerControl>();
        foreach (var p in Pavlovsdogs.pavlovsdogs)
        {
            untargetablePlayers.Add(p);
        }
        if (Pavlovsdogs.pavlovsowner != null) untargetablePlayers.Add(Pavlovsdogs.pavlovsowner);
        if (Mini.mini != null && !Mini.isGrownUp()) untargetablePlayers.Add(Mini.mini);
        Pavlovsdogs.killTarget = setTarget(untargetablePlayers: untargetablePlayers);
        setPlayerOutline(Pavlovsdogs.killTarget, Palette.ImpostorRed);
    }

    private static void werewolfSetTarget()
    {
        if (Werewolf.werewolf == null || Werewolf.werewolf != CachedPlayer.LocalPlayer.PlayerControl) return;
        Werewolf.currentTarget = setTarget();
    }

    private static void juggernautSetTarget()
    {
        if (Juggernaut.juggernaut == null || Juggernaut.juggernaut != CachedPlayer.LocalPlayer.PlayerControl) return;
        Juggernaut.currentTarget = setTarget();
    }


    private static void doomsayerSetTarget()
    {
        if (Doomsayer.doomsayer == null || Doomsayer.doomsayer != CachedPlayer.LocalPlayer.PlayerControl) return;
        Doomsayer.currentTarget = setTarget();
    }

    private static void blackMailerSetTarget()
    {
        if (Blackmailer.blackmailer == null ||
            Blackmailer.blackmailer != CachedPlayer.LocalPlayer.PlayerControl) return;
        Blackmailer.currentTarget = setTarget();
        setPlayerOutline(Medic.currentTarget, Blackmailer.blackmailedColor);
    }

    private static void pursuerSetTarget()
    {
        if (Pursuer.Player == null || !Pursuer.Player.Contains(CachedPlayer.LocalPlayer.PlayerControl)) return;
        Pursuer.target = setTarget();
        setPlayerOutline(Pursuer.target, Pursuer.color);
    }

    private static void survivorSetTarget()
    {
        if (Survivor.Player == null || !Survivor.Player.Contains(CachedPlayer.LocalPlayer.PlayerControl)) return;
        Survivor.target = setTarget();
        setPlayerOutline(Survivor.target, Survivor.color);
    }

    private static void witchSetTarget()
    {
        if (Witch.witch == null || Witch.witch != CachedPlayer.LocalPlayer.PlayerControl) return;
        List<PlayerControl> untargetables;
        if (Witch.spellCastingTarget != null)
        {
            // Don't switch the target from the the one you're currently casting a spell on
            untargetables = PlayerControl.AllPlayerControls.ToArray().Where(x => x.PlayerId != Witch.spellCastingTarget.PlayerId).ToList();
        }
        else
        {
            // Also target players that have already been spelled, to hide spells that were blanks/blocked by shields
            untargetables = new();
            if (Spy.spy != null && !Witch.canSpellAnyone) untargetables.Add(Spy.spy);
        }

        Witch.currentTarget = setTarget(!Witch.canSpellAnyone, untargetablePlayers: untargetables);
        setPlayerOutline(Witch.currentTarget, Witch.color);
    }

    private static void ninjaSetTarget()
    {
        if (Ninja.ninja == null || Ninja.ninja != CachedPlayer.LocalPlayer.PlayerControl) return;
        var untargetables = new List<PlayerControl>();
        if (Spy.spy != null && !Spy.impostorsCanKillAnyone) untargetables.Add(Spy.spy);
        if (Mini.mini != null && !Mini.isGrownUp()) untargetables.Add(Mini.mini);
        Ninja.currentTarget =
            setTarget(Spy.spy == null || !Spy.impostorsCanKillAnyone, untargetablePlayers: untargetables);
        setPlayerOutline(Ninja.currentTarget, Ninja.color);
    }

    private static void thiefSetTarget()
    {
        if (Thief.thief == null || Thief.thief != CachedPlayer.LocalPlayer.PlayerControl) return;
        var untargetables = new List<PlayerControl>();
        if (Mini.mini != null && !Mini.isGrownUp()) untargetables.Add(Mini.mini);
        Thief.currentTarget = setTarget(untargetablePlayers: untargetables);
        setPlayerOutline(Thief.currentTarget, Thief.color);
    }

    private static void shifterSetTarget()
    {
        if (Shifter.shifter == null || Shifter.shifter != CachedPlayer.LocalPlayer.PlayerControl) return;
        Shifter.currentTarget = setTarget();
        if (Shifter.futureShift == null) setPlayerOutline(Shifter.currentTarget, Color.yellow);
    }

    private static void morphlingSetTarget()
    {
        if (Morphling.morphling == null || Morphling.morphling != CachedPlayer.LocalPlayer.PlayerControl) return;
        Morphling.currentTarget = setTarget();
        setPlayerOutline(Morphling.currentTarget, Morphling.color);
    }

    private static void sheriffSetTarget()
    {
        if (Sheriff.sheriff == null || Sheriff.sheriff != CachedPlayer.LocalPlayer.PlayerControl) return;
        Sheriff.currentTarget = setTarget();
        setPlayerOutline(Sheriff.currentTarget, Sheriff.color);
    }

    private static void deputySetTarget()
    {
        if (Deputy.deputy == null || Deputy.deputy != CachedPlayer.LocalPlayer.PlayerControl) return;
        Deputy.currentTarget = setTarget();
        setPlayerOutline(Deputy.currentTarget, Deputy.color);
    }

    public static void arsonistSetTarget()
    {
        if (Arsonist.arsonist == null || Arsonist.arsonist != CachedPlayer.LocalPlayer.PlayerControl) return;
        List<PlayerControl> untargetables;
        if (Arsonist.douseTarget != null)
        {
            untargetables = new();
            foreach (var cachedPlayer in CachedPlayer.AllPlayers)
                if (cachedPlayer.PlayerId != Arsonist.douseTarget.PlayerId)
                    untargetables.Add(cachedPlayer);
        }
        else
        {
            untargetables = Arsonist.dousedPlayers;
        }

        Arsonist.currentTarget = setTarget(untargetablePlayers: untargetables);
        if (Arsonist.currentTarget != null) setPlayerOutline(Arsonist.currentTarget, Arsonist.color);

        Arsonist.currentTarget2 = setTarget(false, true);
        if (Arsonist.currentTarget2 != null) setPlayerOutline(Arsonist.currentTarget2, Arsonist.color);
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
        handleVampireBiteOnBodyReport();
        handleBomberExplodeOnBodyReport();
        handleTrapperTrapOnBodyReport();
        return true;
    }

    private static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] NetworkedPlayerInfo target)
    {
        Message($"报告玩家 {__instance.Data.PlayerName} 被报告尸体 {target?.PlayerName}", "CmdReportDeadBody");
        // Medic or Detective report
        var isMedicReport = Medic.medic != null && Medic.medic == CachedPlayer.LocalPlayer.PlayerControl &&
                            __instance.PlayerId == Medic.medic.PlayerId;
        var isDetectiveReport = Detective.detective != null &&
                                Detective.detective == CachedPlayer.LocalPlayer.PlayerControl &&
                                __instance.PlayerId == Detective.detective.PlayerId;
        var isSluethReport = Slueth.slueth != null && Slueth.slueth == CachedPlayer.LocalPlayer.PlayerControl &&
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
                    timer += rnd.Next(-3, 4);
                    timer = Math.Max(0, timer);
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
                        msg = $"尸检报告: 凶手的职业似乎是 {RoleInfo.getRoleInfoForPlayer(killer, false).First().Name} !\n尸体在 {timer} 秒前死亡";
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
                        FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(CachedPlayer.LocalPlayer.PlayerControl, msg);

                        // Ghost Info
                        var writer = StartRPC(CachedPlayer.LocalPlayer.PlayerControl, CustomRPC.ShareGhostInfo);
                        writer.Write(CachedPlayer.LocalPlayer.PlayerId);
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
            var writer = StartRPC(CachedPlayer.LocalPlayer.PlayerControl, CustomRPC.WitnessReport);
            writer.Write(target?.PlayerId ?? byte.MaxValue);
            writer.EndRPC();
            Witness.WitnessReport(target?.PlayerId ?? byte.MaxValue);
        }

        if (isSluethReport)
        {
            var reported = playerById(target.PlayerId);
            Slueth.reported.Add(reported);
        }
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
        // Collect dead player info

        var deadPlayer = new DeadPlayer(target, DateTime.UtcNow, CustomDeathReason.Kill, __instance);
        if (__instance == target) deadPlayer = new DeadPlayer(target, DateTime.UtcNow, CustomDeathReason.Suicide, __instance);
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

            if (Bait.showKillFlash && __instance == CachedPlayer.LocalPlayer.PlayerControl)
                showFlash(new Color(204f / 255f, 102f / 255f, 0f / 255f));
        }

        if (target.Data.Role.IsImpostor && AmongUsClient.Instance.AmHost)
        {
            LastImpostor.promoteToLastImpostor();
        }

        // Sidekick promotion trigger on exile
        if (Jackal.promotesToJackal && Jackal.sidekick.IsAlive() &&
            Jackal.jackal.Any(x => x == __instance && x == PlayerControl.LocalPlayer))
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                (byte)CustomRPC.SidekickPromotes, SendOption.Reliable);
            writer.Write(Jackal.sidekick.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.sidekickPromotes(Jackal.sidekick.PlayerId);
        }

        // Pursuer promotion trigger on murder (the host sends the call such that everyone recieves the update before a possible game End)
        if (target == Lawyer.target && AmongUsClient.Instance.AmHost && Lawyer.lawyer != null)
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                (byte)CustomRPC.LawyerPromotesToPursuer, SendOption.Reliable);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            Lawyer.PromotesToPursuer();
        }

        if (target == Executioner.target && AmongUsClient.Instance.AmHost && Executioner.executioner != null)
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                (byte)CustomRPC.ExecutionerPromotesRole, SendOption.Reliable);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            Executioner.PromotesRole();
        }

        // Undertaker Button Sync
        if (Undertaker.undertaker != null && CachedPlayer.LocalPlayer.PlayerControl == Undertaker.undertaker &&
            __instance == Undertaker.undertaker && HudManagerStartPatch.undertakerDragButton != null)
            HudManagerStartPatch.undertakerDragButton.Timer = Undertaker.dragingDelaiAfterKill;

        // Seer show flash and add dead player position
        if (Seer.seer != null &&
            (CachedPlayer.LocalPlayer.PlayerControl == Seer.seer || shouldShowGhostInfo()) &&
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
        if (LastImpostor.lastImpostor != null && __instance == LastImpostor.lastImpostor && CachedPlayer.LocalPlayer.PlayerControl == __instance)
        {
            LastImpostor.lastImpostor.SetKillTimer(Mathf.Max(0f, ModOption.KillCooddown - LastImpostor.deduce));

            if (Vampire.vampire != null && Vampire.vampire.PlayerId == LastImpostor.lastImpostor.PlayerId)
                HudManagerStartPatch.vampireKillButton.MaxTimer = Vampire.cooldown - LastImpostor.deduce;
        }

        // Set Gambler cooldown
        if (Gambler.gambler != null && __instance == Gambler.gambler && CachedPlayer.LocalPlayer.PlayerControl == __instance)
        {
            var cooldown = Gambler.GetSuc() ? Gambler.minCooldown : Gambler.maxCooldown;
            Gambler.gambler.SetKillTimer(cooldown);
        }

        // Set bountyHunter cooldown
        if (BountyHunter.bountyHunter != null && CachedPlayer.LocalPlayer.PlayerControl == BountyHunter.bountyHunter &&
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
        if (Mini.mini != null && __instance == Mini.mini && __instance == CachedPlayer.LocalPlayer.PlayerControl)
        {
            var multiplier = 1f;
            if (Mini.mini != null && CachedPlayer.LocalPlayer.PlayerControl == Mini.mini)
                multiplier = Mini.isGrownUp() ? 0.66f : 2f;
            Mini.mini.SetKillTimer(__instance.killTimer * multiplier);
        }

        // Cleaner Button Sync
        if (Cleaner.cleaner != null && CachedPlayer.LocalPlayer.PlayerControl == Cleaner.cleaner &&
            __instance == Cleaner.cleaner && HudManagerStartPatch.cleanerCleanButton != null)
            HudManagerStartPatch.cleanerCleanButton.Timer = Cleaner.cleaner.killTimer;

        // Witch Button Sync
        if (Witch.triggerBothCooldowns && Witch.witch != null &&
            CachedPlayer.LocalPlayer.PlayerControl == Witch.witch && __instance == Witch.witch &&
            HudManagerStartPatch.witchSpellButton != null)
            HudManagerStartPatch.witchSpellButton.Timer = HudManagerStartPatch.witchSpellButton.MaxTimer;

        // Bomber Button Sync
        if (Bomber.triggerBothCooldowns && Bomber.bomber != null &&
            CachedPlayer.LocalPlayer.PlayerControl == Bomber.bomber && __instance == Bomber.bomber &&
            HudManagerStartPatch.bomberBombButton != null)
            HudManagerStartPatch.bomberBombButton.Timer = HudManagerStartPatch.bomberBombButton.MaxTimer;

        // Warlock Button Sync
        if (Warlock.warlock != null && CachedPlayer.LocalPlayer.PlayerControl == Warlock.warlock &&
            __instance == Warlock.warlock && HudManagerStartPatch.warlockCurseButton != null)
            if (Warlock.warlock.killTimer > HudManagerStartPatch.warlockCurseButton.Timer)
                HudManagerStartPatch.warlockCurseButton.Timer = Warlock.warlock.killTimer;
        // Ninja Button Sync
        if (Ninja.ninja != null && CachedPlayer.LocalPlayer.PlayerControl == Ninja.ninja && __instance == Ninja.ninja &&
            HudManagerStartPatch.ninjaButton != null)
            HudManagerStartPatch.ninjaButton.Timer = HudManagerStartPatch.ninjaButton.MaxTimer;

        // EvilTrapper peforms normal kills
        if (EvilTrapper.evilTrapper != null && CachedPlayer.LocalPlayer.PlayerControl == EvilTrapper.evilTrapper && __instance == EvilTrapper.evilTrapper)
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
                writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                    (byte)CustomRPC.ClearTrap, SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.clearTrap();
            }
            EvilTrapper.isTrapKill = false;
        }

        // Add Bloody Modifier
        if (Bloody.bloody.FindAll(x => x.PlayerId == target.PlayerId).Count > 0)
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
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
        if (Mini.mini != null && CachedPlayer.LocalPlayer.PlayerControl == Mini.mini)
            multiplier = Mini.isGrownUp() ? 0.66f : 2f;
        if (BountyHunter.bountyHunter != null && CachedPlayer.LocalPlayer.PlayerControl == BountyHunter.bountyHunter)
            addition = BountyHunter.punishmentTime;
        if (Gambler.gambler != null && CachedPlayer.LocalPlayer.PlayerControl == Gambler.gambler)
            addition = Gambler.maxCooldown - GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
        if (LastImpostor.lastImpostor != null && CachedPlayer.LocalPlayer.PlayerControl == LastImpostor.lastImpostor)
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

        _ = new LateTask(() => { if (__instance == PlayerControl.LocalPlayer) CanSeeRoleInfo = true; }, 1f);

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

        if (AmongUsClient.Instance.AmHost)
        {
            LastImpostor.promoteToLastImpostor();
        }

        // Sidekick promotion trigger on exile
        if (Jackal.promotesToJackal && Jackal.sidekick.IsAlive() &&
            Jackal.jackal.Any(x => x == __instance && x == PlayerControl.LocalPlayer))
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                (byte)CustomRPC.SidekickPromotes, SendOption.Reliable);
            writer.Write(Jackal.sidekick.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.sidekickPromotes(Jackal.sidekick.PlayerId);
        }
        if (Lawyer.lawyer != null && __instance == Lawyer.target)
        {
            if (AmongUsClient.Instance.AmHost && ((Lawyer.target != Jester.jester) || Lawyer.targetWasGuessed))
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                    (byte)CustomRPC.LawyerPromotesToPursuer, SendOption.Reliable);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Lawyer.PromotesToPursuer();
            }
        }
        if (Executioner.executioner != null && __instance == Executioner.target)
        {
            if (AmongUsClient.Instance.AmHost && Executioner.targetWasGuessed)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
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
