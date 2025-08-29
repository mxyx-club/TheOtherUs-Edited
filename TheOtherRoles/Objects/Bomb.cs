namespace TheOtherRoles.Objects;

public class Bomb
{
    public static Sprite defuseSprite = new ResourceSprite("Bomb_Button_Defuse.png");
    public static bool canDefuse;

    public GameObject background;
    public GameObject bomb;

    private static Sprite bombSprite => new ResourceSprite("Bomb.png", 300f);
    private static Sprite backgroundSprite => new ResourceSprite("TheOtherRoles.Resources.BombBackground.png", 110f / Terrorist.hearRange);

    public Bomb(Vector2 p)
    {
        bomb = new GameObject("Bomb") { layer = 11 };
        bomb.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
        var position = new Vector3(p.x, p.y, (p.y / 1000) + 0.001f); // just behind player
        bomb.transform.position = position;

        background = new GameObject("Background") { layer = 11 };
        background.transform.SetParent(bomb.transform);
        background.transform.localPosition = new Vector3(0, 0, -1f); // before player
        background.transform.position = position;

        var bombRenderer = bomb.AddComponent<SpriteRenderer>();
        bombRenderer.sprite = bombSprite;
        var backgroundRenderer = background.AddComponent<SpriteRenderer>();
        backgroundRenderer.sprite = backgroundSprite;

        bomb.SetActive(false);
        background.SetActive(false);
        if (PlayerControl.LocalPlayer == Terrorist.terrorist) bomb.SetActive(true);
        Terrorist.bomb = this;
        var c = Color.white;
        var g = Color.red;
        backgroundRenderer.color = Color.white;
        Terrorist.isActive = false;

        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Terrorist.bombActiveAfter,
            new Action<float>(x =>
            {
                if ((int)x != 1) return;
                bomb.SetActive(true);
                background.SetActive(true);
                SoundEffectsManager.playAtPosition("bombFuseBurning", p, Terrorist.destructionTime, Terrorist.hearRange, true);
                Terrorist.isActive = true;

                FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Terrorist.destructionTime,
                    new Action<float>(f =>
                    {
                        // can you feel the pain?
                        var combinedColor = (Mathf.Clamp01(f) * g) + (Mathf.Clamp01(1 - f) * c);
                        if (backgroundRenderer) backgroundRenderer.color = combinedColor;
                        if ((int)f == 1) explode(this);
                    })));
            })));
    }

    public void Destroy()
    {
        background?.Destroy();
        bomb?.Destroy();
    }

    public static void explode(Bomb b)
    {
        if (b?.bomb == null)
        {
            b?.Destroy();
            Error("Bomb or bomb GameObject is null.");
            return;
        }
        if (Terrorist.terrorist != null)
        {
            var position = b.bomb.transform.position;
            // every player only checks that for their own client (desynct with positions sucks)
            var distance = Vector2.Distance(position, PlayerControl.LocalPlayer.transform.position);

            if (distance <= Terrorist.destructionRange && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                if (Terrorist.selfExplosion && PlayerControl.LocalPlayer == Terrorist.terrorist)
                {
                    Terrorist.clearBomb();
                    return;
                }

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

        Terrorist.clearBomb();
        canDefuse = false;
        Terrorist.isActive = false;
    }

    public static void update()
    {
        if (Terrorist.bomb == null || !Terrorist.isActive)
        {
            canDefuse = false;
            return;
        }

        Terrorist.bomb.background.transform.Rotate(Vector3.forward * 50 * Time.fixedDeltaTime);

        if (MeetingHud.Instance && Terrorist.bomb != null) Terrorist.clearBomb();

        canDefuse = Vector2.Distance(PlayerControl.LocalPlayer.GetTruePosition(), Terrorist.bomb.bomb.transform.position) <= 1f;
    }
}