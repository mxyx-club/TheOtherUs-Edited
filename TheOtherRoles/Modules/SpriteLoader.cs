#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using Reactor.Utilities.Attributes;
using Reactor.Utilities.Extensions;
using TheOtherRoles.CustomCosmetics;
using UnityEngine;

namespace TheOtherRoles.Modules;

public class SpriteLoader(DirectoryInfo directory)
{
    
    public SpriteLoader(string path) : this(new DirectoryInfo(path)) { }

    public List<Sprite> LoadAll(string ex, bool DontUnload, Vector2 pivot, float pixelsPerUnit)
    {
        return directory.GetFiles().Where(n => n.Extension == ex)
            .Select(file => LoadSprite(file.OpenRead(),  DontUnload, pivot, pixelsPerUnit)).ToList();
    }
    
    public List<Sprite> LoadAllHatSprite(string ex)
    {
        var list = new List<Sprite>();
        foreach (var file in directory.GetFiles().Where(n => n.Extension == ex))
        {
            try
            {
                var texture = LoadTexture(file.OpenRead(), true);
                var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.53f, 0.575f), texture.width * 0.375f);
                sprite.name = $"{directory.Name}/{file.Name}";
                sprite.DontDe();
                Info($"Load Sprite {sprite.name}");
                list.Add(sprite);
            }
            catch (Exception e)
            {
                Exception(e);
                Info($"Load Error {file.Name}");
            }
        }

        return list;
    }
    
    public static Sprite LoadSprite(Stream stream, bool DontUnload , Vector2 pivot, float pixelsPerUnit)
    {
        var texture = LoadTexture(stream, DontUnload);
        var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), pivot, pixelsPerUnit);
        if (DontUnload)
            sprite.DontDe();
        return sprite;
    }

    public static Texture2D LoadTexture(Stream stream, bool DontUnload)
    {
        var texture = new Texture2D(2, 2, TextureFormat.ARGB32, true);
        var byteTexture = stream.ReadFully();
        ImageConversion.LoadImage(texture, byteTexture, false);
        if (DontUnload)
            texture.DontDe();
        return texture;
    }

    public static Sprite LoadHatSpriteFormDisk(Stream stream, string Name)
    {
        var texture = LoadTexture(stream, true);
        var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.53f, 0.575f), texture.width * 0.375f);
        sprite.name = Name;
        sprite.DontDe();
        return sprite;
    }

    public static void LoadHatSprite(CustomCosmeticsFlags flags, string name)
    {
        var path = Path.Combine(flags switch
        {
            CustomCosmeticsFlags.Hat => CosmeticsManager.HatDir,
            CustomCosmeticsFlags.Visor => CosmeticsManager.HatDir,
            CustomCosmeticsFlags.NamePlate => CosmeticsManager.HatDir,
            _ => throw new ArgumentOutOfRangeException(nameof(flags))
        }, name);
        if (!File.Exists(path)) return;
        SpriteReader.Instance.Paths.Enqueue(path);
    }
}

[RegisterInIl2Cpp]
public sealed class SpriteReader : MonoBehaviour
{

    public readonly Queue<string> Paths = new();
    public bool running;
    public bool createRunning;
    private static SpriteReader? instance;
    public List<Sprite> sprites = CosmeticsManager.Instance.Sprites;

    public static SpriteReader Instance => instance ??= Main.Instance.AddComponent<SpriteReader>();

    public SpriteReader()
    {
        instance = this;
    }

    public void Update()
    {
        if (Paths.Any() && !running) 
            StartCoroutine(Load().WrapToIl2Cpp());
        
        if (!CreateEnd && !createRunning) 
            StartCoroutine(Create().WrapToIl2Cpp());
    }

    public IEnumerator Load()
    {
        running = true;
        dep:
        if (Paths.TryDequeue(out var path))
        {
            if (!File.Exists(path))
                goto dep;
            var info = new FileInfo(path);
            if (sprites.Any(n => n.name.Contains(info.Name)))
                goto dep;
            using var stream = info.OpenRead();
            var sp = SpriteLoader.LoadHatSpriteFormDisk(stream, $"{info.DirectoryName}/{info.Name}");
            CosmeticsManager.Instance.Sprites.Add(sp);
            Info($"AddSprite {sp.name} {path}");
            yield return null;
            goto dep;
        }
        
        running = false;
    }

    private bool CreateEnd;
    public IEnumerator Create()
    {
        createRunning = true;
        var max = CosmeticsManager.Instance.customCosmetics.Count;
        var count = 1;
        foreach (var cosmetic in CosmeticsManager.Instance.customCosmetics)
        {
            var sprite = new List<Sprite>();
            foreach (var r in cosmetic.Resources)
            {
                if (!sprites.Any(n => n.name.EndsWith(r)))
                {
                    var p = CosmeticsManager.GetLocalPath(cosmetic.Flags, r);
                    if (!File.Exists(p))continue;
                    sprites.Add(SpriteLoader.LoadHatSpriteFormDisk(
                        File.OpenRead(p), $"{cosmetic.Flags}/{r}"));
                }
                var sp = sprites.FirstOrDefault(n => n.name.EndsWith(r));
                if (sp) sprite.Add(sp!);
            }
            cosmetic.Create(sprite);
            Info($"Create {count}/{max} {cosmetic.Id} {cosmetic.config.Name}");
            count++;
            yield return null;
        }
        CosmeticsManager.CheckAddAll();
        yield return null;
        CreateEnd = true;
        createRunning = false;
    }
}
