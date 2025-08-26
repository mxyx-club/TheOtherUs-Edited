using TheOtherRoles.Objects;

namespace TheOtherRoles.Roles.Impostor;

public class Marionette
{
    public static PlayerControl Player;
    public static Color color = Palette.ImpostorRed;

    public static Decoy decoy;
    public static int marionetteMode;

    public static Sprite decoyButtonSprite = new ResourceSprite("DecoyButton.png", 115f);
    public static Sprite swapButtonSprite = new ResourceSprite("DecoySwapButton.png", 115f);
    public static Sprite destroyButtonSprite = new ResourceSprite("DecoyDestroyButton.png", 115f);
    public static Sprite monitorButtonSprite = new ResourceSprite("DecoyMonitorButton.png", 115f);

    public static float PlaceCooldown;
    public static float SwapCooldown;
    public static int ShowDecoy;
    public static bool MonitoringCanMove;

    public static void SetMarionetteMode(int mode)
    {
        marionetteMode = mode;
        if (marionetteMode == 0)
        {
            HudManagerStartPatch.marionetteButton.Sprite = swapButtonSprite;
            HudManagerStartPatch.marionetteButton.buttonText = GetString("swapButtonText");
        }
        else
        {
            HudManagerStartPatch.marionetteButton.Sprite = destroyButtonSprite;
            HudManagerStartPatch.marionetteButton.buttonText = GetString("destroyButtonText");
        }
    }

    public static void ClearAndReload()
    {
        Player = null;
        decoy = null;
        PlaceCooldown = CustomOptionHolder.marionettePlaceCooldown.GetFloat();
        SwapCooldown = CustomOptionHolder.marionetteSwapCooldown.GetFloat();
        ShowDecoy = CustomOptionHolder.marionetteShowDecoy.GetQuantity();
        MonitoringCanMove = CustomOptionHolder.marionetteMonitoringCanMove.GetBool();
    }
}
