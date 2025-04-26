using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Planet))]
public class PlanetAtmosphereExtension : MonoBehaviour
{
    [Header("Atmosphere Settings")]
    [SerializeField] private bool enableAtmosphere = true;
    
    private Planet planet;
    private GameObject atmosphereObject;
    private AtmosphereController atmosphereController;
    
    private void Awake()
    {
        planet = GetComponent<Planet>();
    }
    
    private void Start()
    {
        // Wait for the planet to be fully initialized
        StartCoroutine(SetupAtmosphereDelayed());
    }
    
    private IEnumerator SetupAtmosphereDelayed()
    {
        // Wait for one frame to ensure the planet is fully initialized
        yield return null;
        
        if (enableAtmosphere)
        {
            SetupAtmosphere();
        }
    }
    
    private void SetupAtmosphere()
    {
        // Create atmosphere object if it doesn't exist
        if (atmosphereObject == null)
        {
            atmosphereObject = new GameObject("Atmosphere");
            atmosphereObject.transform.parent = transform;
            atmosphereObject.transform.localPosition = Vector3.zero;
            atmosphereObject.transform.localRotation = Quaternion.identity;
            
            // Add required components
            atmosphereController = atmosphereObject.AddComponent<AtmosphereController>();
            
            // Adjust atmosphere to match planet
            atmosphereController.AdjustToMatchPlanet();
        }
    }
    
    public void ToggleAtmosphere(bool enable)
    {
        enableAtmosphere = enable;
        
        if (enable && atmosphereObject == null)
        {
            SetupAtmosphere();
        }
        else if (!enable && atmosphereObject != null)
        {
            Destroy(atmosphereObject);
            atmosphereObject = null;
            atmosphereController = null;
        }
    }
    
    // This method should be called whenever the planet is updated
    public void UpdateAtmosphere()
    {
        if (enableAtmosphere && atmosphereController != null)
        {
            atmosphereController.AdjustToMatchPlanet();
        }
    }
}
