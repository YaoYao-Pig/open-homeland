using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DualTrailController : MonoBehaviour
{
    [Header("Primary Trail")]
    [SerializeField] private Material primaryTrailMaterial;
    [Range(0.5f, 10f)]
    [SerializeField] private float primaryTrailTime = 3.0f;
    [Range(0.1f, 2f)]
    [SerializeField] private float primaryStartWidth = 0.8f;
    [Range(0.05f, 1f)]
    [SerializeField] private float primaryEndWidth = 0.3f;
    [SerializeField] private Gradient primaryColorGradient;
    
    [Header("Secondary Trail (Glow)")]
    [SerializeField] private Material secondaryTrailMaterial;
    [Range(0.5f, 10f)]
    [SerializeField] private float secondaryTrailTime = 3.5f;
    [Range(0.2f, 3f)]
    [SerializeField] private float secondaryStartWidth = 1.2f;
    [Range(0.1f, 1.5f)]
    [SerializeField] private float secondaryEndWidth = 0.5f;
    [SerializeField] private Gradient secondaryColorGradient;
    
    [Header("Animation")]
    [SerializeField] private bool animateWidth = true;
    [SerializeField] private float pulseSpeed = 1.0f;
    [SerializeField] private float pulseAmount = 0.3f;
    
    private TrailRenderer primaryTrail;
    private TrailRenderer secondaryTrail;
    private GameObject secondaryTrailObject;
    private float timer = 0f;
    private float primaryBaseWidth;
    private float secondaryBaseWidth;
    
    private void Awake()
    {
        // Get or add primary trail renderer
        primaryTrail = GetComponent<TrailRenderer>();
        if (primaryTrail == null)
        {
            primaryTrail = gameObject.AddComponent<TrailRenderer>();
        }
        
        // Create secondary trail object
        secondaryTrailObject = new GameObject("GlowTrail");
        secondaryTrailObject.transform.parent = transform;
        secondaryTrailObject.transform.localPosition = Vector3.zero;
        secondaryTrailObject.transform.localRotation = Quaternion.identity;
        secondaryTrail = secondaryTrailObject.AddComponent<TrailRenderer>();
        
        // Initialize default gradients if needed
        if (primaryColorGradient.colorKeys.Length == 0)
        {
            InitializeDefaultGradients();
        }
    }
    
    private void Start()
    {
        // Configure trail renderers
        SetupTrailRenderers();
        
        // Store base widths for animation
        primaryBaseWidth = primaryStartWidth;
        secondaryBaseWidth = secondaryStartWidth;
    }
    
    private void Update()
    {
        // Animate trail widths if enabled
        if (animateWidth)
        {
            timer += Time.deltaTime * pulseSpeed;
            float widthMultiplier = 1f + Mathf.Sin(timer) * pulseAmount;
            
            primaryTrail.startWidth = primaryBaseWidth * widthMultiplier;
            secondaryTrail.startWidth = secondaryBaseWidth * widthMultiplier;
        }
    }
    
    private void SetupTrailRenderers()
    {
        // Setup primary trail
        if (primaryTrail != null)
        {
            if (primaryTrailMaterial != null)
            {
                primaryTrail.material = primaryTrailMaterial;
            }
            primaryTrail.time = primaryTrailTime;
            primaryTrail.startWidth = primaryStartWidth;
            primaryTrail.endWidth = primaryEndWidth;
            primaryTrail.colorGradient = primaryColorGradient;
            primaryTrail.generateLightingData = false;
            primaryTrail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            primaryTrail.receiveShadows = false;
        }
        
        // Setup secondary trail (glow effect)
        if (secondaryTrail != null)
        {
            if (secondaryTrailMaterial != null)
            {
                secondaryTrail.material = secondaryTrailMaterial;
            }
            secondaryTrail.time = secondaryTrailTime;
            secondaryTrail.startWidth = secondaryStartWidth;
            secondaryTrail.endWidth = secondaryEndWidth;
            secondaryTrail.colorGradient = secondaryColorGradient;
            secondaryTrail.generateLightingData = false;
            secondaryTrail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            secondaryTrail.receiveShadows = false;
        }
    }
    
    private void InitializeDefaultGradients()
    {
        // Initialize primary gradient
        primaryColorGradient = new Gradient();
        GradientColorKey[] primaryColorKeys = new GradientColorKey[2];
        primaryColorKeys[0].color = Color.white;
        primaryColorKeys[0].time = 0f;
        primaryColorKeys[1].color = new Color(0.5f, 0.7f, 1f);
        primaryColorKeys[1].time = 1f;
        
        GradientAlphaKey[] primaryAlphaKeys = new GradientAlphaKey[2];
        primaryAlphaKeys[0].alpha = 1f;
        primaryAlphaKeys[0].time = 0f;
        primaryAlphaKeys[1].alpha = 0f;
        primaryAlphaKeys[1].time = 1f;
        
        primaryColorGradient.SetKeys(primaryColorKeys, primaryAlphaKeys);
        
        // Initialize secondary gradient
        secondaryColorGradient = new Gradient();
        GradientColorKey[] secondaryColorKeys = new GradientColorKey[2];
        secondaryColorKeys[0].color = new Color(0.7f, 0.8f, 1f, 0.5f);
        secondaryColorKeys[0].time = 0f;
        secondaryColorKeys[1].color = new Color(0.3f, 0.5f, 0.8f, 0.2f);
        secondaryColorKeys[1].time = 1f;
        
        GradientAlphaKey[] secondaryAlphaKeys = new GradientAlphaKey[2];
        secondaryAlphaKeys[0].alpha = 0.5f;
        secondaryAlphaKeys[0].time = 0f;
        secondaryAlphaKeys[1].alpha = 0f;
        secondaryAlphaKeys[1].time = 1f;
        
        secondaryColorGradient.SetKeys(secondaryColorKeys, secondaryAlphaKeys);
    }
    
    // Public method to adjust trail settings at runtime
    public void UpdateTrailSettings(float newPrimaryWidth, float newSecondaryWidth)
    {
        primaryStartWidth = newPrimaryWidth;
        primaryBaseWidth = newPrimaryWidth;
        primaryEndWidth = newPrimaryWidth * 0.4f;
        
        secondaryStartWidth = newSecondaryWidth;
        secondaryBaseWidth = newSecondaryWidth;
        secondaryEndWidth = newSecondaryWidth * 0.4f;
        
        SetupTrailRenderers();
    }
    
    private void OnDestroy()
    {
        if (secondaryTrailObject != null)
        {
            Destroy(secondaryTrailObject);
        }
    }
}
