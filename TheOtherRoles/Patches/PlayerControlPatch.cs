using System;
using System.Collections.Generic;
using System.Linq;
using AmongUs.Data;
using AmongUs.GameOptions;
using Assets.CoreScripts;
using Hazel;
using InnerNet;
using Reactor.Utilities.Extensions;
using TheOtherRoles.CustomGameModes;
using TheOtherRoles.Helper;
using TheOtherRoles.Objects;
using TheOtherRoles.Utilities;
using TMPro;
using UnityEngine;
using static TheOtherRoles.GameHistory;
using static TheOtherRoles.TheOtherRoles;
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
        var num = GameOptionsData.KillDistances[
            Mathf.Clamp(GameOptionsManager.Instance.currentNormalGameOptions.KillDistance, 0, 2)];
        if (!MapUtilities.CachedShipStatus) return result;
        if (targetingPlayer == null) targetingPlayer = CachedPlayer.LocalPlayer.PlayerControl;
        if (targetingPlayer.Data.IsDead) return result;

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
        foreach (PlayerControl target in CachedPlayer.AllPlayers)
        {
            if (target == null || target.cosmetics?.currentBodySprite?.BodySprite == null) continue;

            var isMorphedMorphling = target == Morphling.morphling && Morphling.morphTarget != null &&
                                     Morphling.morphTimer > 0f;
            var hasVisibleShield = false;
            var color = Medic.shieldedColor;
            if (!Helpers.isCamoComms() && Camouflager.camouflageTimer <= 0f && !Helpers.MushroomSabotageActive() &&
                Medic.shielded != null && ((target == Medic.shielded && !isMorphedMorphling) ||
                                           (isMorphedMorphling && Morphling.morphTarget == Medic.shielded)))
            {
                hasVisibleShield = Medic.showShielded == 0 || Helpers.shouldShowGhostInfo() // Everyone or Ghost info
                                                           || (Medic.showShielded == 1 &&
                                                               (CachedPlayer.LocalPlayer.PlayerControl ==
                                                                Medic.shielded ||
                                                                CachedPlayer.LocalPlayer.PlayerControl ==
                                                                Medic.medic)) // Shielded + Medic
                                                           || (Medic.showShielded == 2 &&
                                                               CachedPlayer.LocalPlayer.PlayerControl ==
                                                               Medic.medic); // Medic only
                // Make shield invisible till after the next meeting if the option is set (the medic can already see the shield)
                hasVisibleShield = hasVisibleShield && (Medic.meetingAfterShielding || !Medic.showShieldAfterMeeting ||
                                                        CachedPlayer.LocalPlayer.PlayerControl == Medic.medic ||
                                                        Helpers.shouldShowGhostInfo());
            }

            if (CachedPlayer.LocalPlayer.Data.IsDead && BodyGuard.guarded != null && target == BodyGuard.guarded)
            {
                hasVisibleShield = true;
                color = BodyGuard.color;
            }

            if (!Helpers.isCamoComms() && Camouflager.camouflageTimer <= 0f && !Helpers.MushroomSabotageActive() &&
                TORMapOptions.firstKillPlayer != null && TORMapOptions.shieldFirstKill &&
                ((target == TORMapOptions.firstKillPlayer && !isMorphedMorphling) ||
                 (isMorphedMorphling && Morphling.morphTarget == TORMapOptions.firstKillPlayer)))
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
                            bool canUse;
                            bool couldUse;
                            vent.CanUse(CachedPlayer.LocalPlayer.Data, out canUse, out couldUse);
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

                if (localPlayerPositions.Count > 1)
                    localPlayerPositions
                        .RemoveAt(0); // Skip every second position to rewinde twice as fast, but never skip the last position
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

    private static void medicSetTarget()
    {
        if (Medic.medic == null || Medic.medic != CachedPlayer.LocalPlayer.PlayerControl) return;
        Medic.currentTarget = setTarget();
        if (!Medic.usedShield) setPlayerOutline(Medic.currentTarget, Medic.shieldedColor);
    }

    private static void bomber2SetTarget()
    {
        setBomber2BombTarget();
        if (Bomber2.bomber2 == null || Bomber2.bomber2 != CachedPlayer.LocalPlayer.PlayerControl) return;
        Bomber2.currentTarget = setTarget();
        if (Bomber2.hasBomb == null) setPlayerOutline(Bomber2.currentTarget, Bomber2.color);
    }

    private static void setBomber2BombTarget()
    {
        if (Bomber2.bomber2 == null || Bomber2.hasBomb != CachedPlayer.LocalPlayer.PlayerControl) return;
        Bomber2.currentBombTarget = setTarget();
        //        if (Bomber2.hasBomb != null) setPlayerOutline(Bomber2.currentBombTarget, Bomber2.color);
    }

    private static void bodyGuardSetTarget()
    {
        if (BodyGuard.bodyguard == null || BodyGuard.bodyguard != CachedPlayer.LocalPlayer.PlayerControl) return;
        BodyGuard.currentTarget = setTarget();
        if (!BodyGuard.usedGuard) setPlayerOutline(Medic.currentTarget, Medic.shieldedColor);
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

    private static void privateInvestigatorSetTarget()
    {
        if (PrivateInvestigator.privateInvestigator == null ||
            PrivateInvestigator.privateInvestigator != CachedPlayer.LocalPlayer.PlayerControl) return;
        PrivateInvestigator.currentTarget = setTarget();
        setPlayerOutline(PrivateInvestigator.currentTarget, PrivateInvestigator.color);
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

    private static void trackerSetTarget()
    {
        if (Tracker.tracker == null || Tracker.tracker != CachedPlayer.LocalPlayer.PlayerControl) return;
        Tracker.currentTarget = setTarget();
        if (!Tracker.usedTracker) setPlayerOutline(Tracker.currentTarget, Tracker.color);
    }

    private static void detectiveUpdateFootPrints()
    {
        if (Detective.detective == null || Detective.detective != CachedPlayer.LocalPlayer.PlayerControl) return;

        Detective.timer -= Time.fixedDeltaTime;
        if (Detective.timer <= 0f)
        {
            Detective.timer = Detective.footprintIntervall;
            foreach (PlayerControl player in CachedPlayer.AllPlayers)
                if (player != null && player != CachedPlayer.LocalPlayer.PlayerControl && !player.Data.IsDead &&
                    !player.inVent)
                    FootprintHolder.Instance.MakeFootprint(player);
        }
    }

    private static void vampireSetTarget()
    {
        if (Vampire.vampire == null || Vampire.vampire != CachedPlayer.LocalPlayer.PlayerControl) return;

        PlayerControl target = null;
        if (Spy.spy != null || Sidekick.wasSpy || Jackal.wasSpy)
        {
            if (Spy.impostorsCanKillAnyone)
                target = setTarget(false, true);
            else
                target = setTarget(true, true,
                    new List<PlayerControl>
                    {
                        Spy.spy, Sidekick.wasTeamRed ? Sidekick.sidekick : null,
                        Jackal.wasTeamRed ? Jackal.jackal : null
                    });
        }
        else
        {
            target = setTarget(true, true,
                new List<PlayerControl>
                    { Sidekick.wasImpostor ? Sidekick.sidekick : null, Jackal.wasImpostor ? Jackal.jackal : null });
        }

        var targetNearGarlic = false;
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
        if (Jackal.jackal == null || Jackal.jackal != CachedPlayer.LocalPlayer.PlayerControl) return;
        var untargetablePlayers = new List<PlayerControl>();
        if (Jackal.canCreateSidekickFromImpostor)
            // Only exclude sidekick from beeing targeted if the jackal can create sidekicks from impostors
            if (Sidekick.sidekick != null)
                untargetablePlayers.Add(Sidekick.sidekick);
        if (Mini.mini != null && !Mini.isGrownUp())
            untargetablePlayers.Add(Mini.mini); // Exclude Jackal from targeting the Mini unless it has grown up
        Jackal.currentTarget = setTarget(untargetablePlayers: untargetablePlayers);
        setPlayerOutline(Jackal.currentTarget, Palette.ImpostorRed);
    }

    private static void sidekickSetTarget()
    {
        if (Sidekick.sidekick == null || Sidekick.sidekick != CachedPlayer.LocalPlayer.PlayerControl) return;
        var untargetablePlayers = new List<PlayerControl>();
        if (Jackal.jackal != null) untargetablePlayers.Add(Jackal.jackal);
        if (Mini.mini != null && !Mini.isGrownUp())
            untargetablePlayers.Add(Mini.mini); // Exclude Sidekick from targeting the Mini unless it has grown up
        Sidekick.currentTarget = setTarget(untargetablePlayers: untargetablePlayers);
        if (Sidekick.canKill) setPlayerOutline(Sidekick.currentTarget, Palette.ImpostorRed);
    }

    private static void sidekickCheckPromotion()
    {
        // If LocalPlayer is Sidekick, the Jackal is disconnected and Sidekick promotion is enabled, then trigger promotion
        if (Sidekick.sidekick == null || Sidekick.sidekick != CachedPlayer.LocalPlayer.PlayerControl) return;
        if (Sidekick.sidekick.Data.IsDead || !Sidekick.promotesToJackal) return;
        if (Jackal.jackal == null || Jackal.jackal?.Data?.Disconnected == true)
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                (byte)CustomRPC.SidekickPromotes, SendOption.Reliable);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.sidekickPromotes();
        }
    }
    static void swooperSetTarget()
    {
        if (Swooper.swooper == null || Swooper.swooper != CachedPlayer.LocalPlayer.PlayerControl) return;
        var untargetablePlayers = new List<PlayerControl>();
        if (Mini.mini != null && !Mini.isGrownUp()) untargetablePlayers.Add(Mini.mini); // Exclude Jackal from targeting the Mini unless it has grown up
        Swooper.currentTarget = setTarget(untargetablePlayers: untargetablePlayers);
        setPlayerOutline(Swooper.currentTarget, Palette.ImpostorRed);
    }
    private static void cultistSetFollower()
    {
        if (Cultist.cultist == null || Cultist.cultist != CachedPlayer.LocalPlayer.PlayerControl) return;
        var untargetablePlayers = new List<PlayerControl>();
        if (Swooper.swooper != null && Swooper.isInvisable) untargetablePlayers.Add(Swooper.swooper);
        if (Mini.mini != null && !Mini.isGrownUp())
            untargetablePlayers.Add(Mini.mini); // Exclude Jackal from targeting the Mini unless it has grown up
        Cultist.currentTarget = setTarget(untargetablePlayers: untargetablePlayers);
        //        Cultist.currentFollower = setTarget(untargetablePlayers: untargetablePlayers);
        //        setPlayerOutline(Cultist.currentTarget, Palette.ImpostorRed);
    }

    private static void eraserSetTarget()
    {
        if (Eraser.eraser == null || Eraser.eraser != CachedPlayer.LocalPlayer.PlayerControl) return;

        var untargetables = new List<PlayerControl>();
        if (Spy.spy != null) untargetables.Add(Spy.spy);
        if (Sidekick.wasTeamRed) untargetables.Add(Sidekick.sidekick);
        if (Jackal.wasTeamRed) untargetables.Add(Jackal.jackal);
        Eraser.currentTarget = setTarget(!Eraser.canEraseAnyone,
            untargetablePlayers: Eraser.canEraseAnyone ? new List<PlayerControl>() : untargetables);
        setPlayerOutline(Eraser.currentTarget, Eraser.color);
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
                              (CachedPlayer.LocalPlayer.PlayerControl == Jackal.jackal ||
                               CachedPlayer.LocalPlayer.PlayerControl == Sidekick.sidekick);
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

    private static void impostorSetTarget()
    {
        if (!CachedPlayer.LocalPlayer.Data.Role.IsImpostor || !CachedPlayer.LocalPlayer.PlayerControl.CanMove ||
            CachedPlayer.LocalPlayer.Data.IsDead)
        {
            // !isImpostor || !canMove || isDead
            FastDestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(null);
            return;
        }

        PlayerControl target = null;
        if (Spy.spy != null || Sidekick.wasSpy || Jackal.wasSpy)
        {
            if (Spy.impostorsCanKillAnyone)
                target = setTarget(false, true);
            else
                target = setTarget(true, true,
                    new List<PlayerControl>
                    {
                        Spy.spy, Sidekick.wasTeamRed ? Sidekick.sidekick : null,
                        Jackal.wasTeamRed ? Jackal.jackal : null
                    });
        }
        else
        {
            target = setTarget(true, true,
                new List<PlayerControl>
                    { Sidekick.wasImpostor ? Sidekick.sidekick : null, Jackal.wasImpostor ? Jackal.jackal : null });
        }

        FastDestroyableSingleton<HudManager>.Instance.KillButton
            .SetTarget(target); // Includes setPlayerOutline(target, Palette.ImpstorRed);
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
    }

    private static void ninjaUpdate()
    {
        if (Ninja.isInvisble && Ninja.invisibleTimer <= 0 && Ninja.ninja == CachedPlayer.LocalPlayer.PlayerControl)
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

    static void prophetSetTarget()
    {
        if (Prophet.prophet == null || CachedPlayer.LocalPlayer.PlayerControl != Prophet.prophet) return;
        Prophet.currentTarget = setTarget();
        if (Prophet.examinesLeft > 0) setPlayerOutline(Prophet.currentTarget, Prophet.color);
    }

    static void prophetUpdate()
    {
        if (Prophet.arrows == null) return;

        foreach (var arrow in Prophet.arrows) arrow.arrow.SetActive(false);

        if (Prophet.prophet == null || Prophet.prophet.Data.IsDead) return;

        if (Prophet.isRevealed && Helpers.isKiller(CachedPlayer.LocalPlayer.PlayerControl))
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
                return;
            }

            if (Tracker.tracker != null && Tracker.tracked != null &&
                CachedPlayer.LocalPlayer.PlayerControl == Tracker.tracker && !Tracker.tracker.Data.IsDead)
            {
                Tracker.timeUntilUpdate -= Time.fixedDeltaTime;

                if (Tracker.timeUntilUpdate <= 0f)
                {
                    var trackedOnMap = !Tracker.tracked.Data.IsDead;
                    var position = Tracker.tracked.transform.position;
                    if (!trackedOnMap)
                    {
                        // Check for dead body
                        var body = Object.FindObjectsOfType<DeadBody>()
                            .FirstOrDefault(b => b.ParentId == Tracker.tracked.PlayerId);
                        if (body != null)
                        {
                            trackedOnMap = true;
                            position = body.transform.position;
                        }
                    }

                    Tracker.arrow.Update(position);
                    Tracker.arrow.arrow.SetActive(trackedOnMap);
                    Tracker.timeUntilUpdate = Tracker.updateIntervall;
                }
                else
                {
                    Tracker.arrow.Update();
                }
            }
        }

        // Handle corpses tracking
        if (Tracker.tracker != null && Tracker.tracker == CachedPlayer.LocalPlayer.PlayerControl &&
            Tracker.corpsesTrackingTimer >= 0f && !Tracker.tracker.Data.IsDead)
        {
            var arrowsCountChanged = Tracker.localArrows.Count != Tracker.deadBodyPositions.Count();
            var index = 0;

            if (arrowsCountChanged)
            {
                foreach (var arrow in Tracker.localArrows) Object.Destroy(arrow.arrow);
                Tracker.localArrows = new List<Arrow>();
            }

            foreach (var position in Tracker.deadBodyPositions)
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
            foreach (var arrow in Tracker.localArrows) Object.Destroy(arrow.arrow);
            Tracker.localArrows = new List<Arrow>();
        }
    }

    public static void playerSizeUpdate(PlayerControl p)
    {
        // Set default player size
        var collider = p.Collider.CastFast<CircleCollider2D>();

        p.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
        collider.radius = Mini.defaultColliderRadius;
        collider.offset = Mini.defaultColliderOffset * Vector2.down;

        // Set adapted player size to Mini and Morphling
        if (Mini.mini == null || Helpers.isCamoComms() || Camouflager.camouflageTimer > 0f ||
            Helpers.MushroomSabotageActive() || (Mini.mini == Morphling.morphling && Morphling.morphTimer > 0)) return;

        var growingProgress = Mini.growingProgress();
        var scale = (growingProgress * 0.35f) + 0.35f;
        var correctedColliderRadius =
            Mini.defaultColliderRadius * 0.7f /
            scale; // scale / 0.7f is the factor by which we decrease the player size, hence we need to increase the collider size by 0.7f / scale

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

    public static void updatePlayerInfo()
    {
        var colorBlindTextMeetingInitialLocalPos = new Vector3(0.3384f, -0.16666f, -0.01f);
        var colorBlindTextMeetingInitialLocalScale = new Vector3(0.9f, 1f, 1f);
        foreach (PlayerControl p in CachedPlayer.AllPlayers)
        {
            // Colorblind Text in Meeting
            var playerVoteArea = MeetingHud.Instance?.playerStates?.FirstOrDefault(x => x.TargetPlayerId == p.PlayerId);
            if (playerVoteArea != null && playerVoteArea.ColorBlindName.gameObject.active)
            {
                playerVoteArea.ColorBlindName.transform.localPosition =
                    colorBlindTextMeetingInitialLocalPos + new Vector3(0f, 0.4f, 0f);
                playerVoteArea.ColorBlindName.transform.localScale = colorBlindTextMeetingInitialLocalScale * 0.8f;
            }

            // Colorblind Text During the round
            if (p.cosmetics.colorBlindText != null && p.cosmetics.showColorBlindText &&
                p.cosmetics.colorBlindText.gameObject.active)
                p.cosmetics.colorBlindText.transform.localPosition = new Vector3(0, -1f, 0f);

            p.cosmetics.nameText.transform.parent
                .SetLocalZ(-0.0001f); // This moves both the name AND the colorblindtext behind objects (if the player is behind the object), like the rock on polus

            //-------------------------- Debug old snitch -------------------------- //

            bool snitchFlag = false;
            if (Snitch.snitch != null)
            {
                var (playerCompleted, playerTotal) = TasksHandler.taskInfo(Snitch.snitch.Data);
                int numberOfTasks = playerTotal - playerCompleted;
                bool completedSnitch = (Snitch.seeInMeeting && CachedPlayer.LocalPlayer.PlayerControl == Snitch.snitch && numberOfTasks == 0);
                snitchFlag = (completedSnitch && (p.Data.Role.IsImpostor));
            }

            if (snitchFlag || (Lawyer.lawyerKnowsRole && CachedPlayer.LocalPlayer.PlayerControl == Lawyer.lawyer && p == Lawyer.target) || p == CachedPlayer.LocalPlayer.PlayerControl || CachedPlayer.LocalPlayer.Data.IsDead || ((CachedPlayer.LocalPlayer.PlayerControl == Slueth.slueth && Slueth.reported.Any(x => x.PlayerId == p.PlayerId))) || TORMapOptions.impostorSeeRoles && Spy.spy == null && CachedPlayer.LocalPlayer.Data.Role.IsImpostor && !CachedPlayer.LocalPlayer.Data.IsDead && p == (p.Data.Role.IsImpostor && !p.Data.IsDead) || ((CachedPlayer.LocalPlayer.PlayerControl == Poucher.poucher && Poucher.killed.Any(x => x.PlayerId == p.PlayerId))))
            {
                Transform playerInfoTransform = p.cosmetics.nameText.transform.parent.FindChild("Info");
                TMPro.TextMeshPro playerInfo = playerInfoTransform != null ? playerInfoTransform.GetComponent<TMPro.TextMeshPro>() : null;
                if (playerInfo == null)
                {
                    playerInfo = UnityEngine.Object.Instantiate(p.cosmetics.nameText, p.cosmetics.nameText.transform.parent);
                    playerInfo.transform.localPosition += Vector3.up * 0.225f;
                    playerInfo.fontSize *= 0.75f;
                    playerInfo.gameObject.name = "Info";
                    playerInfo.color = playerInfo.color.SetAlpha(1f);
                }

                //              PlayerVoteArea playerVoteArea = MeetingHud.Instance?.playerStates?.FirstOrDefault(x => x.TargetPlayerId == p.PlayerId);
                Transform meetingInfoTransform = playerVoteArea != null ? playerVoteArea.NameText.transform.parent.FindChild("Info") : null;
                TMPro.TextMeshPro meetingInfo = meetingInfoTransform != null ? meetingInfoTransform.GetComponent<TMPro.TextMeshPro>() : null;
                if (meetingInfo == null && playerVoteArea != null)
                {
                    meetingInfo = UnityEngine.Object.Instantiate(playerVoteArea.NameText, playerVoteArea.NameText.transform.parent);
                    meetingInfo.transform.localPosition += Vector3.down * 0.2f;
                    meetingInfo.fontSize *= 0.60f;
                    meetingInfo.gameObject.name = "Info";
                }

                // Set player name higher to align in middle
                if (meetingInfo != null && playerVoteArea != null)
                {
                    var playerName = playerVoteArea.NameText;
                    playerName.transform.localPosition = new Vector3(0.3384f, 0.0311f, -0.1f);
                }

                var (tasksCompleted, tasksTotal) = TasksHandler.taskInfo(p.Data);
                string roleNames = RoleInfo.GetRolesString(p, true, false);
                string roleText = RoleInfo.GetRolesString(p, true, TORMapOptions.ghostsSeeModifier);
                string taskInfo = tasksTotal > 0 ? $"<color=#FAD934FF>({tasksCompleted}/{tasksTotal})</color>" : "";

                string playerInfoText = "";
                string meetingInfoText = "";
                if (p == CachedPlayer.LocalPlayer.PlayerControl || (TORMapOptions.impostorSeeRoles && Spy.spy == null && CachedPlayer.LocalPlayer.Data.Role.IsImpostor && !CachedPlayer.LocalPlayer.Data.IsDead && p == (p.Data.Role.IsImpostor && !p.Data.IsDead)))
                {
                    // || CachedPlayer.LocalPlayer.PlayerControl == Snitch.snitch && numberOfTasks == 0 && Snitch.canSeeRoles && p == p.Data.Role.IsImpostor && !CachedPlayer.LocalPlayer.Data.IsDead || CachedPlayer.LocalPlayer.PlayerControl == Snitch.snitch && numberOfTasks == 0 && Snitch.canSeeRoles && Snitch.includeTeamJackal && p == Jackal.jackal && !CachedPlayer.LocalPlayer.Data.IsDead || CachedPlayer.LocalPlayer.PlayerControl == Snitch.snitch && numberOfTasks == 0 && Snitch.canSeeRoles && Snitch.includeTeamJackal && p == Sidekick.sidekick && !CachedPlayer.LocalPlayer.Data.IsDead
                    if (p.Data.IsDead) roleNames = roleText;
                    playerInfoText = $"{roleNames}";
                    if (p == Swapper.swapper) playerInfoText = $"{roleNames}" + Helpers.cs(Swapper.color, $" ({Swapper.charges})");
                    if (HudManager.Instance.TaskPanel != null)
                    {
                        TMPro.TextMeshPro tabText = HudManager.Instance.TaskPanel.tab.transform.FindChild("TabText_TMP").GetComponent<TMPro.TextMeshPro>();
                        tabText.SetText($"Tasks {taskInfo}");
                    }
                    meetingInfoText = $"{roleNames} {taskInfo}".Trim();
                }
                else if (TORMapOptions.ghostsSeeRoles && TORMapOptions.ghostsSeeInformation)
                {
                    playerInfoText = $"{roleText} {taskInfo}".Trim();
                    meetingInfoText = playerInfoText;
                }
                else if (TORMapOptions.ghostsSeeInformation)
                {
                    playerInfoText = $"{taskInfo}".Trim();
                    meetingInfoText = playerInfoText;
                }
                else if (TORMapOptions.ghostsSeeRoles || (Lawyer.lawyerKnowsRole && CachedPlayer.LocalPlayer.PlayerControl == Lawyer.lawyer && p == Lawyer.target))
                {
                    playerInfoText = $"{roleText}";
                    meetingInfoText = playerInfoText;
                }

                playerInfo.text = playerInfoText;
                playerInfo.gameObject.SetActive(p.Visible);
                if (meetingInfo != null) meetingInfo.text = MeetingHud.Instance.state == MeetingHud.VoteStates.Results ? "" : meetingInfoText;
            }

            //-------------------------- Debug old snitch -------------------------- //

            if ((Lawyer.lawyerKnowsRole && CachedPlayer.LocalPlayer.PlayerControl == Lawyer.lawyer && p == Lawyer.target) ||
                   (Akujo.knowsRoles && CachedPlayer.LocalPlayer.PlayerControl == Akujo.akujo &&
                   (p == Akujo.honmei || Akujo.keeps.Any(x => x.PlayerId == p.PlayerId))) || 
                   p == CachedPlayer.LocalPlayer.PlayerControl ||
                CachedPlayer.LocalPlayer.Data.IsDead ||
                (CachedPlayer.LocalPlayer.PlayerControl == Slueth.slueth &&
                 Slueth.reported.Any(x => x.PlayerId == p.PlayerId)) ||
                (TORMapOptions.impostorSeeRoles && Spy.spy == null && CachedPlayer.LocalPlayer.Data.Role.IsImpostor &&
                 !CachedPlayer.LocalPlayer.Data.IsDead && p == (p.Data.Role.IsImpostor && !p.Data.IsDead)) ||
                (CachedPlayer.LocalPlayer.PlayerControl == Poucher.poucher &&
                 Poucher.killed.Any(x => x.PlayerId == p.PlayerId)))
            {
                var playerInfoTransform = p.cosmetics.nameText.transform.parent.FindChild("Info");
                var playerInfo = playerInfoTransform != null ? playerInfoTransform.GetComponent<TextMeshPro>() : null;
                if (playerInfo == null)
                {
                    playerInfo = Object.Instantiate(p.cosmetics.nameText, p.cosmetics.nameText.transform.parent);
                    playerInfo.transform.localPosition += Vector3.up * 0.225f;
                    playerInfo.fontSize *= 0.75f;
                    playerInfo.gameObject.name = "Info";
                    playerInfo.color = playerInfo.color.SetAlpha(1f);
                }

                var meetingInfoTransform = playerVoteArea != null
                    ? playerVoteArea.NameText.transform.parent.FindChild("Info")
                    : null;
                var meetingInfo = meetingInfoTransform != null
                    ? meetingInfoTransform.GetComponent<TextMeshPro>()
                    : null;
                if (meetingInfo == null && playerVoteArea != null)
                {
                    meetingInfo = Object.Instantiate(playerVoteArea.NameText, playerVoteArea.NameText.transform.parent);
                    meetingInfo.transform.localPosition += Vector3.down * 0.2f;
                    meetingInfo.fontSize *= 0.60f;
                    meetingInfo.gameObject.name = "Info";
                }

                // Set player name higher to align in middle
                if (meetingInfo != null && playerVoteArea != null)
                {
                    var playerName = playerVoteArea.NameText;
                    playerName.transform.localPosition = new Vector3(0.3384f, 0.0311f, -0.1f);
                }

                var (tasksCompleted, tasksTotal) = TasksHandler.taskInfo(p.Data);
                var roleNames = RoleInfo.GetRolesString(p, true, false);
                var roleText = RoleInfo.GetRolesString(p, true, TORMapOptions.ghostsSeeModifier);
                var taskInfo = tasksTotal > 0 ? $"<color=#FAD934FF>({tasksCompleted}/{tasksTotal})</color>" : "";

                var playerInfoText = "";
                var meetingInfoText = "";
                if (p == CachedPlayer.LocalPlayer.PlayerControl || (TORMapOptions.impostorSeeRoles && Spy.spy == null &&
                                                                    CachedPlayer.LocalPlayer.Data.Role.IsImpostor &&
                                                                    !CachedPlayer.LocalPlayer.Data.IsDead &&
                                                                    p == (p.Data.Role.IsImpostor && !p.Data.IsDead)))
                {
                    if (p.Data.IsDead) roleNames = roleText;
                    playerInfoText = $"{roleNames}";
                    if (p == Swapper.swapper)
                        playerInfoText = $"{roleNames}" + Helpers.cs(Swapper.color, $" ({Swapper.charges})");
                    if (HudManager.Instance.TaskPanel != null)
                    {
                        var tabText = HudManager.Instance.TaskPanel.tab.transform.FindChild("TabText_TMP")
                            .GetComponent<TextMeshPro>();
                        tabText.SetText($"Tasks {taskInfo}");
                    }

                    meetingInfoText = $"{roleNames} {taskInfo}".Trim();
                }
                else if (TORMapOptions.ghostsSeeRoles && TORMapOptions.ghostsSeeInformation)
                {
                    playerInfoText = $"{roleText} {taskInfo}".Trim();
                    meetingInfoText = playerInfoText;
                }
                else if (TORMapOptions.ghostsSeeInformation)
                {
                    playerInfoText = $"{taskInfo}".Trim();
                    meetingInfoText = playerInfoText;
                }
                else if (TORMapOptions.ghostsSeeRoles || (Lawyer.lawyerKnowsRole &&
                                                          CachedPlayer.LocalPlayer.PlayerControl == Lawyer.lawyer &&
                                                          p == Lawyer.target))
                {
                    playerInfoText = $"{roleText}";
                    meetingInfoText = playerInfoText;
                }

                playerInfo.text = playerInfoText;
                playerInfo.gameObject.SetActive(p.Visible);
                if (meetingInfo != null)
                    meetingInfo.text = MeetingHud.Instance.state == MeetingHud.VoteStates.Results
                        ? ""
                        : meetingInfoText;
            }
        }
    }

    public static void securityGuardSetTarget()
    {
        if (SecurityGuard.securityGuard == null ||
            SecurityGuard.securityGuard != CachedPlayer.LocalPlayer.PlayerControl ||
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

    public static void arsonistSetTarget()
    {
        if (Arsonist.arsonist == null || Arsonist.arsonist != CachedPlayer.LocalPlayer.PlayerControl) return;
        List<PlayerControl> untargetables;
        if (Arsonist.douseTarget != null)
        {
            untargetables = new List<PlayerControl>();
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
    }
    /*
    private static void snitchUpdate()
    {
        if (Snitch.snitch == null) return;
        if (!Snitch.needsUpdate) return;

        var snitchIsDead = Snitch.snitch.Data.IsDead;
        var (playerCompleted, playerTotal) = TasksHandler.taskInfo(Snitch.snitch.Data);

        if (playerTotal == 0) return;
        var local = CachedPlayer.LocalPlayer.PlayerControl;

        var numberOfTasks = playerTotal - playerCompleted;

        if (Snitch.isRevealed && ((Snitch.targets == Snitch.Targets.EvilPlayers && Helpers.isEvil(local)) ||
                                  (Snitch.targets == Snitch.Targets.Killers && Helpers.isKiller(local))))
        {
            if (Snitch.text == null)
            {
                Snitch.text =
                    Object.Instantiate(FastDestroyableSingleton<HudManager>.Instance.KillButton.cooldownTimerText,
                        FastDestroyableSingleton<HudManager>.Instance.transform);
                Snitch.text.enableWordWrapping = false;
                Snitch.text.transform.localScale = Vector3.one * 0.75f;
                Snitch.text.transform.localPosition += new Vector3(0f, 1.8f, -69f);
                Snitch.text.gameObject.SetActive(true);
            }
            else
            {
                Snitch.text.text = "告密者还活着: " + playerCompleted + "/" + playerTotal;
                if (snitchIsDead) Snitch.text.text = "告密者已死亡!";
            }
        }
        else if (Snitch.text != null)
        {
            Snitch.text.Destroy();
        }

        if (snitchIsDead)
        {
            if (MeetingHud.Instance == null) Snitch.needsUpdate = false;
            return;
        }

        if (numberOfTasks <= Snitch.taskCountForReveal) Snitch.isRevealed = true;
    }
    */

    static void snitchUpdate()
    {
        if (Snitch.localArrows == null) return;

        foreach (Arrow arrow in Snitch.localArrows) arrow.arrow.SetActive(false);

        if (Snitch.snitch == null || Snitch.snitch.Data.IsDead) return;

        var (playerCompleted, playerTotal) = TasksHandler.taskInfo(Snitch.snitch.Data);
        int numberOfTasks = playerTotal - playerCompleted;

        if (numberOfTasks <= Snitch.taskCountForReveal && (CachedPlayer.LocalPlayer.Data.Role.IsImpostor || (Snitch.includeTeamJackal && (CachedPlayer.LocalPlayer.PlayerControl == Jackal.jackal || CachedPlayer.LocalPlayer.PlayerControl == Sidekick.sidekick || CachedPlayer.LocalPlayer.PlayerControl == Swooper.swooper))))
        {
            if (Snitch.localArrows.Count == 0) Snitch.localArrows.Add(new Arrow(Color.blue));
            if (Snitch.localArrows.Count != 0 && Snitch.localArrows[0] != null)
            {
                Snitch.localArrows[0].arrow.SetActive(true);
                Snitch.localArrows[0].Update(Snitch.snitch.transform.position);
            }
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl == Snitch.snitch && numberOfTasks == 0)
        {
            int arrowIndex = 0;
            foreach (PlayerControl p in CachedPlayer.AllPlayers)
            {
                bool arrowForImp = p.Data.Role.IsImpostor;
                if (Mimic.mimic == p) arrowForImp = true;
                bool arrowForTeamJackal = Snitch.includeTeamJackal && (p == Jackal.jackal || p == Sidekick.sidekick);
                bool arrowForTeamSwoop = Snitch.includeTeamJackal && (p == Swooper.swooper);

                if (!p.Data.IsDead && (arrowForImp || arrowForTeamJackal || arrowForTeamSwoop))
                {
                    if (arrowIndex >= Snitch.localArrows.Count)
                    {
                        Snitch.localArrows.Add(new Arrow(Palette.ImpostorRed));
                    }
                    if (arrowIndex < Snitch.localArrows.Count && Snitch.localArrows[arrowIndex] != null)
                    {
                        Snitch.localArrows[arrowIndex].arrow.SetActive(true);
                        if (arrowForTeamSwoop)
                        {
                            Snitch.localArrows[arrowIndex].Update(p.transform.position, Swooper.color);
                        }
                        else
                            Snitch.localArrows[arrowIndex].Update(p.transform.position, (arrowForTeamJackal && Snitch.teamJackalUseDifferentArrowColor ? Jackal.color : Palette.ImpostorRed));
                    }
                    arrowIndex++;
                }
            }
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
        if (BountyHunter.bountyHunter == null ||
            CachedPlayer.LocalPlayer.PlayerControl != BountyHunter.bountyHunter) return;

        if (BountyHunter.bountyHunter.Data.IsDead)
        {
            if (BountyHunter.arrow != null || BountyHunter.arrow.arrow != null)
                Object.Destroy(BountyHunter.arrow.arrow);
            BountyHunter.arrow = null;
            if (BountyHunter.cooldownText != null && BountyHunter.cooldownText.gameObject != null)
                Object.Destroy(BountyHunter.cooldownText.gameObject);
            BountyHunter.cooldownText = null;
            BountyHunter.bounty = null;
            foreach (var p in TORMapOptions.playerIcons.Values)
                if (p != null && p.gameObject != null)
                    p.gameObject.SetActive(false);
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
                    (p != Sidekick.sidekick || !Sidekick.wasTeamRed) && (p != Jackal.jackal || !Jackal.wasTeamRed) &&
                    (p != Mini.mini || Mini.isGrownUp()) && (BountyHunter.bountyHunter.getPartner() == null ||
                                                             p != BountyHunter.bountyHunter.getPartner()))
                    possibleTargets.Add(p);
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
                foreach (var pp in TORMapOptions.playerIcons.Values) pp.gameObject.SetActive(false);
                if (TORMapOptions.playerIcons.ContainsKey(BountyHunter.bounty.PlayerId) &&
                    TORMapOptions.playerIcons[BountyHunter.bounty.PlayerId].gameObject != null)
                    TORMapOptions.playerIcons[BountyHunter.bounty.PlayerId].gameObject.SetActive(true);
            }
        }

        // Hide in meeting
        if (MeetingHud.Instance && TORMapOptions.playerIcons.ContainsKey(BountyHunter.bounty.PlayerId) &&
            TORMapOptions.playerIcons[BountyHunter.bounty.PlayerId].gameObject != null)
            TORMapOptions.playerIcons[BountyHunter.bounty.PlayerId].gameObject.SetActive(false);

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
            if (BountyHunter.arrow == null) BountyHunter.arrow = new Arrow(Color.red);
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
            Vulture.localArrows = new List<Arrow>();
            return;
        }

        DeadBody[] deadBodies = Object.FindObjectsOfType<DeadBody>();
        var arrowUpdate = Vulture.localArrows.Count != deadBodies.Count();
        var index = 0;

        if (arrowUpdate)
        {
            foreach (var arrow in Vulture.localArrows) Object.Destroy(arrow.arrow);
            Vulture.localArrows = new List<Arrow>();
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
        if (Amnisiac.amnisiac == null || CachedPlayer.LocalPlayer.PlayerControl != Amnisiac.amnisiac ||
            Amnisiac.localArrows == null || !Amnisiac.showArrows) return;
        if (Amnisiac.amnisiac.Data.IsDead)
        {
            foreach (var arrow in Amnisiac.localArrows) Object.Destroy(arrow.arrow);
            Amnisiac.localArrows = new List<Arrow>();
            return;
        }

        DeadBody[] deadBodies = Object.FindObjectsOfType<DeadBody>();
        var arrowUpdate = Amnisiac.localArrows.Count != deadBodies.Count();
        var index = 0;

        if (arrowUpdate)
        {
            foreach (var arrow in Amnisiac.localArrows) Object.Destroy(arrow.arrow);
            Amnisiac.localArrows = new List<Arrow>();
        }

        foreach (var db in deadBodies)
        {
            if (arrowUpdate)
            {
                Amnisiac.localArrows.Add(new Arrow(Color.blue));
                Amnisiac.localArrows[index].arrow.SetActive(true);
            }

            if (Amnisiac.localArrows[index] != null) Amnisiac.localArrows[index].Update(db.transform.position);
            index++;
        }
    }

    private static void radarUpdate()
    {
        if (Radar.radar == null || CachedPlayer.LocalPlayer.PlayerControl != Radar.radar || Radar.localArrows == null ||
            !Radar.showArrows) return;
        if (Radar.radar.Data.IsDead)
        {
            foreach (var arrow in Radar.localArrows) Object.Destroy(arrow.arrow);
            Radar.localArrows = new List<Arrow>();
            return;
        }

        var arrowUpdate = true;
        var index = 0;

        if (arrowUpdate && !CachedPlayer.LocalPlayer.Data.IsDead)
        {
            foreach (var arrow in Radar.localArrows) Object.Destroy(arrow.arrow);
            Radar.ClosestPlayer = GetClosestPlayer(PlayerControl.LocalPlayer,
                PlayerControl.AllPlayerControls.ToArray().ToList());
            Radar.localArrows = new List<Arrow>();
        }


        foreach (PlayerControl player in CachedPlayer.AllPlayers)
        {
            if (arrowUpdate && !CachedPlayer.LocalPlayer.Data.IsDead)
            {
                Radar.localArrows.Add(new Arrow(Radar.color));
                Radar.localArrows[index].arrow.SetActive(true);
            }

            if (Radar.localArrows[index] != null)
                Radar.localArrows[index].Update(Radar.ClosestPlayer.transform.position);
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
            //           if (PhysicsHelpers.AnyNonTriggersBetween(
            //                refPosition, vector.normalized, vector.magnitude, Constants.ShipAndObjectsMask
            //            )) continue;
            num = distBetweenPlayers;
            result = player;
        }

        return result;
    }

    public static PlayerControl GetClosestPlayer(PlayerControl refplayer)
    {
        return GetClosestPlayer(refplayer, PlayerControl.AllPlayerControls.ToArray().ToList());
    }

    public static void SetTarget(
        ref PlayerControl closestPlayer,
        KillButton button,
        float maxDistance = float.NaN,
        List<PlayerControl> targets = null
    )
    {
        if (!button.isActiveAndEnabled) return;

        button.SetTarget(
            SetClosestPlayer(ref closestPlayer, maxDistance, targets)
        );
    }

    public static PlayerControl SetClosestPlayer(
        ref PlayerControl closestPlayer,
        float maxDistance = float.NaN,
        List<PlayerControl> targets = null
    )
    {
        if (float.IsNaN(maxDistance))
            maxDistance =
                GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
        var player = GetClosestPlayer(
            PlayerControl.LocalPlayer,
            targets ?? PlayerControl.AllPlayerControls.ToArray().ToList()
        );
        var closeEnough = player == null || GetDistBetweenPlayers(PlayerControl.LocalPlayer, player) < maxDistance;
        return closestPlayer = closeEnough ? player : null;
    }

    public static double GetDistBetweenPlayers(PlayerControl player, PlayerControl refplayer)
    {
        var truePosition = refplayer.GetTruePosition();
        var truePosition2 = player.GetTruePosition();
        return Vector2.Distance(truePosition, truePosition2);
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

    private static void morphlingAndCamouflagerUpdate()
    {
        var mushRoomSaboIsActive = Helpers.MushroomSabotageActive();
        if (!mushroomSaboWasActive) mushroomSaboWasActive = mushRoomSaboIsActive;

        if (Helpers.isCamoComms() && !Helpers.isActiveCamoComms())
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
        if (Helpers.isCamoComms()) return;
        if (Helpers.wasActiveCamoComms() && Camouflager.camouflageTimer <= 0f) Helpers.camoReset();

        // Camouflage reset and set Morphling look if necessary
        if (oldCamouflageTimer > 0f && Camouflager.camouflageTimer <= 0f)
        {
            Camouflager.resetCamouflage();
            Helpers.camoReset();
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
            RPCProcedure.lawyerPromotesToPursuer();
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

    private static void pursuerSetTarget()
    {
        if (Pursuer.pursuer == null || Pursuer.pursuer != CachedPlayer.LocalPlayer.PlayerControl) return;
        Pursuer.target = setTarget();
        setPlayerOutline(Pursuer.target, Pursuer.color);
    }

    private static void witchSetTarget()
    {
        if (Witch.witch == null || Witch.witch != CachedPlayer.LocalPlayer.PlayerControl) return;
        List<PlayerControl> untargetables;
        if (Witch.spellCastingTarget != null)
        {
            untargetables = PlayerControl.AllPlayerControls.ToArray()
                .Where(x => x.PlayerId != Witch.spellCastingTarget.PlayerId)
                .ToList(); // Don't switch the target from the the one you're currently casting a spell on
        }
        else
        {
            untargetables =
                new List<PlayerControl>(); // Also target players that have already been spelled, to hide spells that were blanks/blocked by shields
            if (Spy.spy != null && !Witch.canSpellAnyone) untargetables.Add(Spy.spy);
            if (Sidekick.wasTeamRed && !Witch.canSpellAnyone) untargetables.Add(Sidekick.sidekick);
            if (Jackal.wasTeamRed && !Witch.canSpellAnyone) untargetables.Add(Jackal.jackal);
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
        if (Sidekick.wasTeamRed && !Spy.impostorsCanKillAnyone) untargetables.Add(Sidekick.sidekick);
        if (Jackal.wasTeamRed && !Spy.impostorsCanKillAnyone) untargetables.Add(Jackal.jackal);
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
                if (entry.Key.killerIfExisting != null &&
                    entry.Key.killerIfExisting.PlayerId == CachedPlayer.LocalPlayer.PlayerId)
                {
                    Helpers
                        .handleVampireBiteOnBodyReport(); // Manually call Vampire handling, since the CmdReportDeadBody Prefix won't be called
                    Helpers
                        .handleBomber2ExplodeOnBodyReport(); // Manually call Vampire handling, since the CmdReportDeadBody Prefix won't be called
                    RPCProcedure.uncheckedCmdReportDeadBody(entry.Key.killerIfExisting.PlayerId,
                        entry.Key.player.PlayerId);

                    var writer = AmongUsClient.Instance.StartRpcImmediately(
                        CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.UncheckedCmdReportDeadBody,
                        SendOption.Reliable);
                    writer.Write(entry.Key.killerIfExisting.PlayerId);
                    writer.Write(entry.Key.player.PlayerId);
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
            var player = Helpers.playerById(entry.Key);
            var bloodyPlayer = Helpers.playerById(Bloody.bloodyKillerMap[player.PlayerId]);

            Bloody.active[entry.Key] = entry.Value - Time.fixedDeltaTime;
            if (entry.Value <= 0 || player.Data.IsDead)
            {
                Bloody.active.Remove(entry.Key);
                continue; // Skip the creation of the next blood drop, if the killer is dead or the time is up
            }

            new Bloodytrail(player, bloodyPlayer);
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
            HudManagerStartPatch.sidekickKillButton.MaxTimer = Sidekick.cooldown * multiplier;
            HudManagerStartPatch.warlockCurseButton.MaxTimer = Warlock.cooldown * multiplier;
            HudManagerStartPatch.cleanerCleanButton.MaxTimer = Cleaner.cooldown * multiplier;
            HudManagerStartPatch.witchSpellButton.MaxTimer =
                (Witch.cooldown + Witch.currentCooldownAddition) * multiplier;
            HudManagerStartPatch.ninjaButton.MaxTimer = Ninja.cooldown * multiplier;
            HudManagerStartPatch.thiefKillButton.MaxTimer = Thief.cooldown * multiplier;
            //!!!
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

    private static void hunterUpdate()
    {
        if (!HideNSeek.isHideNSeekGM) return;
        var minutes = (int)HideNSeek.timer / 60;
        var seconds = (int)HideNSeek.timer % 60;
        var suffix = $" {minutes:00}:{seconds:00}";

        if (HideNSeek.timerText == null)
        {
            var roomTracker = FastDestroyableSingleton<HudManager>.Instance?.roomTracker;
            if (roomTracker != null)
            {
                var gameObject = Object.Instantiate(roomTracker.gameObject);

                gameObject.transform.SetParent(FastDestroyableSingleton<HudManager>.Instance.transform);
                Object.DestroyImmediate(gameObject.GetComponent<RoomTracker>());
                HideNSeek.timerText = gameObject.GetComponent<TMP_Text>();

                // Use local position to place it in the player's view instead of the world location
                gameObject.transform.localPosition = new Vector3(0, -1.8f, gameObject.transform.localPosition.z);
                if (DataManager.Settings.Gameplay.StreamerMode)
                    gameObject.transform.localPosition = new Vector3(0, 2f, gameObject.transform.localPosition.z);
            }
        }
        else
        {
            if (HideNSeek.isWaitingTimer)
            {
                HideNSeek.timerText.text = "<color=#0000cc>" + suffix + "</color>";
                HideNSeek.timerText.color = Color.blue;
            }
            else
            {
                HideNSeek.timerText.text = "<color=#FF0000FF>" + suffix + "</color>";
                HideNSeek.timerText.color = Color.red;
            }
        }

        if (HideNSeek.isHunted() && !Hunted.taskPunish && !HideNSeek.isWaitingTimer)
        {
            var (playerCompleted, playerTotal) = TasksHandler.taskInfo(CachedPlayer.LocalPlayer.Data);
            var numberOfTasks = playerTotal - playerCompleted;
            if (numberOfTasks == 0)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                    (byte)CustomRPC.ShareTimer, SendOption.Reliable);
                writer.Write(HideNSeek.taskPunish);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.shareTimer(HideNSeek.taskPunish);

                Hunted.taskPunish = true;
            }
        }

        if (!HideNSeek.isHunter()) return;

        var playerId = CachedPlayer.LocalPlayer.PlayerId;
        foreach (var arrow in Hunter.localArrows) arrow.arrow.SetActive(false);
        if (Hunter.arrowActive)
        {
            var arrowIndex = 0;
            foreach (PlayerControl p in CachedPlayer.AllPlayers)
                if (!p.Data.IsDead && !p.Data.Role.IsImpostor)
                {
                    if (arrowIndex >= Hunter.localArrows.Count) Hunter.localArrows.Add(new Arrow(Color.blue));
                    if (arrowIndex < Hunter.localArrows.Count && Hunter.localArrows[arrowIndex] != null)
                    {
                        Hunter.localArrows[arrowIndex].arrow.SetActive(true);
                        Hunter.localArrows[arrowIndex].Update(p.transform.position, Color.blue);
                    }

                    arrowIndex++;
                }
        }
    }

    private static void cultistUpdate()
    {
        if (Cultist.localArrows == null) return;

        foreach (var arrow in Cultist.localArrows) arrow.arrow.SetActive(false);

        if (Cultist.cultist == null || Cultist.cultist.Data.IsDead) return;


        if (CachedPlayer.LocalPlayer.PlayerControl == Cultist.cultist)
        {
            var arrowIndex = 0;
            foreach (PlayerControl p in CachedPlayer.AllPlayers)
            {
                var arrowForImp = p == Follower.follower;
                if (!p.Data.IsDead && arrowForImp)
                {
                    if (arrowIndex >= Cultist.localArrows.Count)
                        Cultist.localArrows.Add(new Arrow(Palette.ImpostorRed));
                    if (arrowIndex < Cultist.localArrows.Count && Cultist.localArrows[arrowIndex] != null)
                    {
                        Cultist.localArrows[arrowIndex].arrow.SetActive(true);
                        Cultist.localArrows[arrowIndex].Update(p.transform.position, Palette.ImpostorRed);
                    }

                    arrowIndex++;
                }
            }
        }
    }

    private static void followerUpdate()
    {
        if (Follower.localArrows == null) return;

        foreach (var arrow in Follower.localArrows) arrow.arrow.SetActive(false);

        if (Follower.follower == null || Follower.follower.Data.IsDead) return;


        if (CachedPlayer.LocalPlayer.PlayerControl == Follower.follower)
        {
            var arrowIndex = 0;
            foreach (PlayerControl p in CachedPlayer.AllPlayers)
            {
                var arrowForImp = p == Cultist.cultist;
                if (!p.Data.IsDead && arrowForImp)
                {
                    if (arrowIndex >= Follower.localArrows.Count)
                        Follower.localArrows.Add(new Arrow(Palette.ImpostorRed));
                    if (arrowIndex < Follower.localArrows.Count && Follower.localArrows[arrowIndex] != null)
                    {
                        Follower.localArrows[arrowIndex].arrow.SetActive(true);
                        Follower.localArrows[arrowIndex].Update(p.transform.position, Palette.ImpostorRed);
                    }

                    arrowIndex++;
                }
            }
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
            if (Akujo.honmei == null)
            {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.AkujoSuicide, Hazel.SendOption.Reliable, -1);
                writer.Write(Akujo.akujo.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.akujoSuicide(Akujo.akujo.PlayerId);
            }
        }
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
    public static void Postfix(PlayerControl __instance)
    {
        if (AmongUsClient.Instance.GameState != InnerNetClient.GameStates.Started ||
            GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return;

        // Mini and Morphling shrink
        if (!PropHunt.isPropHuntGM) playerSizeUpdate(__instance);

        // set position of colorblind text
        foreach (var pc in PlayerControl.AllPlayerControls)
        {
            //pc.cosmetics.colorBlindText.gameObject.transform.localPosition = new Vector3(0, 0, -0.0001f);
        }

        if (CachedPlayer.LocalPlayer.PlayerControl == __instance)
        {
            // Update player outlines
            setBasePlayerOutlines();

            // Update Role Description
            Helpers.refreshRoleDescription(__instance);

            // Update Player Info
            updatePlayerInfo();

            //Update pet visibility
            setPetVisibility();

            // Time Master
            bendTimeUpdate();
            // Morphling
            morphlingSetTarget();
            // PrivateInvestigator
            privateInvestigatorSetTarget();
            // Medic
            medicSetTarget();
            // Bomber2
            bomber2SetTarget();
            // Set Werewolf Target
            werewolfSetTarget();
            //天启
            juggernautSetTarget();
            //末日预言家
            doomsayerSetTarget();
            // Swooper
            swooperSetTarget();
            swooperUpdate();
            // Prophet
            prophetSetTarget();
            prophetUpdate();
            // Shifter
            shifterSetTarget();
            // Sheriff
            sheriffSetTarget();
            // Deputy
            deputySetTarget();
            deputyUpdate();
            // Detective
            detectiveUpdateFootPrints();
            // Tracker
            trackerSetTarget();
            // Vampire
            vampireSetTarget();
            Garlic.UpdateAll();
            Trap.Update();
            // Eraser
            eraserSetTarget();
            // Engineer
            engineerUpdate();
            // Tracker
            trackerUpdate();
            // Jackal
            jackalSetTarget();
            // Sidekick
            sidekickSetTarget();
            // Impostor
            impostorSetTarget();
            // Warlock
            warlockSetTarget();
            // Check for deputy promotion on Sheriff disconnect
            deputyCheckPromotion();
            // Check for sidekick promotion on Jackal disconnect
            sidekickCheckPromotion();
            // SecurityGuard
            securityGuardSetTarget();
            securityGuardUpdate();
            // Arsonist
            arsonistSetTarget();
            // Snitch
            snitchUpdate();
            // BodyGuard
            bodyGuardSetTarget();
            // undertaker
            undertakerDragBodyUpdate();
            // Amnisiac
            amnisiacUpdate();
            // BountyHunter
            bountyHunterUpdate();
            // Vulture
            vultureUpdate();
            radarUpdate();
            // Medium
            mediumSetTarget();
            // Morphling and Camouflager
            morphlingAndCamouflagerUpdate();
            // Lawyer
            lawyerUpdate();
            // Pursuer
            pursuerSetTarget();
            // Blackmailer
            blackMailerSetTarget();
            // Witch
            witchSetTarget();
            // Cultist
            cultistUpdate();
            followerUpdate();
            //Cultist
            cultistSetFollower();
            // Ninja
            ninjaSetTarget();
            NinjaTrace.UpdateAll();
            ninjaUpdate();
            // Thief
            thiefSetTarget();

            hackerUpdate();
            swapperUpdate();
            // Hacker
            hackerUpdate();
            // Trapper
            trapperUpdate();
            //魅魔
            akujoUpdate();
            akujoSetTarget();
            // -- MODIFIER--
            // Bait
            baitUpdate();
            // Bloody
            bloodyUpdate();
            // mini (for the cooldowns)
            miniCooldownUpdate();
            // Chameleon (invis stuff, timers)
            Chameleon.update();
            Bomb.update();

            // -- GAME MODE --
            hunterUpdate();
            PropHunt.update();
        }
    }
}

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.WalkPlayerTo))]
internal class PlayerPhysicsWalkPlayerToPatch
{
    private static Vector2 offset = Vector2.zero;

    public static void Prefix(PlayerPhysics __instance)
    {
        var correctOffset = !Helpers.isCamoComms() && Camouflager.camouflageTimer <= 0f &&
                            !Helpers.MushroomSabotageActive() && (__instance.myPlayer == Mini.mini ||
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

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdReportDeadBody))]
internal class PlayerControlCmdReportDeadBodyPatch
{
    public static bool Prefix(PlayerControl __instance)
    {
        if (HideNSeek.isHideNSeekGM || PropHunt.isPropHuntGM) return false;
        Helpers.handleVampireBiteOnBodyReport();
        Helpers.handleBomber2ExplodeOnBodyReport();
        return true;
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(CachedPlayer.LocalPlayer.PlayerControl.CmdReportDeadBody))]
internal class BodyReportPatch
{
    private static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] GameData.PlayerInfo target)
    {
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
            var deadPlayer = deadPlayers?.Where(x => x.player?.PlayerId == target?.PlayerId)?.FirstOrDefault();
            if (deadPlayer != null && deadPlayer.killerIfExisting != null)
            {
                var timeSinceDeath = (float)(DateTime.UtcNow - deadPlayer.timeOfDeath).TotalMilliseconds;
                var msg = "";

                if (isMedicReport)
                {
                    if (timeSinceDeath < Medic.ReportNameDuration * 1000)
                    {
                        msg = $"尸检报告: 凶手似乎是 {deadPlayer.killerIfExisting.Data.PlayerName}!\n尸体在 {Math.Round(timeSinceDeath / 1000)} 秒前死亡";
                    }
                    else if (timeSinceDeath < Medic.ReportColorDuration * 1000)
                    {
                        var typeOfColor = Helpers.isLighterColor(deadPlayer.killerIfExisting) ? "浅" : "深";
                        msg = $"尸检报告: 凶手似乎是 {typeOfColor} 色的!\n尸体在{Math.Round(timeSinceDeath / 1000)}秒前死亡";
                    }
                    else
                    {
                        msg = $"尸检报告: 死亡时间太久，无法获取信息! \n尸体在{Math.Round(timeSinceDeath / 1000)}秒前死亡";
                    }
                }
                else if (isDetectiveReport)
                {
                    if (timeSinceDeath < Detective.reportNameDuration * 1000)
                    {
                        msg = $"尸检报告: 凶手似乎是 {RoleInfo.getRoleInfoForPlayer(deadPlayer.killerIfExisting).First(x => !x.isModifier).name} !\n尸体在 {Math.Round(timeSinceDeath / 1000)} 秒前死亡";
                    }
                    else if (timeSinceDeath < Detective.reportColorDuration * 1000)
                    {
                        var typeOfColor = Helpers.isLighterColor(deadPlayer.killerIfExisting) ? "浅" : "深";
                        msg = $"尸检报告: 凶手似乎是 {typeOfColor} 色的!\n尸体在{Math.Round(timeSinceDeath / 1000)}秒前死亡";
                    }
                    else
                    {
                        msg = $"尸检报告: 死亡时间太久，无法获取信息\n尸体在 {Math.Round(timeSinceDeath / 1000)} 秒前死亡";
                    }
                }

                if (!string.IsNullOrWhiteSpace(msg))
                {
                    if (AmongUsClient.Instance.AmClient && FastDestroyableSingleton<HudManager>.Instance)
                    {
                        FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(
                            CachedPlayer.LocalPlayer.PlayerControl, msg);

                        // Ghost Info
                        var writer = AmongUsClient.Instance.StartRpcImmediately(
                            CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.ShareGhostInfo,
                            SendOption.Reliable);
                        writer.Write(CachedPlayer.LocalPlayer.PlayerId);
                        writer.Write((byte)RPCProcedure.GhostInfoTypes.DetectiveOrMedicInfo);
                        writer.Write(msg);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }

                    if (msg.IndexOf("who", StringComparison.OrdinalIgnoreCase) >= 0)
                        FastDestroyableSingleton<UnityTelemetry>.Instance.SendWho();
                }
            }
        }

        if (isSluethReport)
        {
            var reported = Helpers.playerById(target.PlayerId);
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
        var deadPlayer = new DeadPlayer(target, DateTime.UtcNow, DeadPlayer.CustomDeathReason.Kill, __instance);
        deadPlayers.Add(deadPlayer);

        // Reset killer to crewmate if resetToCrewmate
        if (resetToCrewmate) __instance.Data.Role.TeamType = RoleTeamTypes.Crewmate;
        if (resetToDead) __instance.Data.IsDead = true;

        // Remove fake tasks when player dies
        if (target.hasFakeTasks() || target == Lawyer.lawyer || target == Pursuer.pursuer || target == Thief.thief)
            target.clearAllTasks();

        // First kill (set before lover suicide)
        if (TORMapOptions.firstKillName == "") TORMapOptions.firstKillName = target.Data.PlayerName;

        // Lover suicide trigger on murder
        if ((Lovers.lover1 != null && target == Lovers.lover1) || (Lovers.lover2 != null && target == Lovers.lover2))
        {
            var otherLover = target == Lovers.lover1 ? Lovers.lover2 : Lovers.lover1;
            if (otherLover != null && !otherLover.Data.IsDead && Lovers.bothDie)
            {
                otherLover.MurderPlayer(otherLover);
                overrideDeathReasonAndKiller(otherLover, DeadPlayer.CustomDeathReason.LoverSuicide);
            }
        }

        if (target.Data.Role.IsImpostor && AmongUsClient.Instance.AmHost)
        {
            LastImpostor.promoteToLastImpostor();
        }

        // Sidekick promotion trigger on murder
        if (Sidekick.promotesToJackal && Sidekick.sidekick != null && !Sidekick.sidekick.Data.IsDead &&
            target == Jackal.jackal && Jackal.jackal == CachedPlayer.LocalPlayer.PlayerControl)
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                (byte)CustomRPC.SidekickPromotes, SendOption.Reliable);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.sidekickPromotes();
        }

        // Pursuer promotion trigger on murder (the host sends the call such that everyone recieves the update before a possible game End)
        if (target == Lawyer.target && AmongUsClient.Instance.AmHost && Lawyer.lawyer != null)
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                (byte)CustomRPC.LawyerPromotesToPursuer, SendOption.Reliable);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.lawyerPromotesToPursuer();

            // Undertaker Button Sync
            if (Undertaker.undertaker != null && CachedPlayer.LocalPlayer.PlayerControl == Undertaker.undertaker &&
                __instance == Undertaker.undertaker && HudManagerStartPatch.undertakerDragButton != null)
                HudManagerStartPatch.undertakerDragButton.Timer = Undertaker.dragingDelaiAfterKill;
        }

        // Seer show flash and add dead player position
        if (Seer.seer != null &&
            (CachedPlayer.LocalPlayer.PlayerControl == Seer.seer || Helpers.shouldShowGhostInfo()) &&
            !Seer.seer.Data.IsDead && Seer.seer != target && Seer.mode <= 1)
            Helpers.showFlash(new Color(42f / 255f, 187f / 255f, 245f / 255f), message: "灵媒信息: 有人死亡");
        if (Seer.deadBodyPositions != null) Seer.deadBodyPositions.Add(target.transform.position);

        // Tracker store body positions
        if (Tracker.deadBodyPositions != null) Tracker.deadBodyPositions.Add(target.transform.position);

        // Medium add body
        if (Medium.deadBodies != null)
            Medium.futureDeadBodies.Add(new Tuple<DeadPlayer, Vector3>(deadPlayer, target.transform.position));

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
                BountyHunter.bountyHunter.SetKillTimer(
                    GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + BountyHunter.punishmentTime);
            }
        }

        if (LastImpostor.lastImpostor != null && __instance == LastImpostor.lastImpostor && CachedPlayer.LocalPlayer.PlayerControl == __instance)
        {
            LastImpostor.lastImpostor.SetKillTimer(Mathf.Min(1f, GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown - LastImpostor.Deduce));
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

        // Warlock Button Sync
        if (Warlock.warlock != null && CachedPlayer.LocalPlayer.PlayerControl == Warlock.warlock &&
            __instance == Warlock.warlock && HudManagerStartPatch.warlockCurseButton != null)
            if (Warlock.warlock.killTimer > HudManagerStartPatch.warlockCurseButton.Timer)
                HudManagerStartPatch.warlockCurseButton.Timer = Warlock.warlock.killTimer;
        // Ninja Button Sync
        if (Ninja.ninja != null && CachedPlayer.LocalPlayer.PlayerControl == Ninja.ninja && __instance == Ninja.ninja &&
            HudManagerStartPatch.ninjaButton != null)
            HudManagerStartPatch.ninjaButton.Timer = HudManagerStartPatch.ninjaButton.MaxTimer;

        // Bait
        if (Bait.bait.FindAll(x => x.PlayerId == target.PlayerId).Count > 0)
        {
            float reportDelay = rnd.Next((int)Bait.reportDelayMin, (int)Bait.reportDelayMax + 1);
            Bait.active.Add(deadPlayer, reportDelay);

            if (Bait.showKillFlash && __instance == CachedPlayer.LocalPlayer.PlayerControl)
                Helpers.showFlash(new Color(204f / 255f, 102f / 255f, 0f / 255f));
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
                else if (RoleInfo.getRoleInfoForPlayer(target, false).FirstOrDefault().isNeutral) color = Color.blue;
            }

            Helpers.showFlash(color, 1.5f);
        }

        // HideNSeek
        if (HideNSeek.isHideNSeekGM)
        {
            var visibleCounter = 0;
            var bottomLeft = IntroCutsceneOnDestroyPatch.bottomLeft + new Vector3(-0.25f, -0.25f, 0);
            foreach (PlayerControl p in CachedPlayer.AllPlayers)
            {
                if (!TORMapOptions.playerIcons.ContainsKey(p.PlayerId) || p.Data.Role.IsImpostor) continue;
                if (p.Data.IsDead || p.Data.Disconnected)
                {
                    TORMapOptions.playerIcons[p.PlayerId].gameObject.SetActive(false);
                }
                else
                {
                    TORMapOptions.playerIcons[p.PlayerId].transform.localPosition =
                        bottomLeft + (Vector3.right * visibleCounter * 0.35f);
                    visibleCounter++;
                }
            }
        }
        // Snitch
        if (Snitch.snitch != null && CachedPlayer.LocalPlayer.PlayerId == Snitch.snitch.PlayerId &&
            MapBehaviourPatch.herePoints.Keys.Any(x => x.PlayerId == target.PlayerId))
        {
            foreach (var a in MapBehaviourPatch.herePoints.Where(x => x.Key.PlayerId == target.PlayerId))
            {
                Object.Destroy(a.Value);
                MapBehaviourPatch.herePoints.Remove(a.Key);
            }
        }
        // Akujo Lovers trigger suicide
        if ((Akujo.akujo != null && target == Akujo.akujo) || (Akujo.honmei != null && target == Akujo.honmei))
        {
            PlayerControl akujoPartner = target == Akujo.akujo ? Akujo.honmei : Akujo.akujo;
            if (akujoPartner != null && !akujoPartner.Data.IsDead)
            {
                akujoPartner.MurderPlayer(akujoPartner, MurderResultFlags.Succeeded);
                GameHistory.overrideDeathReasonAndKiller(akujoPartner, DeadPlayer.CustomDeathReason.LoverSuicide);
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
        if (LastImpostor.lastImpostor != null && CachedPlayer.LocalPlayer.PlayerControl == LastImpostor.lastImpostor)
            addition = -Mathf.Max((GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown - LastImpostor.Deduce), __instance.killTimer - 1f);

        __instance.killTimer = Mathf.Clamp(time, 0f,
            (GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown * multiplier) + addition);
        FastDestroyableSingleton<HudManager>.Instance.KillButton.SetCoolDown(__instance.killTimer,
            (GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown * multiplier) + addition);
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
        var deadPlayer = new DeadPlayer(__instance, DateTime.UtcNow, DeadPlayer.CustomDeathReason.Exile, null);
        deadPlayers.Add(deadPlayer);


        // Remove fake tasks when player dies
        if (__instance.hasFakeTasks() || __instance == Lawyer.lawyer || __instance == Pursuer.pursuer ||
            __instance == Thief.thief)
            __instance.clearAllTasks();

        // Lover suicide trigger on exile
        if ((Lovers.lover1 != null && __instance == Lovers.lover1) ||
            (Lovers.lover2 != null && __instance == Lovers.lover2))
        {
            var otherLover = __instance == Lovers.lover1 ? Lovers.lover2 : Lovers.lover1;
            if (otherLover != null && !otherLover.Data.IsDead && Lovers.bothDie)
            {
                otherLover.Exiled();
                overrideDeathReasonAndKiller(otherLover, DeadPlayer.CustomDeathReason.LoverSuicide);
            }
        }

        if (__instance.Data.Role.IsImpostor && AmongUsClient.Instance.AmHost)
        {
            LastImpostor.promoteToLastImpostor();
        }

        // Sidekick promotion trigger on exile
        if (Sidekick.promotesToJackal && Sidekick.sidekick != null && !Sidekick.sidekick.Data.IsDead &&
            __instance == Jackal.jackal && Jackal.jackal == CachedPlayer.LocalPlayer.PlayerControl)
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                (byte)CustomRPC.SidekickPromotes, SendOption.Reliable);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.sidekickPromotes();
        }

        // Pursuer promotion trigger on exile & suicide (the host sends the call such that everyone recieves the update before a possible game End)
        if (Lawyer.lawyer != null && __instance == Lawyer.target)
        {
            var lawyer = Lawyer.lawyer;
            if (AmongUsClient.Instance.AmHost &&
                ((Lawyer.target != Jester.jester && !Lawyer.isProsecutor) || Lawyer.targetWasGuessed))
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                    (byte)CustomRPC.LawyerPromotesToPursuer, SendOption.Reliable);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.lawyerPromotesToPursuer();
            }

            if (!Lawyer.targetWasGuessed && !Lawyer.isProsecutor)
            {
                if (Lawyer.lawyer != null) Lawyer.lawyer.Exiled();
                if (Pursuer.pursuer != null) Pursuer.pursuer.Exiled();

                var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                    (byte)CustomRPC.ShareGhostInfo, SendOption.Reliable);
                writer.Write(CachedPlayer.LocalPlayer.PlayerId);
                writer.Write((byte)RPCProcedure.GhostInfoTypes.DeathReasonAndKiller);
                writer.Write(lawyer.PlayerId);
                writer.Write((byte)DeadPlayer.CustomDeathReason.LawyerSuicide);
                writer.Write(lawyer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                overrideDeathReasonAndKiller(lawyer, DeadPlayer.CustomDeathReason.LawyerSuicide,
                    lawyer); // TODO: only executed on host?!
            }
        }
        // Akujo Partner suicide
        if ((Akujo.akujo != null && Akujo.akujo == __instance) || (Akujo.honmei != null && Akujo.honmei == __instance))
        {
            PlayerControl akujoPartner = __instance == Akujo.akujo ? Akujo.honmei : Akujo.akujo;
            if (akujoPartner != null && !akujoPartner.Data.IsDead)
            {
                akujoPartner.Exiled();
                GameHistory.overrideDeathReasonAndKiller(akujoPartner, DeadPlayer.CustomDeathReason.LoverSuicide);
            }

            if (MeetingHud.Instance && akujoPartner != null)
            {
                foreach (PlayerVoteArea pva in MeetingHud.Instance.playerStates)
                {
                    if (pva.VotedFor != akujoPartner.PlayerId) continue;
                    pva.UnsetVote();
                    var voteAreaPlayer = Helpers.playerById(pva.TargetPlayerId);
                    if (!voteAreaPlayer.AmOwner) continue;
                    MeetingHud.Instance.ClearVote();
                }

                if (AmongUsClient.Instance.AmHost)
                    MeetingHud.Instance.CheckForEndVoting();
            }
        }
    }
}

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
public static class PlayerPhysicsFixedUpdate
{
    public static void Postfix(PlayerPhysics __instance)
    {
        bool shouldInvert = Invert.invert.FindAll(x => x.PlayerId == CachedPlayer.LocalPlayer.PlayerId).Count > 0 && Invert.meetings > 0;
        if (__instance.AmOwner &&
            AmongUsClient.Instance &&
            AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started &&
            !CachedPlayer.LocalPlayer.Data.IsDead &&
            shouldInvert &&
            GameData.Instance &&
            __instance.myPlayer.CanMove)
            __instance.body.velocity *= -1; 
        if (__instance.AmOwner &&
                AmongUsClient.Instance &&
                AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started &&
                !CachedPlayer.LocalPlayer.Data.IsDead &&
                GameData.Instance &&
                __instance.myPlayer.CanMove &&
                Flash.flash.Any(x => x.PlayerId == CachedPlayer.LocalPlayer.PlayerId))
        {
            __instance.body.velocity *= Flash.speed;
        }
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.IsFlashlightEnabled))]
public static class IsFlashlightEnabledPatch
{
    public static bool Prefix(ref bool __result)
    {
        if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek)
            return true;
        __result = false;
        if (!CachedPlayer.LocalPlayer.Data.IsDead && Lighter.lighter != null &&
            Lighter.lighter.PlayerId == CachedPlayer.LocalPlayer.PlayerId) __result = true;

        return false;
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.AdjustLighting))]
public static class AdjustLight
{
    public static bool Prefix(PlayerControl __instance)
    {
        if (__instance == null || CachedPlayer.LocalPlayer == null || Lighter.lighter == null) return true;

        var hasFlashlight = !CachedPlayer.LocalPlayer.Data.IsDead &&
                            Lighter.lighter.PlayerId == CachedPlayer.LocalPlayer.PlayerId;
        __instance.SetFlashlightInputMethod();
        __instance.lightSource.SetupLightingForGameplay(hasFlashlight, Lighter.flashlightWidth,
            __instance.TargetFlashlight.transform);

        return false;
    }
}

[HarmonyPatch(typeof(GameData), nameof(GameData.HandleDisconnect), typeof(PlayerControl), typeof(DisconnectReasons))]
public static class GameDataHandleDisconnectPatch
{
    public static void Prefix(GameData __instance, PlayerControl player, DisconnectReasons reason)
    {
        if (MeetingHud.Instance) MeetingHudPatch.swapperCheckAndReturnSwap(MeetingHud.Instance, player.PlayerId);
    }
}
