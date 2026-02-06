using PowerTools;
using TheOtherRoles.Attributes;

namespace TheOtherRoles.Objects;

public class JackInTheBox : CustomObjectBase<JackInTheBox>
{
    public static readonly int JackInTheBoxLimit = 3;
    public static bool boxesConvertedToVents;
    public static ResourceSpriteArray boxAnimationSprites;

    private readonly SpriteRenderer ventRenderer;
    public readonly Vent vent;

    public JackInTheBox(PlayerControl player, Vector3 pos)
    {
        GameObject.layer = 11;
        GameObject.name = "JackInTheBoxVent_" + Id;
        // Add collider offset that DoMove moves the player up at a valid position
        pos += (Vector3)PlayerControl.LocalPlayer.Collider.offset;
        // Create the marker
        GameObject.transform.position = pos;
        Renderer.sprite = getBoxAnimationSprite(0);
        Renderer.color = Renderer.color.SetAlpha(0.5f);

        // Create the vent
        var referenceVent = UObject.FindObjectOfType<Vent>();
        vent = UObject.Instantiate(referenceVent);
        vent.gameObject.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
        vent.transform.position = GameObject.transform.position;
        vent.Left = null;
        vent.Right = null;
        vent.Center = null;
        vent.EnterVentAnim = null;
        vent.ExitVentAnim = null;
        vent.Offset = new Vector3(0f, 0.25f, 0f);
        vent.GetComponent<SpriteAnim>()?.Stop();
        vent.Id = MapUtilities.CachedShipStatus.AllVents.Max(x => x.Id) + 1; // Make sure we have a unique id
        ventRenderer = vent.GetComponent<SpriteRenderer>();
        if (isFungle)
        {
            ventRenderer = vent.transform.GetChild(3).GetComponent<SpriteRenderer>();
            var animator = vent.transform.GetChild(3).GetComponent<SpriteAnim>();
            animator.Stop();
        }

        //ventRenderer.Destroy();
        ventRenderer.sprite = null; // Use the box.boxRenderer instead
        vent.myRend = ventRenderer;
        var allVentsList = MapUtilities.CachedShipStatus.AllVents.ToList();
        allVentsList.Add(vent);
        MapUtilities.CachedShipStatus.AllVents = allVentsList.ToArray();
        vent.gameObject.SetActive(false);
        vent.name = "JackInTheBoxVent_" + vent.Id;
        // Only render the box for the Trickster and for Ghosts
        var showBoxToLocalPlayer = PlayerControl.LocalPlayer == Trickster.trickster || CanSeeGhostInfo;
        GameObject.SetActive(showBoxToLocalPlayer);
    }

    public override void OnDestroy()
    {
        vent?.Destroy();
        base.OnDestroy();
    }

    public static Sprite getBoxAnimationSprite(int index)
    {
        if (boxAnimationSprites == null) return null;
        index = Mathf.Clamp(index, 0, boxAnimationSprites.Sprites.Length - 1);
        return boxAnimationSprites.GetSprite(index);
    }

    public static void preloadBoxAnimationSprites()
    {
        var sprites = new (string, float)[18];
        for (int i = 0; i < sprites.Length; i++)
            sprites[i] = ($"TricksterAnimation.trickster_box_00{i + 1:00}.png", 175f);
        boxAnimationSprites = new ResourceSpriteArray(sprites, true);
    }

    public static void startAnimation(int ventId)
    {
        var box = AllObjects.FirstOrDefault(x => x?.vent != null && x.vent.Id == ventId);
        if (box == null) return;

        int frameCount = boxAnimationSprites.Sprites.Length;
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(0.6f, new Action<float>(p =>
        {
            if (box.Renderer == null) return;
            int frameIndex = (int)(p * frameCount);
            box.Renderer.sprite = getBoxAnimationSprite(frameIndex);
            if ((int)p == 1) box.Renderer.sprite = getBoxAnimationSprite(0);
        })));
    }

    public static void UpdateStates()
    {
        if (boxesConvertedToVents) return;
        foreach (var box in AllObjects)
        {
            var showBoxToLocalPlayer = PlayerControl.LocalPlayer == Trickster.trickster || CanSeeGhostInfo;
            box.GameObject?.SetActive(showBoxToLocalPlayer);
        }
    }

    public void convertToVent()
    {
        GameObject.SetActive(true);
        vent.gameObject.SetActive(true);
        Renderer.color = Renderer.color.SetAlpha(1f);
        ventRenderer.sprite = null;
    }

    public static void MeetingEnd()
    {
        if (!hasJackInTheBoxLimitReached()) return;
        foreach (var box in AllObjects) box.convertToVent();
        connectVents();
        boxesConvertedToVents = true;
    }

    public static bool hasJackInTheBoxLimitReached()
    {
        return AllObjects.Count >= JackInTheBoxLimit;
    }

    private static void connectVents()
    {
        for (var i = 0; i < AllObjects.Count - 1; i++)
        {
            var a = AllObjects[i];
            var b = AllObjects[i + 1];
            a.vent.Right = b.vent;
            b.vent.Left = a.vent;
        }

        // Connect first with last
        AllObjects.First().vent.Left = AllObjects.Last().vent;
        AllObjects.Last().vent.Right = AllObjects.First().vent;
    }

    [OnGameStart, OnGameEnd]
    public static void clearJackInTheBoxes()
    {
        boxesConvertedToVents = false;
        preloadBoxAnimationSprites();
    }
}