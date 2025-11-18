namespace TheOtherRoles.Roles.Modifier;

public class GuesserModifier : ModifierBase, IGuesser
{
    public static Color color = Color.yellow;

    public static readonly RoleInfo roleinfo = new(
        typeof(GuesserModifier),
        (p) => new GuesserModifier(p),
        RoleId.Guesser,
        "Guesser",
        color,
        408000
    );

    public GuesserModifier(PlayerControl p) : base(p, roleinfo) { }
    public override RoleId[] RemoveRole { get; set; } = [RoleId.Vigilante, RoleId.Doomsayer];
    public int Charges { get; set; }

    public override void Initialize() { }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        // Add Guesser Buttons
        if (Charges > 0 && Player.IsAlive() && PlayerControl.LocalPlayer == Player && (Player.GetRole() is not RoleId.Doomsayer and not RoleId.Vigilante))
        {
            foreach (var pva in __instance.playerStates)
            {
                if (pva.AmDead || pva.TargetPlayerId == PlayerControl.LocalPlayer.PlayerId) continue;

                if (Player.TryGetRole<Eraser>(out var eraser) && eraser.alreadyErased.Contains(pva.TargetPlayerId)) continue;

                var template = pva.Buttons.transform.Find("CancelButton").gameObject;
                var targetBox = UObject.Instantiate(template, pva.transform);
                targetBox.name = "ShootButton";
                targetBox.transform.localPosition = new Vector3(-0.95f, 0.03f, -1.3f);
                var renderer = targetBox.GetComponent<SpriteRenderer>();
                renderer.sprite = HandleGuesser.targetSprite;
                var button = targetBox.GetComponent<PassiveButton>();
                button.OnClick.RemoveAllListeners();
                button.OnClick.AddListener((Action)(() => Guesser.guesserOnClick(pva, __instance)));
            }
        }
    }
}
