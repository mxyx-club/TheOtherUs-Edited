using AmongUs.GameOptions;
using System.IO;
using TheOtherRoles.Patches;

namespace TheOtherRoles.Helper;

public enum SabotageTypes
{
    Comms,
    O2,
    Reactor,
    OxyMask,
    Lights,
    None
}

public enum CustomGameModes
{
    Classic,
    Anonymous
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
    public static PlayerControl HostPlayer => GameData.Instance.GetHost().Object;

    public static SRandom rnd { get; } = new(Environment.TickCount);

    public static Sprite ZoomIn = new ResourceSprite("ZoomIn.png", 21f);
    public static Sprite ZoomOut = new ResourceSprite("ZoomOut.png", 85f);

    /// <summary>
    /// 强力船员判定
    /// </summary>
    public static bool PowerCrewAlive()
    {
        var powerCrewAlive = false;
        // This functions blocks the game from ending if specified crewmate roles are alive
        if (!CustomOptionHolder.blockGameEnd.GetBool()) return false;

        if (Sheriff.Player.Any(x => x.IsAlive())) powerCrewAlive = true;
        if (isRoleAlive(Sheriff.Deputy)) powerCrewAlive = true;
        if (isRoleAlive(Veteran.veteran)) powerCrewAlive = true;
        if (isRoleAlive(Mayor.mayor)) powerCrewAlive = true;
        //if (isRoleAlive(Swapper.swapper)) powerCrewAlive = true;
        if (isRoleAlive(Prosecutor.prosecutor)) powerCrewAlive = true;
        if (isRoleAlive(Vigilante.vigilante)) powerCrewAlive = true;

        return powerCrewAlive;
    }

    /// <summary>
    /// 红狼视野
    /// </summary>
    public static bool HasImpVision(GameData.PlayerInfo player)
    {
        return player.Role.IsImpostor
               || (Jackal.jackal.Any(p => p.PlayerId == player.PlayerId) && Jackal.hasImpostorVision)
               || (Jackal.Sidekick != null && Jackal.Sidekick.PlayerId == player.PlayerId && Jackal.hasImpostorVision)
               || (Pavlovsdogs.pavlovsowner != null && Pavlovsdogs.pavlovsowner.PlayerId == player.PlayerId && Pavlovsdogs.hasImpostorVision)
               || (Pavlovsdogs.pavlovsdogs.Any(p => p.PlayerId == player.PlayerId) && Pavlovsdogs.hasImpostorVision)
               || (Infected.Player.Any(p => p.PlayerId == player.PlayerId) && Infected.hasImpostorVision)
               || (Spy.spy != null && Spy.spy.PlayerId == player.PlayerId && Spy.hasImpostorVision)
               || (Juggernaut.juggernaut != null && Juggernaut.juggernaut.PlayerId == player.PlayerId && Juggernaut.hasImpostorVision)
               || (Jester.Player.Any(p => p.PlayerId == player.PlayerId) && Jester.hasImpostorVision)
               || (Thief.thief != null && Thief.thief.PlayerId == player.PlayerId && Thief.hasImpostorVision)
               || (Avenger.Player != null && Avenger.Player.PlayerId == player.PlayerId && Avenger.hasImpostorVision)
               || (Swooper.swooper != null && Swooper.swooper.PlayerId == player.PlayerId && Swooper.hasImpVision)
               || (Pelican.Player != null && Pelican.Player.PlayerId == player.PlayerId && Pelican.hasImpVision)
               || (SchrodingersCat.Player != null && SchrodingersCat.Player.PlayerId == player.PlayerId && SchrodingersCat.hasImpVision)
               || (Werewolf.werewolf != null && Werewolf.werewolf.PlayerId == player.PlayerId && Werewolf.hasImpostorVision);
    }

    public static string teamString(PlayerControl player)
    {
        var killerTeam = "";
        if (player.IsNeutral()) killerTeam = "NeutralRolesText".Translate();
        else if (player.IsImpostor()) killerTeam = "ImpostorRolesText".Translate();
        else if (player.IsCrew()) killerTeam = "CrewmateRolesText".Translate();
        return killerTeam;
    }

    public static PlayerControl SetTarget(IEnumerable<PlayerControl> ignoreList = null, bool onlyCrewmates = false,
        bool inVented = false, float distances = 0f, IEnumerable<PlayerControl> targetPlayers = null, PlayerControl sourcePlayer = null)
    {
        return PlayerControlFixedUpdatePatch.SetTarget(onlyCrewmates, inVented, ignoreList, sourcePlayer, targetPlayers, distances);
    }

