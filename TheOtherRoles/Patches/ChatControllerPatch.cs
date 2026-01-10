namespace TheOtherRoles.Patches;

[HarmonyPatch]
public static class ChatControllerPatch
{
    public enum ChatTypes
    {
        Default = 0,
        HostChat,
        LoverChat,
        JailorChat,
        ImpostorChat,
        JackalChat,
        PavlovsChat,
        InfectedChat,
        GuesserMessage,
    }

    public enum ChannelType
    {
        Default = 0,
        HostAll,
        Impostor,
        Lover,
        Jailor,
        Jackal,
        Pavlovs,
        Infected
    }

    public static ChatTypes CurrentChatType = ChatTypes.Default;
    public static List<ChannelType> ActiveChannels = new() { ChannelType.Default };
    public static ChannelType CurrentChannel
    {
        get;
        set
        {
            field = value;
            if (!ActiveChannels.Contains(value) && value != ChannelType.HostAll)
            {
                field = ChannelType.Default;
            }
        }
    } = ChannelType.Default;

    public static List<string> SentHistory = new();
    public static int CurrentHistorySelection = -1;

    public static bool CanEnableChat()
    {
        if (ModOption.DebugMode) return true;
        if (AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay) return true;
        if (ModOption.ImpostorChatChannel >= 2 && PlayerControl.LocalPlayer.IsImpostor()) return true;
        if (ModOption.JackalChatChannel >= 2 && (PlayerControl.LocalPlayer == Jackal.Sidekick || Jackal.jackal.Any(x => x == PlayerControl.LocalPlayer))) return true;
        if (ModOption.PavlovsChatChannel >= 2 && (PlayerControl.LocalPlayer == Pavlovsdogs.pavlovsowner || Pavlovsdogs.pavlovsdogs.Any(x => x == PlayerControl.LocalPlayer))) return true;
        if (ModOption.InfectedChatChannel >= 2 && Infected.Player.Any(x => x == PlayerControl.LocalPlayer)) return true;
        if (ModOption.LoverChatChannel >= 2 && PlayerControl.LocalPlayer.isLover()) return true;
        return false;
    }

