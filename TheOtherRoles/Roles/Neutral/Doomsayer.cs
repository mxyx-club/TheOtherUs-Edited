using System.Text;

namespace TheOtherRoles.Roles.Neutral;

public class Doomsayer : GuesserBase, INeutral, IGuesser
{
    public static Color color = new Color32(0, 255, 128, byte.MaxValue);

    public static RoleInfo roleinfo = new(
        typeof(Doomsayer),
        (p) => new Doomsayer(p),
        RoleId.Doomsayer,
        RoleType.Neutral,
        "Doomsayer",
        color,
        203500,
        AddOptions
    );

    public Doomsayer(PlayerControl player) : base(player, roleinfo) { }

    public NeutralType NeutralType => NeutralType.Evil;

    public override int Charges { get; set; } = 100;

    public PlayerControl currentTarget;
    public bool CanShoot = true;
    public float killedToWin;
    public bool triggerDoomsayerrWin;

    public static float cooldown = 30f;
    public static int formationNum = 1;
    public static bool hasMultipleShotsPerMeeting;
    public static bool canGuessImpostor;
    public static bool canGuessNeutral;
    public static bool onlineTarger;
    public static float killToWin = 3;

    public static CustomOption doomsayerCooldown;
    public static CustomOption doomsayerHasMultipleShotsPerMeeting;
    public static CustomOption doomsayerOnlineTarger;
    public static CustomOption doomsayerDormationNum;
    public static CustomOption doomsayerCanGuessImpostor;
    public static CustomOption doomsayerCanGuessNeutral;
    public static CustomOption doomsayerKillToWin;

    public CustomButton doomsayerButton;
    public static Sprite buttonSprite = new ResourceSprite("SeerButton.png");

