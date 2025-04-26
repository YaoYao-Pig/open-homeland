Shader "Planet/Atmosphere"
{
    Properties
    {
        _AtmosphereColor("Atmosphere Color", Color) = (0.5, 0.7, 1.0, 1.0)
        _AtmosphereAlpha("Atmosphere Alpha", Range(0, 1)) = 0.5
        _AtmosphereHeight("Atmosphere Height", Range(0, 0.5)) = 0.1
        _AtmosphereFalloff("Atmosphere Falloff", Range(0, 10)) = 3
        _AtmosphereRimPower("Atmosphere Rim Power", Range(0, 20)) = 5
        _SunDir("Sun Direction", Vector) = (0, 1, 0, 0)
        _SunIntensity("Sun Intensity", Range(0, 10)) = 1
        _RayleighCoefficient("Rayleigh Coefficient", Range(0, 10)) = 1
        _MieCoefficient("Mie Coefficient", Range(0, 10)) = 0.5
        _MieDirectionalG("Mie Directional G", Range(0, 1)) = 0.8
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Front
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            
            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
                float3 normal : TEXCOORD2;
                float3 planetCenter : TEXCOORD3;
            };
            
            float4 _AtmosphereColor;
            float _AtmosphereAlpha;
            float _AtmosphereHeight;
            float _AtmosphereFalloff;
            float _AtmosphereRimPower;
            float3 _SunDir;
            float _SunIntensity;
            float _RayleighCoefficient;
            float _MieCoefficient;
            float _MieDirectionalG;
            
            // Helper function for Rayleigh scattering
            float3 RayleighScattering(float cosTheta)
            {
                float rayleigh = 0.75 * (1.0 + cosTheta * cosTheta);
                return _RayleighCoefficient * rayleigh * _AtmosphereColor.rgb;
            }
            
            // Helper function for Mie scattering
            float3 MieScattering(float cosTheta)
            {
                float g = _MieDirectionalG;
                float g2 = g * g;
                float mie = (1.0 - g2) / pow(1.0 + g2 - 2.0 * g * cosTheta, 1.5);
                return _MieCoefficient * mie * _AtmosphereColor.rgb;
            }
            
            v2f vert (appdata v)
            {
                v2f o;
                
                // Scale the atmosphere mesh slightly larger than the planet
                float3 scaledVertex = v.vertex.xyz * (1.0 + _AtmosphereHeight);
                o.vertex = UnityObjectToClipPos(float4(scaledVertex, 1.0));
                
                // Get world position and view direction
                o.worldPos = mul(unity_ObjectToWorld, float4(scaledVertex, 1.0)).xyz;
                o.viewDir = normalize(_WorldSpaceCameraPos - o.worldPos);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.planetCenter = mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz;
                
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // Calculate view direction and sun direction
                float3 viewDir = normalize(i.viewDir);
                float3 sunDir = normalize(_SunDir);
                
                // Calculate the dot product between view direction and normal
                float NdotV = saturate(dot(normalize(-i.normal), viewDir));
                
                // Calculate the dot product between view direction and sun direction
                float VdotL = saturate(dot(viewDir, sunDir));
                
                // Calculate the fresnel effect (stronger at the edges)
                float fresnel = pow(1.0 - NdotV, _AtmosphereRimPower);
                
                // Calculate the distance from the camera to the planet center
                float distToPlanetCenter = length(_WorldSpaceCameraPos - i.planetCenter);
                
                // Calculate the atmosphere density based on height
                float heightFactor = saturate((distToPlanetCenter - length(i.worldPos - i.planetCenter)) / _AtmosphereHeight);
                float density = pow(heightFactor, _AtmosphereFalloff);
                
                // Calculate scattering
                float3 rayleighScattering = RayleighScattering(VdotL);
                float3 mieScattering = MieScattering(VdotL);
                float3 totalScattering = (rayleighScattering + mieScattering) * _SunIntensity;
                
                // Combine all factors
                float3 finalColor = totalScattering * fresnel * density;
                float alpha = fresnel * _AtmosphereAlpha * density;
                
                return float4(finalColor, alpha);
            }
            ENDCG
        }
    }
}
