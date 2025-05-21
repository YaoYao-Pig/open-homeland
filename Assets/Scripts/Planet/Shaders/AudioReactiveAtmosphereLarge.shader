Shader "Planet/AudioReactiveAtmosphereLarge"
{
    Properties
    {
        // 基本属性
        _AtmoColor("Atmosphere Color", Color) = (0.5, 0.7, 1.0, 1.0)
        _PulseColor("Pulse Color", Color) = (0.7, 0.9, 1.0, 1.0)
        _ColorBlend("Color Blend", Range(0, 1)) = 0
        
        // 大气层属性
        _AtmosphereAlpha("Atmosphere Alpha", Range(0, 1)) = 0.5
        _AtmosphereHeight("Atmosphere Height", Range(0, 0.5)) = 0.1
        _AtmosphereFalloff("Atmosphere Falloff", Range(0, 10)) = 3
        _AtmosphereRimPower("Atmosphere Rim Power", Range(0, 20)) = 5
        
        // 光照属性
        _SunDir("Sun Direction", Vector) = (0, 1, 0, 0)
        _SunIntensity("Sun Intensity", Range(0, 10)) = 1
        
        // 散射属性
        _RayleighCoefficient("Rayleigh Coefficient", Range(0, 10)) = 1
        _MieCoefficient("Mie Coefficient", Range(0, 10)) = 0.5
        _MieDirectionalG("Mie Directional G", Range(0, 1)) = 0.8
        
        // 音频反应属性
        _AudioPulse("Audio Pulse", Range(0, 1)) = 0
        _PulseIntensity("Pulse Intensity", Range(0, 5)) = 1
        _WaveSpeed("Wave Speed", Range(0, 10)) = 2
        _WaveFrequency("Wave Frequency", Range(0, 20)) = 10
        _WaveAmplitude("Wave Amplitude", Range(0, 10)) = 2.0
        _ScaleFactor("Scale Factor", Range(1, 2000)) = 1000
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
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
                float3 normal : TEXCOORD2;
                float3 planetCenter : TEXCOORD3;
                float2 uv : TEXCOORD4;
            };
            
            // 基本属性
            float4 _AtmoColor;
            float4 _PulseColor;
            float _ColorBlend;
            
            // 大气层属性
            float _AtmosphereAlpha;
            float _AtmosphereHeight;
            float _AtmosphereFalloff;
            float _AtmosphereRimPower;
            
            // 光照属性
            float3 _SunDir;
            float _SunIntensity;
            
            // 散射属性
            float _RayleighCoefficient;
            float _MieCoefficient;
            float _MieDirectionalG;
            
            // 音频反应属性
            float _AudioPulse;
            float _PulseIntensity;
            float _WaveSpeed;
            float _WaveFrequency;
            float _WaveAmplitude;
            float _ScaleFactor;
            
            // 时间变量
            //float _Time;
            
            // Helper function for Rayleigh scattering
            float3 RayleighScattering(float cosTheta)
            {
                float rayleigh = 0.75 * (1.0 + cosTheta * cosTheta);
                return _RayleighCoefficient * rayleigh * lerp(_AtmoColor.rgb, _PulseColor.rgb, _ColorBlend);
            }
            
            // Helper function for Mie scattering
            float3 MieScattering(float cosTheta)
            {
                float g = _MieDirectionalG;
                float g2 = g * g;
                float mie = (1.0 - g2) / pow(1.0 + g2 - 2.0 * g * cosTheta, 1.5);
                return _MieCoefficient * mie * lerp(_AtmoColor.rgb, _PulseColor.rgb, _ColorBlend);
            }
            
            v2f vert (appdata v)
            {
                v2f o;
                
                // 添加基于音频的波动效果
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                
                // 创建波动效果 - 调整频率以适应大型星球
                float scaledFrequency = _WaveFrequency / (_ScaleFactor * 0.01); // 根据星球尺寸调整频率
                float time = _Time.y * _WaveSpeed;
                
                // 使用更复杂的波动模式，适合大型星球
                float wave = sin(time + worldPos.x * scaledFrequency) * 
                             cos(time * 0.7 + worldPos.y * scaledFrequency) * 
                             sin(time * 1.3 + worldPos.z * scaledFrequency);
                
                // 添加第二层波动，增加复杂性
                wave += 0.5 * sin(time * 1.5 + worldPos.z * scaledFrequency * 1.2) * 
                              sin(time * 0.8 + worldPos.x * scaledFrequency * 0.8);
                
                wave *= 0.5; // 归一化波动值
                
                // 应用音频脉冲调制波动 - 为大型星球增加振幅
                float scaledAmplitude = _WaveAmplitude * (_ScaleFactor * 0.01); // 根据星球尺寸调整振幅
                float audioWave = wave * scaledAmplitude * _AudioPulse * _PulseIntensity;
                
                // 向法线方向偏移顶点
                float3 offset = worldNormal * audioWave;
                worldPos += offset;
                
                // 缩放大气层
                float heightFactor = 1.0 + _AtmosphereHeight + (_AudioPulse * 0.05 * _PulseIntensity);
                float3 scaledVertex = v.vertex.xyz * heightFactor;
                
                // 转换回本地坐标
                float4 localPos = mul(unity_WorldToObject, float4(worldPos, 1.0));
                
                // 最终顶点位置
                o.vertex = UnityObjectToClipPos(localPos);
                
                // 传递其他数据
                o.worldPos = worldPos;
                o.viewDir = normalize(_WorldSpaceCameraPos - worldPos);
                o.normal = worldNormal;
                o.planetCenter = mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz;
                o.uv = v.uv;
                
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // 计算视角方向和太阳方向
                float3 viewDir = normalize(i.viewDir);
                float3 sunDir = normalize(_SunDir);
                
                // 计算视角方向和法线的点积
                float NdotV = saturate(dot(normalize(-i.normal), viewDir));
                
                // 计算视角方向和太阳方向的点积
                float VdotL = saturate(dot(viewDir, sunDir));
                
                // 计算菲涅尔效应（边缘更强）
                float rimPower = _AtmosphereRimPower + (_AudioPulse * 2.0 * _PulseIntensity);
                float fresnel = pow(1.0 - NdotV, rimPower);
                
                // 计算相机到行星中心的距离
                float distToPlanetCenter = length(_WorldSpaceCameraPos - i.planetCenter);
                
                // 计算基于高度的大气层密度
                float heightFactor = saturate((distToPlanetCenter - length(i.worldPos - i.planetCenter)) / _AtmosphereHeight);
                float density = pow(heightFactor, _AtmosphereFalloff);
                
                // 计算散射
                float3 rayleighScattering = RayleighScattering(VdotL);
                float3 mieScattering = MieScattering(VdotL);
                float3 totalScattering = (rayleighScattering + mieScattering) * _SunIntensity;
                
                // 添加基于音频的脉冲效果
                float pulse = 1.0 + (_AudioPulse * _PulseIntensity);
                
                // 组合所有因素
                float3 finalColor = totalScattering * fresnel * density * pulse;
                float alpha = fresnel * _AtmosphereAlpha * density;
                
                // 添加基于时间的颜色变化
                float timeFactor = sin(_Time.y * 0.5) * 0.5 + 0.5;
                finalColor = lerp(finalColor, finalColor * lerp(_AtmoColor.rgb, _PulseColor.rgb, _AudioPulse), _AudioPulse * 0.3);
                
                return float4(finalColor, alpha);
            }
            ENDCG
        }
    }
}
