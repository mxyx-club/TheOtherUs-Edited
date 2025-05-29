using AmongUs.GameOptions;
using Assets.CoreScripts;
using TheOtherRoles.Objects;
using static TheOtherRoles.GameHistory;

namespace TheOtherRoles.Patches;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
public static class PlayerControlFixedUpdatePatch
{
    private static bool mushroomSaboWasActive;

    public static PlayerControl SetTarget(bool onlyCrewmates = false, bool targetPlayersInVents = false,
        IEnumerable<PlayerControl> untargetablePlayers = null, PlayerControl targetingPlayer = null, float KillDistances = 0f)
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

    private static void setPetVisibility()
    {
        var localalive = !PlayerControl.LocalPlayer.Data.IsDead;
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            var playeralive = !player.Data.IsDead;
            player.cosmetics.SetPetVisible((localalive && playeralive) || !localalive);
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
        if (Undertaker.undertaker.IsDead() || InMeeting) return;

        if (Undertaker.dragedBody != null)
        {
            Undertaker.dragedBody.transform.position = Undertaker.undertaker.transform.position;
        }
    }

    private static void jesterDragBodyUpdate()
    {
        if (Jester.jester.IsDead() || InMeeting) return;

        if (Jester.dragedBody != null)
        {
            Jester.dragedBody.transform.position = Jester.jester.transform.position;
        }
    }

