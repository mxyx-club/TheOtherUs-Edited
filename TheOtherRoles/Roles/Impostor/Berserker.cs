using UnityEngine;

namespace TheOtherRoles.Roles.Impostor;
public class Berserker
{
    public static PlayerControl Player;
    public static Color color = Palette.ImpostorRed;
    public static PlayerControl currentTarget;

    public static float KillCooldown = 25f;
    public static float RampageCooldown = 10f;
    public static float RampageDuration = 3f;
    private static float defaultTimer = 10f;

    public static float Timer;
    public static float Duration;

    public static void ClearAndReload()
    {
        Player = null;
        currentTarget = null;
        Timer = 0f;
        defaultTimer = CustomOptionHolder.berserkerRampageCooldown.GetFloat();
        RampageDuration = CustomOptionHolder.berserkerRampageDuration.GetFloat();
        KillCooldown = CustomOptionHolder.berserkerKillCooldown.GetFloat();
    }

    public static void UpdateTimer()
    {
        if (Timer >= 0)
        {
            if (Timer <= defaultTimer) Timer += Time.deltaTime;
        }
        else
        {
            Timer = 0;
        }
    }

    public static float GetTimerRatio()
    {
        return Mathf.Clamp01(Timer / defaultTimer);
    }

    public static float GetDuration()
    {
        float remainingRatio = GetTimerRatio();
        return Mathf.Lerp(0.1f, RampageDuration, remainingRatio);
    }

    public static float GetDurationPercentage()
    {
        if (RampageDuration <= 0) return 0f;

        float currentDuration = GetDuration();
        return Mathf.Clamp01(currentDuration / RampageDuration);
    }
}
