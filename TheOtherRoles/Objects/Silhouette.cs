using TheOtherRoles.Attributes;

namespace TheOtherRoles.Objects;
public class Silhouette
{
    public GameObject gameObject;
    public float timeRemaining;
    public bool permanent;
    private bool visibleForEveryOne;
    private SpriteRenderer renderer;

    public static List<Silhouette> silhouettes = new();


    private static Sprite SilhouetteSprite = new ResourceSprite("Silhouette.png", 225f);

    public Silhouette(Vector3 p, float duration = 1f, bool visibleForEveryOne = true)
    {
        if (duration <= 0f)
        {
            Message("silhouette: permanent!");
            permanent = true;
        }
        this.visibleForEveryOne = visibleForEveryOne;
        gameObject = new GameObject("Silhouette");
        gameObject.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
        //Vector3 position = new Vector3(p.x, p.y, PlayerControl.LocalPlayer.transform.localPosition.z + 0.001f); // just behind player
        Vector3 position = new Vector3(p.x, p.y, (p.y / 1000f) + 0.01f);
        gameObject.transform.position = position;
        gameObject.transform.localPosition = position;

        renderer = gameObject.AddComponent<SpriteRenderer>();
        renderer.sprite = SilhouetteSprite;

        timeRemaining = duration;

        renderer.color = renderer.color.SetAlpha(Yoyo.SilhouetteVisibility);

        bool visible = visibleForEveryOne || PlayerControl.LocalPlayer == Yoyo.yoyo || PlayerControl.LocalPlayer.Data.IsDead;

        gameObject.SetActive(visible);
        silhouettes.Add(this);
    }

    [OnGameStart, OnGameEnd]
    public static void clearSilhouettes()
    {
        foreach (var sil in silhouettes)
            sil.gameObject.Destroy();
        silhouettes = new();
    }

    public static void UpdateAll()
    {
        foreach (Silhouette current in new List<Silhouette>(silhouettes))
        {
            current.timeRemaining -= Time.fixedDeltaTime;
            bool visible = current.visibleForEveryOne || PlayerControl.LocalPlayer == Yoyo.yoyo || CanSeeGhostInfo;
            current.gameObject.SetActive(visible);

            if (visible && current.timeRemaining > 0 && current.timeRemaining < 0.5)
            {
                var alphaRatio = current.timeRemaining / 0.5f;
                current.renderer.color = current.renderer.color.SetAlpha(Yoyo.SilhouetteVisibility * alphaRatio);
            }

            if (current.timeRemaining < 0 && !current.permanent)
            {
                Message($"update: permanent: {current.permanent}, time: {current.timeRemaining}");
                current.gameObject.SetActive(false);
                UObject.Destroy(current.gameObject);
                silhouettes.Remove(current);
            }
        }
    }
}

