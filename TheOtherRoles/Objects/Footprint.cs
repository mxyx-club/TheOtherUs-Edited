using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using TheOtherRoles.Helper;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TheOtherRoles.Objects;

public class FootprintHolder : MonoBehaviour
{
    private static FootprintHolder _instance;

    private static Sprite _footprintSprite;
    private readonly List<Footprint> _activeFootprints = new();


    private readonly ConcurrentBag<Footprint> _pool = new();
    private readonly List<Footprint> _toRemove = new();

    static FootprintHolder()
    {
        ClassInjector.RegisterTypeInIl2Cpp<FootprintHolder>();
    }

    public FootprintHolder(IntPtr ptr) : base(ptr)
    {
    }

    public static FootprintHolder Instance
    {
        get => _instance ? _instance : _instance = new GameObject("FootprintHolder").AddComponent<FootprintHolder>();
        set => _instance = value;
    }

    private static Sprite FootprintSprite => _footprintSprite ??=
        Helpers.loadSpriteFromResources("TheOtherRoles.Resources.Footprint.png", 600f);

    private static bool AnonymousFootprints => TheOtherRoles.Detective.anonymousFootprints;
    private static float FootprintDuration => TheOtherRoles.Detective.footprintDuration;

    private void Update()
    {
        var dt = Time.deltaTime;
        _toRemove.Clear();
        foreach (var activeFootprint in _activeFootprints)
        {
            var p = activeFootprint.Lifetime / FootprintDuration;

            if (activeFootprint.Lifetime <= 0)
            {
                _toRemove.Add(activeFootprint);
                continue;
            }

            Color color;
            if (AnonymousFootprints || Camouflager.camouflageTimer > 0 || Helpers.MushroomSabotageActive())
                color = Palette.PlayerColors[6];
            else if (activeFootprint.Owner == Morphling.morphling && Morphling.morphTimer > 0 &&
                     Morphling.morphTarget && Morphling.morphTarget.Data != null)
                color = Palette.PlayerColors[Morphling.morphTarget.Data.DefaultOutfit.ColorId];
            else
                color = Palette.PlayerColors[activeFootprint.Data.DefaultOutfit.ColorId];

            color.a = Math.Clamp(p, 0f, 1f);
            activeFootprint.Renderer.color = color;

            activeFootprint.Lifetime -= dt;
        }

        foreach (var footprint in _toRemove)
        {
            footprint.GameObject.SetActive(false);
            _activeFootprints.Remove(footprint);
            _pool.Add(footprint);
        }
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    [HideFromIl2Cpp]
    public void MakeFootprint(PlayerControl player)
    {
        if (!_pool.TryTake(out var print)) print = new Footprint();

        print.Lifetime = FootprintDuration;

        var pos = player.transform.position;
        pos.z = (pos.y / 1000f) + 0.001f;
        print.Transform.SetPositionAndRotation(pos, Quaternion.EulerRotation(0, 0, Random.Range(0.0f, 360.0f)));
        print.GameObject.SetActive(true);
        print.Owner = player;
        print.Data = player.Data;
        _activeFootprints.Add(print);
    }

    private class Footprint
    {
        public readonly GameObject GameObject;
        public readonly SpriteRenderer Renderer;
        public readonly Transform Transform;
        public GameData.PlayerInfo Data;
        public float Lifetime;
        public PlayerControl Owner;

        public Footprint()
        {
            GameObject = new GameObject("Footprint") { layer = 8 };
            Transform = GameObject.transform;
            Renderer = GameObject.AddComponent<SpriteRenderer>();
            Renderer.sprite = FootprintSprite;
            Renderer.color = Color.clear;
            GameObject.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
        }
    }
}