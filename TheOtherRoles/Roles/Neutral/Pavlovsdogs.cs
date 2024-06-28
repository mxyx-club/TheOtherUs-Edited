using System.Collections.Generic;
using System.Linq;
using TheOtherRoles.Modules;
using TheOtherRoles.Objects;
using UnityEngine;

namespace TheOtherRoles.Roles.Neutral;

public class Pavlovsdogs
{
    public static List<Arrow> arrow;
    public static PlayerControl pavlovsowner;
    public static List<PlayerControl> pavlovsdogs = new();

    public static Color color = new Color32(244, 169, 106, byte.MaxValue);
    public static PlayerControl currentTarget;
    public static PlayerControl killTarget;

    public static bool andJackalAsWell = true;
    public static float cooldown = 30f;
    public static float createDogCooldown = 30f;
    public static int createDogNum;
    public static bool enableRampage;
    public static float rampageKillCooldown;
    public static int rampageDeathTime;
    public static bool rampageDeathTimeIsMeetingReset;

    public static float canUseVents;
    public static bool canSabotage;
    public static bool hasImpostorVision;

    public static float deathTime;
    public static ResourceSprite CreateDogButton = new("SidekickButton.png");

    public static bool canCreateDog => (pavlovsdogs == null || pavlovsdogs.All(p => p.Data.IsDead || p.Data.Disconnected)) && createDogNum > 0;
    public static bool ownerIsDead => pavlovsowner == null || pavlovsowner.Data.Disconnected || pavlovsowner.Data.IsDead;
    public static bool loser => pavlovsdogs.All(p => p.Data.IsDead || p.Data.Disconnected) && createDogNum == 0;

    public static void clear(byte playerId)
    {
        foreach (var item in pavlovsdogs.Where(item => item.PlayerId == playerId && pavlovsdogs != null))
            pavlovsdogs = null;
    }

    public static void clearAndReload()
    {
        if (arrow != null)
        {
            foreach (var arrow in arrow)
                if (arrow?.arrow != null) Object.Destroy(arrow.arrow);
        }
        arrow = [];

        pavlovsowner = null;
        pavlovsdogs = [];
        currentTarget = null;
        killTarget = null;

        deathTime = CustomOptionHolder.pavlovsownerRampageDeathTime.GetInt();
        andJackalAsWell = CustomOptionHolder.pavlovsownerAndJackalAsWell.getBool();
        cooldown = CustomOptionHolder.pavlovsownerKillCooldown.getFloat();
        createDogCooldown = CustomOptionHolder.pavlovsownerCreateDogCooldown.getFloat();
        createDogNum = CustomOptionHolder.pavlovsownerCreateDogNum.GetInt();
        canUseVents = CustomOptionHolder.pavlovsownerCanUseVents.getSelection();
        canSabotage = CustomOptionHolder.pavlovsownerCanUseSabo.getBool();
        hasImpostorVision = CustomOptionHolder.pavlovsownerHasImpostorVision.getBool();
        enableRampage = CustomOptionHolder.pavlovsownerRampage.getBool();
        rampageKillCooldown = CustomOptionHolder.pavlovsownerRampageKillCooldown.getFloat();
        rampageDeathTime = CustomOptionHolder.pavlovsownerRampageDeathTime.GetInt();
        rampageDeathTimeIsMeetingReset = CustomOptionHolder.pavlovsownerRampageDeathTimeIsMeetingReset.getBool();
    }
}
