using HarmonyLib;
using KSP;
using KSP.Game;
using KSP.Iteration.UI.Binding;
using KSP.Modules;
using KSP.OAB;
using KSP.Sim.Definitions;
using KSP.Sim.ResourceSystem;
using KSP.UI;
// using Microsoft.CodeAnalysis.CSharp.Syntax;

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

    [HarmonyPostfix,
     HarmonyPatch(typeof(Module_ResourceCapacities), nameof(Module_ResourceCapacities.OnInitialize))]
    private static void ResourceCapacities_GetUnits(Module_ResourceCapacities __instance)
    {
        var resourceDefinitionDatabase = GameManager.Instance.Game.ResourceDefinitionDatabase;
        if (__instance.PartBackingMode == PartBehaviourModule.PartBackingModes.OAB)
        {
            foreach (var container in __instance.OABPart.Containers)
            {
                foreach (var definition in container)
                {
                    var data = resourceDefinitionDatabase.GetDefinitionData(definition);
                    if (data.name != "IntakeAir")
                    {
                        if (ResourceUnits.TryGetValue(data.name, out var newUnit) && __instance.dataResourceCapacities.TryGetProperty<float>(data.name, out var property))
                        {
                            __instance.dataResourceCapacities.SetToStringDelegate(property,
                                o => __instance.RoundToSignificantFigures((float)o,
                                         3) +
                                     newUnit);
                        }
                    }
                }
            }
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PartInfoOverlay), nameof(PartInfoOverlay.PopulateResourceInfoFromPart))]
    private static bool PopulateResourceInfoFromPart(List<KeyValuePair<string, string>> dict,
        IObjectAssemblyResource[] resourceArray)
    {
        foreach (var resource in resourceArray)
        {
            if (!ResourceUnits.TryGetValue(resource.Name, out var unitName))
            {
                unitName = resource.Name == "ElectricCharge" ? Units.SymbolUnits : Units.SymbolTonne;
            }

            dict.Add(new KeyValuePair<string, string>(PartInfoOverlay.LocalizeVABTooltipString(resource.Name),
                $"{resource.Count} {PartInfoOverlay.LocalizeVABTooltipString(unitName)}"));
        }

        return false;
    }
}