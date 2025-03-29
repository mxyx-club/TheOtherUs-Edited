using TheOtherRoles.Utilities;
using UnityEngine;

namespace TheOtherRoles.Roles.Crewmate;

public static class SecurityGuard
{
    public static PlayerControl securityGuard;
    public static Color color = new Color32(195, 178, 95, byte.MaxValue);

    public static float cooldown = 30f;
    public static int remainingScrews = 7;
    public static int totalScrews = 7;
    public static int ventPrice = 1;
    public static int camPrice = 2;
    public static int placedCameras;
    public static float duration = 10f;
    public static int maxCharges = 5;
    public static int rechargeTasksNumber = 3;
    public static int rechargedTasks = 3;
    public static int charges = 1;
    public static bool cantMove = true;
    public static Vent ventTarget;
    public static Minigame minigame;

    private static float lastPPU;

    private static Sprite animatedVentSealedSprite;
    private static Sprite camSprite;
    private static Sprite logSprite;
    public static ResourceSprite closeVentButtonSprite = new("CloseVentButton.png");
    public static ResourceSprite placeCameraButtonSprite = new("PlaceCameraButton.png");
    public static ResourceSprite staticVentSealedSprite = new("StaticVentSealed.png", 160);
    public static ResourceSprite fungleVentSealedSprite = new("FungleVentSealed.png", 160);
    public static ResourceSprite submergedCentralUpperVentSealedSprite = new("CentralUpperBlocked.png", 145);
    public static ResourceSprite submergedCentralLowerVentSealedSprite = new("CentralLowerBlocked.png", 145);

    public static Sprite getAnimatedVentSealedSprite()
    {
        var ppu = 185f;
        if (SubmergedCompatibility.IsSubmerged) ppu = 120f;
        if (lastPPU != ppu)
        {
            animatedVentSealedSprite = null;
            lastPPU = ppu;
        }

        if (animatedVentSealedSprite) return animatedVentSealedSprite;
        animatedVentSealedSprite = UnityHelper.loadSpriteFromResources("TheOtherRoles.Resources.AnimatedVentSealed.png", ppu);
        return animatedVentSealedSprite;
    }

    public static Sprite getCamSprite()
    {
        if (camSprite) return camSprite;
        camSprite = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.CamsButton].Image;
        return camSprite;
    }

    public static Sprite getLogSprite()
    {
        if (logSprite) return logSprite;
        logSprite = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.DoorLogsButton].Image;
        return logSprite;
    }

    public static void clearAndReload()
    {
        securityGuard = null;
        ventTarget = null;
        minigame = null;
        duration = CustomOptionHolder.securityGuardCamDuration.GetFloat();
        maxCharges = CustomOptionHolder.securityGuardCamMaxCharges.GetInt();
        rechargeTasksNumber = CustomOptionHolder.securityGuardCamRechargeTasksNumber.GetInt();
        rechargedTasks = CustomOptionHolder.securityGuardCamRechargeTasksNumber.GetInt();
        charges = CustomOptionHolder.securityGuardCamMaxCharges.GetInt() / 2;
        placedCameras = 0;
        cooldown = CustomOptionHolder.securityGuardCooldown.GetFloat();
        totalScrews = remainingScrews = CustomOptionHolder.securityGuardTotalScrews.GetInt();
        camPrice = CustomOptionHolder.securityGuardCamPrice.GetInt();
        ventPrice = CustomOptionHolder.securityGuardVentPrice.GetInt();
        cantMove = CustomOptionHolder.securityGuardNoMove.GetBool();
    }
}
