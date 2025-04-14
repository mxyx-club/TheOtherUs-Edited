using UnityEngine;

namespace TheOtherRoles.Roles.Impostor;
public class Berserker
{
    public static PlayerControl Player;
    public static Color color = Palette.ImpostorRed;
    public static PlayerControl currentTarget;

    public static float KillCooldown = 25f; // 角色的击杀冷却时间
    private static float ChargingTimer = 10f;  // 角色的狂暴蓄力时间（需要在击杀冷却完毕后才会进行蓄力）
    public static float RampageDuration = 3f; // 狂暴持续时间

    public static float Timer; // 计时器
    public static float Duration; // 效果时长

    public static void ClearAndReload()
    {
        Player = null;
        currentTarget = null;
        Timer = 0f;
        KillCooldown = CustomOptionHolder.berserkerKillCooldown.GetFloat(); 
        ChargingTimer = CustomOptionHolder.berserkerRampageCooldown.GetFloat();
        RampageDuration = CustomOptionHolder.berserkerRampageDuration.GetFloat();
    }

    public static void UpdateTimer()
    {
        if (Timer >= 0)
        {
            if (Timer <= ChargingTimer) Timer += Time.deltaTime;
        }
        else
        {
            Timer = 0;
        }
    }

    public static float GetTimerRatio()
    {
        return Mathf.Clamp01(Timer / ChargingTimer);
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
