using TheOtherRoles.Helper;
using UnityEngine;

namespace TheOtherRoles.Objects;

public class Arrow
{
    private static Sprite sprite;
    private readonly ArrowBehaviour arrowBehaviour;
    public GameObject arrow;
    public SpriteRenderer image;
    private Vector3 oldTarget;
    public float perc = 0.925f;


    public Arrow(Color color)
    {
        arrow = new GameObject("Arrow");
        arrow.layer = 5;
        image = arrow.AddComponent<SpriteRenderer>();
        image.sprite = getSprite();
        image.color = color;
        arrowBehaviour = arrow.AddComponent<ArrowBehaviour>();
        arrowBehaviour.image = image;
    }

    public static Sprite getSprite()
    {
        if (sprite) return sprite;
        sprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.Arrow.png", 200f);
        return sprite;
    }

    public void Update()
    {
        var target = oldTarget;
        Update(target);
    }

    public void Update(Vector3 target, Color? color = null)
    {
        if (arrow == null) return;
        oldTarget = target;

        if (color.HasValue) image.color = color.Value;

        arrowBehaviour.target = target;
        arrowBehaviour.Update();
    }
}