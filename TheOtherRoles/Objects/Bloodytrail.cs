using System;
using System.Collections.Generic;
using TheOtherRoles.Helper;
using TheOtherRoles.Utilities;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace TheOtherRoles.Objects;

internal class Bloodytrail
{
    private static readonly List<Bloodytrail> bloodytrail = new();
    private static readonly List<Sprite> sprites = new();
    private readonly GameObject blood;
    private readonly Color color;
    private readonly SpriteRenderer spriteRenderer;

    public Bloodytrail(PlayerControl player, PlayerControl bloodyPlayer)
    {
        color = Palette.PlayerColors[bloodyPlayer.Data.DefaultOutfit.ColorId];
        var sp = getBloodySprites();
        var index = rnd.Next(0, sp.Count);


        blood = new GameObject("Blood" + index);
        var position = new Vector3(player.transform.position.x, player.transform.position.y,
            (player.transform.position.y / 1000) + 0.001f);
        blood.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
        blood.transform.position = position;
        blood.transform.localPosition = position;
        blood.transform.SetParent(player.transform.parent);

        blood.transform.Rotate(0.0f, 0.0f, Random.Range(0.0f, 360.0f));

        spriteRenderer = blood.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sp[index];
        spriteRenderer.material = FastDestroyableSingleton<HatManager>.Instance.PlayerMaterial;
        bloodyPlayer.SetPlayerMaterialColors(spriteRenderer);
        // spriteRenderer.color = color;

        blood.SetActive(true);
        bloodytrail.Add(this);

        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(10f, new Action<float>(p =>
        {
            var c = color;
            if (Camouflager.camouflageTimer > 0 || Helpers.MushroomSabotageActive()) c = Palette.PlayerColors[6];
            if (spriteRenderer) spriteRenderer.color = new Color(c.r, c.g, c.b, Mathf.Clamp01(1 - p));

            if (p == 1f && blood != null)
            {
                Object.Destroy(blood);
                bloodytrail.Remove(this);
            }
        })));
    }

    public static List<Sprite> getBloodySprites()
    {
        if (sprites.Count > 0) return sprites;
        sprites.Add(Helpers.loadSpriteFromResources("TheOtherRoles.Resources.Blood1.png", 700));
        sprites.Add(Helpers.loadSpriteFromResources("TheOtherRoles.Resources.Blood2.png", 500));
        sprites.Add(Helpers.loadSpriteFromResources("TheOtherRoles.Resources.Blood3.png", 300));
        return sprites;
    }

    public static void resetSprites()
    {
        sprites.Clear();
    }
}