
using UnityEngine;

namespace TheOtherRoles.Roles.Neutral;
public static class Executioner
{
    public static PlayerControl executioner;
    public static PlayerControl target;
    public static Color color = new Color32(140, 64, 5, byte.MaxValue);
    public static bool canCallEmergency;
    public static bool triggerExecutionerWin;
    public static bool promotesToLawyer;
    public static bool targetWasGuessed;
    /*
    public enum targetDeadBecame
    {
        Pursuer = 0,
        Jester = 1,
        Amnisiac = 2,
        Crewmate = 3,
    };
    public static targetDeadBecame role;
    */
    public static void clearAndReload(bool clearTarget = true)
    {
        if (clearTarget)
        {
            target = null;
            targetWasGuessed = false;
        }
        executioner = null;
        triggerExecutionerWin = false;
        promotesToLawyer = CustomOptionHolder.executionerPromotesToLawyer.getBool();
        canCallEmergency = CustomOptionHolder.executionerCanCallEmergency.getBool();
        //role = (targetDeadBecame)CustomOptionHolder.executionerOnTargetDead.getSelection();
    }
}
