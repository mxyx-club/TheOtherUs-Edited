using TheOtherRoles.Objects;

namespace TheOtherRoles.Roles.Modifier;

public class Radar : ModifierBase
{
    public static Color color = Color.yellow;

    public static readonly RoleInfo roleinfo = new(
        typeof(Radar),
        (p) => new Radar(p),
        RoleId.Radar,
        "Radar",
        color,
        402600
    );

    public Radar(PlayerControl p) : base(p, roleinfo) { }

    public Arrow localArrows;
    public PlayerControl ClosestPlayer;

    public override void Initialize()
    {
        localArrows?.arrow?.Destroy();
        localArrows = null;
    }

    public override void OnFixedUpdate(PlayerControl player)
    {
        if (InMeeting) return;
        if (Player.IsDead() && localArrows != null)
        {
            localArrows.arrow.Destroy();
            return;
        }

        if (PlayerControl.LocalPlayer.IsAlive())
        {
            ClosestPlayer = GetClosestPlayer(PlayerControl.LocalPlayer, PlayerControl.AllPlayerControls.ToList());

            if (Player.IsAlive())
            {
                localArrows ??= new Arrow(color);
                localArrows.arrow.SetActive(true);
            }

            localArrows?.Update(ClosestPlayer.transform.position);
        }
    }
}
