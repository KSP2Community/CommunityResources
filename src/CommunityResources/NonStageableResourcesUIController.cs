using KSP.Game;
using KSP.Sim.impl;
using KSP.Sim.ResourceSystem;
using UnityEngine;

namespace CommunityResourceUnits;

/// <summary>
/// Handles dynamic resizing of non-stageable resources flight HUD window based on number of resources
/// </summary>
internal class NonStageableResourcesUIController : KerbalMonoBehaviour
{
    private VesselComponent _activeVessel;
    private GameObject _nsResourcesUI;
    private int _lastNSResourcesCount = 0;

    private void Update()
    {
        // Get active vessel
        _activeVessel = Game?.ViewController?.GetActiveSimVessel(true);

        if (_activeVessel is null)
            return;

        // Get number of non-stageable resources in the active vessel
        int nsResourcesCount = GetNonStageableResourcesCount(_activeVessel);

        // Update UI only if resource count has changed
        if (nsResourcesCount != _lastNSResourcesCount)
        {
            // Find the non-stageable resources window game object
            _nsResourcesUI = GameObject.Find("GameManager/Default Game Instance(Clone)/UI Manager(Clone)/Scaled Main Canvas/FlightHudRoot(Clone)/NonStageableResources(Clone)/KSP2UIWindow/Root/UIPanel");

            if (_nsResourcesUI != null)
            {
                // Number of resources above the default maximum of 2 (EC & Monoprop)
                int extraResources = Math.Max(0, nsResourcesCount - 2);

                // Get the UI object's RectTransform
                RectTransform rect = _nsResourcesUI.GetComponent<RectTransform>();

                // Increase the UI object's vertical size
                rect.sizeDelta = new Vector2(0, 30 * extraResources);

                // Shift the UI object down
                rect.localPosition = extraResources * 15 * Vector3.down;

                _lastNSResourcesCount = nsResourcesCount;
            }
        }
    }

    /// <summary>
    /// Returns the number of non-stageable resources present on the vessel
    /// </summary>
    private int GetNonStageableResourcesCount(VesselComponent vessel)
    {
        int count = 0;

        PartComponent controlOwner = vessel.GetControlOwner();
        if (controlOwner != null && controlOwner.PartOwner != null)
        {
            ResourceContainerGroup containerGroup = controlOwner.PartOwner.ContainerGroup;
            foreach (ContainedResourceData containedResourceData in containerGroup.GetAllResourcesContainedData())
            {
                ResourceDefinitionData definitionData = Game.ResourceDefinitionDatabase.GetDefinitionData(containedResourceData.ResourceID);
                if (definitionData.resourceProperties.NonStageable)
                    ++count;
            }
        }

        return count;
    }
}
