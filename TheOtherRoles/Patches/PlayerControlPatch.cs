using AmongUs.GameOptions;

namespace TheOtherRoles.Patches;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
public static class PlayerControlFixedUpdatePatch
{
    public static PlayerControl SetTarget(bool onlyCrewmates = false, bool targetPlayersInVents = false,
        object untargetablePlayers = null, PlayerControl targetingPlayer = null, float KillDistances = 0f)
    {
        PlayerControl result = null;
        var num = GameOptionsData.KillDistances[Mathf.Clamp(GameOptionsManager.Instance.currentNormalGameOptions.KillDistance, 0, 3)];
        if (!MapUtilities.CachedShipStatus) return result;
        if (targetingPlayer == null) targetingPlayer = PlayerControl.LocalPlayer;
        if (targetingPlayer.Data.IsDead) return result;
        num += KillDistances;

        IEnumerable<PlayerControl> untargetablePlayersList = null;
        IEnumerable<RoleId> untargetableRoleList = null;
        if (untargetablePlayers != null)
        {
            if (untargetablePlayers is PlayerControl singlePlayer)
            {
                untargetablePlayersList = [singlePlayer];
            }
            else if (untargetablePlayers is IEnumerable<PlayerControl> enumerable)
            {
                untargetablePlayersList = enumerable;
            }
            else if (untargetablePlayers is IEnumerable<RoleId> list)
            {
                untargetableRoleList = list;
            }
        }

        var truePosition = targetingPlayer.GetTruePosition();
        foreach (var playerInfo in GameData.Instance.AllPlayers.GetFastEnumerator())
            if (!playerInfo.Disconnected && playerInfo.PlayerId != targetingPlayer.PlayerId && !playerInfo.IsDead &&
                (!onlyCrewmates || !playerInfo.Role.IsImpostor))
            {
                var player = playerInfo.Object;

                bool isPlayerUntargetable = untargetablePlayersList?.Any(x => x == player) ?? false;
                bool isRoleUntargetable = untargetableRoleList?.Any(role => role == player.GetRole()) ?? false;

                if (isPlayerUntargetable || isRoleUntargetable)
                    continue;

                if (player && (!player.inVent || targetPlayersInVents))
                {
                    var vector = player.GetTruePosition() - truePosition;
                    var magnitude = vector.magnitude;
                    if (magnitude <= num && !PhysicsHelpers.AnyNonTriggersBetween(truePosition, vector.normalized,
                            magnitude, Constants.ShipAndObjectsMask))
                    {
                        result = player;
                        num = magnitude;
                    }
                }
            }

        return result;
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

    private static bool CamoState;
    private static void CamouflagerUpdate()
    {
        Camouflager.CamoTimer = Mathf.Max(0f, Camouflager.CamoTimer - Time.fixedDeltaTime);

        if (MushroomSabotageActive) return;

        if (isActiveCamoComms && !CamoState)
        {
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                player.setLook("", 6, "", "", "", "");
            CamoState = true;
        }
        else if (!isActiveCamoComms && CamoState)
        {
            Camouflager.resetCamouflage();
            camoReset();
        }
    }


    private static void ImpostorSetTarget()
    {
        if (!PlayerControl.LocalPlayer.IsImpostor() || !PlayerControl.LocalPlayer.CanMove || PlayerControl.LocalPlayer.IsDead())
        {
            FastDestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(null);
            return;
        }
        // Includes setPlayerOutline(target, Palette.ImpstorRed);
        FastDestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(RoleHelpers.ImpostorSetTarget());
    }


    public static void Postfix(PlayerControl __instance)
    {
        if (AmongUsClient.Instance.GameState != InnerNetClient.GameStates.Started ||
            GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return;

        if (PlayerControl.LocalPlayer == __instance)
        {
            // Update Role Description
            refreshRoleDescription(__instance);

            //Update pet visibility
            setPetVisibility();

            if (!InGame) return;
            CustomRoleManager.OnFixedUpdate(__instance);
            ImpostorSetTarget();
            CamouflagerUpdate();
        }
    }

}

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.WalkPlayerTo))]
internal class PlayerPhysicsWalkPlayerToPatch
{
    private static Vector2 offset = Vector2.zero;

