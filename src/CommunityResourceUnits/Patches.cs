using HarmonyLib;
using KSP.Game;
using KSP.UI;

namespace CommunityResourceUnits;

internal static class Patches
{
    internal static readonly Dictionary<string, string> ResourceUnits = new Dictionary<string, string>();

    [HarmonyPatch(typeof(ResourceInfoDataContext), nameof(ResourceInfoDataContext.GetUnits))]
    [HarmonyPrefix]
    private static bool ResourceInfoDataContext_GetUnits(string resourceName, ref string __result)
    {
        if (ResourceUnits.TryGetValue(resourceName, out __result))
        {
            return false;
        }

        return true;
    }

    [HarmonyPatch(typeof(ResourceManagerContainerEntry), nameof(ResourceManagerContainerEntry.GetUnits))]
    [HarmonyPrefix]
    private static bool ResourceManagerContainerEntry_GetUnits(string resourceName, ref string __result)
    {
        if (ResourceUnits.TryGetValue(resourceName, out __result))
        {
            return false;
        }

        return true;
    }
}