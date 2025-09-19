namespace TheOtherRoles.Objects;
#nullable enable
internal class Garlic : CustomObjectBase<Garlic>
{
    private static Sprite garlicSprite = new ResourceSprite("Garlic.png", 300);
    private static Sprite backgroundSprite = new ResourceSprite("GarlicBackground.png", 60);
    private readonly GameObject Background;

    public Garlic(Vector3 pos)
    {
        GameObject!.layer = 11;
        GameObject.name = "Garlic " + Id;
        Background = new GameObject("Background") { layer = 11 };
        Background.transform.SetParent(GameObject.transform);
        var position = new Vector3(pos.x, pos.y, pos.z + 0.001f); // just behind player
        GameObject.transform.position = position;
        Background.transform.localPosition = new Vector3(0, 0, +0.001f); // before player
        Renderer!.sprite = garlicSprite;
        var backgroundRenderer = Background.AddComponent<SpriteRenderer>();
        backgroundRenderer.sprite = backgroundSprite;

        GameObject.SetActive(true);
    }

    public override void OnDestroy()
    {
        Background?.Destroy();
        base.OnDestroy();
    }

    public override void Update()
    {
        Background?.transform?.Rotate(Vector3.forward * 6 * Time.fixedDeltaTime);
    }
}