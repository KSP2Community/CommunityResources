using BepInEx;
using HarmonyLib;
using JetBrains.Annotations;
using KSP.Game;
using Newtonsoft.Json.Linq;
using PatchManager;
using SpaceWarp;
using SpaceWarp.API.Mods;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CommunityResourceUnits;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(SpaceWarpPlugin.ModGuid, SpaceWarpPlugin.ModVer)]
[BepInDependency(PatchManagerPlugin.ModGuid, PatchManagerPlugin.ModVer)]
public class CommunityResourceUnitsPlugin : BaseSpaceWarpPlugin
{
    // These are useful in case some other mod wants to add a dependency to this one
    [PublicAPI] public const string ModGuid = MyPluginInfo.PLUGIN_GUID;
    [PublicAPI] public const string ModName = MyPluginInfo.PLUGIN_NAME;
    [PublicAPI] public const string ModVer = MyPluginInfo.PLUGIN_VERSION;

    // Singleton instance of the plugin class
    public static CommunityResourceUnitsPlugin Instance { get; set; }

    private void Awake()
    {
        Harmony.CreateAndPatchAll(typeof(Patches));
        Instance = this;
    }

    /// <summary>
    /// Runs when the mod is first initialized.
    /// </summary>
    public override void OnInitialized()
    {
        base.OnInitialized();

        GameManager.Instance.Assets.LoadByLabel<TextAsset>("resource_units", RegisterUnits, delegate (IList<TextAsset> assetLocations)
        {
            if (assetLocations != null)
            {
                Addressables.Release(assetLocations);
            }
        });

        // Non-Stageable Resources flight HUD window resizing
        gameObject.AddComponent<NonStageableResourcesUIController>();
    }

    private static void RegisterUnits(TextAsset asset)
    {
        var dict = JObject.Parse(asset.text);
        foreach (var value in dict)
        {
            Patches.ResourceUnits[value.Key] = value.Value.Value<string>();
        }
    }
}
