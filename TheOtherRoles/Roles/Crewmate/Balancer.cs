using TheOtherRoles.Patches;
using UnityEngine.Events;

namespace TheOtherRoles.Roles.Crewmate;
public class Balancer
{
    public static PlayerControl balancer;
    public static Color color = new Color32(255, 128, 0, byte.MaxValue);
    public static PlayerControl currentAbilityUser;

    public static int IsAbilityUsed;
    public static int BalancerVoteTime;

    public static Sprite BackSprite = new ResourceSprite("Balancer.MeetingBack.png", 170);
    public static Sprite iconSprite = new ResourceSprite("Balancer.icon_average.png", 225);
    public static Sprite BackObjectSprite = new ResourceSprite("Balancer.FlareEffect.png", 115);
    public static Sprite eyeBackRenderSprite = new ResourceSprite("Balancer.eye-of-horus_parts.png", 115);
    public static Sprite eyeRenderSprite = new ResourceSprite("Balancer.eye-of-horus.png", 115);

    public static void clearAndReload()
    {
        balancer = null;
        currentAbilityUser = null;
        IsAbilityUsed = CustomOptionHolder.balancerCount.GetInt();
        BalancerVoteTime = CustomOptionHolder.balancerVoteTime.GetInt();
        CurrentState = BalancerState.NotBalance;
        IsDoubleExile = false;
        currentTarget = null;
        exiled1 = false;
    }

    public static SpriteRenderer BackObject;
    public static SpriteRenderer BackPictureObject;
    public static List<(SpriteRenderer, float, int)> ChainObjects;
    private static SpriteRenderer eyebackrender;
    private static SpriteRenderer eyerender;
    private static TextMeshPro textuseability;
    private static TextMeshPro textpleasevote;
    private static float textpleasetimer;
    public enum BalancerState
    {
        NotBalance,
        Animation_Chain,
        Animation_Eye,
        Animation_Open,
        WaitVote
    }
    public static BalancerState CurrentState = BalancerState.NotBalance;
    private static List<Sprite> chainsprites = new();
    private static int animIndex;
    private static int pleasevoteanimIndex;
    private static float rotate;
    private static float openMADENOtimer;
    private static bool exiled1;

