using System.Globalization;

namespace TheOtherRoles.Utilities;

public static class GithubUtils
{
    public static bool IsCN() => RegionInfo.CurrentRegion.ThreeLetterISORegionName == "CHN";

    public static string GithubUrl(this string url) 
    {
        if (IsCN() && !url.Contains("github.moeyy.xyz"))
        {
            if (url.Contains("github.com"))
            {
                return url.Replace("https://github.com", "https://github.moeyy.xyz/https://github.com");
            }

            if (url.Contains("raw.githubusercontent.com"))
            {
                return url.Replace("https://raw.githubusercontent.com", "https://github.moeyy.xyz/https://raw.githubusercontent.com");
            }
        }

        return url;
    }
}