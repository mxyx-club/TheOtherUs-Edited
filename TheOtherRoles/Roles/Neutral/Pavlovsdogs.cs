using System.Collections.Generic;
using System.Linq;
using TheOtherRoles.Objects;
using UnityEngine;

namespace TheOtherRoles.Roles.Neutral;

public class Pavlovsdogs
{
    public static PlayerControl pavlovsowner;
    public static List<PlayerControl> pavlovsdogs = new();

    public static Color color = new Color32(244, 169, 106, byte.MaxValue);
    public static PlayerControl currentTarget;
    public static PlayerControl killTarget;
    public static List<Arrow> arrow;

    public static float cooldown = 30f;
    public static float createDogCooldown = 30f;
    public static int createDogNum;
    public static bool enableRampage;
    public static float rampageKillCooldown;
    public static int rampageDeathTime;

    public static float canUseVents;
    public static bool canSabotage;
    public static bool hasImpostorVision;

    public static float deathTime;
    public static ResourceSprite CreateDogButton = new("SidekickButton.png");

    public static bool canCreateDog => (pavlovsdogs == null || pavlovsdogs.All(p => p.Data.IsDead || p.Data.Disconnected)) && createDogNum > 0;
    public static bool loser => pavlovsdogs.All(p => p.IsDead()) && createDogNum == 0;

    public static void clearAndReload()
    {
        if (arrow != null)
        {
            foreach (var arrow in arrow)
                if (arrow?.arrow != null) Object.Destroy(arrow.arrow);
        }
        arrow = new();

        pavlovsowner = null;
        pavlovsdogs = new();
        currentTarget = null;
        killTarget = null;

        deathTime = CustomOptionHolder.pavlovsownerRampageDeathTime.GetInt();
        cooldown = CustomOptionHolder.pavlovsownerKillCooldown.GetFloat();
        createDogCooldown = CustomOptionHolder.pavlovsownerCreateDogCooldown.GetFloat();
        createDogNum = CustomOptionHolder.pavlovsownerCreateDogNum.GetInt();
        canUseVents = CustomOptionHolder.pavlovsownerCanUseVents.GetSelection();
        canSabotage = CustomOptionHolder.pavlovsownerCanUseSabo.GetBool();
        hasImpostorVision = CustomOptionHolder.pavlovsownerHasImpostorVision.GetBool();
        enableRampage = CustomOptionHolder.pavlovsownerRampage.GetBool();
        rampageKillCooldown = CustomOptionHolder.pavlovsownerRampageKillCooldown.GetFloat();
        rampageDeathTime = CustomOptionHolder.pavlovsownerRampageDeathTime.GetInt();
    }
}
