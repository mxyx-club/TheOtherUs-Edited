using AmongUs.GameOptions;

namespace TheOtherRoles.Patches;

[HarmonyPatch(typeof(HudManager), nameof(HudManager.SetHudActive), typeof(PlayerControl), typeof(RoleBehaviour), typeof(bool))]
public static class HudManagerPatch
{
    private static bool Prefix(HudManager __instance, PlayerControl localPlayer, RoleBehaviour role, bool isActive)
    {
        __instance.AbilityButton.ToggleVisible(isActive);
        if (isActive)
        {
            __instance.UseButton.Refresh();
            __instance.AbilityButton.Refresh(role.Ability);
        }
        else
        {
            __instance.UseButton.ToggleVisible(false);
            __instance.PetButton.ToggleVisible(false);
        }
        bool flag = localPlayer.Data != null && localPlayer.Data.IsDead;
        __instance.ReportButton.ToggleVisible(isActive && !flag && GameManager.Instance.CanReportBodies() && ShipStatus.Instance != null);
        __instance.KillButton.ToggleVisible(isActive && role.IsImpostor && !flag);
        __instance.SabotageButton.ToggleVisible(isActive && role.IsImpostor);
        if (IsHideNSeek) __instance.AdminButton.ToggleVisible(isActive && role.IsImpostor);
        __instance.ImpostorVentButton.ToggleVisible(isActive && !flag && role.IsImpostor && GameOptionsManager.Instance.CurrentGameOptions.GameMode != GameModes.HideNSeek);
        __instance.TaskPanel.gameObject.SetActive(isActive);
        __instance.roomTracker.gameObject.SetActive(isActive);
        if (__instance.joystick != null)
        {
            __instance.joystick.ToggleVisuals(isActive);
        }
        __instance.ToggleRightJoystick(isActive);
        return false;
    }
}