using BepInEx.Unity.IL2CPP.Utils.Collections;
using TheOtherRoles.Attributes;
using TheOtherRoles.Mode;
using static TheOtherRoles.Patches.RoleManagerSelectRolesPatch;

namespace TheOtherRoles.Modules;

[HarmonyPatch]
internal class RoleDraft
{
    public static bool isEnabled => CustomOptionHolder.isDraftMode.GetBool() &&
        (ModOption.GameMode is CustomGameModes.Classic or CustomGameModes.Anonymous);
    public static bool isRunning;

    public static List<byte> pickOrder = new();
    public static bool picked;
    public static float timer;
    private static List<Component> buttons = new();
    private static TextMeshPro feedText;
    public static List<byte> alreadyPicked = new();
    private static Dictionary<byte, byte> playerRoles = new();

    private static readonly SimpleTable _pickTable = new SimpleTable()
        .AddColumn(alignment: SimpleTable.Alignment.Right)
        .AddColumn(manualWidth: 20);

    private static Sprite CrewmatePick = new ResourceSprite("RoleDraft.Crewmate.png");
    private static Sprite ImpostorPick = new ResourceSprite("RoleDraft.Impostor.png");
    private static Sprite NeutralPick = new ResourceSprite("RoleDraft.Neutral.png");
    private static Sprite RandomPick = new ResourceSprite("RoleDraft.Random.png");

    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.ShowTeam))]
    private class ShowRolePatch
    {
        [HarmonyPostfix]
        public static void ShowTeamPostfix(IntroCutscene __instance, ref Il2CppSystem.Collections.IEnumerator __result)
        {
            if (!isEnabled) return;
            var newEnumerator = new PatchedEnumerator()
            {
                enumerator = __result.WrapToManaged(),
                Postfix = CoSelectRoles(__instance)
            };
            __result = newEnumerator.GetEnumerator().WrapToIl2Cpp();
        }
    }

    public static IEnumerator CoSelectRoles(IntroCutscene __instance)
    {
        isRunning = true;

        // SoundEffectsManager.play("draft",  volume: 1f, true, true);
        bool playedAlert = false;
        feedText = UObject.Instantiate(__instance.TeamTitle, __instance.transform);
        var aspectPosition = feedText.gameObject.AddComponent<AspectPosition>();
        aspectPosition.Alignment = AspectPosition.EdgeAlignments.LeftTop;
        aspectPosition.DistanceFromEdge = new Vector2(1.62f, 1.2f);
        aspectPosition.AdjustPosition();
        feedText.transform.localScale = new Vector3(0.6f, 0.6f, 1);
        feedText.transform.position += new Vector3(0f, 0.6f);
        feedText.text = GetString("RoleDraft.FeedText");
        feedText.alignment = TextAlignmentOptions.TopLeft;
        feedText.autoSizeTextContainer = true;
        feedText.fontSize = 3f;
        feedText.enableAutoSizing = false;
        __instance.TeamTitle.transform.localPosition = __instance.TeamTitle.transform.localPosition + new Vector3(1f, 0f);
        __instance.TeamTitle.text = "Currently Picking:";
        __instance.BackgroundBar.enabled = false;
        __instance.TeamTitle.transform.localScale = new Vector3(0.25f, 0.25f, 1f);
        __instance.TeamTitle.autoSizeTextContainer = true;
        __instance.TeamTitle.enableAutoSizing = false;
        __instance.TeamTitle.fontSize = 5;
        __instance.TeamTitle.alignment = TextAlignmentOptions.Top;
        __instance.ImpostorText.gameObject.SetActive(false);
        GameObject.Find("BackgroundLayer")?.SetActive(false);
        foreach (var player in UObject.FindObjectsOfType<PoolablePlayer>())
        {
            if (player.name.Contains("Dummy"))
            {
                player.gameObject.SetActive(false);
            }
        }
        __instance.FrontMost.gameObject.SetActive(false);

        if (AmongUsClient.Instance.AmHost)
        {
            sendPickOrder();
        }

        while (pickOrder.Count == 0)
        {
            yield return null;
        }

        var roleData = getRoleAssignmentData();
        roleData.crewSettings.Add((byte)RoleId.Sheriff, CustomOptionHolder.sheriffSpawnRate.GetSelection());
        if (CustomOptionHolder.sheriffSpawnRate.GetSelection() > 0)
            roleData.crewSettings.Add((byte)RoleId.Deputy, CustomOptionHolder.deputySpawnRate.GetSelection());

        while (pickOrder.Count > 0)
        {
            picked = false;
            timer = 0;
            float maxTimer = CustomOptionHolder.draftModeTimeToChoose.GetFloat();
            string playerText = "";
            while (timer < maxTimer || !picked)
            {
                if (pickOrder.Count == 0)
                    break;

                // wait for pick
                timer += Time.deltaTime;
                if (PlayerControl.LocalPlayer.PlayerId == pickOrder[0])
                {
                    if (!playedAlert)
                    {
                        playedAlert = true;
                        SoundManager.Instance.PlaySound(ShipStatus.Instance.SabotageSound, false, 1f, null);
                    }
                    // Animate beginning of choice, by changing background color
                    float min = 50 / 255f;
                    float max = 200 / 255f;
                    Color backGroundColor = new Color(min, min, min, 1);
                    if (timer < 1)
                    {
                        if (timer < 0.5f)
                        { // White flash                              
                            float p = timer / 0.5f;
                            float value = (float)Math.Pow(p, 2f) * max;
                            backGroundColor = new Color(value, value, value, 1f);
                        }
                        else
                        {
                            float p = (1 - timer) / 0.5f;
                            float value = ((float)Math.Pow(p, 2f) * max) + ((1 - (float)Math.Pow(p, 2f)) * min);
                            backGroundColor = new Color(value, value, value, 1);
                        }

                    }
                    HudManager.Instance.FullScreen.color = backGroundColor;
                    GameObject.Find("BackgroundLayer")?.SetActive(false);

                    // enable pick, wait for pick
                    Color youColor = timer - (int)timer > 0.5 ? Color.red : Color.yellow;
                    playerText = Cs(youColor, "RoleDraft.You".Translate());
                    // Available Roles:
                    List<RoleInfo> availableRoles = new();
                    foreach (RoleInfo roleInfo in RoleInfo.allRoleInfos)
                    {
                        // Handle role pairings that are blocked, e.g. Vampire Warlock, Cleaner Vulture etc.
                        var blocked = blockedRolePairings.Any(group =>
                            group.Contains(roleInfo.roleId) &&
                            group.Any(rid => alreadyPicked.Contains((byte)rid) && rid != roleInfo.roleId)
                            );

                        if (blocked)
                        {
                            roleData.crewSettings.Remove((byte)roleInfo.roleId);
                            roleData.impSettings.Remove((byte)roleInfo.roleId);
                            roleData.neutralSettings.Remove((byte)roleInfo.roleId);
                            roleData.killerNeutralSettings.Remove((byte)roleInfo.roleId);
                            //Message($"Blocked role: {roleInfo.Name} ({roleInfo.roleId})");
                            continue;
                        }

                        if (roleInfo.roleType is RoleType.Modifier or RoleType.Ghost or RoleType.Special) continue;
                        int impostorCount = PlayerControl.AllPlayerControls.ToList().Count(x => x.Data.Role.IsImpostor);
                        // Remove Impostor Roles
                        if (PlayerControl.LocalPlayer.IsImpostor() && roleInfo.roleType != RoleType.Impostor) continue;
                        if (!PlayerControl.LocalPlayer.IsImpostor() && roleInfo.roleType == RoleType.Impostor) continue;
                        if (roleInfo.roleId == RoleId.Poucher && Poucher.spawnModifier) continue;

                        // 跳过概率为0的职业
                        if (roleData.neutralSettings.ContainsKey((byte)roleInfo.roleId) && roleData.neutralSettings[(byte)roleInfo.roleId] == 0)
                            continue;
                        else if (roleData.killerNeutralSettings.ContainsKey((byte)roleInfo.roleId) && roleData.killerNeutralSettings[(byte)roleInfo.roleId] == 0)
                            continue;
                        else if (roleData.impSettings.ContainsKey((byte)roleInfo.roleId) && roleData.impSettings[(byte)roleInfo.roleId] == 0)
                            continue;
                        else if (roleData.crewSettings.ContainsKey((byte)roleInfo.roleId) && roleData.crewSettings[(byte)roleInfo.roleId] == 0)
                            continue;

                        bool isNeutral = roleData.neutralSettings.TryGetValue((byte)roleInfo.roleId, out var neutralRate);
                        bool isKillerNeutral = roleData.killerNeutralSettings.TryGetValue((byte)roleInfo.roleId, out var killerNeutralRate);
                        bool isCrewmate = roleData.crewSettings.TryGetValue((byte)roleInfo.roleId, out var crewRate);
                        bool isImpostor = roleData.impSettings.TryGetValue((byte)roleInfo.roleId, out _);

                        if (!isNeutral && !isKillerNeutral && !isCrewmate && !isImpostor) continue;

                        // 排除不应该直接分配的职业
                        if (roleInfo.roleId is RoleId.Sidekick or RoleId.Pavlovsdogs or RoleId.Pursuer) continue;
                        if (roleInfo.roleId is RoleId.Crewmate or RoleId.Impostor or RoleId.Spy) continue;
                        if (GuesserGM.Enabled && (roleInfo.roleId == RoleId.Vigilante)) continue;
                        if (alreadyPicked.Contains((byte)roleInfo.roleId)) continue;

                        int impsPicked = alreadyPicked.Count(x => RoleInfo.RoleInfoById[(RoleId)x].roleType == RoleType.Impostor);

                        // Handle forcing of 100% roles for impostors
                        if (PlayerControl.LocalPlayer.IsImpostor())
                        {
                            int impsMax = roleData.maxImpostorRoles;
                            int impsLeft = pickOrder.Count(x => PlayerById(x).IsImpostor());
                            int imps100 = roleData.impSettings.Count(x => x.Value == 10);
                            imps100 = Math.Min(imps100, impsMax);
                            int imps100Picked = alreadyPicked.Count(x => roleData.impSettings.GetValueSafe(x) == 10);
                            if (impsLeft <= (imps100 - imps100Picked))
                            {
                                if (!roleData.impSettings.TryGetValue((byte)roleInfo.roleId, out var rate) || rate != 10)
                                    continue;
                            }
                            else
                            {
                                if (impsPicked >= impsMax) continue;
                            }
                        }
                        // Player is no impostor! Handle forcing of 100% roles for crew and neutral
                        else
                        {
                            int neutralsMax = roleData.maxNeutralRoles;
                            int killerNeutralsMax = roleData.maxKillerNeutralRoles;
                            int crewmateMax = roleData.maxCrewmateRoles;

                            int neutralsPicked = alreadyPicked.Count(roleData.neutralSettings.ContainsKey);
                            int killerNeutralsPicked = alreadyPicked.Count(roleData.killerNeutralSettings.ContainsKey);
                            int crewPicked = alreadyPicked.Count(roleData.crewSettings.ContainsKey);

                            int neutrals100 = Math.Min(roleData.neutralSettings.Count(x => x.Value == 10), neutralsMax);
                            int killerNeutrals100 = Math.Min(roleData.killerNeutralSettings.Count(x => x.Value == 10), killerNeutralsMax);
                            int crew100 = Math.Min(roleData.crewSettings.Count(x => x.Value == 10), crewmateMax);

                            int neutrals100Picked = alreadyPicked.Count(x => roleData.neutralSettings.TryGetValue(x, out var r) && r == 10);
                            int killerNeutrals100Picked = alreadyPicked.Count(x => roleData.killerNeutralSettings.TryGetValue(x, out var r) && r == 10);
                            int crew100Picked = alreadyPicked.Count(x => roleData.crewSettings.TryGetValue(x, out var r) && r == 10);

                            if ((isNeutral && neutralsPicked >= neutralsMax) ||
                                (isKillerNeutral && killerNeutralsPicked >= killerNeutralsMax) ||
                                (isCrewmate && crewPicked >= crewmateMax))
                            {
                                continue;
                            }

                            if (isNeutral && roleData.neutralSettings.Count(x => x.Value == 10) > neutralsMax)
                            {
                                if ((neutrals100 - neutrals100Picked > 0) && (neutralRate != 10)) continue;
                            }
                            else if (isKillerNeutral && roleData.killerNeutralSettings.Count(x => x.Value == 10) > killerNeutralsMax)
                            {
                                if ((killerNeutrals100 - killerNeutrals100Picked > 0) && (killerNeutralRate != 10)) continue;
                            }
                            else if (isCrewmate && roleData.crewSettings.Count(x => x.Value == 10) > crewmateMax)
                            {
                                if ((crew100 - crew100Picked > 0) && (crewRate != 10)) continue;
                            }

                            if (isNeutral &&
                                roleData.neutralSettings.Count(x => x.Value == 10 && !alreadyPicked.Contains(x.Key)) >=
                                neutralsMax - alreadyPicked.Count(roleData.neutralSettings.ContainsKey))
                            {
                                if (neutralRate < 10) continue;
                            }

                            if (isKillerNeutral &&
                                roleData.killerNeutralSettings.Count(x => x.Value == 10 && !alreadyPicked.Contains(x.Key)) >=
                                killerNeutralsMax - alreadyPicked.Count(roleData.killerNeutralSettings.ContainsKey))
                            {
                                if (killerNeutralRate < 10) continue;
                            }

                            if (isCrewmate &&
                                roleData.crewSettings.Count(x => x.Value == 10 && !alreadyPicked.Contains(x.Key)) >=
                                crewmateMax - alreadyPicked.Count(roleData.crewSettings.ContainsKey))
                            {
                                if (crewRate < 10) continue;
                            }
                        }

                        availableRoles.TryAdd(roleInfo);
                    }

                    availableRoles = availableRoles.OrderBy(_ => Guid.NewGuid()).ToList();

                    // Fallback for if all roles are somehow removed. (This is only the case if there is a bug, hence print a warning
                    if (availableRoles.Count == 0)
                    {
                        availableRoles.TryAdd(PlayerControl.LocalPlayer.IsImpostor()
                            ? RoleInfo.impostor
                            : RoleInfo.crewmate);
                    }

                    var originalAvailable = availableRoles.ToList();

                    // remove some roles, so that you can't always get the same roles:
                    if (availableRoles.Count > CustomOptionHolder.draftModeAmountOfChoices.GetFloat())
                    {
                        int countToRemove = availableRoles.Count - (int)CustomOptionHolder.draftModeAmountOfChoices.GetFloat();
                        while (countToRemove-- > 0)
                        {
                            var toRemove = availableRoles.OrderBy(_ => Guid.NewGuid()).First();
                            availableRoles.Remove(toRemove);
                        }
                    }

                    if (timer >= maxTimer)
                    {
                        sendPick((byte)originalAvailable.OrderBy(_ => Guid.NewGuid()).First().roleId, SelectFlags.Random);
                    }

                    if (GameObject.Find("RoleButton") == null)
                    {
                        SoundEffectsManager.play("timemasterShield");

                        int totalButtons = availableRoles.Count + 1;
                        int buttonsPerRow = 4;
                        int lastRow = totalButtons / buttonsPerRow;
                        int buttonsInLastRow = totalButtons % buttonsPerRow;

                        for (var t = 0; t < totalButtons; t++)
                        {
                            float row = t / buttonsPerRow;
                            float col = t % buttonsPerRow;
                            if (buttonsInLastRow != 0 && row == lastRow)
                            {
                                col += (buttonsPerRow - buttonsInLastRow) / 2f;
                            }

                            // planned rows: maximum of 4, hence the following calculation for rows as well:
                            row += (4 - lastRow - 1) / 2f;

                            var pos = new Vector2(-8.4f + (col * 5.5f), -6.8f - (row * 5.33f));

                            var renderer = UnityHelper.CreateObject<SpriteRenderer>("RoleButton", __instance.TeamTitle.transform, pos);
                            renderer.transform.localScale = new Vector3(1.85f, 1.85f);
                            renderer.gameObject.AddComponent<BoxCollider2D>().size = new Vector2(1.7f, 2.5f);

                            GameObject textHolder = new GameObject("textHolder");
                            var text = textHolder.AddComponent<TextMeshPro>();
                            text.horizontalAlignment = HorizontalAlignmentOptions.Center;
                            text.fontSize = 3.5f;
                            text.fontStyle = FontStyles.Bold;
                            text.outlineWidth = 0.1f;
                            text.outlineColor = Color.black;
                            textHolder.layer = renderer.gameObject.layer;
                            textHolder.transform.SetParent(renderer.transform, false);

                            if (t == availableRoles.Count)
                            {
                                renderer.sprite = RandomPick;
                                text.text = GetString("RoleDraft.Random");
                                text.color = Color.green;
                                textHolder.transform.localPosition = new Vector3(0, text.text.Contains('\n') ? -1.975f : -3.16f, -1);

                                var button = renderer.gameObject.SetUpButton();
                                button.OnClick.AddListener(() =>
                                {
                                    sendPick((byte)originalAvailable.OrderBy(_ => Guid.NewGuid()).First().roleId, SelectFlags.Random);
                                });

                            }
                            else
                            {
                                var roleInfo = availableRoles[t];
                                renderer.sprite = roleInfo.roleType switch
                                {
                                    RoleType.Crewmate => CrewmatePick,
                                    RoleType.Impostor => ImpostorPick,
                                    RoleType.Neutral => NeutralPick,
                                    _ => RandomPick,
                                };

                                text.text = roleInfo.Name.Replace(" ", "\n");
                                text.color = roleInfo.color;
                                textHolder.transform.localPosition = new Vector3(0, text.text.Contains('\n') ? -1.975f : -3.16f, -1);

                                var button = renderer.gameObject.SetUpButton();
                                button.OnClick.AddListener(() =>
                                {
                                    sendPick((byte)roleInfo.roleId);
                                });
                            }
                            buttons.Add(renderer);
                        }
                    }
                }
                else
                {
                    int currentPick = PlayerControl.AllPlayerControls.Count - pickOrder.Count + 1;
                    playerText = string.Format(GetString("RoleDraft.PlayerText"), currentPick);
                    HudManager.Instance.FullScreen.color = Color.black;
                }

                __instance.TeamTitle.text = string.Format(GetString("RoleDraft.TeamTitle1"), playerText);
                int waitMore = pickOrder.IndexOf(PlayerControl.LocalPlayer.PlayerId);

                if (waitMore > 0)
                {
                    __instance.TeamTitle.text += string.Format(GetString("RoleDraft.TeamTitle2"), waitMore);
                }

                __instance.TeamTitle.text += string.Format(GetString("RoleDraft.TeamTitle3"), (int)(maxTimer + 1 - timer));
                yield return null;
            }
        }

        HudManager.Instance.FullScreen.color = Color.black;
        __instance.FrontMost.gameObject.SetActive(true);
        GameObject.Find("BackgroundLayer")?.SetActive(true);
        if (AmongUsClient.Instance.AmHost)
        {
            assignRoleTargets(null); // Assign targets for Lawyer & Prosecutor
            if (GuesserGM.Enabled) assignGuesserGamemode();
            assignModifiers(); // Assign modifier
        }

        float myTimer = 0f;
        while (myTimer < 3f)
        {
            myTimer += Time.deltaTime;
            Color c = new Color(0, 0, 0, myTimer / 3.0f);
            __instance.FrontMost.color = c;
            yield return null;
        }

        // SoundEffectsManager.stop("draft"); 
        isRunning = false;
        yield break;
    }

    public static void receivePick(byte playerId, byte roleId, byte flag = 0)
    {
        if (!isEnabled) return;
        RPCProcedure.setRole(playerId, roleId);
        alreadyPicked.Add(roleId);
        playerRoles.TryAdd(playerId, roleId);
        var isRandom = flag > 0;
        var reasons = ((SelectFlags)flag).ToString();

        try
        {
            pickOrder.Remove(playerId);
            timer = 0;
            picked = true;
            var roleInfo = RoleInfo.allRoleInfos?.First(x => (byte)x.roleId == roleId) ?? RoleInfo.crewmate;
            var isLocalPlayer = playerId == PlayerControl.LocalPlayer.PlayerId;
            var reasonString = isRandom ? $" ({"RoleDraft.Random".Translate()})" : "";
            var roleString = isLocalPlayer
                ? isRandom ? $"{Cs(roleInfo.color, roleInfo.Name + reasonString)}" : $"{Cs(roleInfo.color, roleInfo.Name)}"
                : BuildRoleString(roleInfo, isRandom, reasons);

            string prefix = playerId == PlayerControl.LocalPlayer.PlayerId ? "RoleDraft.You".Translate() : alreadyPicked.Count.ToString();

            _pickTable.AddRow(prefix, roleString);

            feedText.text = GetString("RoleDraft.FeedText") + _pickTable.ToString();

            SoundEffectsManager.play("select");
        }
        catch (Exception e) { Error(e); }

        static string BuildRoleString(RoleInfo roleInfo, bool isRandom, string reason)
        {
            if (isRandom) return Cs(Color.green, $"RoleDraft.{reason}".Translate());

            if (!CustomOptionHolder.draftModeShowRoles.GetBool())
                return "RoleDraft.UnknownRole".Translate();

            return roleInfo.roleType switch
            {
                RoleType.Impostor when CustomOptionHolder.draftModeHideImpRoles.GetBool() =>
                    Cs(Palette.ImpostorRed, "RoleDraft.ImpostorRole".Translate()),
                RoleType.Neutral when CustomOptionHolder.draftModeHideNeutralRoles.GetBool() =>
                    Cs(Color.white, "RoleDraft.NeutralRole".Translate()),
                RoleType.Crewmate when CustomOptionHolder.draftModeHideCrewmateRoles.GetBool() =>
                    Cs(Palette.CrewmateBlue, "RoleDraft.CrewmateRole".Translate()),
                _ => Cs(roleInfo.color, roleInfo.Name)
            };
        }
    }

    public static void sendPick(byte RoleId, SelectFlags flag = SelectFlags.Normal)
    {
        if (playerRoles.TryGetValue(PlayerControl.LocalPlayer.PlayerId, out _)) return;
        SoundEffectsManager.stop("timeMasterShield");
        var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.DraftModePick);
        writer.Write(PlayerControl.LocalPlayer.PlayerId);
        writer.Write(RoleId);
        writer.Write((byte)flag);
        writer.EndRPC();
        receivePick(PlayerControl.LocalPlayer.PlayerId, RoleId, (byte)flag);

        try
        {
            // destroy all the buttons:
            foreach (var button in buttons)
            {
                button?.gameObject?.Destroy();
            }
            buttons = new();
        }
        catch (Exception e) { Message(e); }
    }

    public static void sendPickOrder()
    {
        pickOrder = PlayerControl.AllPlayerControls.ToArray().Select(x => x.PlayerId).OrderBy(_ => Guid.NewGuid()).ToList().ToList();
        var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.DraftModePickOrder);
        writer.Write((byte)pickOrder.Count);
        foreach (var item in pickOrder)
        {
            writer.Write(item);
        }
        writer.EndRPC();
    }

    public static void receivePickOrder(int amount, MessageReader reader)
    {
        pickOrder.Clear();
        for (int i = 0; i < amount; i++)
        {
            pickOrder.Add(reader.ReadByte());
        }
    }

    [OnGameStart]
    public static void Clear()
    {

        isRunning = false;
        alreadyPicked = new();
        playerRoles = new();
        buttons = new();
        _pickTable.ClearRows();

    }

    private class PatchedEnumerator() : IEnumerable
    {
        public IEnumerator enumerator;
        public IEnumerator Postfix;
        public IEnumerator GetEnumerator()
        {
            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }
            while (Postfix.MoveNext())
                yield return Postfix.Current;
        }
    }

    public enum SelectFlags
    {
        Normal,
        Disconnect,
        Random
    }
}
