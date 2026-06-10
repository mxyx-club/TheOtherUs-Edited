namespace TheOtherRoles.Roles.Neutral;

public static class Akujo
{
    public static Color color = new Color32(142, 69, 147, byte.MaxValue);
    public static PlayerControl akujo;
    public static PlayerControl honmei;
    public static List<PlayerControl> keeps = new();
    public static PlayerControl currentTarget;
    public static DateTime startTime;

    public static float timeLimit = 1300f;
    public static bool knowsRoles = true;
    public static bool honmeiCannotFollowWin;
    public static bool honmeiOptimizeWin;
    public static int timeLeft;
    public static bool forceKeeps;
    public static int keepsLeft;
    public static int numKeeps;

    public static bool hasUsedUnifiedVote;               // 整局是否已使用（成功消耗）
    public static bool isUnifiedVoteActiveThisMeeting;   // 本次会议是否激活同步（仅用于投票阶段）
    public static Sprite unifiedButtonSprite = new ResourceSprite("Calling.png", 150);

    public static Sprite honmeiSprite = new ResourceSprite("AkujoHonmeiButton.png");
    public static Sprite keepSprite = new ResourceSprite("AkujoKeepButton.png");

    public static bool IsKillerLover()
    {
        return honmei.IsAlive() && honmei.IsKiller();
    }

    public static bool isAkujoTeam(PlayerControl player)
    {
        return player != null && (player == akujo || player == honmei);
    }

    public static PlayerControl otherLover(PlayerControl player)
    {
        if (akujo == null || honmei == null) return null;
        if (player == akujo) return honmei;
        if (player == honmei) return akujo;
        return null;
    }

    public static void breakLovers(PlayerControl target)
    {
        if (Lovers.isLover(target))
        {
            var otherLover = Lovers.otherLover(target);
            if (otherLover != null)
            {
                Lovers.clearAndReload();
                otherLover.MurderPlayer(otherLover, MurderResultFlags.Succeeded);
                PlayerData.SetDeathReason(otherLover, CustomDeathReason.LoveStolen, akujo);
            }
        }
    }

    public static void clearAndReload()
    {
        akujo = null;
        honmei = null;
        keeps.Clear();
        currentTarget = null;
        startTime = DateTime.UtcNow;
        timeLimit = CustomOptionHolder.akujoTimeLimit.GetFloat();
        forceKeeps = CustomOptionHolder.akujoForceKeeps.GetBool();
        knowsRoles = CustomOptionHolder.akujoKnowsRoles.GetBool();
        honmeiCannotFollowWin = CustomOptionHolder.akujoHonmeiCannotFollowWin.GetBool();
        honmeiOptimizeWin = CustomOptionHolder.akujoHonmeiOptimizeWin.GetBool();
        timeLeft = (int)Math.Ceiling(timeLimit - (DateTime.UtcNow - startTime).TotalSeconds);
        numKeeps = Math.Min(CustomOptionHolder.akujoNumKeeps.GetInt(), PlayerControl.AllPlayerControls.Count - 2);
        keepsLeft = numKeeps;
        hasUsedUnifiedVote = false;
        isUnifiedVoteActiveThisMeeting = false;
    }

    [HarmonyPatch]
    public static class Akujo_Patch
    {
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start)), HarmonyPostfix]
        public static void MeetingStart(MeetingHud __instance)
        {
            if (akujo.IsAlive() && akujo == PlayerControl.LocalPlayer && !hasUsedUnifiedVote && PlayerControl.LocalPlayer.CanUseMeetingAbility())
            {
                if (honmei.IsDead()) return;

                bool hasVoted = false;
                foreach (var pva in __instance.playerStates)
                    if (pva.TargetPlayerId == akujo.PlayerId && pva.DidVote) { hasVoted = true; break; }
                if (hasVoted) return;

                if (__instance.transform.Find("AkujoButtons") != null) return;

                var binder = UnityHelper.CreateObject("AkujoButtons", __instance.SkipVoteButton.transform.parent, __instance.SkipVoteButton.transform.localPosition);
                binder.transform.localPosition = new Vector3(0f, -2.5f, 0f);

                var renderer = UnityHelper.CreateObject<SpriteRenderer>("AkujoUnifiedButton", binder.transform, Vector3.zero);
                renderer.sprite = unifiedButtonSprite;
                renderer.color = Color.white;
                renderer.sortingOrder = 200;

                var collider = renderer.gameObject.AddComponent<BoxCollider2D>();
                collider.size = new Vector2(0.5f, 0.5f);

                var button = renderer.gameObject.SetUpButton();
                button.OnMouseOver.AddListener(() => renderer.color = Color.gray);
                button.OnMouseOut.AddListener(() => renderer.color = Color.white);
                button.OnClick.AddListener(() =>
                {
                    if (__instance.state is MeetingHud.VoteStates.Discussion or MeetingHud.VoteStates.Results) return;

                    if (Gaoler.IsPrisoner(PlayerControl.LocalPlayer))
                    {
                        FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, GetString("发动失败，您已被狱卒关押 \n--现在这里只允许我的声音"));
                        return;
                    }

                    if (hasUsedUnifiedVote) return;
                    hasUsedUnifiedVote = true;
                    isUnifiedVoteActiveThisMeeting = true;

                    FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, GetString("你愿意为我献上一切,包括生命吗? \n本轮会议，真爱和所有备胎将会跟随你的投票"));

                    var writer = StartRPC(CustomRPC.AkujoSetUnifiedVote);
                    writer.Write(true);
                    writer.EndRPC();
                    RPCProcedure.AkujoSetUnifiedVote(true);

                    binder.Destroy();
                });
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.OnDestroy)), HarmonyPostfix]
        public static void MeetingEnd(MeetingHud __instance)
        {
            var binder = GameObject.Find("AkujoButtons");
            if (binder != null) binder.Destroy();

            // 如果使用了技能但未投票，退还
            if (akujo != null && akujo == PlayerControl.LocalPlayer && hasUsedUnifiedVote)
            {
                bool hasVoted = false;
                if (MeetingHud.Instance != null)
                {
                    foreach (var pva in MeetingHud.Instance.playerStates)
                    {
                        if (pva.TargetPlayerId == akujo.PlayerId && pva.DidVote)
                        {
                            hasVoted = true;
                            break;
                        }
                    }
                }
                if (!hasVoted)
                {
                    hasUsedUnifiedVote = false;
                    isUnifiedVoteActiveThisMeeting = false;
                    var writer = StartRPC(CustomRPC.AkujoSetUnifiedVote);
                    writer.Write(false);
                    writer.EndRPC();
                    RPCProcedure.AkujoSetUnifiedVote(false);
                }
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update)), HarmonyPostfix]
        public static void MeetingUpdate(MeetingHud __instance)
        {
            if (akujo != null && akujo == PlayerControl.LocalPlayer && !hasUsedUnifiedVote)
            {
                foreach (var pva in __instance.playerStates)
                {
                    if (pva.TargetPlayerId == akujo.PlayerId && pva.DidVote)
                    {
                        var binder = GameObject.Find("AkujoButtons");
                        if (binder != null) UObject.Destroy(binder);
                        break;
                    }
                }
            }
        }
    }
}
