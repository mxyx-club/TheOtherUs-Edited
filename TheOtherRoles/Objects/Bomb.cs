namespace TheOtherRoles.Objects;

public class Bomb : CustomObject
{
    public static List<Bomb> AllBombs = new();
    public static Sprite defuseSprite = new ResourceSprite("Bomb_Button_Defuse.png");
    public static Bomb TargetBomb;
    public GameObject Background;
    public PlayerControl Player;

    private static Sprite bombSprite => new ResourceSprite("Bomb.png", 300f);
    private static Sprite backgroundSprite => new ResourceSprite("TheOtherRoles.Resources.BombBackground.png", 110f / Terrorist.hearRange);

    public Bomb(PlayerControl player, Vector3 pos)
    {
        GameObject.name = "Bomb " + Id;
        GameObject.layer = 11;
        var position = new Vector3(pos.x, pos.y, pos.z - 0.001f); // just behind player
        GameObject.transform.position = position;
        Renderer.sprite = bombSprite;

        Background = new GameObject("Background") { layer = 11 };
        Background.transform.SetParent(GameObject.transform);
        Background.transform.localPosition = new Vector3(0, 0, -0.01f);

        var backgroundRenderer = Background.AddComponent<SpriteRenderer>();
        backgroundRenderer.sprite = backgroundSprite;

        GameObject.SetActive(false);
        Background.SetActive(false);
        if (PlayerControl.LocalPlayer == Terrorist.terrorist) GameObject.SetActive(true);
        var c = Color.white;
        var g = Color.red;
        backgroundRenderer.color = Color.white;
        IsActive = false;
        Player = player;
        AllBombs.Add(this);

        _ = new LateTask(() =>
        {
            GameObject.SetActive(!Terrorist.selfExplosion);
            Background.SetActive(!Terrorist.selfExplosion);
            IsActive = true;
            SoundEffectsManager.playAtPosition("bombFuseBurning", pos, Terrorist.destructionTime, Terrorist.hearRange, true);

            HudManager.Instance.StartCoroutine(Effects.Lerp(Terrorist.destructionTime,
                new Action<float>(f =>
                {
                    // can you feel the pain?
                    var combinedColor = (Mathf.Clamp01(f) * g) + (Mathf.Clamp01(1 - f) * c);
                    if (backgroundRenderer) backgroundRenderer.color = combinedColor;
                    if ((int)f == 1) explode(this);
                })));
        }, Terrorist.bombActiveAfter);
    }

    public override void Destroy()
    {
        Background?.Destroy();
        Background = null;
        SoundEffectsManager.stop("bombFuseBurning");
        AllBombs.Remove(this);
        base.Destroy();
    }

    public static void explode(Bomb b)
    {
        if (b?.GameObject == null)
        {
            b?.Destroy();
            Error("Bomb or bomb GameObject is null.");
            return;
        }
        if (Terrorist.terrorist != null)
        {
            var position = b.GameObject.transform.position;
            // every player only checks that for their own client (desynct with positions sucks)

            var distance = Vector2.Distance(position, PlayerControl.LocalPlayer.transform.position);

            if (distance <= Terrorist.destructionRange && PlayerControl.LocalPlayer.IsAlive())
            {
                RpcCustomMurderPlayer(Terrorist.terrorist, PlayerControl.LocalPlayer, false, false, CustomDeathReason.Bomb);
            }
            try
            {
                SoundEffectsManager.playAtPosition("bombExplosion", position, maxDuration: 1.6f, range: Terrorist.hearRange);
            }
            catch (Exception e)
            {
                Warn($"Exception in Sound Effect for Bomb explosion: {e}");
            }
        }

        b.Destroy();
    }

    public override void OnMeetingStart()
    {
        this?.Destroy();
    }

    public override void Update()
    {
        Background?.transform?.Rotate(Vector3.forward * 50 * Time.fixedDeltaTime);
    }
}