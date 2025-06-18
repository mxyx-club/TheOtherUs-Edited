using System.IO;
using UnityEngine.Audio;
using UnityEngine.Events;

namespace TheOtherRoles.Helper;

public static class UnityHelper
{
    public static Dictionary<string, Sprite> CachedSprites = new();
    public static IRegionInfo CurrentServer => FastDestroyableSingleton<ServerManager>.Instance.CurrentRegion;
    public static bool IsCustomServer => CurrentServer.TranslateName
        is StringNames.NoTranslation || CurrentServer.TranslateName != StringNames.ServerAS && CurrentServer.TranslateName != StringNames.ServerEU && CurrentServer.TranslateName != StringNames.ServerNA;

    public static readonly List<Sprite> CacheSprite = new();

    public static T Dont<T>(this T obj) where T : UObject
    {
        obj.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontSaveInEditor;
        return obj;
    }

    public static Sprite loadSpriteFromResources(string path, float pixelsPerUnit, bool cache = true)
    {
        try
        {
            if (cache && CachedSprites.TryGetValue(path + pixelsPerUnit, out var sprite)) return sprite;
            var texture = loadTextureFromResources(path);
            sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f),
                pixelsPerUnit);
            if (cache) sprite.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontSaveInEditor;
            if (!cache) return sprite;
            return CachedSprites[path + pixelsPerUnit] = sprite;
        }
        catch
        {
            Error("Error loading sprite from path: " + path);
        }
        return null;
    }

    public static Sprite loadSpriteFromResources(Texture2D texture, float pixelsPerUnit, Rect textureRect)
    {
        return Sprite.Create(texture, textureRect, new Vector2(0.5f, 0.5f), pixelsPerUnit);
    }

    public static AudioClip FindSound(string sound)
    {
        foreach (var audio in UObject.FindObjectsOfTypeIncludingAssets(Il2CppType.Of<AudioClip>()))
        {
            if (audio.name == sound) return audio.Cast<AudioClip>();
        }
        return null;
    }

    public static string readTextFromFile(string path)
    {
        Stream stream = File.OpenRead(path);
        var textStreamReader = new StreamReader(stream);
        return textStreamReader.ReadToEnd();
    }

    public static unsafe Texture2D loadTextureFromResources(string path)
    {
        try
        {
            var texture = new Texture2D(2, 2, TextureFormat.ARGB32, true);
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(path);
            var length = stream!.Length;
            var byteTexture = new Il2CppStructArray<byte>(length);
            _ = stream.Read(new Span<byte>(IntPtr.Add(byteTexture.Pointer, IntPtr.Size * 4).ToPointer(), (int)length));
            ImageConversion.LoadImage(texture, byteTexture, false);
            return texture;
        }
        catch
        {
            //Error("loading texture from resources: " + path);
        }

        return null;
    }

    public static Texture2D loadTextureFromDisk(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                var texture = new Texture2D(2, 2, TextureFormat.ARGB32, true);
                var byteTexture = File.ReadAllBytes(path);
                ImageConversion.LoadImage(texture, byteTexture, false);
                return texture;
            }
        }
        catch
        {
            Error("Error loading texture from disk: " + path);
        }

        return null;
    }

    public static AudioSource PlaySound(Transform parent, AudioClip clip, bool loop, float volume = 1f, AudioMixerGroup audioMixer = null)
    {
        if (audioMixer == null)
        {
            audioMixer = loop ? SoundManager.Instance.MusicChannel : SoundManager.Instance.SfxChannel;
        }
        AudioSource value = parent.GetComponent<AudioSource>() ?? parent.gameObject.AddComponent<AudioSource>();
        value.outputAudioMixerGroup = audioMixer;
        value.playOnAwake = false;
        value.volume = volume;
        value.loop = loop;
        value.clip = clip;
        value.Play();
        return value;
    }

    public static Sprite LoadSprite(this Stream stream, bool DontUnload, Vector2 pivot, float pixelsPerUnit)
    {
        var texture = stream.LoadTexture(DontUnload);
        var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), pivot, pixelsPerUnit);
        if (DontUnload)
            sprite.Dont();
        return sprite;
    }

    public static Texture2D LoadTexture(this Stream stream, bool DontUnload)
    {
        var texture = new Texture2D(2, 2, TextureFormat.ARGB32, true);
        var byteTexture = stream.ReadFully();
        ImageConversion.LoadImage(texture, byteTexture, false);
        if (DontUnload)
            texture.Dont();
        return texture;
    }

    public static Sprite LoadHatSpriteFormDisk(this Stream stream, string Name)
    {
        var texture = stream.LoadTexture(true);
        var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.53f, 0.575f), texture.width * 0.375f);
        sprite.name = Name;
        sprite.Dont();
        return sprite;
    }

    public static AudioClip loadAudioClipFromResources(string path, string clipName = "UNNAMED_TOR_AUDIO_CLIP")
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(path);
            var byteAudio = stream!.ReadFully();
            var samples = new float[byteAudio.Length / 4]; // 4 bytes per sample
            for (var i = 0; i < samples.Length; i++)
            {
                var offset = i * 4;
                samples[i] = (float)BitConverter.ToInt32(byteAudio, offset) / int.MaxValue;
            }

            const int channels = 2;
            const int sampleRate = 48000;
            var audioClip = AudioClip.Create(clipName, samples.Length / 2, channels, sampleRate, false);
            audioClip.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontSaveInEditor;
            audioClip.SetData(samples, 0);
            return audioClip;
        }
        catch (Exception e)
        {
            Error($"loading AudioClip from resources: {path}\n{e}");
        }
        return null;
    }

    public static void AddListener(this UnityEvent @event, Action action)
    {
        @event.AddListener(action);
    }

    public static void AddListener<T>(this UnityEvent<T> @event, Action<T> action)
    {
        @event.AddListener(action);
    }

    public static GameObject DestroyAllChildren<T>(this GameObject obj) where T : MonoBehaviour
    {
        var list = obj.GetComponentsInChildren<T>();
        list.Do(UObject.Destroy);
        return obj;
    }

    public static IRegionInfo CreateHttpRegion(string name, string ip, ushort port)
    {
        return new StaticHttpRegionInfo(name,
                StringNames.NoTranslation,
                ip,
                new Il2CppReferenceArray<ServerInfo>(
                [
                    new ServerInfo(name, ip, port, false)
                ])
            )
            .CastFast<IRegionInfo>();
    }
}