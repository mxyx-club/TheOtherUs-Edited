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
    public static bool showInfoInGhostChat = true;
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
        var random = new Random();
        var allRoleInfo = (onlineTarger ? onlineRoleInfos() : allRoleInfos()).OrderBy(_ => random.Next()).ToList();
        var OtherRoles = allRoleInfos().Where(n => allRoleInfo.All(y => y != n)).OrderBy(_ => random.Next()).ToList();
        var OtherIndex = -1;
        var AllMessage = new List<string>();
        allRoleInfo.Remove(RoleInfo.doomsayer);
        OtherRoles.Remove(RoleInfo.doomsayer);

        var formation = formationNum;
        var x = random.Next(1, formation) - 1;
        var roleInfoTarget = RoleInfo.getRoleInfoForPlayer(target, false).FirstOrDefault();
        var message = new StringBuilder();
        var tempNumList = Enumerable.Range(0, allRoleInfo.Count - 1).ToList();
        var temp =
            (tempNumList.Count > formation ? tempNumList.Take(formation) : tempNumList)
            .OrderBy(_ => random.Next()).ToList();

        message.AppendLine($"{target.Data.PlayerName} 的职业可能是：\n");

        for (int num = 0, tempNum = 0; num < formation; num++, tempNum++)
        {
            var info = tempNum > temp.Count - 1
                ? GetOther()
                : allRoleInfo[temp[tempNum]];

            if (info == roleInfoTarget)
            {
                num--;
                continue;
            }
            message.Append(num == x ? roleInfoTarget.name : info.name);
            message.Append(num < formation - 1 ? ',' : ';');
        }

        var writer = AmongUsClient.Instance.StartRpcImmediately(
                    CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.DoomsayerMeeting,
                    SendOption.Reliable);
        writer.WritePacked(AllMessage.Count);
        AllMessage.Do(writer.Write);
        AmongUsClient.Instance.FinishRpcImmediately(writer);
        playerTargetinformation.Clear();

        RoleInfo GetOther()
        {
            OtherIndex++;
            return OtherRoles[OtherIndex];
        }

        AllMessage.Add(message.ToString());
        return $"{message}";
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
        showInfoInGhostChat = CustomOptionHolder.doomsayerShowInfoInGhostChat.getBool();
        canGuessNeutral = CustomOptionHolder.doomsayerCanGuessNeutral.getBool();
        canGuessImpostor = CustomOptionHolder.doomsayerCanGuessImpostor.getBool();
        formationNum = CustomOptionHolder.doomsayerDormationNum.GetInt();
        killToWin = CustomOptionHolder.doomsayerKillToWin.getFloat();
        onlineTarger = CustomOptionHolder.doomsayerOnlineTarger.getBool();
    }
}
