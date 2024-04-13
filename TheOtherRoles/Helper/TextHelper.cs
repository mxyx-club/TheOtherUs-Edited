using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TheOtherRoles.Helper;

public static class TextHelper
{
    public static readonly Dictionary<SupportedLangs, string> LangNameDictionary = new()
    {
        { SupportedLangs.English, "English" },
        { SupportedLangs.Latam, "Latam" },
        { SupportedLangs.Brazilian, "Brazilian" },
        { SupportedLangs.Portuguese, "Portuguese" },
        { SupportedLangs.Korean, "Korean" },
        { SupportedLangs.Russian, "Russian" },
        { SupportedLangs.Dutch, "Dutch" },
        { SupportedLangs.Filipino, "Filipino" },
        { SupportedLangs.French, "French" },
        { SupportedLangs.German, "German" },
        { SupportedLangs.Italian, "Italian" },
        { SupportedLangs.Japanese, "Japanese" },
        { SupportedLangs.Spanish, "Spanish" },
        { SupportedLangs.SChinese, "SChinese" },
        { SupportedLangs.TChinese, "TChinese" },
        { SupportedLangs.Irish, "Irish" }
    };

    public static string ColorString(this string s, Color c)
    {
        return $"<color=#{ToByte(c.r):X2}{ToByte(c.g):X2}{ToByte(c.b):X2}{ToByte(c.a):X2}>{s}</color>";
    }

    public static byte ToByte(float f)
    {
        f = Mathf.Clamp01(f);
        return (byte)(f * 255);
    }


#nullable enable
    internal static void StartRead(this Stream stream, Action<string, int> OnRead, out string AllText)
    {
        var index = 1;
        var stringBuilder = new StringBuilder();
        using var reader = new StreamReader(stream);
        while (reader.ReadLine() is { } Line)
        {
            stringBuilder.AppendLine(Line);
            OnRead(Line, index);
            index++;
        }

        AllText = stringBuilder.ToString();
    }
#nullable disable

    public static SupportedLangs PareNameToLangId(this string text)
    {
        return LangNameDictionary.First(n => n.Value == text).Key;
    }

    public static SupportedLangs PareIndexToLangId(this string text)
    {
        return (SupportedLangs)int.Parse(text);
    }

    public static string PareString(this SupportedLangs lang)
    {
        return LangNameDictionary[lang];
    }

    public static int GetIndex(this SupportedLangs lang)
    {
        return (int)lang;
    }

    public static string[] SplitText(this string text)
    {
        return text.Split(':');
    }

    public static string SplitText(params string[] texts)
    {
        var builder = new StringBuilder();
        var index = 1;
        foreach (var text in texts)
        {
            builder.Append(text);
            if (index != texts.Length)
                builder.Append(':');

            index++;
        }

        return builder.ToString();
    }

    public static string GetNoColorText(this string text)
    {
        var s = string.Empty;
        var co = false;
        foreach (var c in text)
        {
            switch (c)
            {
                case '<':
                    co = true;
                    break;
                case '>':
                    co = false;
                    continue;
            }

            if (co)
                continue;

            s += c;
        }

        return s;
    }
}