    public static void FixedUpdate()
    {
        if (BackObject != null)
        {
            //切断したなら
            if (targetplayerleft.IsDead() || targetplayerright.IsDead())
            {
                PlayerControl target = null;
                if (targetplayerright == null || targetplayerright.IsDead())
                {
                    target = targetplayerleft;
                }
                if (targetplayerleft == null || targetplayerleft.IsDead())
                {
                    target = targetplayerright;
                }
                if (AmongUsClient.Instance.AmHost && !exiled1)
                    MeetingHud.Instance.RpcVotingComplete(new List<MeetingHud.VoterState>().ToArray(), target.Data, false);
                exiled1 = true;
                return;
            }
            switch (CurrentState)
            {
                case BalancerState.NotBalance:
                    return;
                case BalancerState.Animation_Chain:
                    bool flag = true;

                    MeetingHud.Instance.ClearVote();
                    MeetingHud.Instance.TitleText.gameObject.SetActive(false);
                    MeetingHud.Instance.TimerText.gameObject.SetActive(false);
                    MeetingHud.Instance.SkipVoteButton.gameObject.SetActive(false);
                    MeetingHud.Instance.SkippedVoting.SetActive(false);

                    for (int i = 0; i <= animIndex; i++)
                    {
                        var cobj = ChainObjects[i];
                        if (cobj.Item3 < chainsprites.Count)
                        {
                            cobj.Item1.sprite = chainsprites[ChainObjects[i].Item3];
                            ChainObjects[i] = (cobj.Item1, cobj.Item2, cobj.Item3 + 1);
                            flag = false;
                        }
                    }
                    if ((animIndex + 1) < ChainObjects.Count)
                        animIndex++;
                    if (flag)
                    {
                        textpleasetimer -= Time.fixedDeltaTime;
                        if (textpleasetimer <= 0)
                        {
                            CurrentState = BalancerState.Animation_Eye;
                            animIndex = 0;
                            BackObject.sprite = BackObjectSprite;
                            BackObject.color = new Color32(255, 255, 255, 0);
                            eyerender.enabled = true;
                            eyerender.color = new Color32(255, 255, 255, 0);
                            eyebackrender.enabled = true;
                            eyebackrender.color = new Color32(255, 255, 255, 0);
                            textuseability.enabled = true;
                            textuseability.color = new Color32(255, 255, 255, 0);
                            rotate = 360;
                            textpleasetimer = 0.8f;
                            pleasevoteanimIndex = 0;
                            //なんか分からんけどピッチが変だから0.9倍にして解決！(無理やり)
                            UnityHelper.PlaySound(MeetingHud.Instance.transform, UnityHelper.loadAudioClipFromResources("TheOtherRoles.Resources.Balancer.backsound.raw"), false).pitch = 0.9f;
                        }
                    }
                    break;
                case BalancerState.Animation_Eye:
                    animIndex++;
                    if (animIndex <= 40)
                    {
                        byte alpha = 255;
                        if (animIndex * 6.2f < 255)
                        {
                            alpha = (byte)(animIndex * 6.2f);
                        }
                        if (BackObject != null) BackObject.color = new Color32(255, 255, 255, alpha);
                        if (eyerender != null) eyerender.color = new Color32(255, 255, 255, alpha);
                        if (eyebackrender != null) eyebackrender.color = new Color32(255, 255, 255, alpha);
                        if (textuseability != null) textuseability.color = new Color32(255, 255, 255, alpha);
                    }
                    else
                    {
                        if (textpleasetimer > 0)
                        {
                            textpleasetimer -= Time.fixedDeltaTime;
                            if (textpleasetimer <= 0)
                            {
                                if (textpleasevote != null) textpleasevote.enabled = true;
                                if (textpleasevote != null) textpleasevote.color = new Color32(255, 255, 255, 0);
                            }
                        }
                        else if (pleasevoteanimIndex <= 20)
                        {
                            pleasevoteanimIndex++;
                            byte alpha = 255;
                            if (pleasevoteanimIndex * 13f < 255)
                            {
                                alpha = (byte)(pleasevoteanimIndex * 13f);
                            }
                            if (textpleasevote != null) textpleasevote.color = new Color32(255, 255, 255, alpha);
                            if (pleasevoteanimIndex > 20)
                            {
                                openMADENOtimer = 1f;
                            }
                        }
                        else
                        {
                            openMADENOtimer -= Time.fixedDeltaTime;
                            if (openMADENOtimer <= 0)
                            {
                                CurrentState = BalancerState.Animation_Open;
                                animIndex = 0;
                                BackPictureObject.enabled = true;
                                BackPictureObject.color = new Color32(255, 255, 255, 0);
                            }
                        }
                    }
                    if (eyebackrender != null) eyebackrender.transform.localEulerAngles = new(0, 0, rotate);
                    rotate -= 0.1f;
                    if (rotate <= 0)
                    {
                        rotate = 360;
                    }
                    break;
                case BalancerState.Animation_Open:
                    animIndex++;
                    if (animIndex <= 20)
                    {
                        byte alpha = 255;
                        if (animIndex * 16f < 255)
                        {
                            alpha = (byte)(animIndex * 16f);
                        }
                        if (BackPictureObject != null) BackPictureObject.color = new Color32(255, 255, 255, alpha);
                    }
                    Vector3 speed = new(0.6f, 0, 0);
                    foreach (var objs in ChainObjects)
                    {
                        objs.Item1.transform.localPosition -= speed;
                    }
                    eyebackrender.transform.localPosition -= speed;
                    eyerender.transform.localPosition -= speed;
                    BackObject.transform.localPosition -= speed;
                    textpleasevote.transform.localPosition -= speed;
                    textuseability.transform.localPosition -= speed;
                    if (BackObject.transform.localPosition.x <= -10)
                    {
                        CurrentState = BalancerState.WaitVote;
                        MeetingHud.Instance.HostIcon.gameObject.SetActive(false);
                        MeetingHudPatch.ShowHost.Text.gameObject.SetActive(false);
                        MeetingHud.Instance.transform.FindChild("MeetingContents/PhoneUI/baseGlass").transform.localPosition = new(0.012f, 0, 0);
                        MeetingHud.Instance.TitleText.GetComponent<TextTranslatorTMP>().enabled = false;
                        MeetingHud.Instance.TitleText.transform.localPosition = new(0, 2, -1);
                        MeetingHud.Instance.TitleText.transform.localScale = Vector3.one * 2f;
                        MeetingHud.Instance.TitleText.text = GetString("BalancerTitleTextYouVoteEither");
                        leftplayerarea.transform.localPosition = leftpos;
                        rightplayerarea.transform.localPosition = rightpos;
                        MeetingHud.Instance.discussionTimer = GameOptionsManager.Instance.CurrentGameOptions.GetInt(AmongUs.GameOptions.Int32OptionNames.VotingTime) - BalancerVoteTime;
                        MeetingHud.Instance.TimerText.gameObject.SetActive(true);
                        MeetingHud.Instance.TimerText.transform.localPosition = new(2.05f, -2, -1);
                        MeetingHud.Instance.ProceedButton.transform.localPosition = new(3.5f, -2, -1.05f);
                    }
                    break;
            }
        }
    }

