using UnityEngine;

namespace TheOtherRoles.Roles.Crewmate;

public static class Sheriff
{
    public static PlayerControl sheriff;
    public static Color color = new Color32(248, 205, 70, byte.MaxValue);

    public static float cooldown = 30f;
    public static bool canKillNeutrals;
    public static bool canKillArsonist;
    public static bool canKillLawyer;
    public static bool canKillJester;
    public static bool canKillPursuer;
    public static bool canKillVulture;
    public static bool canKillThief;
    public static bool canKillAmnesiac;
    public static bool canKillExecutioner;
    public static bool canKillDoomsayer;
    public static bool spyCanDieToSheriff;
    public static int misfireKills; // Self: 0, Target: 1, Both: 2

    public static PlayerControl currentTarget;

    public static PlayerControl formerDeputy; // Needed for keeping handcuffs + shifting
    public static PlayerControl formerSheriff; // When deputy gets promoted...

    public static void replaceCurrentSheriff(PlayerControl deputy)
    {
        if (!formerSheriff) formerSheriff = sheriff;
        sheriff = deputy;
        currentTarget = null;
        cooldown = CustomOptionHolder.sheriffCooldown.getFloat();
    }

    public static void clearAndReload()
    {
        sheriff = null;
        currentTarget = null;
        formerDeputy = null;
        formerSheriff = null;
        misfireKills = CustomOptionHolder.sheriffMisfireKills.getSelection();
        cooldown = CustomOptionHolder.sheriffCooldown.getFloat();
        canKillNeutrals = CustomOptionHolder.sheriffCanKillNeutrals.getBool();
        canKillArsonist = CustomOptionHolder.sheriffCanKillArsonist.getBool();
        canKillLawyer = CustomOptionHolder.sheriffCanKillLawyer.getBool();
        canKillJester = CustomOptionHolder.sheriffCanKillJester.getBool();
        canKillPursuer = CustomOptionHolder.sheriffCanKillPursuer.getBool();
        canKillVulture = CustomOptionHolder.sheriffCanKillVulture.getBool();
        canKillThief = CustomOptionHolder.sheriffCanKillThief.getBool();
        canKillAmnesiac = CustomOptionHolder.sheriffCanKillAmnesiac.getBool();
        canKillExecutioner = CustomOptionHolder.sheriffCanKillExecutioner.getBool();
        spyCanDieToSheriff = CustomOptionHolder.spyCanDieToSheriff.getBool();
        canKillDoomsayer = CustomOptionHolder.sheriffCanKillDoomsayer.getBool();
    }
}
