using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class CometTrailController : MonoBehaviour
{
    [Header("Trail Settings")]
    [SerializeField] private Material trailMaterial;
    [Range(0.1f, 10f)]
    [SerializeField] private float trailTime = 2.0f;
    [Range(0.01f, 1f)]
    [SerializeField] private float startWidth = 0.2f;
    [Range(0.01f, 1f)]
    [SerializeField] private float endWidth = 0.05f;
    [SerializeField] private AnimationCurve widthCurve;
    [SerializeField] private Gradient colorGradient;
    
    [Header("Animation")]
    [SerializeField] private bool animateWidth = false;
    [SerializeField] private float pulseSpeed = 1.0f;
    [SerializeField] private float pulseAmount = 0.2f;
    
    [Header("Particles")]
    [SerializeField] private bool emitParticles = false;
    [SerializeField] private GameObject particlePrefab;
    [SerializeField] private float particleEmissionRate = 10f;
    
    private TrailRenderer trailRenderer;
    private float timer = 0f;
    private float baseWidth;
    
    private void Awake()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        
        // If no material is assigned, try to find the CometTrail material
        if (trailMaterial == null)
        {
            trailMaterial = Resources.Load<Material>("CometTrail");
        }
        
        // If no width curve is defined, create a default one
        if (widthCurve == null || widthCurve.keys.Length == 0)
        {
            widthCurve = new AnimationCurve();
            widthCurve.AddKey(0f, 1f);
            widthCurve.AddKey(1f, 0f);
        }
        
        // If no color gradient is defined, create a default one
        if (colorGradient.colorKeys.Length == 0)
        {
            GradientColorKey[] colorKeys = new GradientColorKey[2];
            colorKeys[0].color = Color.white;
            colorKeys[0].time = 0f;
            colorKeys[1].color = new Color(0.5f, 0.7f, 1f);
            colorKeys[1].time = 1f;
            
            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
            alphaKeys[0].alpha = 1f;
            alphaKeys[0].time = 0f;
            alphaKeys[1].alpha = 0f;
            alphaKeys[1].time = 1f;
            
            colorGradient.SetKeys(colorKeys, alphaKeys);
        }
    }
    
    private void Start()
    {
        // Configure the trail renderer
        SetupTrailRenderer();
        
        // Store the base width for animation
        baseWidth = startWidth;
        
        // Start particle emission if enabled
        if (emitParticles && particlePrefab != null)
        {
            StartCoroutine(EmitParticles());
        }
    }
    
    private void Update()
    {
        // Animate the trail width if enabled
        if (animateWidth)
        {
            timer += Time.deltaTime * pulseSpeed;
            float widthMultiplier = 1f + Mathf.Sin(timer) * pulseAmount;
            trailRenderer.startWidth = baseWidth * widthMultiplier;
        }
    }
    
    private void SetupTrailRenderer()
    {
        if (trailRenderer != null)
        {
            // Set the material
            if (trailMaterial != null)
            {
                trailRenderer.material = trailMaterial;
            }
            
            // Configure trail properties
            trailRenderer.time = trailTime;
            trailRenderer.startWidth = startWidth;
            trailRenderer.endWidth = endWidth;
            trailRenderer.widthCurve = widthCurve;
            trailRenderer.colorGradient = colorGradient;
            trailRenderer.generateLightingData = false;
            trailRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            trailRenderer.receiveShadows = false;
            trailRenderer.allowOcclusionWhenDynamic = false;
        }
    }
    
    private IEnumerator EmitParticles()
    {
        while (true)
        {
            // Calculate wait time based on emission rate
            float waitTime = 1f / particleEmissionRate;
            
            // Wait for the next emission
            yield return new WaitForSeconds(waitTime);
            
            // Emit a particle
            if (particlePrefab != null)
            {
                GameObject particle = Instantiate(particlePrefab, transform.position, Quaternion.identity);
                
                // Set the particle to destroy itself after a time
                Destroy(particle, trailTime);
            }
        }
    }
    
    // Public method to adjust trail settings at runtime
    public void UpdateTrailSettings(float newTrailTime, float newStartWidth, float newEndWidth)
    {
        trailTime = newTrailTime;
        startWidth = newStartWidth;
        baseWidth = newStartWidth;
        endWidth = newEndWidth;
        
        SetupTrailRenderer();
    }
}
