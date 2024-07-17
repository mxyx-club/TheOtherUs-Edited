using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hazel;
using TheOtherRoles.Modules;
using TheOtherRoles.Utilities;
using UnityEngine;
using Random = System.Random;

namespace TheOtherRoles.Roles.Neutral;

public static class Doomsayer
{
    public static PlayerControl doomsayer;

    public static Color color = new Color32(0, 255, 128, byte.MaxValue);
    public static PlayerControl currentTarget;
    public static List<PlayerControl> playerTargetinformation = new();
    public static float cooldown = 30f;
    public static int formationNum = 1;
    public static bool hasMultipleShotsPerMeeting;
    public static bool canGuessNeutral;
    public static bool canGuessImpostor;
    public static bool triggerDoomsayerrWin;
    public static bool canGuess = true;
    public static bool onlineTarger;
    public static float killToWin = 3;
    public static float killedToWin;
    public static bool CanShoot = true;

    public static ResourceSprite buttonSprite = new("SeerButton.png");

    public static string GetInfo(PlayerControl target)
    {
        try
        {
            var random = new Random();
            var allRoleInfo = (onlineTarger ? onlineRoleInfos() : allRoleInfos()).OrderBy(_ => random.Next()).ToList();
            var roleInfoTarget = RoleInfo.getRoleInfoForPlayer(target, false).FirstOrDefault();
            var AllMessage = new List<string>();
            allRoleInfo.Remove(RoleInfo.doomsayer);
            allRoleInfo.Remove(roleInfoTarget);

            if (allRoleInfo.Count < formationNum + 2) return $"There are fewer than {formationNum + 2} players.\n玩家人数不足 {formationNum + 2} 无法揭示。";

            var formation = formationNum;
            var x = random.Next(0, formation);
            var message = new StringBuilder();
            var tempNumList = Enumerable.Range(0, allRoleInfo.Count).ToList();
            var temp = (tempNumList.Count > formation ? tempNumList.Take(formation) : tempNumList).OrderBy(_ => random.Next()).ToList();

            message.AppendLine($"{target.Data.PlayerName} 的职业可能是：\n");

            for (int num = 0, tempNum = 0; num < formation; num++, tempNum++)
            {
                var info = allRoleInfo[temp[tempNum]];

                message.Append(num == x ? roleInfoTarget.name : info.name);
                message.Append(num < formation - 1 ? ", " : ';');
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId,
                (byte)CustomRPC.DoomsayerMeeting, SendOption.Reliable);
            writer.WritePacked(AllMessage.Count);
            AllMessage.Do(writer.Write);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            playerTargetinformation.Clear();

            AllMessage.Add(message.ToString());
            return $"{message}";
        }
        catch
        {
            return "Doomsayer Error\n末日预言家揭示出错";
        }
    }

    public static void clearAndReload()
    {
        doomsayer = null;
        currentTarget = null;
        killedToWin = 0;
        canGuess = true;
        triggerDoomsayerrWin = false;
        cooldown = CustomOptionHolder.doomsayerCooldown.getFloat();
        hasMultipleShotsPerMeeting = CustomOptionHolder.doomsayerHasMultipleShotsPerMeeting.getBool();
        canGuessNeutral = CustomOptionHolder.doomsayerCanGuessNeutral.getBool();
        canGuessImpostor = CustomOptionHolder.doomsayerCanGuessImpostor.getBool();
        formationNum = CustomOptionHolder.doomsayerDormationNum.GetInt();
        killToWin = CustomOptionHolder.doomsayerKillToWin.getFloat();
        onlineTarger = CustomOptionHolder.doomsayerOnlineTarger.getBool();
    }
}
