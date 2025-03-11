using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AmongUs.GameOptions;
using InnerNet;
using Reactor.Utilities.Extensions;
using TheOtherRoles.Buttons;
using TheOtherRoles.CustomCosmetics;
using TheOtherRoles.Patches;
using TheOtherRoles.Utilities;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TheOtherRoles.Helper;

public enum MurderAttemptResult
{
    ReverseKill,
    PerformKill,
    SuppressKill,
    BlankKill,
    BodyGuardKill,
    DelayVampireKill
}

public enum SabotageTypes
{
    Comms,
    O2,
    Reactor,
    OxyMask,
    Lights,
    None
}

public enum CustomGamemodes
{
    Classic,
    Guesser,
}

public static class Helpers
{
    public static bool zoomOutStatus;
    public static bool InGame => AmongUsClient.Instance != null && AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started;
    public static bool IsCountDown => GameStartManager.InstanceExists && GameStartManager.Instance.startState == GameStartManager.StartingStates.Countdown;
    public static bool InMeeting => InGame && MeetingHud.Instance;

    public static bool ShowButtons => !(MapBehaviour.Instance && MapBehaviour.Instance.IsOpen) && !MeetingHud.Instance && !ExileController.Instance;
    public static bool IsHideNSeek => GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek;
    public static bool isSkeld => GameOptionsManager.Instance.CurrentGameOptions.MapId == 0;
    public static bool isMira => GameOptionsManager.Instance.CurrentGameOptions.MapId == 1;
    public static bool isPolus => GameOptionsManager.Instance.CurrentGameOptions.MapId == 2;
    public static bool isDleks => GameOptionsManager.Instance.CurrentGameOptions.MapId == 3;
    public static bool isAirship => GameOptionsManager.Instance.CurrentGameOptions.MapId == 4;
    public static bool isFungle => GameOptionsManager.Instance.CurrentGameOptions.MapId == 5;

    public static string previousEndGameSummary = "";
    public static PlayerControl GetHostPlayer => GameData.Instance.GetHost().Object;
    public static System.Random rnd => new(Guid.NewGuid().GetHashCode());

    public static bool isUsingTransportation(this PlayerControl pc) => pc.inMovingPlat || pc.onLadder;


    /// <summary>
    /// 假任务
    /// </summary>
    public static bool hasFakeTasks(this PlayerControl player)
    {
        return player == Werewolf.werewolf ||
               player == Doomsayer.doomsayer ||
               player == Juggernaut.juggernaut ||
               player == Jester.jester ||
               player == Arsonist.arsonist ||
               player == Witness.Player ||
               player == PartTimer.partTimer ||
               player == Akujo.akujo ||
               player == Pelican.Player ||
               player == Specter.Player ||
               player == BandLeader.Player ||
               player == Swooper.swooper ||
               player == Lawyer.lawyer ||
               player == Executioner.executioner ||
               player == Vulture.vulture ||
               player == Jackal.Sidekick ||
               player == Pavlovsdogs.pavlovsowner ||
               Jackal.jackal.Any(x => x == player) ||
               Pursuer.Player.Any(x => x == player) ||
               Survivor.Player.Any(x => x == player) ||
               Pavlovsdogs.pavlovsdogs.Any(x => x == player);
    }

    /// <summary>
    /// 强力船员判定
    /// </summary>
    public static bool killingCrewAlive()
    {
        var powerCrewAlive = false;
        // This functions blocks the game from ending if specified crewmate roles are alive
        if (!CustomOptionHolder.blockGameEnd.GetBool()) return false;

        if (Sheriff.Player.Any(x => x.IsAlive())) powerCrewAlive = true;
        if (isRoleAlive(Sheriff.Deputy)) powerCrewAlive = true;
        if (isRoleAlive(Veteran.veteran)) powerCrewAlive = true;
        if (isRoleAlive(Mayor.mayor)) powerCrewAlive = true;
        if (isRoleAlive(Swapper.swapper)) powerCrewAlive = true;
        if (isRoleAlive(Prosecutor.prosecutor)) powerCrewAlive = true;
        if (isRoleAlive(Vigilante.vigilante)) powerCrewAlive = true;

        return powerCrewAlive;
    }

    /// <summary>
    /// 红狼视野
    /// </summary>
    public static bool hasImpVision(GameData.PlayerInfo player)
    {
        return player.Role.IsImpostor
               || (Jackal.jackal.Any(p => p.PlayerId == player.PlayerId) && Jackal.hasImpostorVision)
               || (Jackal.Sidekick != null && Jackal.Sidekick.PlayerId == player.PlayerId && Jackal.hasImpostorVision)
               || (Pavlovsdogs.pavlovsowner != null && Pavlovsdogs.pavlovsowner.PlayerId == player.PlayerId && Pavlovsdogs.hasImpostorVision)
               || (Pavlovsdogs.pavlovsdogs.Any(p => p.PlayerId == player.PlayerId) && Pavlovsdogs.hasImpostorVision)
               || (Spy.spy != null && Spy.spy.PlayerId == player.PlayerId && Spy.hasImpostorVision)
               || (Juggernaut.juggernaut != null && Juggernaut.juggernaut.PlayerId == player.PlayerId && Juggernaut.hasImpostorVision)
               || (Jester.jester != null && Jester.jester.PlayerId == player.PlayerId && Jester.hasImpostorVision)
               || (Thief.thief != null && Thief.thief.PlayerId == player.PlayerId && Thief.hasImpostorVision)
               || (Swooper.swooper != null && Swooper.swooper.PlayerId == player.PlayerId && Swooper.hasImpVision)
               || (Pelican.Player != null && Pelican.Player.PlayerId == player.PlayerId && Pelican.hasImpVision)
               || (SchrodingersCat.Player != null && SchrodingersCat.Player.PlayerId == player.PlayerId && SchrodingersCat.hasImpVision)
               || (Werewolf.werewolf != null && Werewolf.werewolf.PlayerId == player.PlayerId && Werewolf.hasImpostorVision);
    }

