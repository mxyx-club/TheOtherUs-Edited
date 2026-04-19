namespace TheOtherRoles.Roles.Neutral;

public class SoulSight
{
    public static PlayerControl Player;
    public static Color color = new Color32(162, 141, 207, byte.MaxValue);
    public static Sprite ButtonSprite = new ResourceSprite("soulSightRevive.png");

    public static float RespawnTimer;
    public static int ScoreToWin;
    public static float Cooldown;
    public static bool SoloWin;

    public static bool IsKilled;
    public static int Score;
    public static bool TriggerWin;
    public static bool Reviveing;
    public static bool CanRevive;

    public static void Suicide(PlayerControl player)
    {
        Reviveing = true;
        player?.MyPhysics?.StartCoroutine(player.KillAnimations.First().CoPerformKill(player, player));
    }

    [HarmonyPatch]
    public class SoulSight_Patch
    {
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update)), HarmonyPostfix]
        public static void Update(HudManager __instance)
        {
            if (Player == null || Player != PlayerControl.LocalPlayer) return;
            if (CanRevive || Reviveing)
            {
                CanSeeGhostInfo = false;
                __instance.ShadowQuad?.gameObject?.SetActive(true);
                __instance.AbilityButton.gameObject?.SetActive(false);
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start)), HarmonyPostfix]
        public static void StartMeeting()
        {
            if (Player == null) return;
            if (Reviveing && !IsKilled)
            {
                Score++;
            }
            Message($"Reviveing: {Reviveing}");

            if (Score >= ScoreToWin) TriggerWin = true;
            if (Player.AmOwner)
            {
                var writer = StartRPC(CustomRPC.SoulSightScore);
                writer.Write(Score);
                writer.Write(TriggerWin);
                writer.EndRPC();
            }
            Message($"Score: {Score} TriggerWin {TriggerWin}");
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Exiled)), HarmonyPostfix]
        public static void Exiled(PlayerControl __instance)
        {
            if (Player == null) return;
            if (__instance == Player)
            {
                CanRevive = false;
                Reviveing = false;
            }
            return;
        }

        //[HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp)), HarmonyPostfix]
        public static void EndMeeting(GameData.PlayerInfo exiled)
        {
            if (Player == null) return;

            if (Reviveing) Player?.Revive();
            Reviveing = false;
            IsKilled = false;

            Message($"Reviveing: {Reviveing}");
        }
    }

    public static void ClearAndReload()
    {
        /*Player = null;
        Score = 0;
        Reviveing = false;
        TriggerWin = false;
        CanRevive = true;
        IsKilled = false;
        SoloWin = CustomOptionHolder.soulSightSoloWin.GetBool();
        Cooldown = CustomOptionHolder.soulSightCooldown.GetFloat();
        RespawnTimer = CustomOptionHolder.soulSightRespawnTimer.GetFloat();
        ScoreToWin = CustomOptionHolder.soulSightScoreToWin.GetInt();*/
    }
}
