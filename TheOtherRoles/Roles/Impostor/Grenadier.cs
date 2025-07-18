namespace TheOtherRoles.Roles.Impostor;

public class Grenadier
{
    public static PlayerControl Player;
    public static Color color = Palette.ImpostorRed;
    public static Color flash = new Color32(150, 150, 150, byte.MaxValue);
    public static List<PlayerControl> controls = new();

    public static float cooldown;
    public static float duration;
    public static float radius;
    public static bool indicatorsMode;

    public static Sprite ButtonSprite = new ResourceSprite("FlashButton.png");

    public static void showFlash(Color color, float duration = 10f, float alpha = 1f)
    {
        if (FastDestroyableSingleton<HudManager>.Instance == null ||
            FastDestroyableSingleton<HudManager>.Instance.FullScreen == null) return;

        FastDestroyableSingleton<HudManager>.Instance.FullScreen.gameObject.SetActive(true);
        FastDestroyableSingleton<HudManager>.Instance.FullScreen.enabled = true;
        DestroyableSingleton<HudManager>.Instance.FullScreen.gameObject.active = true;

        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(duration, new Action<float>(p =>
        {
            var renderer = FastDestroyableSingleton<HudManager>.Instance.FullScreen;
            var fadeFraction = 0.5f / duration;

            if (InMeeting)
            {
                renderer.enabled = false;
                if (PlayerControl.LocalPlayer.PlayerId == Player?.PlayerId && controls?.Count > 0)
                {
                    var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.GrenadierFlash);
                    writer.Write(true);
                    writer.EndRPC();
                    RPCProcedure.grenadierFlash(true);
                }
                return;
            }

            if (p < fadeFraction)
            {
                var fadeInProgress = p / fadeFraction;
                if (renderer != null) renderer.color = new Color(color.r, color.g, color.b, Mathf.Clamp01(fadeInProgress * alpha));
            }
            else if (p > 1 - fadeFraction)
            {
                var fadeOutProgress = (p - (1 - fadeFraction)) / fadeFraction;
                if (renderer != null) renderer.color = new Color(color.r, color.g, color.b, Mathf.Clamp01((1 - fadeOutProgress) * alpha));

                if (PlayerControl.LocalPlayer.PlayerId == Player?.PlayerId && controls?.Count > 0)
                {
                    var writer = StartRPC(PlayerControl.LocalPlayer, CustomRPC.GrenadierFlash);
                    writer.Write(true);
                    writer.EndRPC();
                    RPCProcedure.grenadierFlash(true);
                }
            }
            else
            {
                if (renderer != null) renderer.color = new Color(color.r, color.g, color.b, alpha);
            }

            if (p == 1f && renderer != null) renderer.enabled = false;
        })));
    }

    public static void clearAndReload()
    {
        Player = null;
        controls.Clear();
        cooldown = CustomOptionHolder.grenadierCooldown.GetFloat();
        duration = CustomOptionHolder.grenadierDuration.GetFloat() + 0.5f;
        radius = CustomOptionHolder.grenadierFlashRadius.GetFloat();
        indicatorsMode = CustomOptionHolder.grenadierTeamIndicators.GetBool();
    }
}
