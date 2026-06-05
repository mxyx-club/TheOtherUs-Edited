namespace TheOtherRoles.Objects;

public class GhostObject : CustomObjectBase<GhostObject>
{
    public PlayerControl Player;
    private float timer;
    private CircleCollider2D collider;
    public static Sprite GhostSprite = new ResourceSprite("Ghost.png", 160);

    public GhostObject(PlayerControl player, Vector3 pos)
    {
        Player = player;

        GameObject.name = "GhostObject_" + Id;
        GameObject.transform.position = pos + new Vector3(0, 0, -1f);
        Renderer.gameObject.layer = CanSeeGhostInfo ? LayerExpansion.GetObjectsLayer() : LayerExpansion.GetDefaultLayer();
        Renderer.sprite = GhostSprite;
        Renderer.color = Color.white * new Color(1, 1, 1, 1f);
        collider = UnityHelper.CreateObject<CircleCollider2D>("Collider", Renderer.transform, Vector3.zero);
        collider.radius = Clog.GhostRange;

        var local = PlayerData.LocalPlayer;
        float distance = Vector2.Distance(local.GetTruePosition(), GameObject.transform.position);

        if (local.IsAlive() && local.Collider.enabled && distance <= Clog.GhostRange + 0.24f)
        {
            collider.radius = 0f;
        }

        timer = 0f;
        GameObject.SetActive(true);
    }

    public override void Update()
    {
        timer += Time.deltaTime;
        var totalDuration = Clog.GhostDuration;

        if (totalDuration <= 0 || timer >= totalDuration)
        {
            Destroy();
            return;
        }


        var local = PlayerData.LocalPlayer;
        float distance = Vector2.Distance(local.GetTruePosition(), GameObject.transform.position);

        if (local.IsAlive() && local.Collider.enabled && distance > Clog.GhostRange + 0.12f)
        {
            collider.radius = Clog.GhostRange;
        }


        var timeRemaining = totalDuration - timer;

        if (timeRemaining > 5.0f)
        {
            Renderer.color = new Color(1, 1, 1, 1);
            return;
        }

        var timeInAnim = 5.0f - timeRemaining;

        if (timeInAnim > 5.0f - 0.5f)
        {
            var progress = (timeInAnim - (5.0f - 0.5f)) / 0.5f;
            Renderer.color = new Color(1, 1, 1, Mathf.Lerp(1, 0, progress));
            return;
        }


        var cycleTime = timeInAnim % 1.5f;
        var alpha = 1f;

        if (cycleTime < 0.25f)
        {
            alpha = Mathf.Lerp(1, 0, cycleTime / 0.25f);
        }
        else if (cycleTime < 0.25f + 0.25f)
        {
            alpha = Mathf.Lerp(0, 1, (cycleTime - 0.25f) / 0.25f);
        }

        Renderer.color = new Color(1, 1, 1, alpha);
    }

    public override void OnMeetingStart()
    {
        this?.Destroy();
    }
}