    public static void handleTrapperTrapOnBodyReport()
    {
        var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.TrapperMeetingFlag);
        writer.EndRPC();
        RPCProcedure.trapperMeetingFlag();
    }

    /// <summary>
    /// 管道技能相关
    /// </summary>
    public static bool roleCanUseVents(this PlayerControl player)
    {
        var roleCouldUse = false;
        if (player.inVent) return true;

        if (Engineer.engineer != null && Engineer.engineer == player)
        {
            roleCouldUse = true;
        }
        else if (Werewolf.canUseVents && Werewolf.werewolf != null && Werewolf.werewolf == player)
        {
            roleCouldUse = true;
        }
        else if (Jackal.canUseVents && Jackal.jackal != null && Jackal.jackal.Any(x => x == player))
        {
            roleCouldUse = true;
        }
        else if (Jackal.canUseVents && Jackal.Sidekick != null && Jackal.Sidekick == player)
        {
            roleCouldUse = true;
        }
        else if ((Pavlovsdogs.canUseVents is 1 or 2) && Pavlovsdogs.pavlovsowner != null && Pavlovsdogs.pavlovsowner == player)
        {
            roleCouldUse = true;
        }
        else if ((Pavlovsdogs.canUseVents is 0 or 2) && Pavlovsdogs.pavlovsdogs != null && Pavlovsdogs.pavlovsdogs.Any(p => p == player))
        {
            roleCouldUse = true;
        }
        else if (Spy.canEnterVents && Spy.spy != null && Spy.spy == player)
        {
            roleCouldUse = true;
        }
        else if (Vulture.canUseVents && Vulture.vulture != null && Vulture.vulture == player)
        {
            roleCouldUse = true;
        }
        else if (Undertaker.deadBodyDraged != null && !Undertaker.canDragAndVent && Undertaker.undertaker == player)
        {
            roleCouldUse = false;
        }
        else if (Thief.canUseVents && Thief.thief != null && Thief.thief == player)
        {
            roleCouldUse = true;
        }
        else if (Jester.jester != null && Jester.jester == player && Jester.canUseVents)
        {
            roleCouldUse = true;
        }
        else if (Juggernaut.juggernaut != null && Juggernaut.juggernaut == player && Juggernaut.canUseVents)
        {
            roleCouldUse = true;
        }
        else if (Pelican.Player != null && Juggernaut.juggernaut == player && Juggernaut.canUseVents)
        {
            roleCouldUse = true;
        }
        else if (player.Data?.Role != null && player.Data.Role.CanVent)
        {
            roleCouldUse = true;
        }
        else if (Swooper.swooper != null && Swooper.swooper == player && Swooper.canUseVents)
        {
            roleCouldUse = true;
        }
        if (Tunneler.tunneler != null && Tunneler.tunneler == player)
        {
            var (playerCompleted, playerTotal) = TasksHandler.taskInfo(Tunneler.tunneler.Data);
            if (playerTotal - playerCompleted == 0 || Tunneler.NoTask) roleCouldUse = true;
        }

        return roleCouldUse;
    }

    /// <summary>
    /// 触发老兵反弹
    /// </summary>
    public static bool checkAndDoVetKill(PlayerControl target)
    {
        var shouldVetKill = Veteran.veteran == target && Veteran.alertActive;
        if (shouldVetKill)
        {
            var writer = StartRPC(CustomRPC.VeteranKill);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            writer.EndRPC();
            RPCProcedure.veteranKill(PlayerControl.LocalPlayer.PlayerId);
        }

        return shouldVetKill;
    }

    public static bool IsNeutral(this PlayerControl player)
    {
        var roleInfo = RoleInfo.getRoleInfoForPlayer(player, false, false).FirstOrDefault();
        return roleInfo != null && roleInfo.roleType == RoleType.Neutral;
    }

    public static bool isKillerNeutral(PlayerControl player)
    {
        return IsNeutral(player) && (
                player == Juggernaut.juggernaut ||
                player == Werewolf.werewolf ||
                player == Swooper.swooper ||
                player == Arsonist.arsonist ||
                player == Pelican.Player ||
                player == Jackal.Sidekick ||
                player == Pavlovsdogs.pavlovsowner ||
                Jackal.jackal.Any(x => x.PlayerId == player.PlayerId) ||
                Pavlovsdogs.pavlovsdogs.Any(x => x.PlayerId == player.PlayerId)
                );
    }

    public static bool isEvilNeutral(PlayerControl player)
    {
        return IsNeutral(player) && (
                player == Jester.jester ||
                player == Vulture.vulture ||
                player == Lawyer.lawyer ||
                player == Executioner.executioner ||
                player == Witness.Player ||
                player == Doomsayer.doomsayer ||
                player == Akujo.akujo ||
                player == Thief.thief ||
                (player == SchrodingersCat.Player && SchrodingersCat.IsEvil)
                );
    }

    public static bool IsKiller(this PlayerControl player)
    {
        return player != null && (player.IsImpostor() || isKillerNeutral(player));
    }

    public static bool IsCrew(this PlayerControl player)
    {
        return player != null && !player.Data.Role.IsImpostor && !IsNeutral(player);
    }

    public static bool IsImpostor(this PlayerControl player, bool AndSpy = false)
    {
        if (player == null) return false;
        return player.Data.Role.IsImpostor || (AndSpy && Spy.spy == player);
    }

    public static string teamString(PlayerControl player)
    {
        var killerTeam = "";
        if (IsNeutral(player)) killerTeam = "NeutralRolesText".Translate();
        else if (player.IsImpostor()) killerTeam = "ImpostorRolesText".Translate();
        else if (player.IsCrew()) killerTeam = "CrewmateRolesText".Translate();
        return killerTeam;
    }

    public static void NoCheckStartMeeting(this PlayerControl reporter, GameData.PlayerInfo target, bool force = false)
    {
        if (InMeeting) return;

        handleVampireBiteOnBodyReport();
        handleBomberExplodeOnBodyReport();
        handleTrapperTrapOnBodyReport();
        //if (Options.DisableMeeting.GetBool()) return;

        MeetingRoomManager.Instance.AssignSelf(reporter, target);
        DestroyableSingleton<HudManager>.Instance.OpenMeetingRoom(reporter);
        reporter.RpcStartMeeting(target);
    }

    public static void enableCursor(bool initalSetCursor)
    {
        if (initalSetCursor)
        {
            var sprite = UnityHelper.loadSpriteFromResources("TheOtherRoles.Resources.Cursor.png", 115f);
            Cursor.SetCursor(sprite.texture, Vector2.zero, CursorMode.Auto);
            return;
        }

        if (Main.ToggleCursor.Value)
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            var sprite = UnityHelper.loadSpriteFromResources("TheOtherRoles.Resources.Cursor.png", 115f);
            Cursor.SetCursor(sprite.texture, Vector2.zero, CursorMode.Auto);
        }
    }

    public static bool roleCanSabotage(this PlayerControl player)
    {
        var roleCouldUse = false;
        if (ModOption.disableSabotage) return false;
        if (Jackal.canSabotage && (Jackal.jackal.Contains(player) || player == Jackal.Sidekick) && !ModOption.disableSabotage)
            roleCouldUse = true;
        if (Pavlovsdogs.canSabotage && (player == Pavlovsdogs.pavlovsowner || Pavlovsdogs.pavlovsdogs.Any(p => p == player)) && !ModOption.disableSabotage)
            roleCouldUse = true;
        if (player.Data?.Role != null && player.Data.Role.IsImpostor)
            roleCouldUse = true;
        return roleCouldUse;
    }

    public static SabotageTypes GetActiveSabo()
    {
        foreach (var task in PlayerControl.LocalPlayer.myTasks.GetFastEnumerator())
            if (task.TaskType == TaskTypes.FixLights)
                return SabotageTypes.Lights;
            else if (task.TaskType == TaskTypes.RestoreOxy)
                return SabotageTypes.O2;
            else if (task.TaskType is TaskTypes.ResetReactor or TaskTypes.StopCharles or TaskTypes.StopCharles)
                return SabotageTypes.Reactor;
            else if (task.TaskType == TaskTypes.FixComms)
                return SabotageTypes.Comms;
            else if (SubmergedCompatibility.IsSubmerged && task.TaskType == SubmergedCompatibility.RetrieveOxygenMask)
                return SabotageTypes.OxyMask;
        return SabotageTypes.None;
    }

    public static bool isLightsActive => GetActiveSabo() == SabotageTypes.Lights;

    public static bool isCommsActive => GetActiveSabo() == SabotageTypes.Comms;

    public static bool isReactor => GetActiveSabo() is SabotageTypes.Reactor or SabotageTypes.O2;

    public static bool isCamoComms => isCommsActive && ModOption.camoComms;

    public static bool isActiveCamoComms => isCamoComms && Camouflager.camoComms;

    public static bool wasActiveCamoComms => !isCamoComms && Camouflager.camoComms;

    public static bool sabotageActive => ShipStatus.Instance.Systems[SystemTypes.Sabotage].CastFast<SabotageSystemType>().AnyActive;

    public static float sabotageTimer => ShipStatus.Instance.Systems[SystemTypes.Sabotage].CastFast<SabotageSystemType>().Timer;

    public static bool MushroomSabotageActive => PlayerControl.LocalPlayer.myTasks.ToArray().Any(x => x.TaskType == TaskTypes.MushroomMixupSabotage);

    public static bool canUseSabotage()
    {
        var sabSystem = ShipStatus.Instance.Systems[SystemTypes.Sabotage].CastFast<SabotageSystemType>();
        IActivatable doors = null;
        if (ShipStatus.Instance.Systems.TryGetValue(SystemTypes.Doors, out ISystemType systemType))
        {
            doors = systemType.CastFast<IActivatable>();
        }
        return GameManager.Instance.SabotagesEnabled() && sabSystem.Timer <= 0f && !sabSystem.AnyActive && !(doors != null && doors.IsActive);
    }

    public static void camoReset()
    {
        Camouflager.resetCamouflage();
        if (Morphling.morphTimer > 0f && Morphling.morphling != null && Morphling.morphTarget != null)
        {
            var target = Morphling.morphTarget;
            Morphling.morphling.setLook(target.Data.PlayerName, target.Data.DefaultOutfit.ColorId,
                target.Data.DefaultOutfit.HatId, target.Data.DefaultOutfit.VisorId, target.Data.DefaultOutfit.SkinId,
                target.Data.DefaultOutfit.PetId);
        }
    }

    public static IEnumerator BlackmailShhh()
    {
        //Helpers.showFlash(new Color32(49, 28, 69, byte.MinValue), 3f, "Blackmail", false, 0.75f);
        yield return HudManager.Instance.CoFadeFullScreen(Color.clear, new Color(0f, 0f, 0f, 0.98f));
        var TempPosition = HudManager.Instance.shhhEmblem.transform.localPosition;
        var TempDuration = HudManager.Instance.shhhEmblem.HoldDuration;
        HudManager.Instance.shhhEmblem.transform.localPosition = new Vector3(
            HudManager.Instance.shhhEmblem.transform.localPosition.x,
            HudManager.Instance.shhhEmblem.transform.localPosition.y,
            HudManager.Instance.FullScreen.transform.position.z + 1f);
        HudManager.Instance.shhhEmblem.TextImage.text = ModTranslation.GetString("BlackmailShhhText");
        HudManager.Instance.shhhEmblem.HoldDuration = 3f;
        yield return HudManager.Instance.ShowEmblem(true);
        HudManager.Instance.shhhEmblem.transform.localPosition = TempPosition;
        HudManager.Instance.shhhEmblem.HoldDuration = TempDuration;
        yield return HudManager.Instance.CoFadeFullScreen(new Color(0f, 0f, 0f, 0.98f), Color.clear);
        yield return null;
    }

    public static int getAvailableId()
    {
        var id = 0;
        while (true)
        {
            if (ShipStatus.Instance.AllVents.All(v => v.Id != id)) return id;
            id++;
        }
    }

    public static void turnToImpostorRPC(PlayerControl player)
    {
        var writer = StartRPC(CustomRPC.TurnToImpostor);
        writer.Write(player.PlayerId);
        writer.EndRPC();
        RPCProcedure.turnToImpostor(player.PlayerId);
    }

    public static void turnToImpostor(PlayerControl player)
    {
        player.Data.Role.TeamType = RoleTeamTypes.Impostor;
        RoleManager.Instance.SetRole(player, RoleTypes.Impostor);
        player.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);

        Message("PROOF I AM IMP VANILLA ROLE: " + player.Data.Role.IsImpostor);

        foreach (var player2 in PlayerControl.AllPlayerControls)
            if (player2.Data.Role.IsImpostor && PlayerControl.LocalPlayer.Data.Role.IsImpostor)
                player.cosmetics.nameText.color = Palette.ImpostorRed;
    }
