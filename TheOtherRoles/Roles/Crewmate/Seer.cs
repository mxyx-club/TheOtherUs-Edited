namespace TheOtherRoles.Roles.Crewmate;

public class Seer : RoleBase
{
    public static Color color = new Color32(97, 178, 108, byte.MaxValue);

    public static readonly RoleInfo roleinfo = new(
        typeof(Seer),
        (p) => new Seer(p),
        RoleId.Seer,
        RoleType.Crewmate,
        "Seer",
        color,
        302400,
        AddOptions
    );

    public Seer(PlayerControl p) : base(p, roleinfo) { }

    public HashSet<Vector3> deadBodyPositions = new();

    public static float soulDuration = 15f;
    public static bool limitSoulDuration;
    public static int mode;

    public static Sprite soulSprite = new ResourceSprite("Soul.png", 500f);

    public static CustomOption seerMode;
    public static CustomOption seerLimitSoulDuration;
    public static CustomOption seerSoulDuration;

    private static void AddOptions()
    {
        var configId = roleinfo.ConfigId + 2;
        seerMode = CustomOption.Create(configId++, CustomOptionType.Crewmate, "seerMode", ["seerMode1", "seerMode2", "seerMode3"], roleinfo.RoleOption);
        seerLimitSoulDuration = CustomOption.Create(configId++, CustomOptionType.Crewmate, "seerLimitSoulDuration", false, roleinfo.RoleOption);
        seerSoulDuration = CustomOption.Create(configId++, CustomOptionType.Crewmate, "seerSoulDuration", 30f, 0f, 120f, 2.5f, seerLimitSoulDuration);
    }

    public override void Initialize()
    {
        Player = null;
        deadBodyPositions.Clear();
        limitSoulDuration = seerLimitSoulDuration.GetBool();
        soulDuration = seerSoulDuration.GetFloat();
        mode = seerMode.GetSelection();
    }

    public override void OnMurderPlayer(MurderInfo Info)
    {
        // Seer show flash and add dead player position
        if ((Player == PlayerControl.LocalPlayer || CanSeeGhostInfo) && Player.IsAlive() && Player != Info.Target && mode <= 1)
            showFlash(new Color32(42, 187, 245, 255), message: GetString("seerShowInfoText"));
        deadBodyPositions.Add(Info.Target.transform.position);

    }

    public override void OnExiledWrapUp(GameData.PlayerInfo exiled)
    {
        // Seer spawn souls
        if (deadBodyPositions != null && Player != null && PlayerControl.LocalPlayer == Player && (mode == 0 || mode == 2))
        {
            foreach (var pos in deadBodyPositions)
            {
                var soul = new GameObject();
                //soul.transform.position = pos;
                soul.transform.position = new Vector3(pos.x, pos.y, (pos.y / 1000) - 1f);
                soul.layer = 5;
                var rend = soul.AddComponent<SpriteRenderer>();
                soul.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
                rend.sprite = soulSprite;

                if (limitSoulDuration)
                {
                    FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(soulDuration,
                        new Action<float>(p =>
                        {
                            if (rend != null)
                            {
                                var tmp = rend.color;
                                tmp.a = Mathf.Clamp01(1 - p);
                                rend.color = tmp;
                            }

                            if (p == 1f && rend != null && rend.gameObject != null) UObject.Destroy(rend.gameObject);
                        })));
                }
            }
            deadBodyPositions = new();
        }

    }
}
