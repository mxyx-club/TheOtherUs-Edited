using BepInEx.Unity.IL2CPP.Utils.Collections;
using TheOtherRoles.Attributes;
using TheOtherRoles.Mode;
using static TheOtherRoles.Roles.RoleAssignmentPatch;

namespace TheOtherRoles.Modules;

[HarmonyPatch]
internal class RoleDraft
{
    public static bool isEnabled => CustomOptionHolder.isDraftMode.GetBool() && (ModOption.GameMode is CustomGameModes.Classic);
    public static bool isRunning;

    public static List<byte> pickOrder = new();
    public static bool picked;
    public static float timer;
    private static List<ActionButton> buttons = new();
    private static TextMeshPro feedText;
    public static List<byte> alreadyPicked = new();
    private static Dictionary<byte, byte> playerRoles = new();

    private static readonly SimpleTable _pickTable = new SimpleTable()
        .AddColumn(8, minWidth: 4, Alignment.Right)
        .AddColumn(minWidth: 9);

    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.ShowTeam))]
    private class ShowRolePatch
    {
        [HarmonyPostfix]
        public static void Postfix(IntroCutscene __instance, ref Il2CppSystem.Collections.IEnumerator __result)
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

        var roleData = RoleManagerSelectRolesPatch.SelectorRoles();
        var neutralSettings = roleData.neutralSettings.ToDictionary(pair => pair.Key, pair => pair.Value.rate);
        var killerNeutralSettings = roleData.killerNeutralSettings.ToDictionary(pair => pair.Key, pair => pair.Value.rate);
        var crewSettings = roleData.crewSettings.ToDictionary(pair => pair.Key, pair => pair.Value.rate);
        var impSettings = roleData.impSettings.ToDictionary(pair => pair.Key, pair => pair.Value.rate);
        /*roleData.crewSettings.Add((byte)RoleId.Sheriff, CustomOptionHolder.sheriffSpawnRate.GetSelection());
        if (CustomOptionHolder.sheriffSpawnRate.GetSelection() > 0)
            roleData.crewSettings.Add((byte)RoleId.Deputy, CustomOptionHolder.deputySpawnRate.GetSelection());*/

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
                    Color backGroundColor = new Color(min, min, min, 1);
                    if (timer < 1)
                    {
                        float max = 230 / 255f;
                        if (timer < 0.5f)
                        { // White flash                              
                            float p = timer / 0.5f;
                            float value = (float)Math.Pow(p, 2f) * max;
                            backGroundColor = new Color(value, value, value, 1);
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
                    foreach (RoleInfo roleInfo in RoleInfo.AllRoleInfo)
                    {
                        // Handle role pairings that are blocked, e.g. Vampire Warlock, Cleaner Vulture etc.
                        /*var blocked = blockedRolePairings.Any(p => alreadyPicked.Contains(p.Key) && p.Value.Contains((byte)roleInfo.RoleId));

                        if (blocked)
                        {
                            roleData.crewSettings.Remove((byte)roleInfo.RoleId);
                            roleData.impSettings.Remove((byte)roleInfo.RoleId);
                            roleData.neutralSettings.Remove((byte)roleInfo.RoleId);
                            roleData.killerNeutralSettings.Remove((byte)roleInfo.RoleId);
                            Message($"Blocked role: {roleInfo.Name} ({roleInfo.RoleId})");
                            continue;
                        }*/

                        if (roleInfo.RoleType is RoleType.Modifier or RoleType.Ghost or RoleType.Special) continue;
                        int impostorCount = PlayerControl.AllPlayerControls.ToList().Count(x => x.Data.Role.IsImpostor);
                        // Remove Impostor Roles
                        if (PlayerControl.LocalPlayer.IsImpostor() && roleInfo.RoleType != RoleType.Impostor) continue;
                        if (!PlayerControl.LocalPlayer.IsImpostor() && roleInfo.RoleType == RoleType.Impostor) continue;
                        if (roleInfo.RoleId == RoleId.Poucher && PoucherA.spawnModifier) continue;

                        // 跳过概率为0的职业
                        if (neutralSettings.ContainsKey((byte)roleInfo.RoleId) && neutralSettings[(byte)roleInfo.RoleId] == 0)
                            continue;
                        else if (killerNeutralSettings.ContainsKey((byte)roleInfo.RoleId) && killerNeutralSettings[(byte)roleInfo.RoleId] == 0)
                            continue;
                        else if (impSettings.ContainsKey((byte)roleInfo.RoleId) && impSettings[(byte)roleInfo.RoleId] == 0)
                            continue;
                        else if (crewSettings.ContainsKey((byte)roleInfo.RoleId) && crewSettings[(byte)roleInfo.RoleId] == 0)
                            continue;

                        bool isNeutral = neutralSettings.TryGetValue((byte)roleInfo.RoleId, out var neutralRate);
                        bool isKillerNeutral = killerNeutralSettings.TryGetValue((byte)roleInfo.RoleId, out var killerNeutralRate);
                        bool isCrewmate = crewSettings.TryGetValue((byte)roleInfo.RoleId, out var crewRate);
                        bool isImpostor = impSettings.TryGetValue((byte)roleInfo.RoleId, out _);

                        if (!isNeutral && !isKillerNeutral && !isCrewmate && !isImpostor) continue;

                        // 排除不应该直接分配的职业
                        if (roleInfo.RoleId is RoleId.Sidekick or RoleId.Pavlovsdogs or RoleId.Pursuer) continue;
                        if (roleInfo.RoleId is RoleId.Crewmate or RoleId.Impostor or RoleId.Spy) continue;
                        if (GuesserGM.Enabled && roleInfo.RoleId == RoleId.Vigilante) continue;
                        if (alreadyPicked.Contains((byte)roleInfo.RoleId)) continue;

                        int impsPicked = alreadyPicked.Count(x => CustomRoleManager.AllRolesInfo[(RoleId)x].RoleType == RoleType.Impostor);

                        // Handle forcing of 100% roles for impostors
                        if (PlayerControl.LocalPlayer.IsImpostor())
                        {
                            int impsMax = roleData.maxImpostorRoles;
                            int impsLeft = pickOrder.Count(x => PlayerById(x).IsImpostor());
                            int imps100 = impSettings.Count(x => x.Value == 10);
                            imps100 = Math.Min(imps100, impsMax);
                            int imps100Picked = alreadyPicked.Count(x => impSettings.GetValueSafe(x) == 10);
                            if (impsLeft <= (imps100 - imps100Picked))
                            {
                                if (!impSettings.TryGetValue((byte)roleInfo.RoleId, out var rate) || rate != 10)
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

                            int neutralsPicked = alreadyPicked.Count(neutralSettings.ContainsKey);
                            int killerNeutralsPicked = alreadyPicked.Count(killerNeutralSettings.ContainsKey);
                            int crewPicked = alreadyPicked.Count(crewSettings.ContainsKey);

                            int neutrals100 = Math.Min(neutralSettings.Count(x => x.Value == 10), neutralsMax);
                            int killerNeutrals100 = Math.Min(killerNeutralSettings.Count(x => x.Value == 10), killerNeutralsMax);
                            int crew100 = Math.Min(crewSettings.Count(x => x.Value == 10), crewmateMax);

                            int neutrals100Picked = alreadyPicked.Count(x => neutralSettings.TryGetValue(x, out var r) && r == 10);
                            int killerNeutrals100Picked = alreadyPicked.Count(x => killerNeutralSettings.TryGetValue(x, out var r) && r == 10);
                            int crew100Picked = alreadyPicked.Count(x => crewSettings.TryGetValue(x, out var r) && r == 10);

                            if ((isNeutral && neutralsPicked >= neutralsMax) ||
                                (isKillerNeutral && killerNeutralsPicked >= killerNeutralsMax) ||
                                (isCrewmate && crewPicked >= crewmateMax))
                            {
                                continue;
                            }

                            if (isNeutral && neutralSettings.Count(x => x.Value == 10) > neutralsMax)
                            {
                                if ((neutrals100 - neutrals100Picked > 0) && (neutralRate != 10)) continue;
                            }
                            else if (isKillerNeutral && killerNeutralSettings.Count(x => x.Value == 10) > killerNeutralsMax)
                            {
                                if ((killerNeutrals100 - killerNeutrals100Picked > 0) && (killerNeutralRate != 10)) continue;
                            }
                            else if (isCrewmate && crewSettings.Count(x => x.Value == 10) > crewmateMax)
                            {
                                if ((crew100 - crew100Picked > 0) && (crewRate != 10)) continue;
                            }

                            if (isNeutral &&
                                neutralSettings.Count(x => x.Value == 10 && !alreadyPicked.Contains(x.Key)) >=
                                neutralsMax - alreadyPicked.Count(roleData.neutralSettings.ContainsKey))
                            {
                                if (neutralRate < 10) continue;
                            }

                            if (isKillerNeutral &&
                                killerNeutralSettings.Count(x => x.Value == 10 && !alreadyPicked.Contains(x.Key)) >=
                                killerNeutralsMax - alreadyPicked.Count(roleData.killerNeutralSettings.ContainsKey))
                            {
                                if (killerNeutralRate < 10) continue;
                            }

                            if (isCrewmate &&
                                crewSettings.Count(x => x.Value == 10 && !alreadyPicked.Contains(x.Key)) >=
                                crewmateMax - alreadyPicked.Count(roleData.crewSettings.ContainsKey))
                            {
                                if (crewRate < 10) continue;
                            }
                        }
                        availableRoles.TryAdd(roleInfo);
                    }
                    Message($"availableRoles: {availableRoles.Count}");


                    availableRoles = availableRoles.OrderBy(_ => Guid.NewGuid()).ToList();

                    //Message($"availableRoles :  {availableRoles.Count}");
                    // Fallback for if all roles are somehow removed. (This is only the case if there is a bug, hence print a warning
                    if (availableRoles.Count == 0)
                    {
                        availableRoles.TryAdd(PlayerControl.LocalPlayer.IsImpostor()
                            ? Roles.Vanilla.Impostor.roleInfo
                            : Roles.Vanilla.Crewmate.roleInfo);
                    }

                    List<RoleInfo> originalAvailable = new(availableRoles);

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
                        sendPick((byte)originalAvailable.OrderBy(_ => Guid.NewGuid()).First().RoleId, SelectFlags.Random);
                    }

                    if (GameObject.Find("RoleButton") == null)
                    {
                        SoundEffectsManager.play("timemasterShield");
                        int i = 0;
                        int buttonsPerRow = 4;
                        int lastRow = availableRoles.Count / buttonsPerRow;
                        int buttonsInLastRow = availableRoles.Count % buttonsPerRow;

                        foreach (RoleInfo roleInfo in availableRoles)
                        {
                            float row = i / buttonsPerRow;
                            float col = i % buttonsPerRow;
                            if (buttonsInLastRow != 0 && row == lastRow)
                            {
                                col += (buttonsPerRow - buttonsInLastRow) / 2f;
                            }
                            // planned rows: maximum of 4, hence the following calculation for rows as well:
                            row += (4 - lastRow - 1) / 2f;

                            ActionButton actionButton = UObject.Instantiate(HudManager.Instance.KillButton, __instance.TeamTitle.transform);
                            actionButton.gameObject.SetActive(true);
                            actionButton.gameObject.name = "RoleButton";
                            actionButton.transform.localPosition = new Vector3(-8.4f + (col * 5.5f), -10 - (row * 3f));
                            actionButton.transform.localScale = new Vector3(2f, 2f);
                            actionButton.SetCoolDown(0, 0);
                            GameObject textHolder = new GameObject("textHolder");
                            var text = textHolder.AddComponent<TextMeshPro>();
                            text.text = roleInfo.Name.Replace(" ", "\n");
                            text.horizontalAlignment = HorizontalAlignmentOptions.Center;
                            text.fontSize = 5;
                            textHolder.layer = actionButton.gameObject.layer;
                            text.outlineWidth = 0.1f;
                            text.outlineColor = Color.white;
                            text.color = roleInfo.Color;
                            textHolder.transform.SetParent(actionButton.transform, false);
                            textHolder.transform.localPosition = new Vector3(0, text.text.Contains('\n') ? -1.975f : -2.2f, -1);
                            actionButton.graphic.sprite = new ResourceSprite("TheOtherRoles.Resources.RoleDraft.Random.png");
                            SpriteRenderer actionButtonRenderer = actionButton.graphic;
                            actionButtonRenderer.enabled = true;
                            GameObject actionButtonGameObject = actionButton.gameObject;
                            Material actionButtonMat = actionButtonRenderer.material;

                            PassiveButton button = actionButton.GetComponent<PassiveButton>();
                            button.OnClick = new();
                            button.OnClick.AddListener((Action)(() =>
                            {
                                sendPick((byte)roleInfo.RoleId);
                            }));
                            HudManager.Instance.StartCoroutine(Effects.Lerp(0.5f, new Action<float>((p) =>
                            {
                                actionButton.OverrideText("");
                            })));
                            buttons.Add(actionButton);
                            i++;
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
            //assignRoleTargets(null); // Assign targets for Lawyer & Prosecutor
            //if (GuesserGM.Enabled) assignGuesserGamemode();
            //assignModifiers(); // Assign modifier
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
        playerRoles.Add(playerId, roleId);
        var isRandom = flag > 0;
        var reasons = ((SelectFlags)flag).ToString();

        try
        {
            pickOrder.Remove(playerId);
            timer = 0;
            picked = true;
            var roleInfo = RoleInfo.AllRoleInfo?.First(x => (byte)x.RoleId == roleId) ?? Roles.Vanilla.Crewmate.roleInfo;
            var isLocalPlayer = playerId == PlayerControl.LocalPlayer.PlayerId;
            var reasonString = isRandom ? $" ({"RoleDraft.Random".Translate()})" : "";
            var roleString = isLocalPlayer
                ? isRandom ? $"{Cs(roleInfo.Color, roleInfo.Name + reasonString)}" : $"{Cs(roleInfo.Color, roleInfo.Name)}"
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

            return roleInfo.RoleType switch
            {
                RoleType.Impostor when CustomOptionHolder.draftModeHideImpRoles.GetBool() =>
                    Cs(Palette.ImpostorRed, "RoleDraft.ImpostorRole".Translate()),
                RoleType.Neutral when CustomOptionHolder.draftModeHideNeutralRoles.GetBool() =>
                    Cs(Color.white, "RoleDraft.NeutralRole".Translate()),
                RoleType.Crewmate when CustomOptionHolder.draftModeHideCrewmateRoles.GetBool() =>
                    Cs(Palette.CrewmateBlue, "RoleDraft.CrewmateRole".Translate()),
                _ => Cs(roleInfo.Color, roleInfo.Name)
            };
        }
    }

    public static void sendPick(byte RoleId, SelectFlags flag = SelectFlags.Normal)
    {
        SoundEffectsManager.stop("timeMasterShield");
        if (playerRoles.TryGetValue(PlayerControl.LocalPlayer.PlayerId, out _)) return;
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
                UObject.Destroy(button?.gameObject);
                //if (button?.gameObject != null) button.gameObject?.Destroy();
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