    public static void Prefix(PlayerPhysics __instance)
    {
        var minijudge = __instance.myPlayer.TryGetModifier<Mini>(out var mini);
        var morphlingjudge = __instance.myPlayer.TryGetRole<Morphling>(out var morphling);
        var correctOffset = !isActiveCamoComms && !MushroomSabotageActive &&
                                (minijudge ||
                                (morphlingjudge &&
                                 morphling.morphTarget == mini.Player &&
                                 morphling.morphTimer > 0f));
        correctOffset = correctOffset && !(mini.Player == morphling.Player && morphling.morphTimer > 0f);
        if (correctOffset)
        {
            var currentScaling = (mini.growingProgress() + 1) * 0.5f;
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
            CustomButton.ResetAllCooldowns(-1);
            CanSeeGhostInfo = false;
        }

        CustomRoleManager.AllActiveRoles.Values.Do(x => x.OnRevivePlayer(__instance));
        CustomRoleManager.AllActiveModifier.Values.SelectMany(x => x).Do(x => x.OnRevivePlayer(__instance));

        if (__instance == Specter.Player) Specter.Player.clearAllTasks();

        RPCProcedure.clearGhostRoles(__instance.PlayerId);

        var data = PlayerData.GetPlayerData(__instance);
        data.KilledBy = null;
        data.DeathReason = CustomDeathReason.Null;
        data.DeathTimer = DateTime.MinValue;

        __instance.SetKillTimer(ModOption.KillCooldown / 2);
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdReportDeadBody))]
internal class BodyReportPatch
{
    public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] GameData.PlayerInfo target)
    {
        if (ModOption.DisableMeeting) return false;

        var flag = CustomRoleManager.OnCheckReportDeadBody(__instance, target);

        return flag;
    }

    private static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] GameData.PlayerInfo target)
    {
        CustomRoleManager.AllActiveRoles.Values.Do(x => x.OnReportDeadBody(__instance, target));
        CustomRoleManager.AllActiveModifier.Values.SelectMany(x => x).Do(x => x.OnReportDeadBody(__instance, target));
        Message($"报告玩家 {__instance.Data.PlayerName} 被报告尸体 {target?.PlayerName ?? "null"}", "CmdReportDeadBody");
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Die))]
public static class PlayerDiePatch
{
    public static void Postfix(PlayerControl __instance)
    {
        CustomRoleManager.AllActiveRoles.Values.Do(x => x.OnPlayerDeath(__instance, InMeeting));
        if (ModOption.GameMode is CustomGameModes.Classic or CustomGameModes.Guesser) return;
        _ = new LateTask(() => { CanSeeGhostInfo = true; }, 1f, "CanSeeRoleInfo");
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
public static class MurderPlayerPatch
{
    //public static bool resetToCrewmate;
    //public static bool resetToDead;

    public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
    {
        //if (CustomRoleManager.AllActiveRoles)
        return true;
    }

    public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
    {
        HandleMurderPostfix(__instance, target);
    }

    public static void HandleMurderPostfix(PlayerControl __instance, PlayerControl target)
    {
        // Collect dead player info
        var deathReason = __instance == target ? CustomDeathReason.Suicide : CustomDeathReason.Kill;
        PlayerData.SetDeathReason(target, deathReason, __instance);

        // Remove fake tasks when player dies
        if (target.HasFakeTasks() || target.Is(RoleId.Thief) || target.Is(RoleId.Amnisiac))
            target.clearAllTasks();

        // First kill (set before lover suicide)
        if (ModOption.firstKillName == "") ModOption.firstKillName = target.Data.PlayerName;
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
        if (PlayerControl.LocalPlayer.TryGetModifier<Mini>(out var mini))
            multiplier = mini.isGrownUp() ? 0.66f : 2f;
        if (PlayerControl.LocalPlayer.Is(RoleId.BountyHunter))
            addition = BountyHunter.punishmentTime;
        if (PlayerControl.LocalPlayer.Is(RoleId.Gambler))
            addition = Gambler.maxCooldown - ModOption.KillCooldown;
        if (PlayerControl.LocalPlayer.Is(RoleId.Gunsmith))
            addition = Gunsmith.KillCooldown;
        /*if (LastImpostor.lastImpostor != null && PlayerControl.LocalPlayer == LastImpostor.lastImpostor)
            addition -= LastImpostor.deduce;*/

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
        if (hideNextAnimation) source = target;
        hideNextAnimation = false;
    }
}

[HarmonyPatch(typeof(KillAnimation), nameof(KillAnimation.SetMovement))]
internal class KillAnimationSetMovementPatch
{
    private static int? colorId;

    public static void Prefix(PlayerControl source, bool canMove)
    {
        if (!InGame) return;
        var color = source.cosmetics.currentBodySprite.BodySprite.material.GetColor("_BodyColor");

        if (source.TryGetRole<Morphling>(out var morphling) && source.Data.PlayerId == morphling.Player.PlayerId)
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
            data.KilledBy = null;
            data.DeathReason = CustomDeathReason.Exile;
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

        _ = new LateTask(() => { if (__instance == PlayerControl.LocalPlayer) CanSeeGhostInfo = true; }, 0.5f, "CanSeeRoleInfo");

        // Remove fake tasks when player dies
        if (__instance.HasFakeTasks() || __instance.Is(RoleId.Thief))
            __instance.clearAllTasks();

        CustomRoleManager.AllActiveRoles.Values.Do(x => x.OnPlayerExlied(__instance));
        CustomRoleManager.AllActiveModifier.Values.SelectMany(x => x).Do(x => x.OnPlayerExlied(__instance));
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
            CustomRoleManager.OnPlayerDisconnect(player);

            var data = PlayerData.GetPlayerData(player);
            if (data != null && data.DeathReason == CustomDeathReason.Null)
            {
                data.KilledBy = null;
                data.DeathReason = CustomDeathReason.Disconnect;
                data.DeathTimer = DateTime.UtcNow;
            }
        }
    }

    [HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.DisconnectInternal)), HarmonyPrefix]
    public static void InnerNetPrefix(InnerNetClient __instance, DisconnectReasons reason, string stringReason)
    {
        Info($"断开连接 {reason}:{stringReason}, Ping:{__instance.Ping}", "InnerNet");
    }
}