#nullable enable
    public static void showTargetNameOnButton(PlayerControl? target, CustomButton button, string defaultText)
    {
        if (CustomOptionHolder.showButtonTarget.GetBool())
        {
            string text;
            if (Camouflager.camouflageTimer >= 0.1f || isCamoComms) text = defaultText;
            else if (isLightsActive) text = defaultText;
            else if (Trickster.trickster != null && Trickster.lightsOutTimer > 0f) text = defaultText;
            else if (Morphling.morphling != null && Morphling.morphTarget != null && target == Morphling.morphling && Morphling.morphTimer > 0)
                text = Morphling.morphTarget.Data.PlayerName;
            else if (target == Ninja.ninja && Ninja.isInvisable) text = defaultText;
            else if (target == Swooper.swooper && Swooper.isInvisable) text = defaultText;
            else if (Jackal.jackal.Any(p => p == target) && Jackal.isInvisable) text = defaultText;
            else text = target == null ? defaultText : target.Data.PlayerName;

            button.actionButton.OverrideText(text);
            button.showButtonText = true;
        }
        else
        {
            button.actionButton.OverrideText(defaultText);
            button.showButtonText = true;
        }
    }
#nullable disable

    public static void AddUnique<T>(this Il2CppSystem.Collections.Generic.List<T> self, T item) where T : IDisconnectHandler
    {
        if (!self.Contains(item)) self.Add(item);
    }

    public static T GetRandom<T>(this T[] list)
    {
        var indexData = UnityEngine.Random.Range(0, list.Length);
        return list[indexData];
    }

    public static int GetRandom<T>(List<T> list)
    {
        var indexData = UnityEngine.Random.Range(0, list.Count);
        return indexData;
    }

    public static void ForEach<T>(this Il2CppArrayBase<T> list, Action<T> func)
    {
        foreach (T obj in list) func(obj);
    }

    public static List<T> ToList<T>(this Il2CppSystem.Collections.Generic.List<T> list)
    {
        List<T> newList = new(list.Count);
        foreach (T item in list)
        {
            newList.Add(item);
        }
        return newList;
    }

    public static T FirstOrDefault<T>(this Il2CppArrayBase<T> list)
    {
        foreach (T obj in list)
            return obj;
        return default;
    }

    public static T FirstOrDefault<T>(this List<T> list)
    {
        if (list.Count > 0)
            return list[0];
        return default;
    }

    public static T FirstOrDefault<T>(this Il2CppSystem.Collections.Generic.List<T> list, Func<T, bool> func)
    {
        foreach (T obj in list)
            if (func(obj))
                return obj;
        return default;
    }

    public static Il2CppSystem.Collections.Generic.List<T> ToIl2CppList<T>(this IEnumerable<T> list)
    {
        Il2CppSystem.Collections.Generic.List<T> newList = new(list.Count());
        foreach (T item in list)
        {
            newList.Add(item);
        }
        return newList;
    }

    public static T Find<T>(this Il2CppSystem.Collections.Generic.List<T> data, Predicate<T> match)
    {
        return data.ToList().Find(match);
    }

    public static KeyValuePair<TKey, TValue> FirstOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> list, Func<KeyValuePair<TKey, TValue>, bool> func)
    {
        foreach (KeyValuePair<TKey, TValue> obj in list)
            if (func(obj))
                return obj;
        return default;
    }

    public static bool Any<TKey, TValue>(this Dictionary<TKey, TValue> dict, Func<KeyValuePair<TKey, TValue>, bool> func)
    {
        foreach (KeyValuePair<TKey, TValue> obj in dict)
            if (func(obj))
                return true;
        return false;
    }

    public static bool Any<T>(this List<T> list, Func<T, bool> func)
    {
        if (list == null)
            return false;
        foreach (T obj in list)
            if (func(obj))
                return true;
        return false;
    }

    public static bool TryAdd<T>(this List<T> list, T item)
    {
        if (list == null || item == null || list.Contains(item)) return false;
        try
        {
            list.Add(item);
            return true;
        }
        catch (Exception e)
        {
            Message(e, "TryAdd");
            return false;
        }
    }

    public static TKey GetKeyByValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TValue value, TKey defaultvalue = default)
    {
        foreach (var pair in dictionary)
        {
            if (EqualityComparer<TValue>.Default.Equals(pair.Value, value))
            {
                return pair.Key;
            }
        }
        return defaultvalue;
    }

    public static bool Contains<T, TKey>(this IEnumerable<T> list, T item, Func<T, TKey> keySelector)
    {
        return list.Any(x => keySelector(x).Equals(keySelector(item)));
    }

    public static int Count<T>(this Il2CppSystem.Collections.Generic.List<T> list, Func<T, bool> func = null)
    {
        int count = 0;
        foreach (T obj in list)
            if (func == null || func(obj)) count++;
        return count;
    }

    public static Color HexToColor(string hex)
    {
        _ = ColorUtility.TryParseHtmlString("#" + hex, out var color);
        return color;
    }

    public static string readTextFromResources(string path)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var stream = assembly.GetManifestResourceStream(path);
        var textStreamReader = new StreamReader(stream);
        return textStreamReader.ReadToEnd();
    }

    public static string readTextFromFile(string path)
    {
        Stream stream = File.OpenRead(path);
        var textStreamReader = new StreamReader(stream);
        return textStreamReader.ReadToEnd();
    }

    public static List<RoleInfo> allRoleInfos()
    {
        var allRoleInfo = new List<RoleInfo>();
        foreach (var role in RoleInfo.allRoleInfos)
        {
            if (role.roleType is RoleType.Modifier or RoleType.Ghost or RoleType.Special) continue;
            allRoleInfo.Add(role);
        }
        return allRoleInfo;
    }

    public static List<RoleInfo> onlineRoleInfos()
    {
        var role = new List<RoleInfo>();
        role.AddRange(PlayerControl.AllPlayerControls.ToList()
            .Select(n => RoleInfo.getRoleInfoForPlayer(n, false, false)).SelectMany(x => x));
        return role;
    }

    public static PlayerControl playerById(byte? id)
    {
        if (id == null) return null;
        foreach (PlayerControl player in PlayerControl.AllPlayerControls.ToList())
            if (player.PlayerId == id) return player;
        return null;
    }

    public static bool isSabotageActive()
    {
        foreach (var task in PlayerControl.LocalPlayer.myTasks.GetFastEnumerator())
            if (task.TaskType == TaskTypes.FixLights || task.TaskType == TaskTypes.RestoreOxy ||
                task.TaskType == TaskTypes.ResetReactor || task.TaskType == TaskTypes.ResetSeismic ||
                task.TaskType == TaskTypes.FixComms || task.TaskType == TaskTypes.StopCharles
                || (SubmergedCompatibility.IsSubmerged && task.TaskType == SubmergedCompatibility.RetrieveOxygenMask))
                return true;
        return false;
    }

    public static void handleVampireBiteOnBodyReport()
    {
        // Murder the bitten player and reset bitten (regardless whether the kill was successful or not)
        checkMurderAttemptAndKill(Vampire.vampire, Vampire.bitten, false);
        var writer = StartRPC(CustomRPC.VampireSetBitten);
        writer.Write(byte.MaxValue);
        writer.Write(byte.MaxValue);
        writer.EndRPC();
        RPCProcedure.vampireSetBitten(byte.MaxValue, byte.MaxValue);
    }

    public static void handleBomberExplodeOnBodyReport()
    {
        // Murder the bitten player and reset bitten (regardless whether the kill was successful or not)
        checkMurderAttemptAndKill(Bomber.bomber, Bomber.hasBombPlayer, false);
        var writer = StartRPC(CustomRPC.GiveBomb);
        writer.Write(byte.MaxValue);
        writer.Write(false);
        writer.EndRPC();
        RPCProcedure.giveBomb(byte.MaxValue);
    }

    public static void refreshRoleDescription(PlayerControl player)
    {
        var infos = RoleInfo.getRoleInfoForPlayer(player);
        List<string> taskTexts = new(infos.Count);

        foreach (var roleInfo in infos) taskTexts.Add(getRoleString(roleInfo));

        var toRemove = new List<PlayerTask>();
        foreach (var t in player.myTasks.GetFastEnumerator())
        {
            var textTask = t.TryCast<ImportantTextTask>();
            if (textTask == null) continue;

            var currentText = textTask.Text;

            if (taskTexts.Contains(currentText))
                taskTexts.Remove(currentText); // TextTask for this RoleInfo does not have to be added, as it already exists
            else toRemove.Add(t); // TextTask does not have a corresponding RoleInfo and will hence be deleted
        }

        foreach (var t in toRemove)
        {
            t.OnRemove();
            player.myTasks.Remove(t);
            Object.Destroy(t.gameObject);
        }

        // Add TextTask for remaining RoleInfos
        foreach (var title in taskTexts)
        {
            var task = new GameObject("RoleTask").AddComponent<ImportantTextTask>();
            task.transform.SetParent(player.transform, false);
            task.Text = title;
            player.myTasks.Insert(0, task);
        }
    }

    public static void ModRevive(this PlayerControl target, bool cleanBody = true, bool reloadPos = true)
    {
        target?.Revive();

        if (target == null) return;

        DeadBody[] array = Object.FindObjectsOfType<DeadBody>();
        for (var i = 0; i < array.Length; i++)
        {
            if (GameData.Instance.GetPlayerById(array[i].ParentId).PlayerId == target.PlayerId)
            {
                if (reloadPos) target.NetTransform.RpcSnapTo(array[i].transform.position);
                if (cleanBody) Object.Destroy(array[i].gameObject);
                break;
            }
        }
    }

    internal static string getRoleString(RoleInfo roleInfo)
    {
        return cs(roleInfo.color, $"{roleInfo.Name}: {roleInfo.ShortDescription}");
    }

    public static bool isDark(byte playerId)
    {
        return playerId % 2 == 0;
    }

    public static bool isLighterColor(PlayerControl target)
    {
        if (!ModOption.randomLigherPlayer) return CustomColors.lighterColors.Contains(target.Data.DefaultOutfit.ColorId);
        return !isDark(target.PlayerId);
    }

    public static TextMeshPro getFirst(this TextMeshPro[] text)
    {
        if (text == null) return null;
        foreach (var self in text)
            if (self.text == "") return self;
        return text[0];
    }

    public static Color getTeamColor(RoleType team)
    {
        return team switch
        {
            RoleType.Crewmate => Palette.CrewmateBlue,
            RoleType.Impostor => Palette.ImpostorRed,
            RoleType.Neutral => Color.gray,
            RoleType.Modifier => Color.yellow,
            RoleType.Ghost => new Color32(159, 127, 209, byte.MaxValue),
            _ => Palette.White
        };
    }

    public static bool IsAlive(this PlayerControl player)
    {
        return player != null && !player.Data.Disconnected && !player.Data.IsDead;
    }

    public static bool IsDead(this PlayerControl player)
    {
        return player == null || player.Data.Disconnected || player.Data.IsDead;
    }

    public static void setInvisable(PlayerControl player)
    {
        var invisibleWriter = StartRPC(CustomRPC.SetInvisibleGen);
        invisibleWriter.Write(player.PlayerId);
        invisibleWriter.Write(byte.MinValue);
        invisibleWriter.EndRPC();
        RPCProcedure.setInvisibleGen(player.PlayerId, byte.MinValue);
    }

    public static void SetActiveAllObject(this GameObject[] trans, string notdelete, bool IsActive)
    {
        foreach (GameObject tran in trans)
        {
            if (tran.name != notdelete)
            {
                tran.SetActive(IsActive);
            }
        }
    }

    public static GameObject[] GetChildren(this GameObject ParentObject)
    {
        GameObject[] ChildObject = new GameObject[ParentObject.transform.childCount];

        for (int i = 0; i < ParentObject.transform.childCount; i++)
        {
            ChildObject[i] = ParentObject.transform.GetChild(i).gameObject;
        }
        return ChildObject;
    }

    public static bool ZoomButtonActive()
    {
        if (!ShowGhostInfo || InMeeting) return false;
        var (playerCompleted, playerTotal) = TasksHandler.taskInfo(PlayerControl.LocalPlayer.Data);
        var numberOfLeftTasks = playerTotal - playerCompleted;
        return numberOfLeftTasks <= 0 || !CustomOptionHolder.finishTasksBeforeHauntingOrZoomingOut.GetBool();
    }

    public static void clearAllTasks(this PlayerControl player)
    {
        if (player == null) return;
        foreach (var playerTask in player.myTasks.GetFastEnumerator())
        {
            playerTask.OnRemove();
            Object.Destroy(playerTask.gameObject);
        }

        player.myTasks.Clear();

        if (player.Data != null && player.Data.Tasks != null)
            player.Data.Tasks.Clear();
    }

    public static void shareGameVersion()
    {
        var writer = StartRPC(CustomRPC.VersionHandshake);
        writer.Write((byte)Main.Version.Major);
        writer.Write((byte)Main.Version.Minor);
        writer.Write((byte)Main.Version.Build);
        writer.Write(AmongUsClient.Instance.AmHost ? GameStartManagerPatch.timer : -1f);
        writer.WritePacked(AmongUsClient.Instance.ClientId);
        writer.Write((byte)(Main.Version.Revision < 0 ? 0xFF : Main.Version.Revision));
        writer.Write(Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId.ToByteArray());
        writer.EndRPC();
        RPCProcedure.versionHandshake(Main.Version.Major, Main.Version.Minor, Main.Version.Build, Main.Version.Revision, Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId, AmongUsClient.Instance.ClientId);
    }

    public static void RpcRepairSystem(this ShipStatus shipStatus, SystemTypes systemType, byte amount)
    {
        shipStatus.RpcUpdateSystem(systemType, amount);
    }

    public static bool IsCN()
    {
        return (int)AmongUs.Data.DataManager.Settings.Language.CurrentLanguage == 13;
    }

    public static string GithubUrl(this string url)
    {
        if (IsCN() && (url.Contains("github.com") || url.Contains("githubusercontent.com")) && !url.Contains("ghfast.top"))
        {
            return "https://ghfast.top/" + url;
        }
        return url;
    }

    public static void setSemiTransparent(this PoolablePlayer player, bool value, float alpha = 0.25f)
    {
        alpha = value ? alpha : 1f;
        foreach (var r in player.gameObject.GetComponentsInChildren<SpriteRenderer>())
            r.color = new Color(r.color.r, r.color.g, r.color.b, alpha);
        player.cosmetics.nameText.color = new Color(player.cosmetics.nameText.color.r,
            player.cosmetics.nameText.color.g, player.cosmetics.nameText.color.b, alpha);
    }

    public static string GetString(this TranslationController t, StringNames key, params Il2CppSystem.Object[] parts)
    {
        return t.GetString(key, parts);
    }

    public static string cs(Color c, string s)
    {
        return string.Format("<color=#{0:X2}{1:X2}{2:X2}{3:X2}>{4}</color>", ToByte(c.r), ToByte(c.g), ToByte(c.b),
            ToByte(c.a), s);
    }

    public static int lineCount(string text)
    {
        return text.Count(c => c == '\n');
    }

    private static byte ToByte(float f)
    {
        f = Mathf.Clamp01(f);
        return (byte)(f * 255);
    }

    public static KeyValuePair<byte, int> MaxPair(this Dictionary<byte, int> self, out bool tie)
    {
        tie = true;
        var result = new KeyValuePair<byte, int>(byte.MaxValue, int.MinValue);
        foreach (var keyValuePair in self)
            if (keyValuePair.Value > result.Value)
            {
                result = keyValuePair;
                tie = false;
            }
            else if (keyValuePair.Value == result.Value)
            {
                tie = true;
            }

        return result;
    }

    public static bool hidePlayerName(PlayerControl source, PlayerControl target)
    {
        var localPlayer = PlayerControl.LocalPlayer;
        if (Camouflager.camouflageTimer > 0f || MushroomSabotageActive || isCamoComms)
            return true; // No names are visible
        if (SurveillanceMinigamePatch.nightVisionIsActive) return true;
        if (Ninja.isInvisable && Ninja.ninja == target) return true;
        if (Jackal.isInvisable && Jackal.jackal.Any(p => p == target)) return true;
        if (Swooper.isInvisable && Swooper.swooper == target) return true;
        if (ModOption.hideOutOfSightNametags && InGame && source.IsAlive() && !isFungle
            && PhysicsHelpers.AnythingBetween(localPlayer.GetTruePosition(), target.GetTruePosition(), Constants.ShadowMask, false))
            return true;

        if (!ModOption.hidePlayerNames) return false; // All names are visible
        if (source == null || target == null) return true;
        if (source == target) return false; // Player sees his own name
        if (source.Data.Role.IsImpostor && (target.Data.Role.IsImpostor || target == Spy.spy))
            return false; // Members of team Impostors see the names of Impostors/Spies
        if ((source == Lovers.lover1 || source == Lovers.lover2) &&
            (target == Lovers.lover1 || target == Lovers.lover2))
            return false; // Members of team Lovers see the names of each other
        if ((Jackal.jackal.Any(p => p == source) || source == Jackal.Sidekick)
            && (Jackal.jackal.Any(p => p == target) || target == Jackal.Sidekick))
            return false; // Members of team Jackal see the names of each other
        if ((source == Pavlovsdogs.pavlovsowner || Pavlovsdogs.pavlovsdogs.Any(x => x == target))
            && (target == Pavlovsdogs.pavlovsowner || Pavlovsdogs.pavlovsdogs.Any(x => x == target)))
            return false;
        if (Sheriff.knowsSheriff && (Sheriff.Player.Any(x => x == source) || source == Sheriff.Deputy) &&
            (Sheriff.Player.Any(x => x == target) || target == Sheriff.Deputy))
            return false; // Sheriff & Deputy see the names of each other
        return true;
    }

    public static void setDefaultLook(this PlayerControl target, bool enforceNightVisionUpdate = true)
    {
        if (MushroomSabotageActive)
        {
            var instance = ShipStatus.Instance.CastFast<FungleShipStatus>().specialSabotage;
            var condensedOutfit = instance.currentMixups[target.PlayerId];
            var playerOutfit = instance.ConvertToPlayerOutfit(condensedOutfit);
            target.MixUpOutfit(playerOutfit);
        }
        else
        {
            target.setLook(target.Data.PlayerName, target.Data.DefaultOutfit.ColorId, target.Data.DefaultOutfit.HatId,
                target.Data.DefaultOutfit.VisorId, target.Data.DefaultOutfit.SkinId, target.Data.DefaultOutfit.PetId,
                enforceNightVisionUpdate);
        }
    }

    public static void setLook(this PlayerControl target, string playerName, int colorId, string hatId, string visorId,
        string skinId, string petId, bool enforceNightVisionUpdate = true)
    {
        target.RawSetColor(colorId);
        target.RawSetVisor(visorId, colorId);
        target.RawSetHat(hatId, colorId);
        target.RawSetName(hidePlayerName(PlayerControl.LocalPlayer, target) ? "" : playerName);


        SkinViewData nextSkin = null;
        try
        {
            nextSkin = ShipStatus.Instance.CosmeticsCache.GetSkin(skinId);
        }
        catch
        {
            return;
        }

        ;

        var playerPhysics = target.MyPhysics;
        AnimationClip clip = null;
        var spriteAnim = playerPhysics.myPlayer.cosmetics.skin.animator;
        var currentPhysicsAnim = playerPhysics.Animations.Animator.GetCurrentAnimation();


        if (currentPhysicsAnim == playerPhysics.Animations.group.RunAnim) clip = nextSkin.RunAnim;
        else if (currentPhysicsAnim == playerPhysics.Animations.group.SpawnAnim) clip = nextSkin.SpawnAnim;
        else if (currentPhysicsAnim == playerPhysics.Animations.group.EnterVentAnim) clip = nextSkin.EnterVentAnim;
        else if (currentPhysicsAnim == playerPhysics.Animations.group.ExitVentAnim) clip = nextSkin.ExitVentAnim;
        else if (currentPhysicsAnim == playerPhysics.Animations.group.IdleAnim) clip = nextSkin.IdleAnim;
        else clip = nextSkin.IdleAnim;
        var progress = playerPhysics.Animations.Animator.m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        playerPhysics.myPlayer.cosmetics.skin.skin = nextSkin;
        playerPhysics.myPlayer.cosmetics.skin.UpdateMaterial();

        spriteAnim.Play(clip);
        spriteAnim.m_animator.Play("a", 0, progress % 1);
        spriteAnim.m_animator.Update(0f);

        target.RawSetPet(petId, colorId);

        if (enforceNightVisionUpdate) SurveillanceMinigamePatch.enforceNightVision(target);
        Chameleon.update(); // so that morphling and camo wont make the chameleons visible
    }

    public static void showFlash(Color color, float duration = 1f, string message = "", float alpha = 0.75f)
    {
        if (FastDestroyableSingleton<HudManager>.Instance == null ||
            FastDestroyableSingleton<HudManager>.Instance.FullScreen == null) return;
        if (Grenadier.controls.ToList().Any(x => x == PlayerControl.LocalPlayer)) return;
        FastDestroyableSingleton<HudManager>.Instance.FullScreen.gameObject.SetActive(true);
        FastDestroyableSingleton<HudManager>.Instance.FullScreen.enabled = true;
        // Message Text
        var messageText = Object.Instantiate(FastDestroyableSingleton<HudManager>.Instance.KillButton.cooldownTimerText,
            FastDestroyableSingleton<HudManager>.Instance.transform);
        messageText.text = message;
        messageText.enableWordWrapping = false;
        messageText.transform.localScale = Vector3.one * 0.5f;
        messageText.transform.localPosition += new Vector3(0f, 2f, -69f);
        messageText.gameObject.SetActive(true);
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(duration, new Action<float>(p =>
        {
            var renderer = FastDestroyableSingleton<HudManager>.Instance.FullScreen;

            if (p < 0.5)
            {
                if (renderer != null) renderer.color = new Color(color.r, color.g, color.b, Mathf.Clamp01(p * 2 * alpha));
            }
            else
            {
                if (renderer != null) renderer.color = new Color(color.r, color.g, color.b, Mathf.Clamp01((1 - p) * 2 * alpha));
            }

            if (p == 1f && renderer != null) renderer.enabled = false;
            if (p == 1f) messageText.gameObject.Destroy();
        })));
    }

    public static IEnumerator showFlashCoroutine(Color color, float waitfor = 1f, float alpha = 0.3f)
    {
        if (Grenadier.controls.ToList().Any(x => x == PlayerControl.LocalPlayer)) yield return null;
        color.a = alpha;
        if (HudManager.InstanceExists && HudManager.Instance.FullScreen)
        {
            var fullscreen = DestroyableSingleton<HudManager>.Instance.FullScreen;
            fullscreen.enabled = true;
            fullscreen.gameObject.active = true;
            fullscreen.color = color;
        }

        yield return new WaitForSeconds(waitfor);

        if (HudManager.InstanceExists && HudManager.Instance.FullScreen)
        {
            var fullscreen = DestroyableSingleton<HudManager>.Instance.FullScreen;
            if (fullscreen.color.Equals(color))
            {
                fullscreen.color = new Color(1f, 0f, 0f, 0.37254903f);
                fullscreen.enabled = false;
            }
        }
    }

    public static List<PlayerControl> GetClosestPlayers(Vector2 truePosition, float radius, bool includeDead)
    {
        List<PlayerControl> playerControlList = new List<PlayerControl>();
        float lightRadius = radius * ShipStatus.Instance.MaxLightRadius;
        List<GameData.PlayerInfo> allPlayers = GameData.Instance.AllPlayers.ToList();
        for (int index = 0; index < allPlayers.Count; ++index)
        {
            GameData.PlayerInfo playerInfo = allPlayers[index];
            if (!playerInfo.Disconnected && (!playerInfo.Object.Data.IsDead || includeDead))
            {
                Vector2 vector2 = new Vector2(playerInfo.Object.GetTruePosition().x - truePosition.x, playerInfo.Object.GetTruePosition().y - truePosition.y);
                float magnitude = vector2.magnitude;
                if (magnitude <= lightRadius)
                {
                    PlayerControl playerControl = playerInfo.Object;
                    playerControlList.Add(playerControl);
                }
            }
        }
        return playerControlList;
    }

    public static MurderAttemptResult checkMuderAttempt(PlayerControl killer, PlayerControl target, bool ignoreBlank = false, bool ignoreIfKillerIsDead = false)
    {
        var targetRole = RoleInfo.getRoleInfoForPlayer(target, false).FirstOrDefault();

        // Modified vanilla checks
        if (AmongUsClient.Instance.IsGameOver) return MurderAttemptResult.SuppressKill;
        if (killer == null || killer.Data == null || (killer.Data.IsDead && !ignoreIfKillerIsDead) || killer.Data.Disconnected)
            return MurderAttemptResult.SuppressKill; // Allow non Impostor kills compared to vanilla code
        if (target == null || target.Data == null || target.Data.IsDead || target.Data.Disconnected)
            return MurderAttemptResult.SuppressKill; // Allow killing players in vents compared to vanilla code
        if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek)
            return MurderAttemptResult.PerformKill;

        // Handle first kill attempt
        if (ModOption.shieldFirstKill && ModOption.firstKillPlayer == target)
            return MurderAttemptResult.SuppressKill;

        // Handle blank shot
        if (!ignoreBlank && Pursuer.blankedList.Any(x => x.PlayerId == killer.PlayerId))
        {
            var writer = StartRPC(CustomRPC.PursuerSetBlanked);
            writer.Write(killer.PlayerId);
            writer.Write((byte)0);
            writer.EndRPC();
            RPCProcedure.pursuerSetBlanked(killer.PlayerId, 0);

            return MurderAttemptResult.BlankKill;
        }

        // Kill the killer if the Veteran is on alert

        if (Veteran.veteran != null && target == Veteran.veteran && Veteran.alertActive)
        {
            if (Medic.shielded != null && Medic.shielded == target)
            {
                var writer = StartRPC(CustomRPC.ShieldedMurderAttempt);
                writer.Write(target.PlayerId);
                writer.EndRPC();
                RPCProcedure.shieldedMurderAttempt(killer.PlayerId);
            }

            return MurderAttemptResult.ReverseKill;
        } // Kill the killer if the Veteran is on alert

        // Kill the Body Guard and the killer if the target is guarded

        if (BodyGuard.bodyguard != null && target == BodyGuard.guarded && IsAlive(BodyGuard.bodyguard))
        {
            if (Medic.shielded != null && Medic.shielded == target)
            {
                var writer = StartRPC(CustomRPC.ShieldedMurderAttempt);
                writer.Write(target.PlayerId);
                writer.EndRPC();
                RPCProcedure.shieldedMurderAttempt(killer.PlayerId);
            }

            return MurderAttemptResult.BodyGuardKill;
        }

        // Block impostor shielded kill
        if (!Medic.unbreakableShield && Medic.shielded != null && Medic.shielded == target)
        {
            var write = StartRPC(CustomRPC.PursuerSetBlanked);
            write.Write(killer.PlayerId);
            write.Write((byte)0);
            write.EndRPC();
            RPCProcedure.pursuerSetBlanked(killer.PlayerId, 0);
            Medic.shielded = null;
            return MurderAttemptResult.BlankKill;
        }

        if (Medic.shielded != null && Medic.shielded == target)
        {
            var writer = StartRPC(CustomRPC.ShieldedMurderAttempt);
            writer.Write(killer.PlayerId);
            writer.EndRPC();
            RPCProcedure.shieldedMurderAttempt(killer.PlayerId);
            SoundEffectsManager.play("fail");
            return MurderAttemptResult.BlankKill;
        }

        // Block impostor not fully grown mini kill
        if (Mini.mini != null && target == Mini.mini && !Mini.isGrownUp()) return MurderAttemptResult.SuppressKill;
        // Block Time Master with time shield kill
        if (TimeMaster.shieldActive && TimeMaster.timeMaster != null && TimeMaster.timeMaster == target)
        {
            if (!InMeeting)
            {
                // Only rewind the attempt was not called because a meeting startet 
                var writer = StartRPC(CustomRPC.TimeMasterRewindTime);
                writer.EndRPC();
                RPCProcedure.timeMasterRewindTime();
            }

            return MurderAttemptResult.BlankKill;
        }

        if (Survivor.Player != null && Survivor.Player.Contains(target) && Survivor.vestActive)
        {
            CustomButton.resetKillButton(killer, Survivor.vestResetCooldown);
            SoundEffectsManager.play("fail");
            return MurderAttemptResult.SuppressKill;
        }

        if (Cursed.cursed != null && Cursed.cursed == target && killer.Data.Role.IsImpostor)
        {
            var writer = StartRPC(CustomRPC.PursuerSetBlanked);
            writer.Write(killer.PlayerId);
            writer.Write((byte)0);
            writer.EndRPC();
            RPCProcedure.pursuerSetBlanked(killer.PlayerId, 0);

            turnToImpostorRPC(target);

            return MurderAttemptResult.BlankKill;
        }

        // Thief if hit crew only kill if setting says so, but also kill the thief.
        else if (Thief.thief != null && killer == Thief.thief && !Thief.tiefCanKill(target, killer))
        {
            Thief.suicideFlag = true;
            return MurderAttemptResult.SuppressKill;
        }

        if (target.isUsingTransportation() && !InMeeting && killer == Vampire.vampire)
            return MurderAttemptResult.DelayVampireKill;
        if (target.isUsingTransportation())
            return MurderAttemptResult.SuppressKill;
        return MurderAttemptResult.PerformKill;
    }

    public static void MurderPlayer(PlayerControl killer, PlayerControl target, bool showAnimation)
    {
        var writer = StartRPC(CustomRPC.UncheckedMurderPlayer);
        writer.Write(killer.PlayerId);
        writer.Write(target.PlayerId);
        writer.Write(showAnimation ? byte.MaxValue : 0);
        writer.EndRPC();
        RPCProcedure.uncheckedMurderPlayer(killer.PlayerId, target.PlayerId, showAnimation ? byte.MaxValue : (byte)0);
    }

    public static MurderAttemptResult checkMurderAttemptAndKill(PlayerControl killer, PlayerControl target, bool showAnimation = true, bool ignoreBlank = false,
        bool ignoreIfKillerIsDead = false)
    {
        // The local player checks for the validity of the kill and performs it afterwards (different to vanilla, where the host performs all the checks)
        // The kill attempt will be shared using a custom RPC, hence combining modded and unmodded versions is impossible
        var murder = checkMuderAttempt(killer, target, ignoreBlank, ignoreIfKillerIsDead);

        if (murder == MurderAttemptResult.PerformKill)
        {
            if (killer == Poucher.poucher) Poucher.killed.Add(target);
            if (Mimic.mimic != null && killer == Mimic.mimic && !Mimic.hasMimic)
            {
                var writerMimic = StartRPC(CustomRPC.MimicMimicRole);
                writerMimic.Write(target.PlayerId);
                writerMimic.EndRPC();
                Mimic.MimicRole(target.PlayerId);
            }

            MurderPlayer(killer, target, showAnimation);
        }
        else if (murder == MurderAttemptResult.DelayVampireKill)
        {
            HudManager.Instance.StartCoroutine(Effects.Lerp(10f, new Action<float>(p =>
            {
                if (!target.isUsingTransportation() && Vampire.bitten != null)
                {
                    var writer = StartRPC(CustomRPC.VampireSetBitten);
                    writer.Write(byte.MaxValue);
                    writer.Write(byte.MaxValue);
                    writer.EndRPC();
                    RPCProcedure.vampireSetBitten(byte.MaxValue, byte.MaxValue);
                    MurderPlayer(killer, target, showAnimation);
                }
            })));
        }

        if (murder == MurderAttemptResult.BodyGuardKill)
        {
            // Kill the Killer
            var writer = StartRPC(CustomRPC.UncheckedMurderPlayer);
            writer.Write(BodyGuard.bodyguard.PlayerId);
            writer.Write(killer.PlayerId);
            writer.Write(showAnimation ? byte.MaxValue : 0);
            writer.EndRPC();
            RPCProcedure.uncheckedMurderPlayer(BodyGuard.bodyguard.PlayerId, killer.PlayerId, 0);

            // Kill the BodyGuard
            var writer2 = StartRPC(CustomRPC.UncheckedMurderPlayer);
            writer2.Write(killer.PlayerId);
            writer2.Write(BodyGuard.bodyguard.PlayerId);
            writer2.Write(showAnimation ? byte.MaxValue : 0);
            writer2.EndRPC();
            RPCProcedure.uncheckedMurderPlayer(BodyGuard.bodyguard.PlayerId, BodyGuard.bodyguard.PlayerId, 0);

            var writer3 = StartRPC(CustomRPC.ShowBodyGuardFlash);
            writer3.EndRPC();
            RPCProcedure.showBodyGuardFlash();
        }

        if (murder == MurderAttemptResult.ReverseKill) checkMurderAttemptAndKill(target, killer);

        return murder;
    }

    public static void SetKillTimerUnchecked(this PlayerControl player, float time, float max = float.NegativeInfinity)
    {
        if (max == float.NegativeInfinity) max = time;

        player.killTimer = time;
        FastDestroyableSingleton<HudManager>.Instance.KillButton.SetCoolDown(time, max);
    }

    public static bool isRoleAlive(PlayerControl player)
    {
        if (Mimic.mimic != null && player == Mimic.mimic) return false;
        return player != null && player.IsAlive();
    }

    public static PlayerControl getPartner(this PlayerControl player)
    {
        return Akujo.otherLover(player) ?? (Lovers.bothDie ? Lovers.otherLover(player) : null);
    }

    public static void toggleZoom(bool reset = false)
    {
        var orthographicSize = reset || zoomOutStatus ? 3f : 12f;

        zoomOutStatus = !zoomOutStatus && !reset;
        Camera.main.orthographicSize = orthographicSize;
        foreach (var cam in Camera.allCameras)
            if (cam != null && cam.gameObject.name == "UI Camera")
                cam.orthographicSize = orthographicSize;
        // The UI is scaled too, else we cant click the buttons. Downside: map is super small.

        if (HudManagerStartPatch.zoomOutButton != null)
        {
            HudManagerStartPatch.zoomOutButton.Sprite = zoomOutStatus
                ? new ResourceSprite("TheOtherRoles.Resources.ZoomIn.png", 21f)
                : new ResourceSprite("TheOtherRoles.Resources.ZoomOut.png", 85f);
            HudManagerStartPatch.zoomOutButton.PositionOffset = zoomOutStatus ? new Vector3(-0.82f, 11.5f, 0) : new(0.4f, 2.35f, 0f);
        }

        // This will move button positions to the correct position.
        ResolutionManager.ResolutionChanged.Invoke((float)Screen.width / Screen.height, Screen.width, Screen.height, Screen.fullScreen);
    }

    private static long GetBuiltInTicks()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var builtin = assembly.GetType("Builtin");
        if (builtin == null) return 0;
        var field = builtin.GetField("CompileTime");
        if (field == null) return 0;
        var value = field.GetValue(null);
        return value == null ? 0 : (long)value;
    }

    public static object TryCast(this Il2CppObjectBase self, Type type)
    {
        return AccessTools.Method(self.GetType(), nameof(Il2CppObjectBase.TryCast)).MakeGenericMethod(type).Invoke(self, Array.Empty<object>());
    }

    /*public static void ModMurderPlayer(this PlayerControl player, PlayerControl target, bool resetKillCd = true)
    {
        player.isKilling = false;
        if (!target) return;
        Message(string.Format("{0} trying to murder {1}", player.PlayerId, target.PlayerId), null);
        GameData.PlayerInfo data = target.Data;
        if (player.AmOwner)
        {
            StatsManager.Instance.IncrementStat(StringNames.StatsImpostorKills);

            if (player.CurrentOutfitType == PlayerOutfitType.Shapeshifted)
            {
                StatsManager.Instance.IncrementStat(StringNames.StatsShapeshifterShiftedKills);
            }
            if (Constants.ShouldPlaySfx())
            {
                SoundManager.Instance.PlaySound(player.KillSfx, false, 0.8f, null);
            }
            if (resetKillCd) player.SetKillTimer(ModOption.KillCooldown);
        }
        DestroyableSingleton<UnityTelemetry>.Instance.WriteMurder();
        target.gameObject.layer = LayerMask.NameToLayer("Ghost");
        if (target.AmOwner)
        {
            StatsManager.Instance.IncrementStat(StringNames.StatsTimesMurdered);
            if (Minigame.Instance)
            {
                try
                {
                    Minigame.Instance.Close();
                }
                catch { }
            }
            DestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(player.Data, data);
            target.cosmetics.SetNameMask(false);
            target.RpcSetScanner(false);
        }

        player.MyPhysics.StartCoroutine(player.KillAnimations.Random().CoPerformKill(player, target));
        Message(string.Format("{0} succeeded in murdering {1}", player.PlayerId, target.PlayerId), null);
    }*/
}