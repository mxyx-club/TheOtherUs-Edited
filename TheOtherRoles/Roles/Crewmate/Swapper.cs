namespace TheOtherRoles.Roles.Crewmate;

[CustomRpcHolder]
public class Swapper : RoleBase, IPowerCrew
{
    public static Color color = new Color32(134, 55, 86, byte.MaxValue);

    public static readonly RoleInfo roleinfo = new(
        typeof(Swapper),
        (p) => new Swapper(p),
        RoleId.Swapper,
        RoleType.Crewmate,
        "Swapper",
        color,
        302300,
        AddOptions,
        MaxPlayer: 1
    );

    public Swapper(PlayerControl p) : base(p, roleinfo) { }

    public static PlayerControl currentAbilityUser;
    public int charges;
    public static bool canCallEmergency;
    public static bool canOnlySwapOthers;
    public static float rechargeTasksNumber;
    public static bool canFixSabotages;
    public static float rechargedTasks;

    public static byte TargetId1 = byte.MaxValue;
    public static byte TargetId2 = byte.MaxValue;

    public static Sprite spriteCheck = new ResourceSprite("SwapperCheck.png", 150f);

    public static CustomOption swapperCanCallEmergency;
    public static CustomOption swapperCanFixSabotages;
    public static CustomOption swapperCanOnlySwapOthers;
    public static CustomOption swapperSwapsNumber;
    public static CustomOption swapperRechargeTasksNumber;