    private static void vultureUpdate()
    {
        if (Vulture.vulture == null || PlayerControl.LocalPlayer != Vulture.vulture ||
            Vulture.localArrows == null || !Vulture.showArrows) return;
        if (Vulture.vulture.Data.IsDead)
        {
            foreach (var arrow in Vulture.localArrows) UObject.Destroy(arrow.arrow);
            Vulture.localArrows = new();
            return;
        }

        DeadBody[] deadBodies = UObject.FindObjectsOfType<DeadBody>();
        var arrowUpdate = Vulture.localArrows.Count != deadBodies.Length;
        var index = 0;

        if (arrowUpdate)
        {
            foreach (var arrow in Vulture.localArrows) UObject.Destroy(arrow.arrow);
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
                    UObject.Destroy(arrow.arrow);
                Amnisiac.localArrows.Clear();
            }
        }
        if (Amnisiac.Player.Any(x => x.PlayerId == PlayerControl.LocalPlayer.PlayerId && x.IsAlive()))
        {
            DeadBody[] deadBodies = UObject.FindObjectsOfType<DeadBody>();
            bool arrowUpdate = Amnisiac.localArrows.Count != deadBodies.Length;
            int index = 0;

            if (arrowUpdate)
            {
                foreach (var arrow in Amnisiac.localArrows)
                    UObject.Destroy(arrow.arrow);

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

    private static void radarUpdate()
    {
        if (Radar.radar == null || PlayerControl.LocalPlayer != Radar.radar || Radar.localArrows == null || InMeeting) return;
        if (Radar.radar.Data.IsDead)
        {
            foreach (var arrow in Radar.localArrows) UObject.Destroy(arrow.arrow);
            Radar.localArrows = new();
            return;
        }

        var arrowUpdate = true;
        var index = 0;

        if (arrowUpdate && !PlayerControl.LocalPlayer.Data.IsDead)
        {
            foreach (var arrow in Radar.localArrows) UObject.Destroy(arrow.arrow);
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
                if (HudManagerStartPatch.akujoHonmeiButton.ButtonTitle != null)
                {
                    HudManagerStartPatch.akujoHonmeiButton.ButtonTitle.text = TimeSpan.FromSeconds(Akujo.timeLeft).ToString(@"mm\:ss");
                }
                HudManagerStartPatch.akujoHonmeiButton.ButtonTitle.enabled = !(MapBehaviour.Instance && MapBehaviour.Instance.IsOpen) &&
                  !MeetingHud.Instance &&
                  !ExileController.Instance;
            }
            else HudManagerStartPatch.akujoHonmeiButton.ButtonTitle.enabled = false;
        }
        else if (Akujo.timeLeft <= 0)
        {
            if (Akujo.honmei == null || (Akujo.keeps?.Count < 1 && Akujo.forceKeeps))
            {
                var writer = StartRPC(CustomRPC.AkujoSuicide);
                writer.Write(Akujo.akujo.PlayerId);
                writer.EndRPC();
                RPCProcedure.akujoSuicide(Akujo.akujo.PlayerId);
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
            // Update Role Description
            refreshRoleDescription(__instance);

            //Update pet visibility
            setPetVisibility();

            if (!InGame) return;

            // undertaker
            undertakerDragBodyUpdate();
            // Jester
            jesterDragBodyUpdate();
            // Amnisiac
            amnisiacUpdate();
            // Vulture
            vultureUpdate();
            // Radar
            radarUpdate();
            // Morphling and Camouflager
            morphlingAndCamouflagerUpdate();
            // Lawyer
            lawyerUpdate();
            // Executioner
            executionerUpdate();
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
            // Bloody
            bloodyUpdate();
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
        if (PlayerControl.LocalPlayer == __instance)
        {
            CanSeeRoleInfo = false;
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
        DeadPlayers.RemoveAll(x => x.Player == __instance);
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
        Sheriff.deputyCheckPromotion();

        if (!InGame || PlayerControl.LocalPlayer != __instance) return;
        if (Prosecutor.prosecutor == __instance)
        {
            Prosecutor.Prosecuted = false;
            Prosecutor.StartProsecute = false;
            Prosecutor.ProsecuteThisMeeting = false;
        }
        if (ModOption.gameMode is CustomGamemodes.Classic or CustomGamemodes.Guesser) return;
        _ = new LateTask(() => { CanSeeRoleInfo = true; }, 1f, "CanSeeRoleInfo");
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
public static class MurderPlayerPatch
{
    //public static bool resetToCrewmate;
    //public static bool resetToDead;

    public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
    {
        if (SchrodingersCat.Player != null && target == SchrodingersCat.Player && SchrodingersCat.remainingChange > 0)
        {
            var role = RoleInfo.getRoleInfoForPlayer(__instance, false, false).FirstOrDefault();
            var state = SchrodingersCat.CatState.None;
            if (role != null && PlayerControl.LocalPlayer == SchrodingersCat.Player)
            {
                if (role.roleId is RoleId.Jackal or RoleId.Sidekick) state = SchrodingersCat.CatState.Jackal;
                else if (role.roleId is RoleId.Pavlovsdogs or RoleId.Pavlovsowner) state = SchrodingersCat.CatState.Pavlovsowner;
                else if (role.roleId == RoleId.Werewolf) state = SchrodingersCat.CatState.Werewolf;
                else if (role.roleId == RoleId.Juggernaut) state = SchrodingersCat.CatState.Juggernaut;
                else if (role.roleId == RoleId.Swooper) state = SchrodingersCat.CatState.Swooper;
                else if (role.roleId == RoleId.Arsonist) state = SchrodingersCat.CatState.Arsonist;
                else if (role.roleId == RoleId.Pelican) state = SchrodingersCat.CatState.Pelican;
                else if (role.roleType == RoleType.Impostor) state = SchrodingersCat.CatState.Impostor;
                else if (role.roleType == RoleType.Crewmate) state = SchrodingersCat.CatState.Crewmate;

                var writer = StartRPC(CustomRPC.SchrodingersCatSetState);
                writer.Write((byte)state);
                writer.EndRPC();
                SchrodingersCat.State = state;
                HudManagerStartPatch.schrodingersCatKillButton.Timer = HudManagerStartPatch.schrodingersCatKillButton.MaxTimer;
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

        // Allow everyone to murder players
        //resetToCrewmate = !__instance.Data.Role.IsImpostor;
        //resetToDead = __instance.Data.IsDead;
        //__instance.Data.Role.TeamType = RoleTeamTypes.Impostor;
        //__instance.Data.IsDead = false;

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
        var deadPlayer = new DeadPlayer(target, DateTime.UtcNow, deathReason, __instance);
        DeadPlayers.Add(deadPlayer);

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
                otherLover.MurderPlayer(otherLover, MurderResultFlags.Succeeded);
                OverrideDeathReasonAndKiller(otherLover, CustomDeathReason.LoverSuicide);
            }
        }

        // Bait
        if (Bait.bait.FindAll(x => x.PlayerId == target.PlayerId).Count > 0)
        {
            float reportDelay = rnd.Next((int)Bait.reportDelayMin, (int)Bait.reportDelayMax + 1);

            _ = new LateTask(() =>
            {
                if (__instance.AmOwner)
                    __instance?.CmdReportDeadBody(target.Data);
            }, reportDelay);

            if (Bait.showKillFlash && __instance == PlayerControl.LocalPlayer)
                showFlash(new Color(204f / 255f, 102f / 255f, 0f / 255f));
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
                _ = new LateTask(() => { Pelican.Player.Die(DeathReason.Kill, true); }, 0.5f);
            }
        }

        // Undertaker Button Sync
        if (Undertaker.undertaker != null && PlayerControl.LocalPlayer == Undertaker.undertaker &&
            __instance == Undertaker.undertaker && HudManagerStartPatch.undertakerDragButton != null)
            HudManagerStartPatch.undertakerDragButton.Timer = Undertaker.dragingDelaiAfterKill;

        // Seer show flash and add dead player position
        if (Seer.seer != null &&
            (PlayerControl.LocalPlayer == Seer.seer || CanSeeRoleInfo) &&
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
            LastImpostor.lastImpostor.SetKillTimer(Mathf.Max(0f, ModOption.KillCooldown - LastImpostor.deduce));
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
                BountyHunter.bountyHunter.SetKillTimer(ModOption.KillCooldown + BountyHunter.punishmentTime);
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

            showFlash(color, 1.25f);
        }

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

        var multiplier = 1f;
        var addition = 0f;

        if (Mini.mini != null && PlayerControl.LocalPlayer == Mini.mini)
            multiplier = Mini.isGrownUp() ? 0.66f : 2f;
        if (BountyHunter.bountyHunter != null && PlayerControl.LocalPlayer == BountyHunter.bountyHunter)
            addition = BountyHunter.punishmentTime;
        if (Gambler.gambler != null && PlayerControl.LocalPlayer == Gambler.gambler)
            addition = Gambler.maxCooldown - ModOption.KillCooldown;
        if (Gunsmith.Player != null && PlayerControl.LocalPlayer == Gunsmith.Player)
            addition = Gunsmith.KillCooldown;

        if (LastImpostor.lastImpostor != null && PlayerControl.LocalPlayer == LastImpostor.lastImpostor)
            addition -= LastImpostor.deduce;

        __instance.killTimer = Mathf.Clamp(time, 0f, (ModOption.KillCooldown * multiplier) + addition);
        FastDestroyableSingleton<HudManager>.Instance.KillButton.SetCoolDown(__instance.killTimer, (ModOption.KillCooldown * multiplier) + addition);
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
                    break;
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
            foreach (var p in Pelican.eatenPlayers) p.Die(DeathReason.Kill, true);
            Pelican.eatenPlayers = new();

            Pelican.PelicanDie();
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
            if (player.isLover()) Lovers.clearAndReload();

            if (Lawyer.lawyer != null && Lawyer.target == player) Lawyer.PromotesToPursuer();

            if (Executioner.executioner != null && Executioner.target == player) Executioner.PromotesRole();

            if (player == Balancer.currentTarget) Balancer.currentTarget = null;

            if (player == Akujo.akujo) Akujo.clearAndReload();

            if (player == BandLeader.Player) BandLeader.ClearAndReload();

            if (player != null && !player.Data.IsDead) OverrideDeathReasonAndKiller(player, CustomDeathReason.Disconnect, null);

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
                    var voteAreaPlayer = playerById(pva.TargetPlayerId);
                    if (voteAreaPlayer?.AmOwner == false) continue;
                    MeetingHud.Instance.ClearVote();
                }
            }
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
