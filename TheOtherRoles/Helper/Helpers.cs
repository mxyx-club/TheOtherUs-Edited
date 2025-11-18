using AmongUs.GameOptions;
using System.IO;
using TheOtherRoles.CustomCosmetics;
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
    Guesser,
    Anonymous
}

public static class Helpers
{
    public static bool zoomOutStatus;
    public static bool InGame => AmongUsClient.Instance != null && AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started;
    public static bool IsCountDown => GameStartManager.InstanceExists && GameStartManager.Instance.startState == GameStartManager.StartingStates.Countdown;
    public static bool InMeeting => InGame && MeetingHud.Instance;
    public static bool GameEnd => AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Ended;
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

    public static string teamString(PlayerControl player)
    {
        var killerTeam = "";
        if (player.IsNeutral()) killerTeam = "NeutralRolesText".Translate();
        else if (player.IsImpostor()) killerTeam = "ImpostorRolesText".Translate();
        else if (player.IsCrew()) killerTeam = "CrewmateRolesText".Translate();
        return killerTeam;
    }

    public static void NoCheckStartMeeting(this PlayerControl reporter, GameData.PlayerInfo target, bool force = false)
    {
        if (InMeeting) return;

        if (AmongUsClient.Instance.AmHost)
        {
            MeetingRoomManager.Instance.AssignSelf(reporter, target);
            DestroyableSingleton<HudManager>.Instance.OpenMeetingRoom(reporter);
            reporter.RpcStartMeeting(target);
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

    public static bool isCamoComms => isCommsActive && ModOption.camoComms && !InMeeting;

    public static bool isActiveCamoComms => isCamoComms || Camouflager.CamoTimer > 0f;

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
        foreach (var morphling in GetRoles(RoleId.Morphling).Cast<Morphling>())
        {
            if (morphling.morphTimer > 0f && morphling.Player != null && morphling.morphTarget != null)
            {
                var target = morphling.morphTarget;
                morphling.Player.setLook(target.Data.PlayerName, target.Data.DefaultOutfit.ColorId,
                    target.Data.DefaultOutfit.HatId, target.Data.DefaultOutfit.VisorId, target.Data.DefaultOutfit.SkinId,
                    target.Data.DefaultOutfit.PetId);
            }
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

    public static CustomGameModes SetNextGameMode()
    {

        ModOption.GameMode = (CustomGameModes)((int)(ModOption.GameMode + 1) % Enum.GetValues(typeof(CustomGameModes)).Length);
        MessageWriter writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.ShareGameMode);
        writer.Write((byte)ModOption.GameMode);
        writer.EndRPC();
        RPCProcedure.shareGameMode((byte)ModOption.GameMode);
        return ModOption.GameMode;
    }

    public static void AddUnique<T>(this ISystem.List<T> self, T item) where T : IDisconnectHandler
    {
        if (!self.Contains(item)) self.Add(item);
    }

    public static T GetRandom<T>(this T[] list)
    {
        var indexData = URandom.Range(0, list.Length);
        return list[indexData];
    }

    public static int GetRandom<T>(List<T> list)
    {
        var indexData = URandom.Range(0, list.Count);
        return indexData;
    }

    public static T Get<T>(this ISystem.List<T> list, int index)
    {
        return list._items[index];
    }

    public static T Get<T>(this ISystem.List<T> list, Index index)
    {
        return list._items[index];
    }

    public static void ForEach<T>(this Il2CppArrayBase<T> list, Action<T> func)
    {
        foreach (T obj in list) func(obj);
    }

    public static List<T> ToList<T>(this ISystem.List<T> list)
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

    public static T FirstOrDefault<T>(this ISystem.List<T> list, Func<T, bool> func)
    {
        foreach (T obj in list)
            if (func(obj))
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

    public static T Find<T>(this ISystem.List<T> data, Predicate<T> match)
    {
        return data.ToList().Find(match);
    }

    public static List<T> Shuffle<T>(this List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rnd.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
        return list;
    }

    public static T RandomAndRemove<T>(List<T> list)
    {
        if (list.Count == 0) return default;
        int index = rnd.Next(list.Count);
        T item = list[index];
        list.RemoveAt(index);
        return item;
    }

    public static T RandomOrEmpty<T>(List<T> list, T emptyValue, float emptyChance = 0.33f)
    {
        if (list.Count == 0) return default;
        if (rnd.NextSingle() < emptyChance)
            return emptyValue;

        int index = rnd.Next(list.Count);
        T item = list[index];
        list.RemoveAt(index);
        return item;
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

    public static int Count<T>(this ISystem.List<T> list, Func<T, bool> func = null)
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

    public static int ComputeConstantHash(this string str)
    {
        const long MulPrime = 467;
        const int SurPrime = 9670057;

        long val = 0;
        foreach (char c in str)
        {
            val *= MulPrime;
            val += c;
            val %= SurPrime;
        }
        return (int)(val % SurPrime);
    }

    public static string readTextFromFile(string path)
    {
        Stream stream = File.OpenRead(path);
        var textStreamReader = new StreamReader(stream);
        return textStreamReader.ReadToEnd();
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

    public static void refreshRoleDescription(PlayerControl player)
    {
        var info = player.GetRoleInfo();
        List<string> taskTexts = new();

        taskTexts.Add(getRoleString(info));

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

    public static void ModRevive(this PlayerControl target, bool cleanBody = true, bool setPos = true)
    {
        if (target == null) return;

        DeadBody[] array = UObject.FindObjectsOfType<DeadBody>();

        for (var i = 0; i < array.Length; i++)
        {
            if (GameData.Instance.GetPlayerById(array[i].ParentId).PlayerId == target.PlayerId)
            {
                if (setPos) target.NetTransform.RpcSnapTo(array[i].transform.position);
                if (cleanBody) UObject.Destroy(array[i].gameObject);
                break;
            }
        }

        target?.Revive();
    }

    internal static string getRoleString(RoleInfo roleInfo)
    {
        return Cs(roleInfo.Color, $"{roleInfo.Name}: {roleInfo.ShortDescription}");
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

    public static TextMeshPro getFirst(this TextMeshPro[] text)
    {
        if (text == null) return null;
        foreach (var self in text)
            if (self.text == "") return self;
        return text[0];
    }

    public static void ExtendMeetingTime(float time)
    {
        if (MeetingHud.Instance && MeetingHud.Instance.state is not MeetingHud.VoteStates.Discussion and not MeetingHud.VoteStates.Results)
        {
            var currentTime = MeetingHud.Instance.discussionTimer - ModOption.NormalOptions.DiscussionTime;
            MeetingHud.Instance.discussionTimer = Math.Max(currentTime, MeetingHud.Instance.discussionTimer - time);
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

    public static bool IsCustomServer()
    {
        if (FastDestroyableSingleton<ServerManager>.Instance == null) return false;
        StringNames n = FastDestroyableSingleton<ServerManager>.Instance.CurrentRegion.TranslateName;
        return n is not StringNames.ServerNA and not StringNames.ServerEU and not StringNames.ServerAS;
    }

    public static void SetTargetWithLight(this FollowerCamera camera, MonoBehaviour target)
    {
        camera.Target = target;
        PlayerControl.LocalPlayer.lightSource?.transform?.SetParent(target.transform, false);
        if (target != PlayerControl.LocalPlayer) PlayerControl.LocalPlayer.NetTransform.Halt();
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
        return numberOfLeftTasks <= 0 || !CustomOptionHolder.finishTasksBeforeHauntingOrZoomingOut.GetBool();
    }

    public static void clearAllTasks(this PlayerControl player)
    {
        if (player == null) return;
        foreach (var playerTask in player.myTasks.GetFastEnumerator())
        {
            playerTask.OnRemove();
            UObject.Destroy(playerTask.gameObject);
        }

        player.myTasks.Clear();

        if (player.Data != null && player.Data.Tasks != null)
            player.Data.Tasks.Clear();
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
        return string.Format("<color=#{0:X2}{1:X2}{2:X2}{3:X2}>{4}</color>", ToByte(c.r), ToByte(c.g), ToByte(c.b), ToByte(c.a), s);
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

    public static bool hidePlayerName(PlayerControl source, PlayerControl target)
    {
        var localPlayer = PlayerControl.LocalPlayer;
        if (Camouflager.CamoTimer > 0f || MushroomSabotageActive || isCamoComms)
            return true; // No names are visible
        if (SurveillanceMinigamePatch.nightVisionIsActive) return true;
        if (target.TryGetRole<Ninja>(out var ninja) && ninja.isInvisable) return true;
        if (target.TryGetRole<Jackal>(out var jackal) && jackal.IsInvisable) return true;
        if (target.TryGetRole<Swooper>(out var swooper) && swooper.IsInvisable) return true;
        if (ModOption.hideOutOfSightNametags && InGame && source.IsAlive() && !isFungle &&
            PhysicsHelpers.AnythingBetween(localPlayer.GetTruePosition(), target.GetTruePosition(), Constants.ShadowMask, false))
            return true;

        if (!ModOption.hidePlayerNames) return false; // All names are visible
        if (source == null || target == null) return true;
        if (source == target) return false; // Player sees his own name
        if (source.Data.Role.IsImpostor && (target.Data.Role.IsImpostor || target.Is(RoleId.Spy)))
            return false; // Members of team Impostors see the names of Impostors/Spies
        if (source.IsLover() && source.OtherLover())
            return false; // Members of team Lovers see the names of each other
        if ((source.Is(RoleId.Jackal) || source.Is(RoleId.Sidekick)) && (target.Is(RoleId.Jackal) || target.Is(RoleId.Sidekick)))
            return false; // Members of team Jackal see the names of each other
        if ((source.Is(RoleId.Pavlovsdogs) || source.Is(RoleId.Pavlovsowner)) && (target.Is(RoleId.Pavlovsdogs) || target.Is(RoleId.Pavlovsowner)))
            return false;
        if (Deputy.knowsSheriff && (source.Is(RoleId.Sheriff) || source.Is(RoleId.Deputy)) && (target.Is(RoleId.Sheriff) || target.Is(RoleId.Deputy)))
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
        //Chameleon.update(); // so that morphling and camo wont make the chameleons visible
    }

    public static void showFlash(Color color, float duration = 1f, string message = "", float alpha = 0.75f)
    {
        if (FastDestroyableSingleton<HudManager>.Instance == null ||
            FastDestroyableSingleton<HudManager>.Instance.FullScreen == null) return;
        if (Grenadier.controls.ToList().Any(x => x == PlayerControl.LocalPlayer)) return;
        FastDestroyableSingleton<HudManager>.Instance.FullScreen.gameObject.SetActive(true);
        FastDestroyableSingleton<HudManager>.Instance.FullScreen.enabled = true;
        // Message Text
        var messageText = UObject.Instantiate(FastDestroyableSingleton<HudManager>.Instance.KillButton.cooldownTimerText,
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

    public static void SetKillTimerUnchecked(this PlayerControl player, float time, float max = float.NegativeInfinity)
    {
        if (max == float.NegativeInfinity) max = time;

        player.killTimer = time;
        FastDestroyableSingleton<HudManager>.Instance.KillButton.SetCoolDown(time, max);
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
}