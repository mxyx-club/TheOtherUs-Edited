using UnityEngine;

namespace TheOtherRoles.Roles.Crewmate;

public static class Prosecutor
{
    public static PlayerControl prosecutor;
    public static Color color = new Color32(178, 128, 0, byte.MaxValue);
    public static bool diesOnIncorrectPros;
    public static bool canCallEmergency;

    public static bool Prosecuted;
    public static bool StartProsecute;
    public static bool ProsecuteThisMeeting;
    public static PlayerVoteArea Prosecute;


    public static void clearAndReload()
    {
        prosecutor = null;
        ProsecuteThisMeeting = false;
        StartProsecute = false;
        Prosecuted = false;
        diesOnIncorrectPros = CustomOptionHolder.prosecutorDiesOnIncorrectPros.getBool();
        canCallEmergency = CustomOptionHolder.prosecutorCanCallEmergency.getBool();
    }
}
