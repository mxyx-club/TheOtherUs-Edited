using TheOtherRoles.Objects;
using static TheOtherRoles.Options.ModOption;

namespace TheOtherRoles.Roles.Neutral;

public class Amnisiac
{
    public static List<PlayerControl> Player = new();
    public static List<Arrow> localArrows = new();
    public static Color color = new(0.5f, 0.7f, 1f, 1f);

    public static bool showArrows = true;
    public static bool resetRole;

    public static Sprite buttonSprite = new ResourceSprite("Remember.png");

    public static void clearAndReload()
    {
        Player.Clear();
        showArrows = CustomOptionHolder.amnisiacShowArrows.GetBool();
        resetRole = CustomOptionHolder.amnisiacResetRole.GetBool();

        foreach (var arrow in localArrows)
            if (arrow?.arrow != null)
                UObject.Destroy(arrow.arrow);
        localArrows.Clear();
    }

    public static void TakeRole(byte targetId, byte playerId)
    {
        var target = PlayerById(targetId);
        var local = PlayerById(playerId);
        if (target == null || local == null) return;
        var role = RoleInfo.getRoleInfoForPlayer(target, false, false).FirstOrDefault();
        if (role == null) return;

        RPCProcedure.ResetRole(role.roleId, target, resetRole);
        if (role.roleId == RoleId.Avenger) role = RoleInfo.jester;
        RPCProcedure.setRole(local.PlayerId, (byte)role.roleId);


        if (PlayerControl.LocalPlayer == Arsonist.arsonist)
        {
            var playerCounter = 0;
            var bottomLeft = new Vector3(
                -FastDestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition.x,
                FastDestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition.y,
                FastDestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition.z);
            foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                if (playerIcons.ContainsKey(p.PlayerId) && p != Arsonist.arsonist)
                {
                    //Arsonist.poolIcons.Add(p);
                    if (Arsonist.dousedPlayers.Contains(p))
                        playerIcons[p.PlayerId].setSemiTransparent(false);
                    else
                        playerIcons[p.PlayerId].setSemiTransparent(true);

                    playerIcons[p.PlayerId].transform.localPosition = bottomLeft +
                                                                      new Vector3(-0.25f, -0.25f, 0) +
                                                                      (Vector3.right * playerCounter++ * 0.35f);
                    playerIcons[p.PlayerId].transform.localScale = Vector3.one * 0.2f;
                    playerIcons[p.PlayerId].gameObject.SetActive(true);
                }
        }
        else if (PlayerControl.LocalPlayer == BountyHunter.bountyHunter)
        {
            BountyHunter.bountyUpdateTimer = 0f;

            var bottomLeft =
                new Vector3(-FastDestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition.x,
                    FastDestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition.y,
                    FastDestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition.z) +
                new Vector3(-0.25f, 1f, 0);
            BountyHunter.cooldownText =
                UObject.Instantiate(FastDestroyableSingleton<HudManager>.Instance.KillButton.cooldownTimerText,
                    FastDestroyableSingleton<HudManager>.Instance.transform);
            BountyHunter.cooldownText.alignment = TextAlignmentOptions.Center;
            BountyHunter.cooldownText.transform.localPosition = bottomLeft + new Vector3(0f, -1f, -1f);
            BountyHunter.cooldownText.gameObject.SetActive(true);

            foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                if (playerIcons.ContainsKey(p.PlayerId))
                {
                    playerIcons[p.PlayerId].setSemiTransparent(false);
                    playerIcons[p.PlayerId].transform.localPosition = bottomLeft + new Vector3(0f, -1f, 0);
                    playerIcons[p.PlayerId].transform.localScale = Vector3.one * 0.4f;
                    playerIcons[p.PlayerId].gameObject.SetActive(false);
                }
        }
        else if (Sheriff.formerDeputy == target)
        {
            Sheriff.formerDeputy = local;
        }

        Player.RemoveAll(x => x.PlayerId == local.PlayerId);
        foreach (var arrow in localArrows) UObject.Destroy(arrow.arrow);
        localArrows.Clear();
    }

}
