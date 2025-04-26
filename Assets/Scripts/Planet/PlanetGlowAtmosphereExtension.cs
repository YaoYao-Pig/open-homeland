using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Planet))]
public class PlanetGlowAtmosphereExtension : MonoBehaviour
{
    [Header("Atmosphere Settings")]
    [SerializeField] private bool enableAtmosphere = true;
    [SerializeField] private Color atmosphereColor = new Color(0.5f, 0.7f, 1.0f, 1.0f);
    [Range(1, 10)]
    [SerializeField] private float glowFactor = 5f;
    [Range(0, 0.2f)]
    [SerializeField] private float vertexOffset = 0.05f;
    [Range(0, 5)]
    [SerializeField] private float lightIntensity = 1.5f;
    [Range(0, 10)]
    [SerializeField] private float rimPower = 3f;
    
    private Planet planet;
    private GameObject atmosphereObject;
    private AtmosphereGlowController atmosphereController;
    
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
            atmosphereObject = new GameObject("AtmosphereGlow");
            atmosphereObject.transform.parent = transform;
            atmosphereObject.transform.localPosition = Vector3.zero;
            atmosphereObject.transform.localRotation = Quaternion.identity;
            
            // Add required components
            atmosphereController = atmosphereObject.AddComponent<AtmosphereGlowController>();
            
            // Apply settings
            ApplySettings();
            
            // Adjust atmosphere to match planet
            atmosphereController.AdjustToMatchPlanet();
        }
    }
    
    private void ApplySettings()
    {
        if (atmosphereController != null)
        {
            // Use reflection to set private fields
            var colorField = atmosphereController.GetType().GetField("atmosphereColor", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (colorField != null) colorField.SetValue(atmosphereController, atmosphereColor);
            
            var glowFactorField = atmosphereController.GetType().GetField("glowFactor", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (glowFactorField != null) glowFactorField.SetValue(atmosphereController, glowFactor);
            
            var vertexOffsetField = atmosphereController.GetType().GetField("vertexOffset", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (vertexOffsetField != null) vertexOffsetField.SetValue(atmosphereController, vertexOffset);
            
            var lightIntensityField = atmosphereController.GetType().GetField("lightIntensity", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (lightIntensityField != null) lightIntensityField.SetValue(atmosphereController, lightIntensity);
            
            var rimPowerField = atmosphereController.GetType().GetField("rimPower", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (rimPowerField != null) rimPowerField.SetValue(atmosphereController, rimPower);
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
            ApplySettings();
            atmosphereController.AdjustToMatchPlanet();
        }
    }
}
