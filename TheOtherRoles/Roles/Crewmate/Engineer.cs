namespace TheOtherRoles.Roles.Crewmate;

public class Engineer
{
    public static PlayerControl engineer;
    public static Color color = new Color32(0, 40, 245, byte.MaxValue);
    public static Sprite buttonSprite = new ResourceSprite("RepairButton.png");

    public static bool UsedFix;

    public static int resetFixAfterMeeting;
    public static bool expertRepairs;
    public static bool oneFixPerRound;
    public static bool remoteFix;
    public static int remainingFixes;
    public static bool highlightForImpostors;
    public static bool highlightForTeamNeutral;

    public static void clearAndReload()
    {
        engineer = null;
        remoteFix = CustomOptionHolder.engineerRemoteFix.GetBool();
        expertRepairs = CustomOptionHolder.engineerExpertRepairs.GetBool();
        resetFixAfterMeeting = CustomOptionHolder.engineerResetFixAfterMeeting.GetSelection();
        oneFixPerRound = CustomOptionHolder.engineerOneFixPerMeeting.GetBool();
        remainingFixes = CustomOptionHolder.engineerNumberOfFixes.GetInt();
        highlightForImpostors = CustomOptionHolder.engineerHighlightForImpostors.GetBool();
        highlightForTeamNeutral = CustomOptionHolder.engineerHighlightForTeamNeutral.GetBool();
    }

    [HarmonyPatch]
    public static class Engineer_Patch
    {
        [HarmonyPatch(typeof(Console), nameof(Console.Use)), HarmonyPostfix]
        public static void UsePostfix(Console __instance)
        {
            if (PlayerControl.LocalPlayer != engineer) return;
            var task = __instance.TaskTypes.FirstOrDefault();
            if (IsSabotage(task))
            {
                if (!expertRepairs) return;
                var writer = StartRPC(CustomRPC.FixingSabotage);
                writer.Write((byte)task);
                writer.EndRPC();
                FixingSabotage(task);
                Minigame.Instance.Close();
            }
            /*else
            {
                NormalPlayerTask task = __instance.FindTask(PlayerControl.LocalPlayer)?.TryCast<NormalPlayerTask>();
                if (task != null)
                {
                    task.NextStep();
                    Minigame.Instance.Close();
                }
            }*/
        }
    }
}