    public static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        doomsayerCooldown = CustomOption.Create(configId++, CustomOptionType.Neutral, "doomsayerCooldown", 20f, 2.5f, 60f, 2.5f, roleinfo.RoleOption);
        doomsayerHasMultipleShotsPerMeeting = CustomOption.Create(configId++, CustomOptionType.Neutral, "doomsayerHasMultipleShotsPerMeeting", true, roleinfo.RoleOption);
        doomsayerOnlineTarger = CustomOption.Create(configId++, CustomOptionType.Neutral, "doomsayerOnlineTarger", false, roleinfo.RoleOption);
        doomsayerDormationNum = CustomOption.Create(configId++, CustomOptionType.Neutral, "doomsayerDormationNum", 5f, 2f, 10f, 1f, roleinfo.RoleOption);
        doomsayerCanGuessImpostor = CustomOption.Create(configId++, CustomOptionType.Neutral, $"{"doomsayerCanGuess".Translate()} {Cs(Palette.ImpostorRed, "ImpostorRolesText".Translate())}", true, roleinfo.RoleOption);
        doomsayerCanGuessNeutral = CustomOption.Create(configId++, CustomOptionType.Neutral, $"{"doomsayerCanGuess".Translate()} {Cs(Color.gray, "NeutralRolesText".Translate())}", true, roleinfo.RoleOption);
        doomsayerKillToWin = CustomOption.Create(configId++, CustomOptionType.Neutral, "doomsayerKillToWin", 3f, 1f, 10f, 1f, roleinfo.RoleOption);
    }

    public override void Initialize()
    {
        currentTarget = null;
        killedToWin = 0;
        CanShoot = true;
        triggerDoomsayerrWin = false;
        cooldown = doomsayerCooldown.GetFloat();
        hasMultipleShotsPerMeeting = doomsayerHasMultipleShotsPerMeeting.GetBool();
        canGuessNeutral = doomsayerCanGuessNeutral.GetBool();
        canGuessImpostor = doomsayerCanGuessImpostor.GetBool();
        formationNum = doomsayerDormationNum.GetInt();
        killToWin = doomsayerKillToWin.GetFloat();
        onlineTarger = doomsayerOnlineTarger.GetBool();
    }

    public static List<RoleInfo> allRoleInfos()
    {
        var allRoleInfo = new List<RoleInfo>();
        foreach (var role in RoleInfo.AllRoleInfo)
        {
            if (role.RoleType is RoleType.Modifier or RoleType.Ghost or RoleType.Special) continue;
            allRoleInfo.Add(role);
        }
        return allRoleInfo;
    }

    public static List<RoleInfo> onlineRoleInfos()
    {
        var role = new List<RoleInfo>();
        role.AddRange(CustomRoleManager.AllActiveRoles.Values.Select(n => n.RoleInfo).ToArray());
        return role;
    }

    public static string GetInfo(PlayerControl target)
    {
        try
        {
            var allRoleInfo = (onlineTarger ? onlineRoleInfos() : allRoleInfos()).OrderBy(_ => rnd.Next()).ToList();
            var roleInfoTarget = target.GetRoleInfo();
            var AllMessage = new List<string>();
            allRoleInfo.Remove(roleinfo);
            allRoleInfo.Remove(roleInfoTarget);

            if (allRoleInfo.Count < formationNum + 2)
                return $"There are fewer than {formationNum + 3} players.\n玩家人数不足 {formationNum + 3} 无法揭示。";

            var formation = formationNum;
            var x = rnd.Next(0, formation);
            var message = new StringBuilder();
            var tempNumList = Enumerable.Range(0, allRoleInfo.Count).ToList();
            var temp = (tempNumList.Count > formation ? tempNumList.Take(formation) : tempNumList).OrderBy(_ => rnd.Next()).ToList();

            message.AppendLine($"{target.Data.PlayerName} 的职业可能是：\n");

            for (int num = 0, tempNum = 0; num < formation; num++, tempNum++)
            {
                var info = allRoleInfo[temp[tempNum]];

                message.Append(num == x ? roleInfoTarget.Name : info.Name);
                message.Append(num < formation - 1 ? ", " : ';');
            }

            AllMessage.Add(message.ToString());

            return $"{message}";
        }
        catch
        {
            return "Doomsayer Error\n末日预言家揭示出错";
        }
    }

    public override string UpdateMeetingVoteText(MeetingHud __instance)
    {
        var meetingInfoText = string.Format(GetString("DoomsayerKilledToWin"), killToWin - killedToWin);
        return meetingInfoText;
    }

    public override void OnExiledBegin(GameData.PlayerInfo exiled)
    {
        CanShoot = true;
    }

    public override void CleanUp(HudManager __instance)
    {
        doomsayerButton?.Destroy();
        doomsayerButton = null;
    }

    public override void CreateButton(HudManager __instance)
    {
        // doomsayer Shield
        doomsayerButton?.Destroy();
        doomsayerButton = new CustomButton(
            () =>
            {
                if (CheckAndDoVetKill(PlayerControl.LocalPlayer, currentTarget)) return;

                doomsayerButton.Timer = doomsayerButton.MaxTimer;
                SoundEffectsManager.play("knockKnock");
            },
            () =>
            {
                return Player == PlayerControl.LocalPlayer && Player.IsAlive();
            },
            () =>
            {
                currentTarget = SetTarget();
                doomsayerButton.showTargetNameOnButton(currentTarget, GetString("doomsayerText"));
                return PlayerControl.LocalPlayer.CanMove && currentTarget != null;
            },
            () => { doomsayerButton.Timer = doomsayerButton.MaxTimer; },
            buttonSprite,
            __instance,
            __instance.AbilityButton,
            ModInputManager.abilityInput.keyCode,
            true,
            0f,
            () =>
            {
                doomsayerButton.Timer = doomsayerButton.MaxTimer;
                var msg = GetInfo(currentTarget);
                FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"{msg}");

                // Ghost Info
                var writer = StartRPC(CustomRPC.ShareGhostInfo);
                writer.Write(Player.PlayerId);
                writer.Write((byte)RPCProcedure.GhostInfoTypes.GhostChat);
                writer.Write(msg);
                writer.EndRPC();
            },
            buttonText: GetString("doomsayerText")
        );
    }
}
