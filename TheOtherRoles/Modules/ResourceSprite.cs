namespace TheOtherRoles.Modules;

#nullable enable
public class ResourceSprite(string pathName = "", float pixel = 115f, bool cache = true, Action<ResourceSprite>? onGetSprite = null)
{
    private const string ResourcePath = "TheOtherRoles.Resources.";

    private static readonly Assembly assembly = Assembly.GetExecutingAssembly();

    public readonly bool _cache = cache;

    public readonly string _pathName = pathName;

    public float _pixel = pixel;
    private Sprite? _sprite;

    public Sprite? ReturnSprite;

    public string Path => GetPath();

    public object? Instance { get; set; }

    public static implicit operator Sprite(ResourceSprite rs)
    {
        return rs.GetSprite();
    }

    public event Action<ResourceSprite>? OnGetSprite = onGetSprite;

    public Sprite GetSprite()
    {
        OnGetSprite?.Invoke(this);

        if (ReturnSprite != null)
            return ReturnSprite;

        if (_sprite != null && _sprite.pixelsPerUnit == _pixel)
            return _sprite;

        _sprite = UnityHelper.loadSpriteFromResources(GetPath(), _pixel, _cache);
        return _sprite;
    }

    private string GetPath()
    {
        if (assembly.GetManifestResourceNames().Contains(ResourcePath + _pathName)) return ResourcePath + _pathName;

        return _pathName;
    }

    internal void Destroy()
    {
        _sprite?.Destroy();
        ReturnSprite?.Destroy();
    }
}


public class ResourceSpriteArray((string, float)[] sprites, bool cache = true, Action<ResourceSpriteArray>? onGet = null) : List<ResourceSprite>
{
    public (string, float)[] Sprites = sprites;
    public int Current;
    private Action<ResourceSpriteArray>? OnGet = onGet;

    public ResourceSprite GetSprite(int value = -1)
    {
        if (Current != value && value != -1)
            Current = value;
        OnGet?.Invoke(this);
        if (Current >= Count)
        {
            ForEach(n => n.Destroy());
            Clear();

            foreach (var (path, pixel) in Sprites)
            {
                var sp = new ResourceSprite(path, pixel, cache);
                Add(sp);
            }
        }

        return this[Current];
    }

    public ResourceSprite Set(Index index)
    {
        Current = index.Value;
        return this;
    }

    public static implicit operator ResourceSprite(ResourceSpriteArray array) => array.GetSprite();
    public static implicit operator Sprite(ResourceSpriteArray array) => array.GetSprite();
    public static implicit operator (string, float)(ResourceSpriteArray array) => array.Sprites[array.Current];
}