using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TheOtherRoles.Roles.Crewmate;

public static class Medium
{
    public static PlayerControl medium;
    public static DeadPlayer target;
    public static DeadPlayer soulTarget;
    public static Color color = new Color32(98, 120, 115, byte.MaxValue);
    public static List<Tuple<DeadPlayer, Vector3>> deadBodies = new();
    public static List<Tuple<DeadPlayer, Vector3>> futureDeadBodies = new();
    public static List<SpriteRenderer> souls = new();
    public static DateTime meetingStartTime = DateTime.UtcNow;

    public static float cooldown = 30f;
    public static float duration = 3f;
    public static bool oneTimeUse;
    public static float chanceAdditionalInfo;

    private static Sprite soulSprite;

    private static Sprite question;

    public static Sprite getSoulSprite()
    {
        if (soulSprite) return soulSprite;
        soulSprite = loadSpriteFromResources("TheOtherRoles.Resources.Soul.png", 500f);
        return soulSprite;
    }

    public static Sprite getQuestionSprite()
    {
        if (question) return question;
        question = loadSpriteFromResources("TheOtherRoles.Resources.MediumButton.png", 115f);
        return question;
    }

    public static void clearAndReload()
    {
        medium = null;
        target = null;
        soulTarget = null;
        deadBodies = new List<Tuple<DeadPlayer, Vector3>>();
        futureDeadBodies = new List<Tuple<DeadPlayer, Vector3>>();
        souls = new List<SpriteRenderer>();
        meetingStartTime = DateTime.UtcNow;
        cooldown = CustomOptionHolder.mediumCooldown.getFloat();
        duration = CustomOptionHolder.mediumDuration.getFloat();
        oneTimeUse = CustomOptionHolder.mediumOneTimeUse.getBool();
        chanceAdditionalInfo = CustomOptionHolder.mediumChanceAdditionalInfo.getSelection() / 10f;
    }
    public static string getInfo(PlayerControl target, PlayerControl killer)
    {
        var msg = "";

        var infos = new List<SpecialMediumInfo>();
        // collect fitting death info types.
        // suicides:
        if (killer == target)
        {
            if (target == Sheriff.sheriff || target == Sheriff.formerSheriff)
                infos.Add(SpecialMediumInfo.SheriffSuicide);
            if (target == Lovers.lover1 || target == Lovers.lover2) infos.Add(SpecialMediumInfo.PassiveLoverSuicide);
            if (target == Thief.thief) infos.Add(SpecialMediumInfo.ThiefSuicide);
            if (target == Warlock.warlock) infos.Add(SpecialMediumInfo.WarlockSuicide);
        }
        else
        {
            if (target == Lovers.lover1 || target == Lovers.lover2) infos.Add(SpecialMediumInfo.ActiveLoverDies);
            if (target.Data.Role.IsImpostor && killer.Data.Role.IsImpostor && Thief.formerThief != killer)
                infos.Add(SpecialMediumInfo.ImpostorTeamkill);
        }

        if (target == Sidekick.sidekick &&
            (killer == Jackal.jackal || Jackal.formerJackals.Any(x => x.PlayerId == killer.PlayerId)))
            infos.Add(SpecialMediumInfo.JackalKillsSidekick);
        if (target == Lawyer.lawyer && killer == Lawyer.target) infos.Add(SpecialMediumInfo.LawyerKilledByClient);
        if (Medium.target.wasCleaned) infos.Add(SpecialMediumInfo.BodyCleaned);

        if (infos.Count > 0)
        {
            var selectedInfo = infos[rnd.Next(infos.Count)];
            switch (selectedInfo)
            {
                case SpecialMediumInfo.SheriffSuicide:
                    msg = "哎呀，枪走火了！[警长自杀].";
                    break;
                case SpecialMediumInfo.WarlockSuicide:
                    msg = "啊哦，我好像把自己咒死了耶。[术士死于自杀].";
                    break;
                case SpecialMediumInfo.ThiefSuicide:
                    msg = "我试图从他们口袋里偷枪，却把自己害死了。[窃贼自杀].";
                    break;
                case SpecialMediumInfo.ActiveLoverDies:
                    msg = "无论如何，我都想摆脱这种有毒的关系。[带着恋人死去].";
                    break;
                case SpecialMediumInfo.PassiveLoverSuicide:
                    msg = "在天愿作比翼鸟,在地愿为连理枝，所爱以逝，吾亦寻之。[被恋人带死].";
                    break;
                case SpecialMediumInfo.LawyerKilledByClient:
                    msg = "我的客户杀了我。我还能得到报酬吗？[律师被客户杀害]";
                    break;
                case SpecialMediumInfo.JackalKillsSidekick:
                    msg = "既已纳我为伍，何必取我性命，算了，至少不用做任务了。[跟班被豺狼杀害]";
                    break;
                case SpecialMediumInfo.ImpostorTeamkill:
                    msg = "他们肯定是把我当成卧底才杀了我，有没有？[内鬼死于队友]";
                    break;
                case SpecialMediumInfo.BodyCleaned:
                    msg = "我的尸体现在是某种艺术还是。。。啊，它不见了。[尸体被清理或吃了]";
                    break;
            }
        }
        else
        {
            var randomNumber = rnd.Next(4);
            var typeOfColor = isLighterColor(Medium.target.killerIfExisting) ? "浅" : "深";
            var timeSinceDeath = (float)(meetingStartTime - Medium.target.timeOfDeath).TotalMilliseconds;
            var roleString = RoleInfo.GetRolesString(Medium.target.player, false, false, false);
            if (randomNumber == 0)
            {
                msg = "我的职业是 " + roleString + " .";
            }
            else if (randomNumber == 1)
            {
                msg = "我不确定，但我想应该是 " + typeOfColor + " 色的凶手杀了我.";
            }
            else if (randomNumber == 2)
            {
                msg = "如果我数对了，我就在会议前 " + Math.Round(timeSinceDeath / 1000) + " 秒死了.";
            }
            else
            {
                msg = "我好像是被 " + RoleInfo.GetRolesString(Medium.target.killerIfExisting, false, false, false) + " 无情的杀害了.";
            }
        }

        if (rnd.NextDouble() < chanceAdditionalInfo)
        {
            var count = 0;
            var condition = "";
            var alivePlayersList = PlayerControl.AllPlayerControls.ToArray().Where(pc => !pc.Data.IsDead);
            switch (rnd.Next(3))
            {
                case 0:
                    count = alivePlayersList.Count(pc =>
                        pc.Data.Role.IsImpostor || isKiller(pc) ||
                        new List<RoleInfo> { RoleInfo.sheriff, RoleInfo.veteran, RoleInfo.thief }
                            .Contains(RoleInfo.getRoleInfoForPlayer(pc, false).FirstOrDefault()));
                    condition = "个杀手" + (count == 1 ? "" : "");
                    break;
                case 1:
                    count = alivePlayersList.Where(Helpers.roleCanUseVents).Count();
                    condition = "个可以使用管道的玩家" + (count == 1 ? "" : "");
                    break;
                case 2:
                    count = alivePlayersList.Count(pc => !isKiller(pc));
                    condition = "名玩家" + (count == 1 ? "" : "") + "" + (count == 1 ? "是" : "是") + "非击杀型中立";
                    break;
            }

            msg += $"\n你问我的时候,有{count} " + condition + (count == 1 ? "" : "") + " 还活着";
        }

        return Medium.target.player.Data.PlayerName + " 的灵魂说:\n" + msg;
    }

    private enum SpecialMediumInfo
    {
        SheriffSuicide,
        ThiefSuicide,
        ActiveLoverDies,
        PassiveLoverSuicide,
        LawyerKilledByClient,
        JackalKillsSidekick,
        ImpostorTeamkill,
        SubmergedO2,
        WarlockSuicide,
        BodyCleaned
    }
}