    public static void SetPlayerOutline(PlayerControl target, Color color)
    {
        if (target == null || target.cosmetics?.currentBodySprite?.BodySprite == null) return;

        color = color.SetAlpha(Chameleon.visibility(target.PlayerId));

        target.cosmetics.currentBodySprite.BodySprite.material.SetFloat("_Outline", 1f);
        target.cosmetics.currentBodySprite.BodySprite.material.SetColor("_OutlineColor", color);
    }

    public static void SetTargetWithLight(this FollowerCamera camera, MonoBehaviour target)
    {
        Message("SetCam");
        camera.Target = target;
        PlayerControl.LocalPlayer.lightSource?.transform?.SetParent(target.transform, false);
        if (target != PlayerControl.LocalPlayer) PlayerControl.LocalPlayer.NetTransform.Halt();
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

    public static void ExtendMeetingTime(float time)
    {
        if (MeetingHud.Instance == null) return;
        if (!CustomOptionHolder.guessReVote.GetBool()) return;
        if (MeetingHud.Instance.state is not MeetingHud.VoteStates.Discussion and not MeetingHud.VoteStates.Results)
        {
            var maxTime = ModOption.NormalOptions.VotingTime - GetPenaltyVotingTime();
            var newTime = Mathf.Max(0f, MeetingHud.Instance.discussionTimer - time);

            MeetingHud.Instance.discussionTimer = Math.Min(newTime, maxTime);
        }
    }

    public static float GetPenaltyVotingTime()
    {
        if (GameOptionsManager.Instance.CurrentGameOptions.GameMode != GameModes.Normal || !CustomOptionHolder.playerDieReducedTime.GetBool())
        {
            return 0f;
        }
        float total = PlayerControl.AllPlayerControls.Count(static x => x.IsDead()) * CustomOptionHolder.playerDieReducedTime.GetFloat();

        float penalty = Mathf.Min(total, (float)(ModOption.NormalOptions.VotingTime - CustomOptionHolder.minMeetingTime.GetFloat()));

        return penalty;
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

#nullable enable

    public static PlayerControl? PlayerById(byte? id)
    {
        if (id == null) return null;
        foreach (var player in PlayerControl.AllPlayerControls.GetFastEnumerator())
            if (player.PlayerId == id) return player;
        return null;
    }

    public static PlayerControl? PlayerByName(string name)
    {
        if (name.IsNullOrWhiteSpace()) return null;
        foreach (var player in PlayerControl.AllPlayerControls.GetFastEnumerator())
            if (player?.Data?.PlayerName == name) return player;
        return null;
    }

    public static DeadBody[] AllDeadBodies()
    {
        return UObject.FindObjectsOfType<DeadBody>();
    }

    public static DeadBody? GetDeadBody(byte id)
    {
        return AllDeadBodies().FirstOrDefault((p) => p.ParentId == id);
    }

    public static DeadBody? GetDeadBody(PlayerControl player)
    {
        if (player == null) return null;
        return AllDeadBodies().FirstOrDefault((p) => p.ParentId == player.PlayerId);
    }

    public static DeadBody? GetDeadBody(Vector2 pos, float distance = 3f)
    {
        DeadBody? deadBody = null;
        float closestDistSqr = float.MaxValue;

        foreach (var collider in Physics2D.OverlapCircleAll(pos, Mathf.Sqrt(distance), Constants.PlayersOnlyMask))
        {
            if (!collider.CompareTag("DeadBody")) continue;

            var body = collider.GetComponent<DeadBody>();
            if (body == null) continue;

            var player = PlayerById(body.ParentId);
            if (player?.Data == null || !player.Data.IsDead || player.Data.Disconnected) continue;

            float distSqr = (body.TruePosition - pos).sqrMagnitude;
            if (distSqr < distance && distSqr < closestDistSqr)
            {
                deadBody = body;
                closestDistSqr = distSqr;
            }
        }

        if (deadBody == null) return null;
        return deadBody;
    }

    public static PlayerControl? GetDeadPlayer(Vector2 pos, float distance = 3f)
    {
        var db = GetDeadBody(pos, distance);
        if (db == null) return null;
        return PlayerById(db?.ParentId);
    }
#nullable disable
    public static bool Chance(this SRandom rnd, float rate = 50f)
    {
        var value = rnd.NextSingle() * 100.0;

        return value < rate;
    }

    public static T GetRandom<T>(this T[] list)
    {
        var indexData = URandom.Range(0, list.Length);
        return list[indexData];
    }

    public static void ForEach<T>(this Il2CppArrayBase<T> list, Action<T> func)
    {
        foreach (T obj in list) func(obj);
    }

    public static T FirstOrDefault<T>(this Il2CppArrayBase<T> list)
    {
        foreach (T obj in list)
            return obj;
        return default;
    }

    public static ISystem.List<T> ToIl2CppList<T>(this IEnumerable<T> list)
    {
        ISystem.List<T> newList = new(list.Count());
        foreach (T item in list)
        {
            newList.Add(item);
        }
        return newList;
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

    public static CustomGameModes SetNextGameMode()
    {

        ModOption.GameMode = (CustomGameModes)((int)(ModOption.GameMode + 1) % Enum.GetValues(typeof(CustomGameModes)).Length);
        MessageWriter writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.ShareGameMode);
        writer.Write((byte)ModOption.GameMode);
        writer.EndRPC();
        RPCProcedure.shareGameMode((byte)ModOption.GameMode);
        return ModOption.GameMode;
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

    public static bool IsSabotage(TaskTypes taskType)
    {
        return taskType is TaskTypes.FixLights or TaskTypes.RestoreOxy or TaskTypes.ResetReactor or TaskTypes.StopCharles or TaskTypes.ResetSeismic or TaskTypes.FixComms or TaskTypes.MushroomMixupSabotage;
    }
    public static bool IsSabotage(SystemTypes systemType)
    {
        return systemType is SystemTypes.Electrical or SystemTypes.LifeSupp or SystemTypes.Reactor or SystemTypes.HeliSabotage or SystemTypes.Laboratory or SystemTypes.Comms;
    }

    public static void FixingSabotage(TaskTypes taskType)
    {
        switch (taskType)
        {
            case TaskTypes.FixLights:
                var switchSystem = MapUtilities.Systems[SystemTypes.Electrical].CastFast<SwitchSystem>();
                switchSystem.ActualSwitches = switchSystem.ExpectedSwitches;
                break;
            case TaskTypes.RestoreOxy: // (Skeld, Mira)
                ShipStatus.Instance.RpcUpdateSystem(SystemTypes.LifeSupp, 0 | 64);
                ShipStatus.Instance.RpcUpdateSystem(SystemTypes.LifeSupp, 1 | 64);
                break;
            case TaskTypes.ResetReactor: // (Skeld, Mira, Fungle)
                ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Reactor, 16);
                break;
            case TaskTypes.StopCharles: // (Airship)
                ShipStatus.Instance.RpcUpdateSystem(SystemTypes.HeliSabotage, 0 | 16);
                ShipStatus.Instance.RpcUpdateSystem(SystemTypes.HeliSabotage, 1 | 16);
                break;
            case TaskTypes.ResetSeismic: // (Polus)
                ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Laboratory, 16);
                break;
            case TaskTypes.FixComms:
                ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Comms, 16 | 0);
                ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Comms, 16 | 1);
                break;
            case TaskTypes.MushroomMixupSabotage:
                ShipStatus.Instance.RpcUpdateSystem(SystemTypes.MushroomMixupSabotage, 16 | 1);
                break;
            default:
                Info($"リペア処理が異常な呼び出しを受けました。", "Repair Process");
                break;
        }

