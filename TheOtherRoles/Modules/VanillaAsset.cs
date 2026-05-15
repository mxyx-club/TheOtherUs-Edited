using Twitch;

namespace TheOtherRoles.Modules;

public class VanillaAsset
{
    public class VanillaAudioClip
    {
        private string name;
        private AudioClip clip;
        public AudioClip Clip
        {
            get
            {
                if (clip) return clip;
                clip = UnityHelper.FindAsset<AudioClip>(name);
                return clip!;
            }
        }

        public VanillaAudioClip(string name)
        {
            this.name = name;
        }
    }

    public static Sprite CloseButtonSprite { get; private set; } = null!;
    public static TextMeshPro StandardTextPrefab { get; private set; } = null!;
    public static PlayerCustomizationMenu PlayerOptionsMenuPrefab { get; private set; } = null!;
    public static Sprite PopUpBackSprite { get; private set; } = null!;
    public static Sprite FullScreenSprite { get; private set; } = null!;
    public static Sprite TextButtonSprite { get; private set; } = null!;
    public static VanillaAudioClip HoverClip { get; private set; } = new("UI_Hover")!;
    public static VanillaAudioClip SelectClip { get; private set; } = new("UI_Select")!;
    public static bool Loaded = false;

    public static Material OblongMaskedFontMaterial
    {
        get
        {
            if (field == null) field = UnityHelper.FindAsset<Material>("Brook Atlas Material Masked");
            return field!;
        }
    }

    public static TMP_FontAsset PreSpawnFont
    {
        get
        {
            if (field == null) field = UnityHelper.FindAsset<TMP_FontAsset>("DIN_Pro_Bold_700 SDF")!;
            return field;
        }
    }

    public static Material StandardMaskedFontMaterial
    {
        get
        {
            if (field == null) field = UnityHelper.FindAsset<Material>("LiberationSans SDF - BlackOutlineMasked")!;
            return field!;
        }
    }

    public static TMP_FontAsset VersionFont
    {
        get
        {
            if (field == null) field = UnityHelper.FindAsset<TMP_FontAsset>("Barlow-Medium SDF");
            return field!;
        }
    }

    public static TMP_FontAsset BrookFont
    {
        get
        {
            if (field == null) field = UnityHelper.FindAsset<TMP_FontAsset>("Brook SDF")!;
            return field;
        }
    }


    public static Material GetHighlightMaterial
    {
        get
        {
            if (field != null) return new Material(field);
            foreach (var mat in Resources.FindObjectsOfTypeAll(Il2CppType.Of<Material>()))
            {
                if (mat.name == "HighlightMat")
                {
                    field = mat.TryCast<Material>();
                    break;
                }
            }
            return new Material(field);
        }
    }

    public static readonly ShipStatus[] MapAsset = new ShipStatus[6];
    public static Vector2 GetMapCenter(byte mapId) => MapAsset[mapId].MapPrefab.transform.GetChild(5).localPosition;
    public static float GetMapScale(byte mapId) => MapAsset[mapId].MapScale;
    public static Vector2 ConvertToMinimapPos(Vector2 pos, Vector2 center, float scale) => (pos / scale) + center;
    public static Vector2 ConvertToMinimapPos(Vector2 pos, byte mapId) => ConvertToMinimapPos(pos, GetMapCenter(mapId), GetMapScale(mapId));
    public static Vector2 ConvertFromMinimapPosToWorld(Vector2 minimapPos, Vector2 center, float scale) => (minimapPos - center) * scale;
    public static Vector2 ConvertFromMinimapPosToWorld(Vector2 minimapPos, byte mapId) => ConvertFromMinimapPosToWorld(minimapPos, GetMapCenter(mapId), GetMapScale(mapId));

    public static void LoadAssetsOnTitle()
    {
        var twitchPopUp = TwitchManager.Instance.transform.GetChild(0);
        PopUpBackSprite = twitchPopUp.GetChild(3).GetComponent<SpriteRenderer>().sprite;
        FullScreenSprite = twitchPopUp.GetChild(0).GetComponent<SpriteRenderer>().sprite;
        CloseButtonSprite = UnityHelper.FindAsset<Sprite>("closeButton")!;
        TextButtonSprite = twitchPopUp.GetChild(2).GetComponent<SpriteRenderer>().sprite;

        StandardTextPrefab = UObject.Instantiate(twitchPopUp.GetChild(1).GetComponent<TextMeshPro>(), null);
        StandardTextPrefab.gameObject.hideFlags = HideFlags.HideAndDontSave;
        UObject.Destroy(StandardTextPrefab.spriteAnimator);
        UObject.DontDestroyOnLoad(StandardTextPrefab.gameObject);
    }

    public static void PlaySelectSE() => SoundManager.Instance.PlaySound(SelectClip.Clip, false, 0.8f);
    public static void PlayHoverSE() => SoundManager.Instance.PlaySound(HoverClip.Clip, false, 0.8f);

    public static void LoadAssetAtInitialize()
    {
        if (Loaded) return;
        PlayerOptionsMenuPrefab = UnityHelper.FindAsset<PlayerCustomizationMenu>("LobbyPlayerCustomizationMenu")!;
        Loaded = true;
    }

    public static Scroller GenerateScroller(Vector2 size, Transform transform, Vector3 scrollBarLocalPos, Transform target, FloatRange bounds, float scrollerHeight)
    {
        var barBack = UObject.Instantiate(PlayerOptionsMenuPrefab.transform.GetChild(4).FindChild("UI_ScrollbarTrack").gameObject, transform);
        var bar = UObject.Instantiate(PlayerOptionsMenuPrefab.transform.GetChild(4).FindChild("UI_Scrollbar").gameObject, transform);
        barBack.transform.localPosition = scrollBarLocalPos + new Vector3(0.12f, 0f, 0f);
        bar.transform.localPosition = scrollBarLocalPos;

        var scrollBar = bar.GetComponent<Scrollbar>();

        var scroller = UnityHelper.CreateObject<Scroller>("Scroller", transform, new Vector3(0, 0, 5));
        scroller.gameObject.AddComponent<BoxCollider2D>().size = size;

        scrollBar.parent = scroller;
        scrollBar.graphic = bar.GetComponent<SpriteRenderer>();
        scrollBar.trackGraphic = barBack.GetComponent<SpriteRenderer>();
        scrollBar.trackGraphic.size = new Vector2(scrollBar.trackGraphic.size.x, scrollerHeight);

        var ratio = scrollerHeight / 3.88f;

        scroller.Inner = target;
        scroller.SetBounds(bounds, null);
        scroller.allowY = true;
        scroller.allowX = false;
        scroller.ScrollbarYBounds = new FloatRange(-1.8f * ratio + scrollBarLocalPos.y + 0.4f, 1.8f * ratio + scrollBarLocalPos.y - 0.4f);
        scroller.ScrollbarY = scrollBar;
        scroller.active = true;
        //scroller.Colliders = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<Collider2D>(new Collider2D[] { hitBox });

        scroller.ScrollToTop();

        return scroller;
    }
}
