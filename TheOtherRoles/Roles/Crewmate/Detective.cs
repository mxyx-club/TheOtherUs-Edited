using TheOtherRoles.Objects;

namespace TheOtherRoles.Roles.Crewmate;

public static class Detective
{
    public static PlayerControl detective;
    public static Color color = new Color32(8, 180, 180, byte.MaxValue);

    public static float footprintIntervall = 1f;
    public static float footprintDuration = 1f;
    public static int anonymousFootprints;
    public static float reportNameDuration;
    public static float reportColorDuration = 20f;
    public static float timer;

    public static void clearAndReload()
    {
        detective = null;
        anonymousFootprints = CustomOptionHolder.detectiveAnonymousFootprints.GetSelection();
        footprintIntervall = CustomOptionHolder.detectiveFootprintIntervall.GetFloat();
        footprintDuration = CustomOptionHolder.detectiveFootprintDuration.GetFloat();
        reportNameDuration = CustomOptionHolder.detectiveReportNameDuration.GetFloat();
        reportColorDuration = CustomOptionHolder.detectiveReportColorDuration.GetFloat();
    }

    [HarmonyPatch]
    public static class Detective_Patch
    {

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            detectiveUpdateFootPrints();
        }

        private static void detectiveUpdateFootPrints()
        {
            if (detective.IsAlive() && detective == PlayerControl.LocalPlayer && !InMeeting)
            {
                timer -= Time.fixedDeltaTime;
                if (timer <= 0f)
                {
                    timer = footprintIntervall;
                    foreach (var player in PlayerControl.AllPlayerControls.GetFastEnumerator())
                    {
                        if (player.IsAlive() && player != PlayerControl.LocalPlayer && !player.inVent)
                            FootprintHolder.Instance.MakeFootprint(player);
                    }
                }
            }
        }
    }
}
