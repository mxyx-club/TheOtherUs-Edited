using AmongUs.GameOptions;
using Assets.CoreScripts;
using TheOtherRoles.Objects;

namespace TheOtherRoles.Patches;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
public static class PlayerControlFixedUpdatePatch
{
    private static bool mushroomSaboWasActive;

    public static PlayerControl SetTarget(bool onlyCrew = false,
        bool InVents = false,
        IEnumerable<PlayerControl> untarget = null,
        PlayerControl targetingPlayer = null,
        IEnumerable<PlayerControl> targetPlayers = null,
        float range = 0f)
    {
        PlayerControl result = null;
        var num = GameOptionsData.KillDistances[Mathf.Clamp(GameOptionsManager.Instance.currentNormalGameOptions.KillDistance, 0, 3)] * (1 + range);
        if (!MapUtilities.CachedShipStatus) return null;
        if (targetingPlayer == null) targetingPlayer = PlayerControl.LocalPlayer;
        if (targetingPlayer.Data.IsDead) return null;

        var candidates = targetPlayers ?? PlayerControl.AllPlayerControls.GetFastEnumerator();

        var truePosition = targetingPlayer.GetTruePosition();
        foreach (var player in candidates)
        {
            if (player.IsAlive() && player.PlayerId != targetingPlayer.PlayerId && (!onlyCrew || !player.IsImpostor(true, true)))
            {
                var target = player;
                // if that player is not targetable: skip check
                if (untarget != null && untarget.Any(x => x == target))
                    continue;

                if (target && (!target.inVent || InVents))
                {
                    var vector = target.GetTruePosition() - truePosition;
                    var magnitude = vector.magnitude;
                    if (magnitude <= num && !PhysicsHelpers.AnyNonTriggersBetween(truePosition, vector.normalized, magnitude, Constants.ShipAndObjectsMask))
                    {
                        result = target;
                        num = magnitude;
                    }
                }
            }
        }

        return result;
    }

    // Update functions