        if (SubmergedCompatibility.IsSubmerged && taskType == SubmergedCompatibility.RetrieveOxygenMask)
        {
            SubmergedCompatibility.RepairOxygen();
        }
    }
    public static void handleVampireBiteOnBodyReport()
    {
        // Murder the bitten player and reset bitten (regardless whether the kill was successful or not)
        if (Vampire.vampire.IsAlive() && Vampire.bitten != null)
        {
            if (Vampire.bitten.IsAlive()) RpcCustomMurderPlayer(Vampire.vampire, Vampire.bitten, false);
            var writer = StartRPC(CustomRPC.VampireSetBitten);
            writer.Write(byte.MaxValue);
            writer.EndRPC();
            RPCProcedure.vampireSetBitten(byte.MaxValue);
        }
    }

    public static void handleBomberExplodeOnBodyReport()
    {
        // Murder the bitten player and reset bitten (regardless whether the kill was successful or not)
        if (Bomber.bomber != null && Bomber.hasBombPlayer != null)
        {
            if (Bomber.hasBombPlayer.IsAlive()) RpcCustomMurderPlayer(Bomber.bomber, Bomber.hasBombPlayer, false);
            var writer = StartRPC(CustomRPC.GiveBomb);
            writer.Write(byte.MaxValue);
            writer.Write(false);
            writer.EndRPC();
            RPCProcedure.giveBomb(byte.MaxValue);
        }
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
            UObject.Destroy(t.gameObject);
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

    internal static string getRoleString(RoleInfo roleInfo)
    {
        return Cs(roleInfo.color, $"{roleInfo.Name}: {roleInfo.ShortDescription}");
    }

    public static bool isDark(byte playerId)
    {
        return playerId % 2 == 0;
    }

    public static bool IsLightColor(PlayerControl target)
    {
        if (!ModOption.randomLigherPlayer) return CustomColors.lighterColors.Contains(target.Data.DefaultOutfit.ColorId);
        return !isDark(target.PlayerId);
    }

    public static bool IsLightColor(Color color)
    {
        var max = Mathf.Max(color.r, color.g, color.b);
        var sum = color.r + color.g + color.b;
        return max > 0.8f || sum > 2.1f;
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
        if (!CanSeeGhostInfo || InMeeting) return false;
        var (playerCompleted, playerTotal) = TasksHandler.taskInfo(PlayerControl.LocalPlayer.Data);
        var numberOfLeftTasks = playerTotal - playerCompleted;
        return !CustomOptionHolder.finishTasksBeforeHauntingOrZoomingOut.GetBool() || (numberOfLeftTasks <= 0);
    }

    public static void shareGameVersion()
    {
        var writer = StartRPC(CustomRPC.VersionHandshake);
        writer.Write((byte)Main.version.Major);
        writer.Write((byte)Main.version.Minor);
        writer.Write((byte)Main.version.Build);
        writer.Write(AmongUsClient.Instance.AmHost ? GameStartManagerPatch.timer : -1f);
        writer.WritePacked(AmongUsClient.Instance.ClientId);
        writer.Write((byte)(Main.version.Revision < 0 ? 0xFF : Main.version.Revision));
        writer.Write(Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId.ToByteArray());
        writer.EndRPC();
        RPCProcedure.versionHandshake(Main.version.Major, Main.version.Minor, Main.version.Build, Main.version.Revision, Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId, AmongUsClient.Instance.ClientId);
    }

    public static void RpcRepairSystem(this ShipStatus shipStatus, SystemTypes systemType, byte amount)
    {
        shipStatus.RpcUpdateSystem(systemType, amount);
    }

    public static bool IsCN()
    {
        return (int)DataManager.Settings.Language.CurrentLanguage == 13;
    }

    public static string GithubUrl(this string url)
    {
        if (IsCN() && (url.Contains("github.com") || url.Contains("githubusercontent.com")) && !url.Contains("ghfast.top"))
        {
            return "https://ghfast.top/" + url;
        }
        return url;
    }

    public static bool IsCustomServer()
    {
        if (FastDestroyableSingleton<ServerManager>.Instance == null) return false;
        StringNames n = FastDestroyableSingleton<ServerManager>.Instance.CurrentRegion.TranslateName;
        return n is not StringNames.ServerNA and not StringNames.ServerEU and not StringNames.ServerAS;
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

    public static string Cs(Color c, string s)
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
        if (source.IsImpostor(AndCat: true) && target.IsImpostor(true, true))
            return false; // Members of team Impostors see the names of Impostors/Spies
        if (source.isLover() && Lovers.otherLover(target))
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

    public static void ShowNotification(string message)
    {
        var instance = FastDestroyableSingleton<HudManager>.Instance;
        if (instance?.Notifier != null && !string.IsNullOrEmpty(message))
        {
            instance.Notifier.AddItem(message);
        }
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
        if (HudManager.Instance == null || HudManager.Instance.FullScreen == null) return;
        if (Grenadier.controls.ToList().Any(x => x == PlayerControl.LocalPlayer)) return;
        HudManager.Instance.FullScreen.gameObject.SetActive(true);
        HudManager.Instance.FullScreen.enabled = true;
        // Message Text
        var messageText = UObject.Instantiate(HudManager.Instance.KillButton.cooldownTimerText, HudManager.Instance.transform);
        messageText.text = message;
        messageText.enableWordWrapping = false;
        messageText.transform.localScale = Vector3.one * 0.5f;
        messageText.transform.localPosition += new Vector3(0f, 2f, -69f);
        messageText.gameObject.SetActive(true);
        HudManager.Instance.StartCoroutine(Effects.Lerp(duration, new Action<float>(p =>
        {
            var renderer = HudManager.Instance.FullScreen;

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
            var fullscreen = HudManager.Instance.FullScreen;
            fullscreen.enabled = true;
            fullscreen.gameObject.active = true;
            fullscreen.color = color;
        }

        yield return new WaitForSeconds(waitfor);

        if (HudManager.InstanceExists && HudManager.Instance.FullScreen)
        {
            var fullscreen = HudManager.Instance.FullScreen;
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

    public static bool isRoleAlive(PlayerControl player)
    {
        if (Mimic.mimic != null && player == Mimic.mimic) return false;
        return player != null && player.IsAlive();
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
            HudManagerStartPatch.zoomOutButton.Sprite = zoomOutStatus ? ZoomIn : ZoomOut;
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
}