    private static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        swapperCanCallEmergency = CustomOption.Create(configId++, CustomOptionType.Crewmate, "canCallEmergency", true, roleinfo.RoleOption);
        swapperCanFixSabotages = CustomOption.Create(configId++, CustomOptionType.Crewmate, "swapperCanFixSabotages", true, roleinfo.RoleOption);
        swapperCanOnlySwapOthers = CustomOption.Create(configId++, CustomOptionType.Crewmate, "swapperCanOnlySwapOthers", false, roleinfo.RoleOption);
        swapperSwapsNumber = CustomOption.Create(configId++, CustomOptionType.Crewmate, "swapperSwapsNumber", 1f, 0f, 5f, 1f, roleinfo.RoleOption);
        swapperRechargeTasksNumber = CustomOption.Create(configId++, CustomOptionType.Crewmate, "swapperRechargeTasksNumber", 2f, 1f, 10f, 1f, roleinfo.RoleOption);
    }

    public override void Initialize()
    {
        TargetId1 = byte.MaxValue;
        TargetId2 = byte.MaxValue;
        currentAbilityUser = null;
        canCallEmergency = swapperCanCallEmergency.GetBool();
        canOnlySwapOthers = swapperCanOnlySwapOthers.GetBool();
        canFixSabotages = swapperCanFixSabotages.GetBool();
        charges = swapperSwapsNumber.GetInt();
        rechargeTasksNumber = swapperRechargeTasksNumber.GetInt();
        rechargedTasks = swapperRechargeTasksNumber.GetInt();
    }

    public override void OnExiledWrapUp(GameData.PlayerInfo exiled)
    {
        TargetId1 = byte.MaxValue;
        TargetId2 = byte.MaxValue;
        currentAbilityUser = null;
    }

    public override void OnFixedUpdate(PlayerControl player)
    {
        if (player != Player || Player.IsDead()) return;
        var (playerCompleted, _) = TasksHandler.taskInfo(PlayerControl.LocalPlayer.Data);
        if (playerCompleted == rechargedTasks)
        {
            rechargedTasks += rechargeTasksNumber;
            charges++;
        }
    }
    public static RemoteProcess<(byte swapperId, byte playerId1, byte playerId2)> RpcSwapVote = new("RpcSwapVote", (data, _) =>
    {
        if (MeetingHud.Instance)
        {
            var player = PlayerById(data.swapperId);
            if (player.TryGetRole<Swapper>(out var swapper))
            {
                TargetId1 = data.playerId1;
                TargetId2 = data.playerId2;
                currentAbilityUser = player;
            }
        }
    });

    private static bool[] selections;
    private static SpriteRenderer[] renderers;
    private static PassiveButton[] swapperButtonList;

    public override void OnMeetingStart(MeetingHud __instance)
    {
        // Add Swapper Buttons
        if (Player.IsAlive() && PlayerControl.LocalPlayer == Player)
        {
            selections = new bool[__instance.playerStates.Length];
            renderers = new SpriteRenderer[__instance.playerStates.Length];
            swapperButtonList = new PassiveButton[__instance.playerStates.Length];

            for (var i = 0; i < __instance.playerStates.Length; i++)
            {
                var playerVoteArea = __instance.playerStates[i];
                if (playerVoteArea.AmDead || (playerVoteArea.TargetPlayerId == Player.PlayerId && canOnlySwapOthers))
                    continue;

                var template = playerVoteArea.Buttons.transform.Find("CancelButton").gameObject;
                var checkbox = UObject.Instantiate(template, playerVoteArea.transform, true);
                checkbox.transform.position = template.transform.position;
                checkbox.transform.localPosition = new Vector3(-0.95f, 0.03f, -1.3f);
                if (HandleGuesser.isGuesserGm && HandleGuesser.isGuesser(PlayerControl.LocalPlayer.PlayerId)
                    /*|| (Mimic.mimic?.PlayerId == PlayerControl.LocalPlayer.PlayerId)*/)
                    checkbox.transform.localPosition = new Vector3(-0.5f, 0.03f, -1.3f);
                var renderer = checkbox.GetComponent<SpriteRenderer>();
                renderer.sprite = spriteCheck;
                renderer.color = Color.red;

                if (charges <= 0) renderer.color = Color.gray;

                var button = checkbox.GetComponent<PassiveButton>();
                swapperButtonList[i] = button;
                button.OnClick.RemoveAllListeners();
                var copiedIndex = i;
                button.OnClick.AddListener((Action)(() => swapperOnClick(copiedIndex, __instance)));

                selections[i] = false;
                renderers[i] = renderer;
            }
        }
    }

    public override string UpdateMeetingVoteText(MeetingHud __instance)
    {
        var meetingInfoText = string.Format(GetString("SwapperCharges"), charges);
        return meetingInfoText;
    }

    private void swapperOnClick(int i, MeetingHud __instance)
    {
        if (__instance.state == MeetingHud.VoteStates.Results || __instance.playerStates[i].AmDead) return;

        var selectedCount = selections.Count(b => b);
        var renderer = renderers[i];

        byte firstPlayer = byte.MaxValue;
        byte secondPlayer = byte.MaxValue;

        switch (selectedCount)
        {
            case 0:
                renderer.color = Color.green;
                selections[i] = true;
                firstPlayer = __instance.playerStates[i].TargetPlayerId;
                break;
            case 1:
                if (selections[i])
                {
                    renderer.color = Color.red;
                    selections[i] = false;
                    firstPlayer = byte.MaxValue;
                }
                else
                {
                    selections[i] = true;
                    renderer.color = Color.green;

                    for (int A = 0; A < selections.Length; A++)
                    {
                        if (selections[A])
                        {
                            if (firstPlayer != byte.MaxValue)
                            {
                                secondPlayer = __instance.playerStates[A].TargetPlayerId;
                                break;
                            }
                            else
                            {
                                firstPlayer = __instance.playerStates[A].TargetPlayerId;
                            }
                        }
                    }
                }
                break;
            case 2 when !selections[i]:
                //firstPlayer = byte.MaxValue;
                return;
            case 2:
                renderer.color = Color.red;
                selections[i] = false;
                if (__instance.playerStates[i].TargetPlayerId == firstPlayer) firstPlayer = byte.MaxValue;
                else if (__instance.playerStates[i].TargetPlayerId == secondPlayer) secondPlayer = byte.MaxValue;
                break;
        }

        RpcSwapVote.Invoke((PlayerControl.LocalPlayer.PlayerId, firstPlayer, secondPlayer));
    }

    public override void ClearVotes(MeetingHud __instance)
    {
        swapperCheckAndReturnSwap(__instance, byte.MaxValue);
    }

    public override void OnPlayerExlied(PlayerControl player)
    {
        if (MeetingHud.Instance == null) return;
        swapperCheckAndReturnSwap(MeetingHud.Instance, player.PlayerId);
    }

    public void swapperCheckAndReturnSwap(MeetingHud __instance, byte dyingPlayerId)
    {
        // someone was guessed or dced in the meeting, check if this affects the swapper.
        if (Player.IsDead() || __instance.state == MeetingHud.VoteStates.Results) return;

        // reset swap.
        var reset = false;
        if (dyingPlayerId == TargetId1 || dyingPlayerId == TargetId2 || dyingPlayerId == byte.MaxValue - 1)
        {
            reset = true;
            TargetId1 = TargetId2 = byte.MaxValue;
        }

        // Only for the swapper: Reset all the buttons and charges value to their original state.
        if (PlayerControl.LocalPlayer != Player) return;

        // check if dying player was a selected player (but not confirmed yet)
        for (var i = 0; i < __instance.playerStates.Count; i++)
        {
            reset = reset || (selections[i] && __instance?.playerStates[i]?.TargetPlayerId == dyingPlayerId);
            if (reset) break;
        }

        if (!reset) return;


        for (var i = 0; i < selections.Length; i++)
        {
            selections[i] = false;
            var playerVoteArea = __instance.playerStates[i];
            if (playerVoteArea.AmDead || (playerVoteArea.TargetPlayerId == Player.PlayerId && canOnlySwapOthers))
                continue;
            renderers[i].color = Color.red;
            var copyI = i;
            swapperButtonList[i].OnClick.RemoveAllListeners();
            swapperButtonList[i].OnClick.AddListener((Action)(() => swapperOnClick(copyI, __instance)));
        }
    }

}
