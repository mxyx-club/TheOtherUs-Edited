namespace TheOtherRoles.Objects;

public class Silhouette : CustomObjectBase<Silhouette>
{
    public float timeRemaining;
    public bool permanent;
    private bool visibleForEveryOne;

    private static Sprite SilhouetteSprite = new ResourceSprite("Silhouette.png", 225f);

    public Silhouette(Vector3 p, float duration = 1f, bool visibleForEveryOne = true)
    {
        if (duration <= 0f)
        {
            Message("silhouette: permanent!");
            permanent = true;
        }
        this.visibleForEveryOne = visibleForEveryOne;
        GameObject.name = "Silhouette " + Id;
        Vector3 position = new Vector3(p.x, p.y, p.z);
        GameObject.transform.position = position;
        GameObject.transform.localPosition = position;

        Renderer.sprite = SilhouetteSprite;

        timeRemaining = duration;

        Renderer.color = Renderer.color.SetAlpha(Yoyo.SilhouetteVisibility);

        bool visible = visibleForEveryOne || PlayerControl.LocalPlayer == Yoyo.yoyo || PlayerControl.LocalPlayer.Data.IsDead;

        GameObject.SetActive(visible);
    }

    public override void Update()
    {
        timeRemaining -= Time.fixedDeltaTime;
        bool visible = visibleForEveryOne || PlayerControl.LocalPlayer == Yoyo.yoyo || CanSeeGhostInfo;
        GameObject.SetActive(visible);

        if (visible && timeRemaining > 0 && timeRemaining < 0.5)
        {
            var alphaRatio = timeRemaining / 0.5f;
            Renderer.color = Renderer.color.SetAlpha(Yoyo.SilhouetteVisibility * alphaRatio);
        }

        if (timeRemaining < 0 && !permanent)
        {
            Message($"update: permanent: {permanent}, time: {timeRemaining}");
            GameObject.SetActive(false);
            Destroy();
        }
    }
}

