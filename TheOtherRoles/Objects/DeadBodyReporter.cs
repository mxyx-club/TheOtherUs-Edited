namespace TheOtherRoles.Objects;

public class DeadBodyReporter : CustomObjectBase<DeadBodyReporter>
{
    public static Sprite BackgroundSprite = new ResourceSprite("BombBackground.png", 110f / 10f);
    public PlayerControl Killer;
    public PlayerControl Target;
    public DeadBody DeadBody;
    public GameObject Background;
    public bool Reported;
    public bool LocalCanSeeBodies;
    public static float ReportDistance = 1.5f;

    public DeadBodyReporter(PlayerControl player, DeadBody deadBody)
    {
        Killer = player;
        Target = PlayerById(deadBody.ParentId);
        GameObject.name = "DeadBodyReporter " + Id;

        var pos = deadBody.TruePosition;
        var vector = new Vector3(pos.x, pos.y, (pos.y / 1000f) + 0.001f);
        GameObject.transform.position = vector;
        GameObject.layer = 11;
        Background = new GameObject("Background") { layer = 11 };
        Background.transform.SetParent(GameObject.transform);
        Background.transform.localPosition = new Vector3(0, 0, +0.001f);
        var spriteRenderer = Background.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = BackgroundSprite;
        spriteRenderer.color = new Color(1f, 1f, 1f, 0f);

        LocalCanSeeBodies = Professional.CanSeeBodies switch
        {
            Professional.CanSeeBody.Impostors => PlayerControl.LocalPlayer.IsImpostor(),
            Professional.CanSeeBody.KillNeutral => PlayerControl.LocalPlayer.IsImpostor() || isKillerNeutral(PlayerControl.LocalPlayer),
            Professional.CanSeeBody.EvilNeutral => PlayerControl.LocalPlayer.IsImpostor() || isKillerNeutral(PlayerControl.LocalPlayer) || isEvilNeutral(PlayerControl.LocalPlayer),
            _ => Killer != null && Killer == PlayerControl.LocalPlayer,
        };

        if (Killer != null && Killer == PlayerControl.LocalPlayer)
        {
            LocalCanSeeBodies = true;
        }

        if (!LocalCanSeeBodies)
        {
            deadBody.myCollider.tag = "Untagged";
            deadBody.bodyRenderers[0].color = Color.clear;
        }

        deadBody.bloodSplatter.color = Color.clear;
        GameObject.SetActive(true);
        DeadBody = deadBody;

        deadBody.gameObject.AddComponent<DeadBodyReporterMarker>();
    }

    public override void Update()
    {
        if (GameObject == null || Reported || InMeeting) return;

        if (PlayerControl.LocalPlayer.IsDead()) return;

        var skipAutoReport = Professional.CanSeeBodies switch
        {
            Professional.CanSeeBody.Impostors => PlayerControl.LocalPlayer.IsImpostor(),
            Professional.CanSeeBody.KillNeutral => PlayerControl.LocalPlayer.IsImpostor() || isKillerNeutral(PlayerControl.LocalPlayer),
            Professional.CanSeeBody.EvilNeutral => PlayerControl.LocalPlayer.IsImpostor() || isKillerNeutral(PlayerControl.LocalPlayer) || isEvilNeutral(PlayerControl.LocalPlayer),
            _ => Killer != null && Killer == PlayerControl.LocalPlayer,
        };

        if (Killer != null && Killer == PlayerControl.LocalPlayer)
        {
            skipAutoReport = true;
        }

        Background.SetActive(skipAutoReport);
        if (skipAutoReport) return;

        var distance = Vector2.Distance(GameObject.transform.position, PlayerControl.LocalPlayer.GetTruePosition());
        if (distance <= ReportDistance)
        {
            if (Target != null && Target.Data != null)
            {
                Info($"[DeadBodyReporter] Auto-reporting body of {Target.Data.PlayerName} by {PlayerControl.LocalPlayer.Data.PlayerName}", "AutoReport");
                PlayerControl.LocalPlayer.CmdReportDeadBody(Target.Data);

                Reported = true;
            }
        }

    }

    public override void OnMeetingStart()
    {
        Destroy();
    }

    public class DeadBodyReporterMarker : MonoBehaviour
    {
        static DeadBodyReporterMarker()
        {
            ClassInjector.RegisterTypeInIl2Cpp<DeadBodyReporterMarker>();
        }
    }


}