    private static void setPetVisibility(PlayerControl player)
    {
        var localAlive = PlayerControl.LocalPlayer.IsAlive();
        var playerAlive = player.IsAlive();
        var shouldShowPet = (localAlive && playerAlive) || !localAlive;
        player.cosmetics.SetPetVisible(shouldShowPet);
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

        var growingProgress = Mini.growingProgress;
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

    private static void morphlingAndCamouflagerUpdate()
    {
        var mushRoomSaboIsActive = MushroomSabotageActive;
        if (!mushroomSaboWasActive) mushroomSaboWasActive = mushRoomSaboIsActive;

        if (isCamoComms && !isActiveCamoComms)
        {
            var writer = StartRPC(CustomRPC.CamouflagerCamouflage);
            writer.Write(0);
            writer.EndRPC();
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

    public static void Postfix(PlayerControl __instance)
    {
        if (!InGame || IsHideNSeek) return;

        // Mini and Morphling shrink
        MiniSizeUpdate(__instance);
        GiantSizeUpdate(__instance);

        if (PlayerControl.LocalPlayer == __instance)
        {
            // Update Role Description
            refreshRoleDescription(__instance);

            //Update pet visibility
            setPetVisibility(__instance);

            if (!InGame) return;

            // Morphling and Camouflager
            morphlingAndCamouflagerUpdate();

            // Chameleon (invis stuff, timers)
            Chameleon.update();
        }
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
            var currentScaling = (Mini.growingProgress + 1) * 0.5f;
            __instance.myPlayer.Collider.offset = currentScaling * Mini.defaultColliderOffset * Vector2.down;
        }
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Revive))]
internal class PlayerControlRevivePatch
{
    public static void Postfix(PlayerControl __instance)
    {
        if (__instance.AmOwner == true)
        {
            CanSeeGhostInfo = false;
            CustomButton.ResetAllCooldowns(ModOption.KillCooldown / 2);
        }

        if (__instance.isLover() && Lovers.otherLover(__instance)?.IsDead() == true)
        {
            Lovers.otherLover(__instance)?.ModRevive();
        }

        if (Akujo.isAkujoTeam(__instance) && Akujo.otherLover(__instance)?.IsDead() == true)
        {
            Akujo.otherLover(__instance)?.ModRevive();
        }

        if (__instance == Specter.Player) Specter.Player.clearAllTasks();

        if (__instance.IsImpostor()) RoleManager.Instance.SetRole(__instance, RoleTypes.Impostor);
        else RoleManager.Instance.SetRole(__instance, RoleTypes.Crewmate);

        RPCProcedure.clearGhostRoles(__instance.PlayerId);

        var data = PlayerData.GetPlayerData(__instance);
        if (data != null)
        {
            data.DeathReason = CustomDeathReason.Null;
            data.KilledBy = null;
            data.DeathTimer = DateTime.MinValue;
        }
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdReportDeadBody))]
internal class BodyReportPatch
{
    public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] GameData.PlayerInfo target)
    {
        if (ModOption.DisableMeeting) return false;

        if (target == null && CustomOptionHolder.TheFungleMushroomMixupOption.GetBool() &&
            CustomOptionHolder.TheFungleMushroomMixupCantOpenMeeting.GetBool() &&
            __instance.IsMushroomMixupActive())
            return false;

        handleVampireBiteOnBodyReport();
        handleBomberExplodeOnBodyReport();
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
            var deadPlayer = PlayerData.AllPlayerData.Values?.Where(x => x.PlayerId == target?.PlayerId && x.IsDead)?.FirstOrDefault();
            if (deadPlayer != null && deadPlayer.KilledBy != null)
            {
                var timeSinceDeath = (float)(DateTime.UtcNow - deadPlayer.DeathTimer).TotalMilliseconds;
                var msg = "";
                var killer = deadPlayer.KilledBy;
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
                        msg = string.Format(GetString("MedicReport.ReportName"), killer.Data.PlayerName, timer);
                    }
                    else if (timer <= Medic.ReportColorDuration)
                    {
                        var colorKey = IsLightColor(killer) ? "Color.Light" : "Color.Dark";
                        msg = string.Format(GetString("MedicReport.ReportColor"), GetString(colorKey), timer);
                    }
                    else
                    {
                        msg = string.Format(GetString("MedicReport.ReportExpired"), timer);
                    }
                }
                else if (isDetectiveReport)
                {
                    if (timer <= Detective.reportNameDuration)
                    {
                        var roleName = RoleInfo.getRoleInfoForPlayer(killer, false, false).First().Name;
                        msg = string.Format(GetString("DetectiveReport.ReportRole"), roleName, timer);
                    }
                    else if (timer <= Detective.reportColorDuration)
                    {
                        msg = string.Format(GetString("DetectiveReport.ReportTeam"), teamString(killer), timer);
                    }
                    else
                    {
                        msg = string.Format(GetString("DetectiveReport.ReportExpired"), timer);
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
            var killer = PlayerData.AllPlayerData.Values.FirstOrDefault(x => x.PlayerId == target?.PlayerId)?.KilledBy;
            var witnessTarget = Witness.DetermineKillerTarget(killer);
            var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.WitnessReport);
            writer.Write(witnessTarget?.PlayerId ?? byte.MaxValue);
            writer.EndRPC();
            Witness.WitnessReport(witnessTarget?.PlayerId ?? byte.MaxValue);
        }

        if (isSluethReport)
        {
            var reported = PlayerById(target?.PlayerId);
            Slueth.reported.TryAdd(reported);
        }
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Die))]
public static class PlayerDiePatch
{
    public static void Postfix(PlayerControl __instance)
    {
        Sheriff.deputyCheckPromotion();

        if (!InGame || PlayerControl.LocalPlayer != __instance) return;
        if (Prosecutor.prosecutor == __instance)
        {
            Prosecutor.Prosecuted = false;
            Prosecutor.StartProsecute = false;
            Prosecutor.ProsecuteThisMeeting = false;
        }
        if (ModOption.GameMode is CustomGameModes.Classic or CustomGameModes.Anonymous) return;
        _ = new LateTask(() => { CanSeeGhostInfo = true; }, 1f, "CanSeeRoleInfo");
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
public static class MurderPlayerPatch
{

    public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
    {
        if (SchrodingersCat.Player != null && target == SchrodingersCat.Player && SchrodingersCat.remainingChange > 0)
        {
            var role = RoleInfo.getRoleInfoForPlayer(__instance, false, false).FirstOrDefault();
            var state = SchrodingersCat.State;
            if (role != null && PlayerControl.LocalPlayer == SchrodingersCat.Player)
            {
                if (role.roleId is RoleId.Jackal or RoleId.Sidekick) state = SchrodingersCat.CatState.Jackal;
                else if (role.roleId is RoleId.Pavlovsdogs or RoleId.Pavlovsowner) state = SchrodingersCat.CatState.Pavlovsowner;
                else if (role.roleId == RoleId.Werewolf) state = SchrodingersCat.CatState.Werewolf;
                else if (role.roleId == RoleId.Juggernaut) state = SchrodingersCat.CatState.Juggernaut;
                else if (role.roleId == RoleId.Swooper) state = SchrodingersCat.CatState.Swooper;
                else if (role.roleId == RoleId.Infected) state = SchrodingersCat.CatState.Infected;
                else if (role.roleId == RoleId.Arsonist) state = SchrodingersCat.CatState.Arsonist;
                else if (role.roleId == RoleId.Pelican) state = SchrodingersCat.CatState.Pelican;
                else if (role.roleType == RoleType.Impostor) state = SchrodingersCat.CatState.Impostor;
                else if (role.roleType == RoleType.Crewmate) state = SchrodingersCat.CatState.Crewmate;

                var writer = StartRPC(CustomRPC.SchrodingersCatSetState);
                writer.Write((byte)state);
                writer.EndRPC();
                SchrodingersCat.State = state;
                HudManagerStartPatch.schrodingersCatKillButton.Timer = HudManagerStartPatch.schrodingersCatKillButton.MaxTimer / 2;
            }

            if (PlayerControl.LocalPlayer == __instance)
            {
                if (Constants.ShouldPlaySfx())
                {
                    SoundManager.Instance.PlaySound(__instance.KillSfx, false, 0.8f, null);
                }
                if (!KillAnimationCoPerformKillPatch.hideNextAnimation) __instance.NetTransform.RpcSnapTo(target.transform.position);
                __instance.SetKillTimer(ModOption.KillCooldown);
            }
            else if (PlayerControl.LocalPlayer == target)
            {
                //target.SetPlayerMaterialColors(deadBody.bloodSplatter);
                DestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(__instance.Data, target.Data);
            }

            return false;
        }

        return true;
    }

    public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
    {
        if (SchrodingersCat.Player != null && target == SchrodingersCat.Player && SchrodingersCat.remainingChange > 0)
        {
            SchrodingersCat.ChangeCount++;
            Message($"SchrodingersCat.State: {SchrodingersCat.State}");
            return;
        }
        HandleMurderPostfix(__instance, target);
    }

    public static void HandleMurderPostfix(PlayerControl __instance, PlayerControl target)
    {
        // Collect dead player info
        var deathReason = __instance == target ? CustomDeathReason.Suicide : CustomDeathReason.Kill;
        PlayerData.SetDeathReason(target, deathReason, __instance);

        // Remove fake tasks when player dies
        if (target.HasFakeTasks() || target == Lawyer.lawyer || Pursuer.Player.Contains(target) || target == Thief.thief)
            target.clearAllTasks();

        // First kill (set before lover suicide)
        if (ModOption.firstKillName == "") ModOption.firstKillName = target.Data.PlayerName;

        if (__instance == Poucher.poucher) Poucher.killed.Add(target);

        if (PlayerControl.LocalPlayer == __instance && __instance == Mimic.mimic && !Mimic.hasMimic)
        {
            var writerMimic = StartRPC(CustomRPC.MimicMimicRole);
            writerMimic.Write(target.PlayerId);
            writerMimic.EndRPC();
            Mimic.MimicRole(target.PlayerId);
        }

        Avenger.OnPlayerDeath(__instance, target);

        // Bait
        if (Bait.bait.Any(x => x.PlayerId == target.PlayerId))
        {
            var reportDelay = (float)rnd.NextDouble(Bait.reportDelayMin, Bait.reportDelayMax);
            reportDelay = Math.Max(reportDelay, 0.12f);

            if (__instance.AmOwner)
            {
                _ = new LateTask(() =>
                {
                    __instance?.CmdReportDeadBody(target.Data);
                }, reportDelay, "Bait Activate");

            }

            if (Bait.showKillFlash && __instance == PlayerControl.LocalPlayer)
                showFlash(new Color(204f / 255f, 102f / 255f, 0f / 255f));
        }

        if (Bloody.bloody.Any(x => x.PlayerId == target.PlayerId))
        {
            Bloodytrail.StartBloodTrail(__instance, target);
        }

        if (Aftermath.aftermath != null && Aftermath.aftermath == target && PlayerControl.LocalPlayer == __instance)
        {
            _ = new LateTask(() =>
            {
                Aftermath.afterTrigger(target.PlayerId, __instance.PlayerId);

            }, 0.2f, "Aftermath Trigger!");
        }

        // Pursuer promotion trigger on murder (the host sends the call such that everyone recieves the update before a possible game End)
        if (target == Lawyer.target && AmongUsClient.Instance.AmHost && Lawyer.lawyer != null)
        {
            var writer = StartRPC(CustomRPC.LawyerPromotesToPursuer);
            writer.Write(false);
            writer.EndRPC();
            Lawyer.PromotesToPursuer(false);
        }

        if (target == Executioner.target && AmongUsClient.Instance.AmHost && Executioner.executioner != null)
        {
            var writer = StartRPC(CustomRPC.ExecutionerPromotesRole);
            writer.EndRPC();
            Executioner.PromotesRole();
        }

        if (target == Pelican.Player && Pelican.eatenPlayers?.Count > 0)
        {
            foreach (var player in Pelican.eatenPlayers.Where(x => x.Data?.IsDead == true))
            {
                player.Revive();
                if (PlayerControl.LocalPlayer == player)
                {
                    HudManager.Instance.PlayerCam.SetTargetWithLight(PlayerControl.LocalPlayer);
                    _ = new LateTask(() =>
                    {
                        HudManager.Instance.PlayerCam.SetTargetWithLight(PlayerControl.LocalPlayer);
                    }, 0.25f);
                    PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(Pelican.Player.transform.position);
                }
                var data = PlayerData.GetPlayerData(player);
                if (data != null)
                {
                    data.DeathReason = CustomDeathReason.Null;
                    data.KilledBy = null;
                    data.DeathTimer = DateTime.MinValue;
                }
                continue;
            }
            Pelican.eatenPlayers = new();
        }

        // Undertaker Button Sync
        if (Undertaker.undertaker != null && PlayerControl.LocalPlayer == Undertaker.undertaker &&
            __instance == Undertaker.undertaker && HudManagerStartPatch.undertakerDragButton != null)
            HudManagerStartPatch.undertakerDragButton.Timer = Undertaker.dragingDelaiAfterKill;

        // Seer show flash and add dead player position
        if (Seer.seer != null &&
            (PlayerControl.LocalPlayer == Seer.seer || CanSeeGhostInfo) &&
            !Seer.seer.Data.IsDead && Seer.seer != target && Seer.mode <= 1)
            showFlash(new Color(42f / 255f, 187f / 255f, 245f / 255f), message: GetString("seerShowInfoText"));
        Seer.deadBodyPositions?.Add(target.transform.position);

        // Tracker store body positions
        Tracker.deadBodyPositions?.Add(target.transform.position);

        // Medium add body
        var deadPlayer = new Medium.DeadPlayer(target, DateTime.UtcNow, deathReason, __instance, target.transform.position);
        Medium.futureDeadBodies.Add(new Tuple<Medium.DeadPlayer, Vector3>(deadPlayer, target.transform.position));

        // LastImpostor cooldown
        if (LastImpostor.lastImpostor != null && __instance == LastImpostor.lastImpostor && PlayerControl.LocalPlayer == __instance)
        {
            LastImpostor.lastImpostor.SetKillTimer(Mathf.Max(0f, ModOption.KillCooldown - LastImpostor.deduce));
        }

        // Set Gambler cooldown
        if (Gambler.gambler != null && __instance == Gambler.gambler && PlayerControl.LocalPlayer == __instance)
        {
            var cooldown = Gambler.GetSuc() ? Gambler.minCooldown : Gambler.maxCooldown;
            Gambler.gambler.SetKillTimer(cooldown);
        }

        // Set bountyHunter cooldown
        if (BountyHunter.bountyHunter != null && PlayerControl.LocalPlayer == BountyHunter.bountyHunter && __instance == BountyHunter.bountyHunter)
        {
            if (target == BountyHunter.bounty)
            {
                BountyHunter.bountyHunter.SetKillTimer(BountyHunter.bountyKillCooldown);
                BountyHunter.bountyUpdateTimer = 0f; // Force bounty update
            }
            else
            {
                BountyHunter.bountyHunter.SetKillTimer(ModOption.KillCooldown + BountyHunter.punishmentTime);
            }
        }

        // Mini Set Impostor Mini kill timer (Due to mini being a modifier, all "SetKillTimers" must have happened before this!)
        if (Mini.mini != null && __instance == Mini.mini && __instance == PlayerControl.LocalPlayer)
        {
            var multiplier = 1f;
            if (Mini.mini != null && PlayerControl.LocalPlayer == Mini.mini)
                multiplier = Mini.isGrownUp ? 0.66f : 2f;
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
            EvilTrapper.isTrapKill = false;
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

            showFlash(color, 1.25f);
        }

        // Akujo Lovers trigger suicide
        if ((Akujo.akujo != null && target == Akujo.akujo) || (Akujo.honmei != null && target == Akujo.honmei))
        {
            PlayerControl akujoPartner = target == Akujo.akujo ? Akujo.honmei : Akujo.akujo;
            if (akujoPartner != null && !akujoPartner.Data.IsDead)
            {
                akujoPartner.MurderPlayer(akujoPartner, MurderResultFlags.Succeeded);
                PlayerData.SetDeathReason(akujoPartner, CustomDeathReason.LoverSuicide);
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

        var cooldown = ModOption.KillCooldown;
        var multiplier = 1f;
        var addition = 0f;

        if (Mini.mini != null && PlayerControl.LocalPlayer == Mini.mini)
            multiplier = Mini.isGrownUp ? 0.66f : 2f;
        if (BountyHunter.bountyHunter != null && PlayerControl.LocalPlayer == BountyHunter.bountyHunter)
            addition = BountyHunter.punishmentTime;
        if (Gambler.gambler != null && PlayerControl.LocalPlayer == Gambler.gambler)
            addition = Gambler.maxCooldown - ModOption.KillCooldown;
        if (Gunsmith.Player != null && PlayerControl.LocalPlayer == Gunsmith.Player)
            cooldown = Gunsmith.KillCooldown;

        if (LastImpostor.lastImpostor != null && PlayerControl.LocalPlayer == LastImpostor.lastImpostor)
            addition -= LastImpostor.deduce;

        __instance.killTimer = Mathf.Clamp(time, 0f, (cooldown * multiplier) + addition);
        FastDestroyableSingleton<HudManager>.Instance.KillButton.SetCoolDown(__instance.killTimer, (cooldown * multiplier) + addition);
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
        var data = PlayerData.GetPlayerData(__instance);
        if (data != null && data.DeathReason == CustomDeathReason.Null)
        {
            data.DeathReason = CustomDeathReason.Exile;
            data.KilledBy = null;
            data.DeathTimer = DateTime.UtcNow;
        }

        if (MeetingHud.Instance)
        {
            foreach (var p in MeetingHud.Instance.playerStates)
            {
                if (p.TargetPlayerId == __instance.PlayerId)
                {
                    p.SetDead(p.DidReport, true);
                    p.Overlay.gameObject.SetActive(true);
                    break;
                }
            }
        }

        if (__instance == PlayerControl.LocalPlayer) _ = new LateTask(() => { CanSeeGhostInfo = true; }, 0.5f, "CanSeeRoleInfo");

        // Remove fake tasks when player dies
        if (__instance.HasFakeTasks() || __instance == Pursuer.Player.Contains(__instance) || __instance == Thief.thief)
            __instance.clearAllTasks();

        // Lover suicide trigger on exile
        if (Lovers.isLover(__instance)) Avenger.OnPlayerDeath(null, __instance, true);

        if (__instance.PlayerId == Pelican.Player?.PlayerId && Pelican.eatenPlayers?.Count > 0)
        {
            foreach (var player in Pelican.eatenPlayers.Where(p => p != null && p.Data.IsDead))
            {
                if (PlayerControl.LocalPlayer == player)
                {
                    HudManager.Instance.PlayerCam.SetTargetWithLight(PlayerControl.LocalPlayer);
                    PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(Pelican.Player.transform.position);
                }
                continue;
            }
            foreach (var p in Pelican.eatenPlayers) p.Die(DeathReason.Kill, true);
            Pelican.eatenPlayers = new();

            Pelican.PelicanDie();
        }

        if (__instance == Jailor.Player && Jailor.Jailed != null && InMeeting)
        {
            foreach (var playerState in MeetingHud.Instance.playerStates)
            {
                var cell = playerState.transform.FindChild("JailCell");
                cell?.gameObject?.Destroy();

                var icon = playerState.transform.FindChild("JailTargetIcon");
                icon?.gameObject?.Destroy();
            }
            Jailor.Jailed = null;
        }


        if (__instance == Blackmailer.Player && Blackmailer.blackmailed != null && InMeeting)
        {
            foreach (var playerState in MeetingHud.Instance.playerStates)
            {
                var cell = playerState.transform.FindChild("JailCell");
                cell?.gameObject?.Destroy();

                var icon = playerState.transform.FindChild("JailTargetIcon");
                icon?.gameObject?.Destroy();
            }
            Jailor.Jailed = null;
        }

        if (Lawyer.lawyer != null && __instance == Lawyer.target)
        {
            if (AmongUsClient.Instance.AmHost && (!Jester.Player.Any(x => x.PlayerId == Lawyer.target?.PlayerId) || Lawyer.targetWasGuessed))
            {
                var writer = StartRPC(CustomRPC.LawyerPromotesToPursuer);
                writer.Write(false);
                writer.EndRPC();
                Lawyer.PromotesToPursuer(false);
            }
        }
        if (Executioner.executioner != null && __instance == Executioner.target)
        {
            if (AmongUsClient.Instance.AmHost && Executioner.targetWasGuessed)
            {
                var writer = StartRPC(CustomRPC.ExecutionerPromotesRole);
                writer.EndRPC();
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
                PlayerData.SetDeathReason(akujoPartner, CustomDeathReason.LoverSuicide);
            }

            if (MeetingHud.Instance && akujoPartner != null)
            {
                foreach (PlayerVoteArea pva in MeetingHud.Instance.playerStates)
                {
                    if (pva.VotedFor != akujoPartner.PlayerId) continue;
                    pva.UnsetVote();
                    var voteAreaPlayer = PlayerById(pva.TargetPlayerId);
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
            if (player.isLover()) Lovers.clearAndReload();

            if (Lawyer.lawyer != null && Lawyer.target == player) Lawyer.PromotesToPursuer(false);

            if (Executioner.executioner != null && Executioner.target == player) Executioner.PromotesRole();

            if (player == Balancer.currentTarget) Balancer.currentTarget = null;

            if (player == Akujo.akujo) Akujo.clearAndReload();

            if (player == BandLeader.Player) BandLeader.ClearAndReload();

            if (player != null && !player.Data.IsDead) PlayerData.SetDeathReason(player, CustomDeathReason.Disconnect, null);

            Sheriff.deputyCheckPromotion();
        }

        if (InMeeting && MeetingHud.Instance)
        {
            foreach (var pva in MeetingHud.Instance.playerStates)
            {
                if (pva.TargetPlayerId == player.PlayerId)
                {
                    pva.Overlay.gameObject.SetActive(true);

                    pva.UnsetVote();
                    var voteAreaPlayer = PlayerById(pva.TargetPlayerId);
                    if (voteAreaPlayer?.AmOwner == false) continue;
                    MeetingHud.Instance.ClearVote();
                }
            }
        }

        if (Akujo.honmei != null && Akujo.honmei == player)
        {
            if (Akujo.akujo == PlayerControl.LocalPlayer)
            {
                Akujo.timeLeft += 30;
                Akujo.honmei = null;
            }
        }

        if (player == Pelican.Player && Pelican.eatenPlayers?.Count > 0)
        {
            foreach (var p in Pelican.eatenPlayers.ToArray())
            {
                if (p != null && p.Data.IsDead)
                {
                    p.Revive();

                    var data = PlayerData.GetPlayerData(p);
                    if (data != null)
                    {
                        data.DeathReason = CustomDeathReason.Null;
                        data.KilledBy = null;
                        data.DeathTimer = DateTime.MinValue;
                    }

                    if (p.AmOwner)
                    {
                        var pos = HudManager.Instance.PlayerCam.transform.position;
                        HudManager.Instance.PlayerCam.SetTargetWithLight(PlayerControl.LocalPlayer);
                        _ = new LateTask(() =>
                        {
                            HudManager.Instance.PlayerCam.SetTargetWithLight(PlayerControl.LocalPlayer);
                        }, 0.25f);
                        PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(pos);
                    }
                }
            }
            Pelican.clearAndReload();
        }

        if (RoleDraft.isEnabled && RoleDraft.isRunning)
        {
            if (RoleDraft.pickOrder != null && RoleDraft.pickOrder.Count > 0 && RoleDraft.pickOrder.Any(x => x == player.PlayerId))
            {
                RoleDraft.pickOrder.Remove(player.PlayerId);
                RoleDraft.timer = 0;
                RoleDraft.picked = true;
            }
        }
    }

    [HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.DisconnectInternal)), HarmonyPrefix]
    public static void InnerNetPrefix(InnerNetClient __instance, DisconnectReasons reason, string stringReason)
    {
        Info($"断开连接 {reason}:{stringReason}, Ping:{__instance.Ping}", "InnerNet");
    }
}