using System;
using System.Collections.Generic;
using TheOtherRoles.Roles.Impostor;
using TheOtherRoles.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TheOtherRoles.Objects;

internal class NinjaTrace
{
    public static List<NinjaTrace> traces = [];

    private static Sprite TraceSprite;

    private readonly GameObject trace;
    private float timeRemaining;

    public NinjaTrace(Vector2 p, float duration = 1f)
    {
        trace = new GameObject("NinjaTrace");
        trace.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
        //Vector3 position = new Vector3(p.x, p.y, CachedPlayer.LocalPlayer.transform.localPosition.z + 0.001f); // just behind player
        var position = new Vector3(p.x, p.y, (p.y / 1000f) + 0.01f);
        trace.transform.position = position;
        trace.transform.localPosition = position;

        var traceRenderer = trace.AddComponent<SpriteRenderer>();
        traceRenderer.sprite = getTraceSprite();

        timeRemaining = duration;

        // display the ninjas color in the trace
        var colorDuration = CustomOptionHolder.ninjaTraceColorTime.getFloat();
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(colorDuration, new Action<float>(p =>
        {
            Color c = Palette.PlayerColors[Ninja.ninja.Data.DefaultOutfit.ColorId];
            if (Helpers.isLighterColor(Ninja.ninja)) c = Color.white;
            else c = Palette.PlayerColors[6];
            //if (Camouflager.camouflageTimer > 0) {
            //    c = Palette.PlayerColors[6];
            //}

            var g = Color.green; // Usual display color.

            var combinedColor = (Mathf.Clamp01(p) * g) + (Mathf.Clamp01(1 - p) * c);

            if (traceRenderer) traceRenderer.color = combinedColor;
        })));

        var fadeOutDuration = 1f;
        if (fadeOutDuration > duration) fadeOutDuration = 0.5f * duration;
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(duration, new Action<float>(p =>
        {
            var interP = 0f;
            if (p < (duration - fadeOutDuration) / duration)
                interP = 0f;
            else interP = ((p * duration) + fadeOutDuration - duration) / fadeOutDuration;
            if (!traceRenderer) return;
            var color = traceRenderer.color;
            traceRenderer.color = new Color(color.r, color.g, color.b,
                Mathf.Clamp01(1 - interP));
        })));

        trace.SetActive(true);
        traces.Add(this);
    }

    public static Sprite getTraceSprite()
    {
        if (TraceSprite) return TraceSprite;
        TraceSprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.NinjaTraceW.png", 225f);
        return TraceSprite;
    }

    public static void clearTraces()
    {
        traces = [];
    }

    public static void UpdateAll()
    {
        foreach (var traceCurrent in new List<NinjaTrace>(traces))
        {
            traceCurrent.timeRemaining -= Time.fixedDeltaTime;
            if (!(traceCurrent.timeRemaining < 0)) continue;
            traceCurrent.trace.SetActive(false);
            Object.Destroy(traceCurrent.trace);
            traces.Remove(traceCurrent);
        }
    }
}