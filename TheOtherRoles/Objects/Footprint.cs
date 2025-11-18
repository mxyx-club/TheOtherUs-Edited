using System.Collections.Concurrent;

namespace TheOtherRoles.Objects;

public class FootprintHolder : MonoBehaviour
{
    static FootprintHolder()
    {
        ClassInjector.RegisterTypeInIl2Cpp<FootprintHolder>();
    }

    public FootprintHolder(IntPtr ptr) : base(ptr) { }

    public static FootprintHolder Instance
    {
        get => field ? field : field = new GameObject("FootprintHolder").AddComponent<FootprintHolder>();
        set;

    }

    private static Sprite FootprintSprite => field ??= new ResourceSprite("Footprint.png", 600f);

    private static bool AnonymousFootprints => Detective.anonymousFootprints == 2;
    private static bool SabotageActive => (isActiveCamoComms || MushroomSabotageActive) && Detective.anonymousFootprints == 1;
    private static float FootprintDuration => Detective.footprintDuration;

    private class Footprint
    {
        public GameObject GameObject;
        public Transform Transform;
        public SpriteRenderer Renderer;
        public PlayerControl Owner;
        public int ColorId = 6;
        public float Lifetime;

        public Footprint()
        {
            GameObject = new("Footprint") { layer = 8 };
            Transform = GameObject.transform;
            Renderer = GameObject.AddComponent<SpriteRenderer>();
            Renderer.sprite = FootprintSprite;
            Renderer.color = Color.clear;
            GameObject.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
        }
    }



    private readonly ConcurrentBag<Footprint> _pool = new();
    private readonly List<Footprint> _activeFootprints = new();
    private readonly List<Footprint> _toRemove = new();

    [HideFromIl2Cpp]
    public void MakeFootprint(PlayerControl player)
    {
        if (!_pool.TryTake(out var print))
        {
            print = new();
        }

        print.Lifetime = FootprintDuration;

        var pos = player.transform.position;
        pos.z = (pos.y / 1000f) + 0.001f;
        print.Transform.SetPositionAndRotation(pos, Quaternion.EulerRotation(0, 0, URandom.Range(0.0f, 360.0f)));
        print.GameObject.SetActive(true);
        print.Owner = player;
        print.ColorId = player.Data.DefaultOutfit.ColorId;
        _activeFootprints.Add(print);
    }

    private static float updateDt = 0.5f;

    public void Start()
    {
        InvokeRepeating(nameof(FootprintUpdate), updateDt, updateDt);
    }

    public void FootprintUpdate()
    {
        var dt = updateDt;
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
            if (AnonymousFootprints || Camouflager.camouflageTimer > 0 || SabotageActive)
            {
                color = Palette.PlayerColors[6];
            }
            else if (activeFootprint.Owner == Morphling.morphling && Morphling.morphTimer > 0 && Morphling.morphTarget && Morphling.morphTarget.Data != null)
            {
                color = Palette.PlayerColors[Morphling.morphTarget.Data.DefaultOutfit.ColorId];
            }
            else
            {
                color = Palette.PlayerColors[activeFootprint.ColorId];
            }

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

    public void OnDestroy()
    {
        Instance = null;
    }
}