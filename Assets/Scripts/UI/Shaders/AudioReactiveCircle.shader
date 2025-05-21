Shader "UI/AudioReactiveCircle"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        // 基本圆形属性
        _CircleColor ("Circle Color", Color) = (0.5, 0.7, 1.0, 1.0)
        _PulseColor ("Pulse Color", Color) = (0.7, 0.9, 1.0, 1.0)
        _ColorBlend ("Color Blend", Range(0, 1)) = 0
        _Radius ("Circle Radius", Range(0, 1)) = 0.8
        _EdgeSoftness ("Edge Softness", Range(0, 0.5)) = 0.05
        
        // 波动属性
        _WaveCount ("Wave Count", Range(1, 20)) = 8
        _WaveAmplitude ("Wave Amplitude", Range(0, 0.2)) = 0.05
        _WaveSpeed ("Wave Speed", Range(0, 10)) = 2
        
        // 音频反应属性
        _AudioPulse ("Audio Pulse", Range(0, 1)) = 0
        _PulseIntensity ("Pulse Intensity", Range(0, 5)) = 1
        
        // 旋转属性
        _RotationSpeed ("Rotation Speed", Range(-10, 10)) = 1
        
        // 必要的UI属性
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
        
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }
    
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
        
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]
        
        Pass
        {
            Name "Default"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP
            
            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;
            
            // 圆形属性
            float4 _CircleColor;
            float4 _PulseColor;
            float _ColorBlend;
            float _Radius;
            float _EdgeSoftness;
            
            // 波动属性
            float _WaveCount;
            float _WaveAmplitude;
            float _WaveSpeed;
            
            // 音频反应属性
            float _AudioPulse;
            float _PulseIntensity;
            
            // 旋转属性
            float _RotationSpeed;
            
            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                
                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                
                OUT.color = v.color * _Color;
                return OUT;
            }
            
            fixed4 frag(v2f IN) : SV_Target
            {
                // 将UV坐标转换为-1到1的范围
                float2 uv = IN.texcoord * 2.0 - 1.0;
                
                // 计算到中心的距离
                float dist = length(uv);
                
                // 计算角度（用于旋转和波动）
                float angle = atan2(uv.y, uv.x);
                
                // 添加旋转
                float rotationAngle = _Time.y * _RotationSpeed;
                float2 rotatedUV;
                rotatedUV.x = uv.x * cos(rotationAngle) - uv.y * sin(rotationAngle);
                rotatedUV.y = uv.x * sin(rotationAngle) + uv.y * cos(rotationAngle);
                
                // 计算波动
                float time = _Time.y * _WaveSpeed;
                float waveEffect = 0;
                
                // 创建多个波
                for (int i = 1; i <= 3; i++) {
                    float frequency = _WaveCount * i * 0.5;
                    float amplitude = _WaveAmplitude / i;
                    waveEffect += sin(angle * frequency + time * i) * amplitude;
                }
                
                // 应用音频脉冲
                waveEffect *= (1.0 + _AudioPulse * _PulseIntensity);
                
                // 调整半径
                float radius = _Radius + waveEffect;
                float pulseRadius = _Radius + _AudioPulse * 0.05 * _PulseIntensity;
                
                // 创建圆形
                float circle = smoothstep(radius + _EdgeSoftness, radius - _EdgeSoftness, dist);
                
                // 混合颜色
                float4 circleColor = lerp(_CircleColor, _PulseColor, _ColorBlend);
                
                // 添加脉冲效果
                float pulse = 1.0 + _AudioPulse * _PulseIntensity * 0.2;
                circleColor.rgb *= pulse;
                
                // 添加边缘发光
                float edgeGlow = smoothstep(radius + _EdgeSoftness * 2.0, radius, dist) * 
                                 smoothstep(radius - _EdgeSoftness * 2.0, radius, dist);
                edgeGlow *= _AudioPulse * _PulseIntensity;
                circleColor.rgb += _PulseColor.rgb * edgeGlow;
                
                // 应用透明度
                float4 finalColor = circleColor;
                finalColor.a *= circle * IN.color.a;
                
                // 应用UI裁剪
                #ifdef UNITY_UI_CLIP_RECT
                finalColor.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif
                
                #ifdef UNITY_UI_ALPHACLIP
                clip(finalColor.a - 0.001);
                #endif
                
                return finalColor;
            }
            ENDCG
        }
    }
}
