using BepInEx.Unity.IL2CPP.Utils.Collections;

namespace TheOtherRoles.Objects;

public class Bomb : CustomObjectBase<Bomb>
{
    public static Sprite defuseSprite = new ResourceSprite("Bomb_Button_Defuse.png");
    private static Sprite bombSprite = new ResourceSprite("Bomb.png", 300f);
    private static Sprite backgroundSprite => new ResourceSprite("TheOtherRoles.Resources.BombBackground.png", 110f / Terrorist.alertRange);

    public static Bomb TargetBomb;
    public GameObject Background;
    public SpriteRenderer BackgroundRenderer;

    public PlayerControl Player;

    public Bomb(PlayerControl player, Vector3 pos)
    {
        IsActive = false;
        Player = player;

        GameObject.name = "Bomb " + Id;
        GameObject.layer = 11;
        GameObject.transform.position = new Vector3(pos.x, pos.y, pos.z - 0.001f);
        Renderer.sprite = bombSprite;

        if (!Terrorist.selfExplosion)
        {
            Background = new GameObject("Background") { layer = 11 };
            Background.transform.SetParent(GameObject.transform);
            Background.transform.localPosition = new Vector3(0, 0, -0.01f);

            BackgroundRenderer = Background.AddComponent<SpriteRenderer>();
            BackgroundRenderer.sprite = backgroundSprite;
            BackgroundRenderer.color = Color.white;
        }

        bool canSee = !Terrorist.selfExplosion && (player.AmOwner || PlayerControl.LocalPlayer.IsImpostor(AndCat: true) || CanSeeGhostInfo);
        GameObject.SetActive(canSee);
        Background?.SetActive(canSee);
        if (Terrorist.selfExplosion)
        {
            IsActive = true;
            _ = new LateTask(explode, 0.1f);
        }
        else
        {
            _ = new LateTask(() => ActivateBomb(pos), Terrorist.bombActiveAfter);
        }
    }

    private void ActivateBomb(Vector3 pos)
    {
        GameObject.SetActive(!Terrorist.selfExplosion);
        Background.SetActive(!Terrorist.selfExplosion);
        IsActive = true;

        SoundEffectsManager.playAtPosition("bombFuseBurning", pos, Terrorist.destructionTime, Terrorist.soundRange, true);

        Behaviour.StartCoroutine(ExplosionCountdown().WrapToIl2Cpp());
    }

    private IEnumerator ExplosionCountdown()
    {
        float timer = 0f;
        Color startColor = Color.white;
        Color endColor = Color.red;

        while (timer < Terrorist.destructionTime && BackgroundRenderer != null)
        {
            timer += Time.deltaTime;
            float progress = timer / Terrorist.destructionTime;

            BackgroundRenderer.color = Color.Lerp(startColor, endColor, progress);

            yield return null;
        }

        if (timer >= Terrorist.destructionTime) explode();
    }

    public void explode()
    {
        if (GameObject == null)
        {
            Destroy();
            Error("Bomb or bomb GameObject is null.");
            return;
        }

        try
        {
            var position = GameObject.transform.position;

            SoundEffectsManager.playAtPosition("bombExplosion", position, maxDuration: 1.6f, range: Terrorist.soundRange);

            if (!Terrorist.selfExplosion || !(Player == PlayerControl.LocalPlayer))
            {
                if (Player != null && PlayerControl.LocalPlayer.IsAlive())
                {
                    float distance = Vector2.Distance(position, PlayerControl.LocalPlayer.transform.position);
                    if (distance <= Terrorist.destructionRange)
                        RpcCustomMurderPlayer(Player, PlayerControl.LocalPlayer, false, false, CustomDeathReason.Bomb);
                }
            }

            if (Player.IsAlive() && Terrorist.selfExplosion && Player == PlayerControl.LocalPlayer)
            {
                RpcCustomMurderPlayer(Player, Player, false, true, CustomDeathReason.BombVictim);
            }
        }
        catch (Exception e)
        {
            Error($"Exception in Bomb explosion: {e}");
        }
        finally
        {
            Destroy();
        }
    }

    public override void OnDestroy()
    {
        SoundEffectsManager.stop("bombFuseBurning");

        if (Background != null)
        {
            Background.Destroy();
            Background = null;
        }

        base.OnDestroy();
    }

    public override void OnMeetingStart()
    {
        this?.Destroy();
    }

    public override void Update()
    {
        if (!Terrorist.selfExplosion && Background != null && Background.transform != null)
        {
            Background.transform.Rotate(Vector3.forward * 50 * Time.fixedDeltaTime);
        }
    }
}