    [HarmonyPatch(typeof(ChatController), nameof(ChatController.SendChat))]
    private static class SendChatPatch
    {
        private static bool Prefix(ChatController __instance)
        {
            var text = __instance.freeChatField.textArea.text;
            var handled = ChatCommandRegistry.TryHandle(text, PlayerControl.LocalPlayer, __instance);

            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            __instance.timeSinceLastMessage = 5f;

            if (SentHistory.Count == 0 || SentHistory[^1] != text) SentHistory.Add(text);
            CurrentHistorySelection = SentHistory.Count;
            Info(text, "SendChat");

            if (!handled && CurrentChannel != ChannelType.Default)
            {
                switch (CurrentChannel)
                {
                    case ChannelType.HostAll:
                        {
                            var writer = StartRPC(CustomRPC.HostControl);
                            writer.Write(PlayerControl.LocalPlayer.PlayerId);
                            writer.Write((byte)RPCProcedure.HostCommand.HostSay);
                            writer.Write(text);
                            writer.EndRPC();
                            CurrentChatType = ChatTypes.HostChat;
                            __instance.AddChat(GetHostPlayer, text);
                        }
                        break;
                    case ChannelType.Impostor:
                        {
                            var writer = StartRPC(CustomRPC.SendChatToChannel);
                            writer.Write(PlayerControl.LocalPlayer.PlayerId);
                            writer.Write((byte)ChannelType.Impostor);
                            writer.Write(text);
                            writer.EndRPC();
                            RPCProcedure.sendChatToChannel(PlayerControl.LocalPlayer, ChannelType.Impostor, text);
                        }
                        break;
                    case ChannelType.Lover:
                        {
                            var writer = StartRPC(CustomRPC.SendChatToChannel);
                            writer.Write(PlayerControl.LocalPlayer.PlayerId);
                            writer.Write((byte)ChannelType.Lover);
                            writer.Write(text);
                            writer.EndRPC();
                            RPCProcedure.sendChatToChannel(PlayerControl.LocalPlayer, ChannelType.Lover, text);
                        }
                        break;
                    case ChannelType.Jailor:
                        {
                            var writer = StartRPC(CustomRPC.JailorSendMessage);
                            writer.Write(Jailor.Player.PlayerId);
                            writer.Write(text);
                            writer.EndRPC();
                            Jailor.JailorSendMessage(Jailor.Player, text);
                        }
                        break;
                    case ChannelType.Jackal:
                        {
                            var writer = StartRPC(CustomRPC.SendChatToChannel);
                            writer.Write(PlayerControl.LocalPlayer.PlayerId);
                            writer.Write((byte)ChannelType.Jackal);
                            writer.Write(text);
                            writer.EndRPC();
                            RPCProcedure.sendChatToChannel(PlayerControl.LocalPlayer, ChannelType.Jackal, text);
                        }
                        break;
                    case ChannelType.Pavlovs:
                        {
                            var writer = StartRPC(CustomRPC.SendChatToChannel);
                            writer.Write(PlayerControl.LocalPlayer.PlayerId);
                            writer.Write((byte)ChannelType.Pavlovs);
                            writer.Write(text);
                            writer.EndRPC();
                            RPCProcedure.sendChatToChannel(PlayerControl.LocalPlayer, ChannelType.Pavlovs, text);
                        }
                        break;
                    case ChannelType.Infected:
                        {
                            var writer = StartRPC(CustomRPC.SendChatToChannel);
                            writer.Write(PlayerControl.LocalPlayer.PlayerId);
                            writer.Write((byte)ChannelType.Infected);
                            writer.Write(text);
                            writer.EndRPC();
                            RPCProcedure.sendChatToChannel(PlayerControl.LocalPlayer, ChannelType.Infected, text);
                        }
                        break;
                    case ChannelType.Default:
                        break;
                }
                __instance.freeChatField.textArea.Clear();
                return false;
            }

            if (handled)
            {
                __instance.freeChatField.Clear();
                __instance.quickChatMenu.Clear();
            }

            return !handled;
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class EnableChat
    {
        public static bool ForceEnableChat;
        public static void Postfix(HudManager __instance)
        {
            if (!__instance.Chat.isActiveAndEnabled && (ModOption.DebugMode
                    || AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay
                    || ForceEnableChat
                    || CanEnableChat()))
                __instance.Chat.SetVisible(true);

            if (!InMeeting && !ModOption.DebugMode && Specter.Player != null && PlayerControl.LocalPlayer == Specter.Player)
                __instance.Chat?.SetVisible(false);

        }
    }

    [HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetName))]
    public static class SetBubbleName
    {
        private static void Postfix(ChatBubble __instance, [HarmonyArgument(0)] string playerName)
        {
            var sourcePlayer = PlayerByName(playerName);

            if (__instance != null && PlayerControl.LocalPlayer.IsImpostor(false, true) && sourcePlayer.IsImpostor(true, true))
            {
                __instance.NameText.color = Palette.ImpostorRed;
            }

            if (InMeeting && Jailor.Player.IsAlive() && Jailor.Jailed.IsAlive())
            {
                if (PlayerControl.LocalPlayer == Jailor.Player && Jailor.Jailed == PlayerByName(playerName))
                {
                    __instance.NameText.color = Jailor.color;
                    __instance.NameText.text = playerName + GetString("Jailor.InJailSuffix");
                }

                if ((PlayerControl.LocalPlayer == Jailor.Jailed || CanSeeGhostInfo) && Jailor.Jailed == PlayerByName(playerName))
                {
                    __instance.NameText.color = Jailor.color;
                    __instance.NameText.text = playerName + GetString("Jailor.InJailSuffix");
                }

            }

            var currentType = CurrentChatType;
            CurrentChatType = ChatTypes.Default;

            switch (currentType)
            {
                case ChatTypes.HostChat:
                    __instance.NameText.color = Palette.Purple;
                    __instance.NameText.text = $"{GameData.Instance?.GetHost()?.PlayerName ?? ""} {"MessageFromTheHost".Translate()}";
                    break;
                case ChatTypes.JailorChat:
                    if (InMeeting && Jailor.Player.IsAlive() && Jailor.Jailed.IsAlive())
                    {
                        if (PlayerControl.LocalPlayer == Jailor.Jailed || CanSeeGhostInfo)
                        {
                            __instance.NameText.color = Jailor.color;
                            __instance.NameText.text = $"({GetString("Jailor")})";
                        }
                        else if (PlayerControl.LocalPlayer == Jailor.Player || CanSeeGhostInfo)
                        {
                            __instance.NameText.color = Jailor.color;
                            __instance.NameText.text = $"({GetString("Jailor")})";
                        }
                    }
                    break;
                case ChatTypes.Default:
                    break;
                case ChatTypes.LoverChat:
                    __instance.NameText.color = Lovers.color;
                    __instance.NameText.text = $"{__instance.NameText.text} {"MessageFromTheLover".Translate()}";
                    break;
                case ChatTypes.GuesserMessage:
                    __instance.NameText.color = Color.yellow;
                    __instance.NameText.text = "MessageFromTheGuesser".Translate();
                    break;
                case ChatTypes.ImpostorChat:
                    __instance.NameText.color = Palette.ImpostorRed;
                    __instance.NameText.text = $"{__instance.NameText.text} {"MessageFromTheImpostor".Translate()}";
                    break;
                case ChatTypes.JackalChat:
                    __instance.NameText.color = Jackal.color;
                    __instance.NameText.text = $"{__instance.NameText.text} {"MessageFromTheJackal".Translate()}";
                    break;
                case ChatTypes.PavlovsChat:
                    __instance.NameText.color = Pavlovsdogs.color;
                    __instance.NameText.text = $"{__instance.NameText.text} {"MessageFromThePavlovs".Translate()}";
                    break;
                case ChatTypes.InfectedChat:
                    __instance.NameText.color = Infected.color;
                    __instance.NameText.text = $"{__instance.NameText.text} {"MessageFromTheInfected".Translate()}";
                    break;
                default:
                    break;
            }

        }
    }

