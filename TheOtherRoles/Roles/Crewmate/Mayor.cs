namespace TheOtherRoles.Roles.Crewmate;

public static class Mayor
{
    public static PlayerControl mayor;
    public static Color color = new Color32(32, 77, 66, byte.MaxValue);
    public static Sprite emergencySprite = new ResourceSprite("EmergencyButton.png", 550f);

    public static Sprite MeetingLeft = new ResourceSprite("MeetingButtonLeft.png", 110f);
    public static Sprite MeetingRight = new ResourceSprite("MeetingButtonRight.png", 110f);

    public enum MayorMode { Regular, Revealed, StoreVotes }
    public static MayorMode Mode;
    public static bool meetingButton;
    public static bool UsedMeetingButton;
    public static int MeetingCount;

    public static int Votes;
    public static bool MultiVote;
    public static bool Revealed;
    public static int vision = 5;
    public static bool VoteCountToggle;
    public static bool AnonymousVote { get => ((int)Mode != 1 || (Mode == 0 && MultiVote)) && field; set; }

    // Store Votes
    public static float MyVotes;
    public static int CurrentVote;
    public static float InitialVotes => CustomOptionHolder.mayorInitialVotes.GetFloat();
    public static float AddVotes => CustomOptionHolder.mayorAddVotes.GetFloat();
    public static int MaxVotes => CustomOptionHolder.mayorMaxVotes.GetInt();
    public static int VoteLimit => CustomOptionHolder.mayorVoteLimit.GetInt();



    public static int GetVotes()
    {
        var votes = 1;
        switch (Mode)
        {
            case MayorMode.Regular:
                votes = MultiVote ? Votes : 1;
                break;
            case MayorMode.Revealed:
                votes = Revealed ? Votes : 1;
                break;
            case MayorMode.StoreVotes:
                votes = CurrentVote;
                break;
            default:
                break;
        }
        return votes;
    }

    public static void clearAndReload()
    {
        mayor = null;
        Mode = (MayorMode)CustomOptionHolder.mayorMode.GetSelection();
        meetingButton = CustomOptionHolder.mayorMeetingButton.GetBool();
        UsedMeetingButton = false;
        MeetingCount = 0;

        // Regular
        Votes = CustomOptionHolder.mayorVote.GetInt();
        AnonymousVote = CustomOptionHolder.mayorAnonymousVote.GetBool();
        VoteCountToggle = CustomOptionHolder.mayorMultiVoting.GetBool();
        MultiVote = !VoteCountToggle;

        // Revealed
        Revealed = false;
        vision = CustomOptionHolder.mayorRevealVision.GetSelection() + 2;

        // Store Votes
        MyVotes = 0;
        CurrentVote = 0;
    }

    [HarmonyPatch]
    public static class Mayor_Patch
    {
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start)), HarmonyPostfix]
        public static void MeetingStart(MeetingHud __instance)
        {
            MeetingCount++;
            if (mayor.IsDead() || Mode != MayorMode.StoreVotes || !mayor.AmOwner) return;

            MyVotes -= CurrentVote;
            MyVotes += MeetingCount == 1 ? InitialVotes : AddVotes;
            CurrentVote = 0;

            var binder = UnityHelper.CreateObject("MayorButtons", MeetingHud.Instance.SkipVoteButton.transform.parent, MeetingHud.Instance.SkipVoteButton.transform.localPosition);

            var countText = UObject.Instantiate(MeetingHud.Instance.TitleText, binder.transform);
            countText.gameObject.SetActive(true);
            countText.gameObject.GetComponent<TextTranslatorTMP>().enabled = false;
            countText.alignment = TextAlignmentOptions.Center;
            countText.transform.localPosition = new Vector3(3.33f, 0f);
            countText.color = Palette.White;
            countText.transform.localScale *= 0.66f;
            countText.text = "";

            countText.text = $"{CurrentVote}/{(int)MyVotes}";

            var leftRenderer = UnityHelper.CreateObject<SpriteRenderer>("MayorButton-Left", binder.transform, new Vector3(2.75f, 0f));
            leftRenderer.sprite = MeetingLeft;
            var leftButton = leftRenderer.gameObject.SetUpButton();
            leftButton.OnMouseOver.AddListener(() => leftRenderer.color = Color.gray);
            leftButton.OnMouseOut.AddListener(() => leftRenderer.color = Color.white);
            leftButton.OnClick.AddListener(() =>
            {
                UpdateVotes(false);
            });
            leftRenderer.gameObject.AddComponent<BoxCollider2D>().size = new Vector2(0.5f, 0.5f);

            var rightRenderer = UnityHelper.CreateObject<SpriteRenderer>("MayorButton-Right", binder.transform, new Vector3(4f, 0f));
            rightRenderer.sprite = MeetingRight;
            var rightButton = rightRenderer.gameObject.SetUpButton();
            rightButton.OnMouseOver.AddListener(() => rightRenderer.color = Color.gray);
            rightButton.OnMouseOut.AddListener(() => rightRenderer.color = Color.white);
            rightButton.OnClick.AddListener(() =>
            {
                UpdateVotes(true);
            });
            rightRenderer.gameObject.AddComponent<BoxCollider2D>().size = new Vector2(0.5f, 0.5f);

            void UpdateVotes(bool increment)
            {
                if (__instance.state is MeetingHud.VoteStates.Results or MeetingHud.VoteStates.Discussion or MeetingHud.VoteStates.Voted) return;
                if (mayor.IsDead()) return;
                var max = Mathf.Min((int)MyVotes, VoteLimit);
                CurrentVote = Mathf.Clamp(CurrentVote + (increment ? 1 : -1), 0, max);
                countText.text = $"{CurrentVote}/{(int)MyVotes}";

                var writer = StartRPC(CustomRPC.MayorSetVoteCount);
                writer.Write(CurrentVote);
                writer.Write(MyVotes);
                writer.EndRPC();
            }
        }
    }
}