    public static PlayerControl targetplayerleft;
    public static PlayerControl targetplayerright;
    private static readonly Vector3 leftpos = new(-2.0f, -0.62f, -0.9f);
    private static readonly Vector3 rightpos = new(1.4f, -0.62f, -0.9f);
    private static PlayerVoteArea leftplayerarea;
    private static PlayerVoteArea rightplayerarea;
    public static bool IsDoubleExile;
    public static PlayerControl currentTarget;

    public static void WrapUp(PlayerControl exiled)
    {
        if (exiled != null)
        {
            if (IsDoubleExile && exiled.PlayerId == targetplayerleft.PlayerId) return;
        }
        targetplayerright = null;
        targetplayerleft = null;
        IsDoubleExile = false;
        exiled1 = false;
        currentAbilityUser = null;
        CurrentState = BalancerState.NotBalance;
        currentTarget = null;
    }
    public static void StartAbility(PlayerControl source, PlayerControl player1, PlayerControl player2)
    {
        MeetingHud.Instance.discussionTimer = GameOptionsManager.Instance.CurrentGameOptions
            .GetInt(AmongUs.GameOptions.Int32OptionNames.VotingTime) - BalancerVoteTime - 6.5f;
        currentAbilityUser = source;
        targetplayerleft = player1;
        targetplayerright = player2;
        CurrentState = BalancerState.Animation_Chain;
        MeetingHud.Instance.ClearVote();
        if (Guesser.guesserUI != null) Guesser.guesserUIExitButton.OnClick.Invoke();
        foreach (PlayerVoteArea area in MeetingHud.Instance.playerStates)
        {
            area.transform.localPosition = new(999, 999, 999);

            if (area.TargetPlayerId == targetplayerleft.PlayerId)
            {
                leftplayerarea = area;
            }
            else if (area.TargetPlayerId == targetplayerright.PlayerId)
            {
                rightplayerarea = area;
            }
        }

        BackObject = new GameObject("BackObject").AddComponent<SpriteRenderer>();
        BackObject.transform.parent = MeetingHud.Instance.transform;
        // UIレイヤーに移動
        BackObject.gameObject.layer = 5;
        // 位置移動
        BackObject.transform.localPosition = new(0, 0, -11);
        BackObject.transform.localScale = new(2f, 2f, 2f);

        BackPictureObject = new GameObject("BackPictureObject").AddComponent<SpriteRenderer>();
        BackPictureObject.transform.parent = MeetingHud.Instance.transform;
        // UIレイヤーに移動
        BackPictureObject.gameObject.layer = 5;
        // 位置移動
        BackPictureObject.transform.localPosition = new(0, 0, -0.1f);
        BackPictureObject.transform.localScale = Vector3.one * 1.65f;
        //初期化
        BackPictureObject.enabled = false;
        BackPictureObject.sprite = BackSprite;

        // アニメーションの初期化
        animIndex = 0;
        if (chainsprites.Count <= 0)
        {
            for (int i = 0; i < 15; i++)
            {
                chainsprites.Add(UnityHelper.loadSpriteFromResources($"TheOtherRoles.Resources.Balancer.chain.average_anim_chain_0{i + 16}.png", 115f));
            }
        }
        eyebackrender = new GameObject("EyeBackRender").AddComponent<SpriteRenderer>();
        eyebackrender.sprite = eyeBackRenderSprite;
        eyebackrender.enabled = false;
        eyebackrender.gameObject.layer = 5;
        eyebackrender.transform.parent = MeetingHud.Instance.transform;
        eyebackrender.transform.localScale = new(1.7f, 1.7f, 1.7f);
        eyebackrender.transform.localPosition = new(0, 0, -30f);
        eyerender = new GameObject("EyeRender").AddComponent<SpriteRenderer>();
        eyerender.sprite = eyeRenderSprite;
        eyerender.enabled = false;
        eyerender.gameObject.layer = 5;
        eyerender.transform.parent = MeetingHud.Instance.transform;
        eyerender.transform.localScale = new(1.7f, 1.7f, 1.7f);
        eyerender.transform.localPosition = new(0, 0.25f, -30f);
        ChainObjects = new();
        int objectnum = 11;
        for (int i = 0; i < objectnum; i++)
        {
            ChainObjects.Add((createchain(URandom.Range(1.8f, -1.7f), URandom.Range(-15f, 15f)), 0f, 0));
        }
        ChainObjects.Add((createchain(0, 0, -12f), 0f, 0));
        textuseability = createtext(new(0, 2.1f, -30), GetString("BalancerAbilityUseText"), 12);
        textuseability.enabled = false;
        textpleasevote = createtext(new(0, -1f, -30f), GetString("BalancerVoteText"), 8);
        textpleasevote.enabled = false;
        textpleasetimer = 0.35f;
        SoundManager.Instance.PlaySound(UnityHelper.loadAudioClipFromResources("TheOtherRoles.Resources.Balancer.chain.raw"), false);
    }