    [HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))] //test
    private static class AddChatPatch
    {
        private static bool Prefix(ChatController __instance, [HarmonyArgument(0)] PlayerControl sourcePlayer, [HarmonyArgument(1)] string chatText, ref bool __state)
        {
            var local = PlayerControl.LocalPlayer;
            if (sourcePlayer == local) return true;

            var flag = MeetingHud.Instance
                    || LobbyBehaviour.Instance
                    || CanSeeGhostInfo
                    || CurrentChatType != ChatTypes.Default
                    || ModOption.DebugMode;

            __state = flag;

            if (__instance != FastDestroyableSingleton<HudManager>.Instance.Chat)
                return true;
            if (sourcePlayer == Blackmailer.blackmailed && Blackmailer.Player.IsAlive() && Blackmailer.blackmailed.IsAlive())
            { __state = false; return false; }
            if (sourcePlayer == Jailor.Jailed && Jailor.Jailed.IsAlive() && Jailor.Player.IsAlive() && local != Jailor.Player && !CanSeeGhostInfo)
            { __state = false; return false; }

            return flag;
        }

        private static void Postfix(ChatController __instance, [HarmonyArgument(0)] PlayerControl sourcePlayer, [HarmonyArgument(1)] string chatText, bool __state)
        {
            if (!__state) return;
            if (sourcePlayer.IsDead() || sourcePlayer == PlayerControl.LocalPlayer || !InMeeting) return;
            try
            {
                Message($"{sourcePlayer?.Data?.PlayerName ?? "NULL"} : {chatText}", "AddChat");
                var local = PlayerControl.LocalPlayer;
                var targetId = sourcePlayer.PlayerId;
                if (MeetingHud.Instance.state is MeetingHud.VoteStates.Proceeding) return;
                var pva = MeetingHud.Instance?.playerStates?.FirstOrDefault(x => x.TargetPlayerId == targetId);
                if (pva == null) return;
                var rend = new GameObject().AddComponent<SpriteRenderer>();
                rend.transform.SetParent(pva.transform);
                rend.gameObject.layer = pva.Megaphone.gameObject.layer;
                rend.transform.localPosition = new Vector3(-0.5f, 0.2f, -1f);
                rend.sprite = new ResourceSprite("ChatOverlay.png", 130f);
                rend?.gameObject?.SetActive(true);

                _ = new LateTask(() =>
                {
                    rend?.gameObject?.SetActive(false);
                    rend?.gameObject?.Destroy();
                }, 3f);
            }
            catch
            {
                Message("Chat Notification Overlay is Detected");
            }
        }

    }

    [HarmonyPatch(typeof(ChatController))]
    public static class ChatControllerAwakePatch
    {
        [HarmonyPatch(typeof(ChatController), nameof(ChatController.Update)), HarmonyPrefix]
        public static void Update_Prefix(ChatController __instance)
        {
            DataManager.Settings.Multiplayer.ChatMode = QuickChatModes.FreeChatOrQuickChat;

            __instance.timeSinceLastMessage = 5f;
        }

        [HarmonyPatch(typeof(ChatController), nameof(ChatController.Update)), HarmonyPostfix]
        public static void Update_Postfix(ChatController __instance)
        {
            __instance.freeChatField.textArea.AllowPaste = true;
            __instance.chatBubblePool.Prefab.Cast<ChatBubble>().TextArea.overrideColorTags = false;

            if (Input.GetKeyDown(ModInputManager.toggleChat.keyCode))
            {
                if (!__instance.isActiveAndEnabled) return;
                __instance.Toggle();
            }
            if (__instance.IsOpenOrOpening)
                __instance.banButton.MenuButton.enabled = !__instance.IsAnimating;
        }
    }


    [HarmonyPatch]
    public class ChannelPatch
    {
        public static GameObject ChannelShower;
        [HarmonyPatch(typeof(ChatController), nameof(ChatController.Awake)), HarmonyPostfix]
        public static void ChatControllerAwake_Postfix(ChatController __instance)
        {
            __instance.freeChatField.textArea.SetText("");
            __instance.timeSinceLastMessage = 0;
            if (ChannelShower != null) return;
            ChannelShower = UObject.Instantiate(__instance.freeChatField.charCountText.gameObject, __instance.freeChatField.charCountText.transform.parent);
            ChannelShower.name = "Channel Shower";
            ChannelShower.transform.localPosition = new Vector3(1.48f, 0.5f, 0f);
            ChannelShower.transform.localScale = new Vector3(0.9f, 1f);
            ChannelShower.GetComponent<RectTransform>().sizeDelta = new Vector2(5f, 0.1f);
            var tmp = ChannelShower.GetComponent<TextMeshPro>();
            tmp.alignment = TextAlignmentOptions.Left;
            tmp.color = Color.white;
            tmp.outlineColor = Color.black;
            tmp.fontStyle = FontStyles.Bold;
            tmp.outlineWidth = 0.15f;
            tmp.fontSize *= 1.45f;
            CurrentChannel = CurrentChannel; // Check Channel
            Message($"当前频道: {CurrentChannel}");
        }

        [HarmonyPatch(typeof(ChatController), nameof(ChatController.Update)), HarmonyPostfix]
        public static void ChatControllerUpdate_Postfix(ChatController __instance)
        {
            UpdateChatChannels();
            KeyboardInput(__instance);
            if (ChannelShower == null) return;
            try
            {
                var color = CurrentChannel switch
                {
                    ChannelType.Lover => Lovers.color,
                    ChannelType.Jailor => Jailor.color,
                    ChannelType.Impostor => Palette.ImpostorRed,
                    ChannelType.Jackal => Jackal.color,
                    ChannelType.Pavlovs => Pavlovsdogs.color,
                    ChannelType.Infected => Infected.color,
                    _ => Color.white
                };
                string text = string.Format(GetString("ChatChannel.Text"), Cs(color, GetString($"ChatChannel.{CurrentChannel}")));

                if (PlayerControl.LocalPlayer == Jailor.Jailed) text = GetString("ChatChannel.Jailor");
                text += $"{string.Format(GetString("ChannelSwitchNotice"), ModInputManager.nextChatChannel.keyCode.ToString())}";
                ChannelShower?.GetComponent<TextMeshPro>().SetText(text);
                ChannelShower?.SetActive(!ChannelShower.transform.parent.parent.FindChild("RateMessage (TMP)").gameObject.activeSelf);
            }
            catch { }
        }

        public static void UpdateChatChannels()
        {
            var channelConditions = new Dictionary<ChannelType, Func<PlayerControl, bool>>
            {
                [ChannelType.Default] = (x) => !InGame || InMeeting || CanSeeGhostInfo || ModOption.DebugMode,
                //[ChannelType.HostAll] = (x) => AmongUsClient.Instance.AmHost && InGame,
                [ChannelType.Jailor] = (x) => x == Jailor.Player && Jailor.Player.IsAlive() && Jailor.Jailed.IsAlive(),

                [ChannelType.Lover] = (x) => x.isLover() && Lovers.IsAlive() && (ModOption.LoverChatChannel switch
                {
                    1 => InMeeting,
                    2 => !InMeeting,
                    3 => true,
                    _ => false
                }),

                [ChannelType.Impostor] = (x) => x.IsImpostor() && x.IsAlive() && (ModOption.ImpostorChatChannel switch
                {
                    1 => InMeeting,
                    2 => !InMeeting,
                    3 => true,
                    _ => false
                }),

                [ChannelType.Jackal] = (x) => (Jackal.jackal.Any(y => y == x) || x == Jackal.Sidekick) && x.IsAlive() && (ModOption.JackalChatChannel switch
                {
                    1 => InMeeting,
                    2 => !InMeeting,
                    3 => true,
                    _ => false
                }),

                [ChannelType.Pavlovs] = (x) => (Pavlovsdogs.pavlovsdogs.Any(y => y == x) || x == Pavlovsdogs.pavlovsowner) && x.IsAlive() && (ModOption.PavlovsChatChannel switch
                {
                    1 => InMeeting,
                    2 => !InMeeting,
                    3 => true,
                    _ => false
                }),

                [ChannelType.Infected] = (x) => Infected.Player.Any(y => y == x) && x.IsAlive() && (ModOption.InfectedChatChannel switch
                {
                    1 => InMeeting,
                    2 => !InMeeting,
                    3 => true,
                    _ => false
                }),
            };

            foreach (var (channelType, condition) in channelConditions)
            {
                if (condition(PlayerControl.LocalPlayer) && !ActiveChannels.Contains(channelType))
                {
                    ActiveChannels.Add(channelType);
                }
                else if (!condition(PlayerControl.LocalPlayer) && (ActiveChannels.Contains(channelType) || CurrentChannel == channelType))
                {
                    ActiveChannels.Remove(channelType);
                    if (CurrentChannel == channelType)
                    {
                        CurrentChannel = ActiveChannels.FirstOrDefault();
                    }
                }
            }
        }

        private static ChannelType LastType;
        public static void KeyboardInput(ChatController __instance)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) && SentHistory.Count > 0)
            {
                CurrentHistorySelection = Mathf.Clamp(--CurrentHistorySelection, 0, SentHistory.Count - 1);
                __instance.freeChatField.textArea.SetText(SentHistory[CurrentHistorySelection]);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow) && SentHistory.Count > 0)
            {
                CurrentHistorySelection++;
                if (CurrentHistorySelection < SentHistory.Count)
                    __instance.freeChatField.textArea.SetText(SentHistory[CurrentHistorySelection]);
                else __instance.freeChatField.textArea.SetText("");
            }

            if (AmongUsClient.Instance.AmHost && InGame && Input.GetKeyDown(KeyCode.LeftShift))
            {
                LastType = CurrentChannel;
                CurrentChannel = ChannelType.HostAll;
            }
            else if (AmongUsClient.Instance.AmHost && InGame && Input.GetKeyUp(KeyCode.LeftShift))
            {
                CurrentChannel = LastType;
            }

            if (Jailor.Player.IsAlive() && PlayerControl.LocalPlayer == Jailor.Jailed) { CurrentChannel = ChannelType.Default; return; }
            if (Input.GetKeyDown(ModInputManager.nextChatChannel.keyCode))
            {
                var channels = ActiveChannels.ToList();
                if (channels.Count == 0) { CurrentChannel = ChannelType.Default; return; }
                var currentIndex = channels.IndexOf(CurrentChannel);
                var nextIndex = (currentIndex + 1) % channels.Count;
                CurrentChannel = channels[nextIndex];
                Message($"切换频道至: {CurrentChannel}");
            }
        }
    }
}
