using System.Collections.Generic;

namespace TheOtherRoles.Modules.CustomHats.Extensions;

internal static class HatDataExtensions
{
    public static HatExtension GetHatExtension(this HatData hat)
    {
        if (CustomHatManager.TestExtension != null && CustomHatManager.TestExtension.Condition.Equals(hat.name))
            return CustomHatManager.TestExtension;

        return CustomHatManager.ExtensionCache.GetValueOrDefault(hat.name);
    }
}