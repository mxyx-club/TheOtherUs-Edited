using PowerTools;
using System.Text;
using TheOtherRoles.Objects;

namespace TheOtherRoles.Patches;

[HarmonyPatch(typeof(ExileController), nameof(ExileController.Begin))]
[HarmonyPriority(Priority.First)]
internal class ExileControllerBeginPatch
{
    public static GameData.PlayerInfo lastExiled;
    public static TextMeshPro confirmImpostorSecondText;
    private static bool IsSec;
    public static bool ForceExile;
    public static bool OracleBlessed;

    public static bool Prefix(ExileController __instance, [HarmonyArgument(0)] ref GameData.PlayerInfo exiled, [HarmonyArgument(1)] bool tie)
    {
        Message("Begin Prefix", "ExileController");
        lastExiled = exiled;
        //OracleBlessed = false;

        if (Balancer.currentAbilityUser != null && Balancer.IsDoubleExile && !IsSec)
        {
            IsSec = true;

            bool leftIsConfesser = Oracle.Player.IsAlive() && Balancer.targetplayerleft == Oracle.Confesser;
            bool rightIsConfesser = Oracle.Player.IsAlive() && Balancer.targetplayerright == Oracle.Confesser;

            if (leftIsConfesser || rightIsConfesser) OracleBlessed = true;
            if (leftIsConfesser) lastExiled = exiled = null;
            else __instance.exiled = exiled = Balancer.targetplayerleft?.Data;

            if (Balancer.targetplayerright != null && !rightIsConfesser)
            {
                __instance.exiled = null;
                ExileController controller = UObject.Instantiate(__instance, __instance.transform.parent);
                controller.exiled = Balancer.targetplayerright.Data;
                controller.Begin(controller.exiled, false);
                Message($"开始放逐: {controller.exiled?.PlayerName ?? "null"}");
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
                    _ = new LateTask(() => { controller.StopAllCoroutines(); controller.StartCoroutine(controller.Animate()); }, 0.025f + (index * 0.025f));
                }
                _ = new LateTask(() => controller.StartCoroutine(controller.Animate()), 0f);
                for (int i = 0; i < 23; i++)
                {
                    createlate(i);
                }
                _ = new LateTask(() => { controller.StopAllCoroutines(); controller.EjectSound = sound; controller.StartCoroutine(controller.Animate()); }, 0.6f);
                ExileController.Instance = __instance;
                //__instance.exiled = Balancer.targetplayerleft.Data;
                //exiled = __instance.exiled;
                if (isFungle)
                {
                    Helpers.SetActiveAllObject(controller.gameObject.GetChildren(), "RaftAnimation", false);
                    controller.transform.localPosition = new(-3.75f, -0.2f, -60f);
                }
            }

            if (Lawyer.lawyer != null && exiled?.Object.PlayerId == Lawyer.target?.PlayerId && !Jester.Player.Any(x => x.PlayerId == Lawyer.target?.PlayerId))
            {
                var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.LawyerPromotesToPursuer);
                writer.Write(true);
                writer.EndRPC();
                Lawyer.PromotesToPursuer(true);
            }

            IsSec = false;

