using static UnityEngine.GraphicsBuffer;

namespace TheOtherRoles.Roles.Impostor;

public class Gaoler
{
    public static PlayerControl Player;
    public static Color color = Palette.ImpostorRed;
    public static Sprite TargetSprite = new ResourceSprite("TargetIcon.png", 150);

    // 选项
    public static float selectionWindow = 60f;
    public static int maxUses = 2;
    public static bool canPassMapAfterDeath;
    public static bool canJailSamePlayerConsecutively = true;

    // 运行时状态
    public static float timeLeft;
    public static DateTime meetingStartTime;
    public static int remainingUses;
    public static PlayerControl currentPrisoner;
    public static byte lastJailedPlayerId = byte.MaxValue;
    public static bool hasSelectedThisMeeting;
    public static bool endMeetingSelection;
    public static PlayerControl hasMapPlayer;

    public static void ClearAndReload()
    {
        Player = null;
        remainingUses = maxUses;
        currentPrisoner = null;
        hasMapPlayer = null;
        lastJailedPlayerId = byte.MaxValue;
        hasSelectedThisMeeting = false;

        selectionWindow = CustomOptionHolder.gaolerSelectionWindow.GetFloat();
        maxUses = CustomOptionHolder.gaolerMaxUses.GetInt();
        canPassMapAfterDeath = CustomOptionHolder.gaolerCanPassMapAfterDeath.GetBool();
        canJailSamePlayerConsecutively = CustomOptionHolder.gaolerCanJailSamePlayerConsecutively.GetBool();
    }

    public static void OnImpostorDie(PlayerControl player)
    {
        if (!canPassMapAfterDeath) return;
        if (Player == null && hasMapPlayer != null)
        {
            hasMapPlayer = null;
            return;
        }

        if (Player.IsAlive() || (hasMapPlayer.IsAlive() && hasMapPlayer.IsImpostor())) return;

        var allImpostor = GameDataManager.Instance.AllPlayerControl.Where(x => x.IsImpostor() && x.IsAlive() && x != Yoyo.yoyo);
        if (allImpostor.Any() && (player == Player || hasMapPlayer.IsDead() || !hasMapPlayer.IsImpostor()))
        {
            hasMapPlayer = allImpostor.Random();
        }
    }

    public static bool IsPrisoner(PlayerControl player)
    {
        return player != null && currentPrisoner != null && player.PlayerId == currentPrisoner.PlayerId;
    }

    public static bool CanUseAbility()
    {
        return Player.IsAlive() && Player == PlayerControl.LocalPlayer && remainingUses > 0 && !hasSelectedThisMeeting;
    }

    [HarmonyPatch]
    public static class Gaoler_Patch
    {
        private static void OnGaolerJailClick(PlayerVoteArea pva, MeetingHud __instance, byte targetId)
        {
            if (!CanUseAbility()) return;
            if (endMeetingSelection) return;

            PlayerControl target = PlayerById(targetId);
            if (target == null) return;

            // 再次检查连续监禁
            if (!canJailSamePlayerConsecutively && lastJailedPlayerId == target.PlayerId) return;

            // 发送 RPC
            var writer = StartRPC(CustomRPC.GaolerMarkPrisoner);
            writer.Write(target.PlayerId);
            writer.EndRPC();
            RPCProcedure.GaolerMarkPrisoner(target.PlayerId);

            // 本地更新
            remainingUses--;
            hasSelectedThisMeeting = true;
            lastJailedPlayerId = target.PlayerId;
            currentPrisoner = target;

            // 销毁所有监禁图标
            foreach (var playerState in __instance.playerStates)
            {
                var icon = playerState.transform.FindChild("GaolerIcon");
                if (icon != null) UObject.Destroy(icon.gameObject);
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
        [HarmonyPostfix]
        public static void MeetingUpdate(MeetingHud __instance)
        {
            if (Player.IsDead() || endMeetingSelection || currentPrisoner != null) return;
            timeLeft = selectionWindow - (float)(DateTime.UtcNow - meetingStartTime).TotalSeconds;
            if (timeLeft <= 0f)
            {
                foreach (var pva in __instance.playerStates)
                {
                    var icon = pva.transform.FindChild("GaolerIcon");
                    if (icon != null) UObject.Destroy(icon.gameObject);
                }
                endMeetingSelection = true;
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
        [HarmonyPostfix]
        private static void MeetingHudStartPostfix(MeetingHud __instance)
        {
            if (!PlayerControl.LocalPlayer.CanUseMeetingAbility()) return;

            if (PlayerControl.LocalPlayer == Player && PlayerControl.LocalPlayer.IsAlive() && !hasSelectedThisMeeting && remainingUses > 0)
            {
                foreach (var pva in __instance.playerStates)
                {
                    var player = PlayerById(pva.TargetPlayerId);
                    if (player == null || player == Player || player.Data.IsDead) continue;

                    // 不可连续监禁同一玩家检查
                    if (!canJailSamePlayerConsecutively && lastJailedPlayerId == player.PlayerId) continue;

                    GameObject template = pva.Buttons.transform.Find("CancelButton").gameObject;
                    GameObject jailBox = UObject.Instantiate(template, pva.transform);
                    jailBox.name = "GaolerIcon";
                    jailBox.transform.localPosition = new Vector3(1f, 0.03f, -1f); // 位置与 Witness 相同
                    SpriteRenderer renderer = jailBox.GetComponent<SpriteRenderer>();
                    renderer.sprite = TargetSprite ?? pva.Megaphone.sprite; // 临时用地形图标
                    renderer.color = Color.white;
                    PassiveButton button = jailBox.GetComponent<PassiveButton>();
                    button.OnClick.RemoveAllListeners();
                    byte targetId = player.PlayerId;
                    button.OnClick.AddListener(() => OnGaolerJailClick(pva, __instance, targetId));
                }
                endMeetingSelection = false;
                meetingStartTime = DateTime.UtcNow;
            }
        }
    }
}
