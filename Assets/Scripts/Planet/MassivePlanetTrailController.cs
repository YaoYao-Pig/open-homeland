using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassivePlanetTrailController : MonoBehaviour
{
    [Header("Massive Planet Trail Settings")]
    [SerializeField] private Material trailMaterial;
    [Range(1f, 15f)]
    [SerializeField] private float trailTime = 8.0f;
    [Range(1f, 10f)]
    [SerializeField] private float startWidth = 4.0f;
    [Range(0.5f, 5f)]
    [SerializeField] private float endWidth = 1.5f;
    [SerializeField] private AnimationCurve widthCurve;
    [SerializeField] private Gradient colorGradient;
    
    [Header("Glow Effect")]
    [SerializeField] private bool useSecondaryGlow = true;
    [SerializeField] private Material glowMaterial;
    [Range(1f, 10f)]
    [SerializeField] private float glowMultiplier = 1.5f;
    
    [Header("Animation")]
    [SerializeField] private bool animateWidth = true;
    [Range(0.1f, 5f)]
    [SerializeField] private float pulseSpeed = 1.2f;
    [Range(0.1f, 1f)]
    [SerializeField] private float pulseAmount = 0.4f;
    
    [Header("Light Effect")]
    [SerializeField] private bool addPointLight = true;
    [SerializeField] private Color lightColor = Color.white;
    [Range(1f, 50f)]
    [SerializeField] private float lightRange = 15f;
    [Range(0.1f, 5f)]
    [SerializeField] private float lightIntensity = 2f;
    
    private TrailRenderer mainTrail;
    private TrailRenderer glowTrail;
    private GameObject glowTrailObject;
    private Light pointLight;
    private GameObject lightObject;
    private float timer = 0f;
    private float baseWidth;
    
    private void Awake()
    {
        // Get or add main trail renderer
        mainTrail = GetComponent<TrailRenderer>();
        if (mainTrail == null)
        {
            mainTrail = gameObject.AddComponent<TrailRenderer>();
        }
        
        // Create default width curve if needed
        if (widthCurve == null || widthCurve.keys.Length == 0)
        {
            widthCurve = new AnimationCurve();
            widthCurve.AddKey(0f, 1f);
            widthCurve.AddKey(1f, 0f);
        }
        
        // Create default color gradient if needed
        if (colorGradient.colorKeys.Length == 0)
        {
            InitializeDefaultGradient();
        }
        
        // Create secondary glow trail if enabled
        if (useSecondaryGlow)
        {
            glowTrailObject = new GameObject("MassiveGlowTrail");
            glowTrailObject.transform.parent = transform;
            glowTrailObject.transform.localPosition = Vector3.zero;
            glowTrailObject.transform.localRotation = Quaternion.identity;
            glowTrail = glowTrailObject.AddComponent<TrailRenderer>();
        }
        
        // Create point light if enabled
        if (addPointLight)
        {
            lightObject = new GameObject("TrailLight");
            lightObject.transform.parent = transform;
            lightObject.transform.localPosition = Vector3.zero;
            pointLight = lightObject.AddComponent<Light>();
            pointLight.type = LightType.Point;
            pointLight.color = lightColor;
            pointLight.range = lightRange;
            pointLight.intensity = lightIntensity;
        }
    }
    
    private void Start()
    {
        // Configure trail renderers
        SetupTrails();
        
        // Store base width for animation
        baseWidth = startWidth;
    }
    
    private void Update()
    {
        // Animate trail widths if enabled
        if (animateWidth)
        {
            timer += Time.deltaTime * pulseSpeed;
            float widthMultiplier = 1f + Mathf.Sin(timer) * pulseAmount;
            
            mainTrail.startWidth = baseWidth * widthMultiplier;
            
            if (useSecondaryGlow && glowTrail != null)
            {
                glowTrail.startWidth = baseWidth * glowMultiplier * widthMultiplier;
            }
        }
    }
    
    private void SetupTrails()
    {
        // Setup main trail
        if (mainTrail != null)
        {
            if (trailMaterial != null)
            {
                mainTrail.material = trailMaterial;
            }
            mainTrail.time = trailTime;
            mainTrail.startWidth = startWidth;
            mainTrail.endWidth = endWidth;
            mainTrail.widthCurve = widthCurve;
            mainTrail.colorGradient = colorGradient;
            mainTrail.generateLightingData = false;
            mainTrail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            mainTrail.receiveShadows = false;
        }
        
        // Setup glow trail
        if (useSecondaryGlow && glowTrail != null)
        {
            if (glowMaterial != null)
            {
                glowTrail.material = glowMaterial;
            }
            else if (trailMaterial != null)
            {
                glowTrail.material = trailMaterial;
            }
            
            glowTrail.time = trailTime * 1.2f; // Slightly longer than main trail
            glowTrail.startWidth = startWidth * glowMultiplier;
            glowTrail.endWidth = endWidth * glowMultiplier;
            glowTrail.widthCurve = widthCurve;
            glowTrail.colorGradient = colorGradient;
            glowTrail.generateLightingData = false;
            glowTrail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            glowTrail.receiveShadows = false;
        }
        
        // Setup point light
        if (addPointLight && pointLight != null)
        {
            pointLight.color = lightColor;
            pointLight.range = lightRange;
            pointLight.intensity = lightIntensity;
        }
    }
    
    private void InitializeDefaultGradient()
    {
        colorGradient = new Gradient();
        GradientColorKey[] colorKeys = new GradientColorKey[2];
        colorKeys[0].color = Color.white;
        colorKeys[0].time = 0f;
        colorKeys[1].color = new Color(0.3f, 0.7f, 1f);
        colorKeys[1].time = 1f;
        
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0].alpha = 1f;
        alphaKeys[0].time = 0f;
        alphaKeys[1].alpha = 0f;
        alphaKeys[1].time = 1f;
        
        colorGradient.SetKeys(colorKeys, alphaKeys);
    }
    
    // Public methods for preset configurations
    public void ApplyUltraMassivePreset()
    {
        startWidth = 5.0f;
        endWidth = 2.0f;
        trailTime = 8.0f;
        baseWidth = startWidth;
        glowMultiplier = 1.8f;
        lightRange = 20f;
        lightIntensity = 3f;
        
        SetupTrails();
    }
    
    public void ApplyGalacticFlamePreset()
    {
        startWidth = 4.0f;
        endWidth = 1.5f;
        trailTime = 7.0f;
        baseWidth = startWidth;
        glowMultiplier = 1.6f;
        
        // Create fire gradient
        Gradient fireGradient = new Gradient();
        GradientColorKey[] colorKeys = new GradientColorKey[3];
        colorKeys[0].color = Color.yellow;
        colorKeys[0].time = 0f;
        colorKeys[1].color = new Color(1f, 0.5f, 0f);
        colorKeys[1].time = 0.5f;
        colorKeys[2].color = new Color(1f, 0.2f, 0f);
        colorKeys[2].time = 1f;
        
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0].alpha = 1f;
        alphaKeys[0].time = 0f;
        alphaKeys[1].alpha = 0f;
        alphaKeys[1].time = 1f;
        
        fireGradient.SetKeys(colorKeys, alphaKeys);
        colorGradient = fireGradient;
        
        // Update light color
        if (pointLight != null)
        {
            lightColor = new Color(1f, 0.5f, 0f);
            pointLight.color = lightColor;
            lightRange = 18f;
            lightIntensity = 2.5f;
        }
        
        SetupTrails();
    }
    
    public void ApplyCosmicEnergyPreset()
    {
        startWidth = 4.5f;
        endWidth = 1.8f;
        trailTime = 9.0f;
        baseWidth = startWidth;
        glowMultiplier = 1.7f;
        
        // Create energy gradient
        Gradient energyGradient = new Gradient();
        GradientColorKey[] colorKeys = new GradientColorKey[2];
        colorKeys[0].color = new Color(0.5f, 1f, 0.5f);
        colorKeys[0].time = 0f;
        colorKeys[1].color = new Color(0f, 0.8f, 0.4f);
        colorKeys[1].time = 1f;
        
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0].alpha = 1f;
        alphaKeys[0].time = 0f;
        alphaKeys[1].alpha = 0f;
        alphaKeys[1].time = 1f;
        
        energyGradient.SetKeys(colorKeys, alphaKeys);
        colorGradient = energyGradient;
        
        // Update light color
        if (pointLight != null)
        {
            lightColor = new Color(0.3f, 1f, 0.5f);
            pointLight.color = lightColor;
            lightRange = 22f;
            lightIntensity = 2.2f;
        }
        
        SetupTrails();
    }
    
    private void OnDestroy()
    {
        if (glowTrailObject != null)
        {
            Destroy(glowTrailObject);
        }
        
        if (lightObject != null)
        {
            Destroy(lightObject);
        }
    }
}