            HandleBeginPrefix();
            return true;
        }

        if (Oracle.Player.IsAlive() && exiled != null && exiled.Object == Oracle.Confesser)
        {
            lastExiled = exiled = null;
            OracleBlessed = true;
        }

        Message($"开始放逐: {exiled?.PlayerName ?? "null"}");

        if (Lawyer.lawyer != null && exiled?.Object.PlayerId == Lawyer.target?.PlayerId && !Jester.Player.Any(x => x.PlayerId == Lawyer.target?.PlayerId))
        {
            var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.LawyerPromotesToPursuer);
            writer.Write(true);
            writer.EndRPC();
            Lawyer.PromotesToPursuer(true);
        }

        HandleBeginPrefix();
        return true;
    }

    public static void HandleBeginPrefix()
    {
        if (Medic.usedShield) Medic.meetingAfterShielding = true; // Has to be after the setting of the shield

        if (PartTimer.partTimer != null && PartTimer.partTimer.IsAlive())
        {
            if (PartTimer.deathTurn <= 0 && PartTimer.target == null) PartTimer.partTimer.Exiled();
        }

        if (Doomsayer.doomsayer != null && AmongUsClient.Instance.AmHost && !Doomsayer.canGuess) Doomsayer.canGuess = true;

        if (Butcher.butcher != null) Butcher.canDissection = true;

        // Medic shield
        if (Medic.medic != null && AmongUsClient.Instance.AmHost && Medic.futureShielded != null && !Medic.medic.Data.IsDead)
        {
            // We need to send the RPC from the host here, to make sure that the order of shifting and setting the shield is correct(for that reason the futureShifted and futureShielded are being synced)
            var writer = StartRPC(CustomRPC.MedicSetShielded);
            writer.Write(Medic.futureShielded.PlayerId);
            writer.EndRPC();
            RPCProcedure.medicSetShielded(Medic.futureShielded.PlayerId);
        }

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

    }

    public static void Postfix(ExileController __instance, [HarmonyArgument(0)] ref GameData.PlayerInfo exiled)
    {
        Message("Begin Postfix", "ExileController");
        var player = exiled?.Object ?? null;
        confirmImpostorSecondText = UObject.Instantiate(__instance.ImpostorText, __instance.Text.transform);
        StringBuilder changeStringBuilder = new();

        if (ModOption.NormalOptions.ConfirmImpostor) confirmImpostorSecondText.transform.localPosition += new Vector3(0f, -0.4f, 0f);
        else confirmImpostorSecondText.transform.localPosition += new Vector3(0f, -0.2f, 0f);

        confirmImpostorSecondText.text = changeStringBuilder.ToString();
        confirmImpostorSecondText.gameObject.SetActive(true);

        if (CustomOptionHolder.exiledController.GetBool())
        {
            if (player != null)
            {
                switch (CustomOptionHolder.exiledRevealRole.GetQuantity())
                {
                    case 1:
                        __instance.completeString = TranslationController.Instance.GetString(StringNames.ExileTextNonConfirm, player?.Data.PlayerName);
                        break;
                    case 2:
                        var roleName = RoleInfo.getRoleInfoForPlayer(player, false, false)?.FirstOrDefault(x => x.roleType is not RoleType.Special)?.Name;
                        __instance.completeString = string.Format(GetString("ExileController.PlayerRole"), player.Data.PlayerName, roleName);
                        break;
                    case 3:
                        __instance.completeString = string.Format(GetString("ExileController.PlayerTeam"), player.Data.PlayerName, teamString(player));
                        break;
                    default:
                        break;
                }
            }

            if (ForceExile)
            {
                __instance.completeString = string.Format(GetString("ExileController.ForceExile"), exiled?.PlayerName ?? "NULL");
                ForceExile = false;
            }

            if (Prosecutor.ProsecuteThisMeeting && player != null) __instance.completeString += $" {GetString("ExileController.Prosecute")}";

            if (CustomOptionHolder.exiledShowTeamNum.GetBool())
            {
                var players = PlayerControl.AllPlayerControls.ToArray().Where(x => x.IsAlive() && x.PlayerId != player?.PlayerId);

                var showMode = CustomOptionHolder.exiledShowTeamSelect.GetSelection();

                string text = "\n";
                if (showMode != 1) text += $"{Cs(getTeamColor(RoleType.Impostor), GetString("ExileController.ImpNum"))}{players.Count(x => x.IsImpostor())}";
                if (showMode == 2) text += " | ";
                if (showMode != 0) text += $"{Cs(getTeamColor(RoleType.Neutral), GetString("ExileController.NeutralNum"))}{players.Count(x => x.IsNeutral())}";
                __instance.ImpostorText.text = text;
            }
        }

        if (Oracle.Player.IsAlive() && OracleBlessed)
        {
            __instance.completeString = $"神谕者拒绝放逐 {Oracle.Confesser.Data.PlayerName}！";
        }

        if (Balancer.currentAbilityUser != null && Balancer.IsDoubleExile && OracleBlessed)
        {
            __instance.completeString = GetString("ExileController.Balancer");
            if (Oracle.Player.IsAlive() && OracleBlessed)
                __instance.completeString += $"，但 {Oracle.Confesser.Data.PlayerName} 拒绝了放逐！";
            return;
        }
        else if (Balancer.currentAbilityUser != null && Balancer.IsDoubleExile && __instance.exiled?.PlayerId == Balancer.targetplayerleft.PlayerId)
        {
            __instance.completeString = GetString("ExileController.Balancer");
            return;
        }

        OracleBlessed = false;
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
            Message("Object.Destroy", "ExileController");
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
        if (PlayerControl.LocalPlayer.IsDead()) CanSeeGhostInfo = true;

        if (CustomOptionHolder.randomGameStartPosition.GetBool()) MapData.RandomSpawnPlayers();

        DeadBody[] array = UObject.FindObjectsOfType<DeadBody>();
        for (var i = 0; i < array.Length; i++)
        {
            UObject.Destroy(array[i].gameObject);
        }

        // Prosecutor win condition
        if (exiled != null && Executioner.executioner != null && Executioner.target != null &&
            Executioner.target.PlayerId == exiled.PlayerId && !Executioner.executioner.Data.IsDead)
        {
            Executioner.triggerExecutionerWin = true;
            return;
        }
        // Mini exile lose condition
        else if (exiled != null && Mini.mini != null && Mini.mini.PlayerId == exiled.PlayerId && !Mini.isGrownUp &&
                 !Mini.mini.Data.Role.IsImpostor && !Mini.mini.IsNeutral())
        {
            Mini.triggerMiniLose = true;
            return;
        }
        // Jester win condition
        else if (exiled != null && Jester.Player != null && Jester.Player.Any(x => x.PlayerId == exiled.PlayerId))
        {
            Jester.WinnerPlayer = PlayerById(exiled.PlayerId);
            var writer = StartRPC(CustomRPC.JesterWinner);
            writer.Write(exiled.PlayerId);
            writer.EndRPC();
            Jester.triggerJesterWin = true;
            return;
        }
        else if (Executioner.executioner != null && Executioner.executioner == PlayerControl.LocalPlayer && Executioner.target.IsDead())
        {
            var writer = StartRPC(CustomRPC.ExecutionerPromotesRole);
            writer.EndRPC();
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

        // Reset the jailed player
        Jailor.Jailed = null;

        // Reset custom button timers where necessary
        CustomButton.OnMeetingEnd();

        if ((Decoy.ResetPlaceAfterMeeting && Decoy.DecoyPermanent) || !Decoy.DecoyPermanent)
        {
            Marionette.decoy?.Destroy();
            Marionette.decoy = null;
        }

        Balancer.WrapUp(exiled == null ? null : exiled.Object);
        // Mini set adapted cooldown
        if (Mini.mini != null && PlayerControl.LocalPlayer == Mini.mini && Mini.mini.Data.Role.IsImpostor)
        {
            var multiplier = Mini.isGrownUp ? 0.66f : 2f;
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

                            if (p == 1f && rend != null && rend.gameObject != null) UObject.Destroy(rend.gameObject);
                        })));
                }
            }
            Seer.deadBodyPositions = new List<Vector3>();
        }

        // Tracker reset deadBodyPositions
        Tracker.deadBodyPositions = new List<Vector3>();

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
            Prosecutor.StartProsecute = false;
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

        if (Blackmailer.Player != null && Blackmailer.blackmailed != null)
        {
            Blackmailer.blackmailed = null;
            Blackmailer.alreadyShook = false;
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
                var partner = exiled?.Object?.GetPartner();

                var exiledIsWitch = exiled?.PlayerId == Witch.witch.PlayerId;
                var witchDiesWithExiledLover = partner?.PlayerId == Witch.witch.PlayerId || exiled?.PlayerId == Witch.witch.PlayerId;

                if (((witchDiesWithExiledLover || exiledIsWitch) && Witch.witchVoteSavesTargets) || Witch.witchWasGuessed)
                    Witch.futureSpelled = new List<PlayerControl>();

                foreach (var target in Witch.futureSpelled.Where(x => x.IsAlive()))
                {
                    if (Lawyer.lawyer != null && target == Lawyer.target)
                    {
                        var writer2 = StartRPC(PlayerControl.LocalPlayer, CustomRPC.LawyerPromotesToPursuer);
                        writer2.Write(false);
                        writer2.EndRPC();
                        Lawyer.PromotesToPursuer(false);
                    }

                    if (Executioner.executioner.IsAlive() && target == Executioner.target)
                    {
                        var writer2 = StartRPC(PlayerControl.LocalPlayer, CustomRPC.ExecutionerPromotesRole);
                        writer2.EndRPC();
                        Executioner.PromotesRole();
                    }
                    var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.WitchSpelledKill);
                    writer.Write(Witch.witch);
                    writer.Write(target.PlayerId);
                    writer.EndRPC();
                    RPCProcedure.WitchSpelledKill(Witch.witch, target);
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
                foreach (var sr in Medium.souls) UObject.Destroy(sr.gameObject);
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
                Medium.futureDeadBodies = new List<Tuple<Medium.DeadPlayer, Vector3>>();
            }
        }

        if (InfoSleuth.infoSleuth != null && InfoSleuth.target != null && InfoSleuth.infoSleuth == PlayerControl.LocalPlayer)
        {
            var isNotCrew = (InfoSleuth.target.IsNeutral() || InfoSleuth.target.IsImpostor()) ^ Vortox.Reversal;

            string info = InfoSleuth.infoType switch
            {
                0 => GetString(isNotCrew ? "InfoSleuth.NotCrew" : "InfoSleuth.Crew"),
                1 => string.Format(GetString("InfoSleuth.Team"), GetTeamString(InfoSleuth.target)),
                _ => rnd.Next(2) == 0 ? GetString(isNotCrew ? "InfoSleuth.NotCrew" : "InfoSleuth.Crew")
                                      : string.Format(GetString("InfoSleuth.Team"), GetTeamString(InfoSleuth.target))
            };

            string msg = $"{InfoSleuth.target.Data.PlayerName} {info}";

            HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, msg);

            var writer = StartRPC(CustomRPC.ShareGhostInfo);
            writer.Write(InfoSleuth.infoSleuth.PlayerId);
            writer.Write((byte)RPCProcedure.GhostInfoTypes.GhostChat);
            writer.Write(msg);
            writer.EndRPC();

            var writer1 = StartRPC(CustomRPC.InfoSleuthSetTarget);
            writer1.Write(byte.MaxValue);
            writer1.EndRPC();

            static string GetTeamString(PlayerControl player)
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

        foreach (var trap in Trap.AllObjects) trap.triggerable = false;
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(
            (GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown / 2) + 2, new Action<float>(p =>
            { if (p == 1f) foreach (var trap in Trap.AllObjects) trap.triggerable = true; })));

        if (!Yoyo.markStaysOverMeeting) Silhouette.AllObjects.ToArray().Do(x => x.Destroy());

        // AntiTeleport set position
        AntiTeleport.setPosition();
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    private class BaseExileControllerPatch
    {
        public static void Postfix(ExileController __instance)
        {
            Message("WrapUp Postfix", "ExileController");
            WrapUpPostfix(__instance.exiled);
        }
    }

    [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
    private class AirshipExileControllerPatch
    {
        public static void Postfix(AirshipExileController __instance)
        {
            Message("WrapUpAndSpawn", "AirshipExileController");
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
        AntiTeleport.setPosition();
        Chameleon.lastMoved.Clear();
    }
}