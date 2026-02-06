namespace TheOtherRoles.Roles.Modifier;

public static class Mini
{
    public const float defaultColliderRadius = 0.2233912f;
    public const float defaultColliderOffset = 0.3636057f;
    public static PlayerControl mini;
    public static Color color = Color.yellow;

    public static float growingUpDuration = 400f;
    public static bool isGrowingUpInMeeting = true;
    public static bool triggerMiniLose;

    public static float accumulatedGrowthTime;
    private static float lastUpdateTime = -1f;

    public static bool isGrownUp => growingProgress == 1f;
    public static float Age => Mathf.Clamp01(growingProgress) * 18f;
    public static float Multiplier => mini != null && mini.AmOwner ? isGrownUp ? 0.66f : 2f : 1f;
    public static float growingProgress => Mathf.Clamp01(accumulatedGrowthTime / growingUpDuration);

    public static void clearAndReload()
    {
        mini = null;
        triggerMiniLose = false;
        growingUpDuration = CustomOptionHolder.modifierMiniGrowingUpDuration.GetFloat();
        isGrowingUpInMeeting = CustomOptionHolder.modifierMiniGrowingUpInMeeting.GetBool();
        accumulatedGrowthTime = 0f;
        lastUpdateTime = -1f;
        _hasGrownUp = false;
    }

    private static bool _hasGrownUp;
    public static void Update()
    {
        if (mini == null) return;
        float currentTime = Time.time;

        if (lastUpdateTime < 0f)
        {
            lastUpdateTime = currentTime;
            return;
        }

        if (!isGrowingUpInMeeting && InMeeting)
        {
            lastUpdateTime = currentTime;
            return;
        }

        float delta = currentTime - lastUpdateTime;
        lastUpdateTime = currentTime;

        accumulatedGrowthTime += delta;

        if (accumulatedGrowthTime > growingUpDuration)
        {
            accumulatedGrowthTime = growingUpDuration;
        }

        if (isGrownUp && !_hasGrownUp && !InMeeting)
        {
            _hasGrownUp = true;

            foreach (var button in CustomButton.Buttons)
            {
                if (button?.actionButton == null || !button.HasButton.Invoke()) continue;

                float originalMaxTimer = button._MaxTimer; // 未折算值
                if (originalMaxTimer <= 0f) continue;

                float percentage = button.Timer / originalMaxTimer;
                float adjustedTimer = button.MaxTimer * percentage;
                button.SetTimer(adjustedTimer);
            }
        }
    }
}
