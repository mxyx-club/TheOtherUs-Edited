using TheOtherRoles.Attributes;

namespace TheOtherRoles.Objects;

internal class Bloodytrail
{
    private static readonly List<Bloodytrail> bloodytrail = new();
    private static readonly List<Sprite> sprites = new();

    public Bloodytrail(PlayerControl killer, PlayerControl player)
    {
        Color color = Palette.PlayerColors[player.Data.DefaultOutfit.ColorId];
        var sp = getBloodySprites();
        var index = rnd.Next(0, sp.Count);

        var blood = new GameObject("Blood" + index);
        var pos = killer.transform.position;
        var position = new Vector3(pos.x, pos.y, (pos.y / 1000) + 0.001f);
        blood.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
        blood.transform.position = position;
        blood.transform.localPosition = position;
        blood.transform.SetParent(killer.transform.parent);

        blood.transform.Rotate(0.0f, 0.0f, URandom.Range(0.0f, 360.0f));

        var spriteRenderer = blood.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sp[index];
        spriteRenderer.material = FastDestroyableSingleton<HatManager>.Instance.PlayerMaterial;
        player.SetPlayerMaterialColors(spriteRenderer);
        // spriteRenderer.color = color;

        blood.SetActive(true);
        bloodytrail.Add(this);

        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(12f, new Action<float>(p =>
        {
            var c = color;
            if (Camouflager.camouflageTimer > 0 || MushroomSabotageActive) c = Palette.PlayerColors[6];

            if (spriteRenderer)
            {
                var alpha = p < 0.6f ? 1f : Mathf.Lerp(1f, 0f, (p - 0.6f) / 0.4f);
                spriteRenderer.color = new Color(c.r, c.g, c.b, alpha);
            }

            if ((int)p != 1 || blood == null) return;
            UObject.Destroy(blood);
            bloodytrail.Remove(this);
        })));
    }

    public static void StartBloodTrail(PlayerControl killer, PlayerControl blood)
    {
        Coroutines.Start(CreateBlood(killer, blood));
    }

    private static IEnumerator CreateBlood(PlayerControl killer, PlayerControl blood)
    {
        float endTime = Time.time + Bloody.duration;
        if (killer.IsDead()) yield break;
        while (Time.time < endTime)
        {
            _ = new Bloodytrail(killer, blood);

            yield return new WaitForSeconds(0.1f);
        }
    }

    private static List<Sprite> getBloodySprites()
    {
        if (sprites.Count > 0) return sprites;
        sprites.Add(new ResourceSprite("TheOtherRoles.Resources.Blood1.png", 450));
        sprites.Add(new ResourceSprite("TheOtherRoles.Resources.Blood2.png", 470));
        sprites.Add(new ResourceSprite("TheOtherRoles.Resources.Blood3.png", 275));
        return sprites;
    }

    [OnGameStart, OnGameEnd]
    public static void resetSprites()
    {
        sprites.Clear();
    }
}