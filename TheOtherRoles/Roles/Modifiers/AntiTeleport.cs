namespace TheOtherRoles.Roles.Modifier;

public class AntiTeleport : ModifierBase
{
    public static Color color = Color.yellow;
    public static readonly RoleInfo roleinfo = new(
        typeof(AntiTeleport),
        (p) => new AntiTeleport(p),
        RoleId.AntiTeleport,
        "AntiTeleport",
        color,
        401300
    );

    public AntiTeleport(PlayerControl p) : base(p, roleinfo) { }

    public Vector3 position;

    public override void Initialize()
    {
        position = Vector3.zero;
    }

    public void setPosition()
    {
        if (position == Vector3.zero) return;
        // Check if this has been set, otherwise first spawn on submerged will fail
        PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(position);
        if (SubmergedCompatibility.IsSubmerged) SubmergedCompatibility.ChangeFloor(position.y > -7);
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        position = Player.transform.position;
    }

    public override void OnExiledWrapUp(GameData.PlayerInfo exiled)
    {
        setPosition();
    }

    public override void OnGameStart()
    {
        setPosition();
    }
}
