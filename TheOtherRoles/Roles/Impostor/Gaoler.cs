using Steamworks;

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

        var allImpostor = PlayerData.AllPlayerControl.Where(x => x.IsImpostor() && x.IsAlive() && x != Yoyo.yoyo);
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
}