    private static TextMeshPro createtext(Vector3 pos, string text, float fontsize)
    {
        TextMeshPro tmp = UObject.Instantiate(MeetingHud.Instance.TitleText, MeetingHud.Instance.transform);
        tmp.text = text;
        tmp.gameObject.gameObject.layer = 5;
        tmp.transform.localScale = Vector3.one;
        tmp.transform.localPosition = pos;
        tmp.fontSize = fontsize;
        tmp.fontSizeMax = fontsize;
        tmp.fontSizeMin = fontsize;
        tmp.enableWordWrapping = false;
        tmp.gameObject.SetActive(true);
        UObject.Destroy(tmp.GetComponent<TextTranslatorTMP>());
        return tmp;
    }

    private static SpriteRenderer createchain(float pos, float rotate, float zpos = 7f)
    {

        SpriteRenderer obj = new GameObject("Chain").AddComponent<SpriteRenderer>();
        obj.transform.parent = MeetingHud.Instance.transform;
        // UIレイヤーに移動
        obj.gameObject.layer = 5;
        // 位置移動
        obj.transform.localPosition = new(0, pos, zpos);
        obj.transform.localScale = new(2f, 1.7f, 2f);
        obj.transform.Rotate(new(0, 0, rotate));
        return obj;
    }

    internal static void UpdateButton(MeetingHud __instance)
    {
        if (PlayerControl.LocalPlayer.IsDead() || targetplayerright != null)
        {
            __instance.playerStates.ForEach(x =>
            {
                var icon = x.transform.FindChild("BalancerButton");
                if (icon != null) UObject.Destroy(icon.gameObject);
            });
        }
    }

    private static void BalancerOnClick(int Index, MeetingHud __instance)
    {
        if (currentAbilityUser != null || __instance.state is MeetingHud.VoteStates.NotVoted or MeetingHud.VoteStates.Results) return;
        var Target = playerById(__instance.playerStates[Index].TargetPlayerId);

        if (currentTarget == null && Target.IsAlive())
        {
            currentTarget = Target;
            __instance.playerStates.ForEach(x =>
            {
                if (x.TargetPlayerId == currentTarget.PlayerId && x.transform.FindChild("BalancerButton") != null)
                    x.transform.FindChild("BalancerButton")?.gameObject.SetActive(false);
            });
            return;
        }

        if (balancer.IsDead() || Target.IsDead() || IsAbilityUsed <= 0) return;

        var writer = StartRPC(CustomRPC.BalancerBalance);
        writer.Write(PlayerControl.LocalPlayer.PlayerId);
        writer.Write(currentTarget.PlayerId);
        writer.Write(Target.PlayerId);
        writer.EndRPC();
        RPCProcedure.balancerBalance(PlayerControl.LocalPlayer.PlayerId, currentTarget.PlayerId, Target.PlayerId);

        __instance.playerStates.ForEach(x =>
        {
            if (x.transform.FindChild("BalancerButton") != null) UObject.Destroy(x.transform.FindChild("BalancerButton").gameObject);
        });
    }

    internal static void MeetingHudStartPostfix(MeetingHud __instance)
    {
        if (PlayerControl.LocalPlayer.IsAlive() && IsAbilityUsed > 0)
        {
            for (int i = 0; i < __instance.playerStates.Length; i++)
            {
                PlayerVoteArea playerVoteArea = __instance.playerStates[i];
                var player = playerById(__instance.playerStates[i].TargetPlayerId);
                if (player.IsAlive())
                {
                    GameObject template = playerVoteArea.Buttons.transform.Find("CancelButton").gameObject;
                    GameObject targetBox = UObject.Instantiate(template, playerVoteArea.transform);
                    targetBox.name = "BalancerButton";
                    targetBox.transform.localPosition = new Vector3(1.1f, 0.03f, -1f);
                    SpriteRenderer renderer = targetBox.GetComponent<SpriteRenderer>();
                    renderer.sprite = iconSprite;
                    PassiveButton button = targetBox.GetComponent<PassiveButton>();
                    button.OnClick.RemoveAllListeners();
                    int copiedIndex = i;
                    button.OnClick.AddListener((UnityAction)(() => BalancerOnClick(copiedIndex, __instance)));
                }
            }
        }
    }
}
