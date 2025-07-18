using TheOtherRoles.Attributes;

namespace TheOtherRoles.Objects;
#nullable enable
internal class Garlic
{
    public static List<Garlic> garlics = new();
    private static Sprite garlicSprite = new ResourceSprite("Garlic.png", 300);
    private static Sprite backgroundSprite = new ResourceSprite("GarlicBackground.png", 60);
    private readonly GameObject background;
    public readonly GameObject garlic;

    public Garlic(Vector2 p)
    {
        garlic = new GameObject("Garlic") { layer = 11 };
        garlic.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
        background = new GameObject("Background") { layer = 11 };
        background.transform.SetParent(garlic.transform);
        var position = new Vector3(p.x, p.y, (p.y / 1000) + 0.001f); // just behind player
        garlic.transform.position = position;
        background.transform.localPosition = new Vector3(0, 0, -1f); // before player

        var garlicRenderer = garlic.AddComponent<SpriteRenderer>();
        garlicRenderer.sprite = garlicSprite;
        var backgroundRenderer = background.AddComponent<SpriteRenderer>();
        backgroundRenderer.sprite = backgroundSprite;


        garlic.SetActive(true);
        garlics.Add(this);
    }

    [OnGameStart, OnGameEnd]
    public static void clearGarlics()
    {
        garlics.Clear();
    }

    public static void UpdateAll()
    {
        foreach (var garlic in garlics.Where(garlic => garlic != null))
            garlic.Update();
    }

    public void Update()
    {
        if (background != null) background.transform.Rotate(Vector3.forward * 6 * Time.fixedDeltaTime);
    }
}