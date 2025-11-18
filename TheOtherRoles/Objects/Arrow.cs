namespace TheOtherRoles.Objects;

public class Arrow
{
    private static Sprite sprite = new ResourceSprite("TheOtherRoles.Resources.Arrow.png", 200f);
    private readonly ArrowBehaviour arrowBehaviour;
    public readonly GameObject arrow;
    private readonly SpriteRenderer image;
    private Vector3 oldTarget;


    public Arrow(Color color)
    {
        arrow = new GameObject("Arrow")
        {
            layer = 5
        };
        image = arrow.AddComponent<SpriteRenderer>();
        image.sprite = sprite;
        image.color = color;
        arrowBehaviour = arrow.AddComponent<ArrowBehaviour>();
        arrowBehaviour.image = image;
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