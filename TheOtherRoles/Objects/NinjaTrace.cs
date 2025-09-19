namespace TheOtherRoles.Objects;

internal class NinjaTrace : CustomObjectBase<NinjaTrace>
{
    private float timeRemaining;

    public NinjaTrace(Vector3 p, float duration = 1f)
    {
        GameObject.name = "NinjaTrace " + Id;
        var position = new Vector3(p.x, p.y, p.z + 0.001f);
        GameObject.transform.position = position;
        GameObject.transform.localPosition = position;

        Renderer.sprite = new ResourceSprite("NinjaTraceW.png", 225f);

        timeRemaining = duration;

        // display the ninjas color in the trace
        var colorDuration = CustomOptionHolder.ninjaTraceColorTime.GetFloat();
        HudManager.Instance.StartCoroutine(Effects.Lerp(colorDuration, new Action<float>(p =>
        {
            Color c = Palette.PlayerColors[Ninja.ninja.Data.DefaultOutfit.ColorId];

            c = IsLightColor(Ninja.ninja) ? Color.white : (Color)Palette.PlayerColors[6];

            var g = Color.green; // Usual display color.

            var combinedColor = (Mathf.Clamp01(p) * g) + (Mathf.Clamp01(1 - p) * c);

            Renderer.color = combinedColor;
        })));

        var fadeOutDuration = 1f;
        if (fadeOutDuration > duration) fadeOutDuration = 0.5f * duration;
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(duration, new Action<float>(p =>
        {
            var interP = 0f;
            if (p < (duration - fadeOutDuration) / duration)
                interP = 0f;
            else interP = ((p * duration) + fadeOutDuration - duration) / fadeOutDuration;
            if (!Renderer) return;
            var color = Renderer.color;
            Renderer.color = new Color(color.r, color.g, color.b, Mathf.Clamp01(1 - interP));
        })));

        GameObject.SetActive(true);
    }

    public override void Update()
    {
        timeRemaining -= Time.fixedDeltaTime;
        if (!(timeRemaining < 0)) return;
        GameObject.SetActive(false);
        UObject.Destroy(GameObject);
    }
}