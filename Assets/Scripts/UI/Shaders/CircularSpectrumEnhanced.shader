Shader "UI/CircularSpectrumEnhanced"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        // 频谱属性
        _BarCount ("Bar Count", Range(8, 128)) = 64
        _MinBarHeight ("Min Bar Height", Range(0, 1)) = 0.1
        _MaxBarHeight ("Max Bar Height", Range(0, 1)) = 0.5
        _BarWidth ("Bar Width", Range(0, 1)) = 0.7
        _InnerRadius ("Inner Radius", Range(0, 1)) = 0.3
        
        // 颜色属性
        _BaseColor ("Base Color", Color) = (0.2, 0.4, 1.0, 1.0)
        _PeakColor ("Peak Color", Color) = (1.0, 0.5, 0.2, 1.0)
        _BgColor ("Background Color", Color) = (0.1, 0.1, 0.1, 0.5)
        _GlowColor ("Glow Color", Color) = (1.0, 0.8, 0.4, 1.0)
        _GlowIntensity ("Glow Intensity", Range(0, 2)) = 0.5
        
        // 动画属性
        _RotationSpeed ("Rotation Speed", Range(-1, 1)) = 0.1
        _PulseSpeed ("Pulse Speed", Range(0, 10)) = 5
        _PulseAmount ("Pulse Amount", Range(0, 0.2)) = 0.05
        
        // 音频数据
        _AudioData ("Audio Data", 2D) = "black" {}
        
        // 视觉增强
        _BarRounding ("Bar Rounding", Range(0, 1)) = 0.2
        _BarGap ("Bar Gap", Range(0, 0.5)) = 0.1
        _PeakWidth ("Peak Width", Range(0, 0.2)) = 0.05
        _PeakHeight ("Peak Height", Range(0, 0.2)) = 0.05
        _Responsiveness ("Responsiveness", Range(1, 3)) = 1.5
        
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
            
            // 频谱属性
            float _BarCount;
            float _MinBarHeight;
            float _MaxBarHeight;
            float _BarWidth;
            float _InnerRadius;
            
            // 颜色属性
            float4 _BaseColor;
            float4 _PeakColor;
            float4 _BgColor;
            float4 _GlowColor;
            float _GlowIntensity;
            
            // 动画属性
            float _RotationSpeed;
            float _PulseSpeed;
            float _PulseAmount;
            
            // 音频数据
            sampler2D _AudioData;
            
            // 视觉增强
            float _BarRounding;
            float _BarGap;
            float _PeakWidth;
            float _PeakHeight;
            float _Responsiveness;
            
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
            
            // 辅助函数：获取音频数据
            float getAudioValue(float angle, float barCount) {
                // 将角度映射到0-1范围
                float normalizedIndex = angle / (2.0 * 3.14159);
                
                // 获取对应的音频数据
                float audioValue = tex2D(_AudioData, float2(normalizedIndex, 0)).r;
                
                // 增强响应性
                audioValue = pow(audioValue, 1.0 / _Responsiveness);
                
                return audioValue;
            }
            
            // 平滑步进函数
            float smoothStep(float edge0, float edge1, float x) {
                float t = saturate((x - edge0) / (edge1 - edge0));
                return t * t * (3.0 - 2.0 * t);
            }
            
            fixed4 frag(v2f IN) : SV_Target
            {
                // 将UV坐标转换为-1到1的范围
                float2 uv = IN.texcoord * 2.0 - 1.0;
                
                // 计算到中心的距离和角度
                float dist = length(uv);
                float angle = atan2(uv.y, uv.x);
                
                // 添加旋转
                angle += _Time.y * _RotationSpeed;
                
                // 确保角度在0-2π范围内
                if (angle < 0) angle += 2.0 * 3.14159;
                
                // 计算当前位置对应的条形索引
                float barIndex = floor(angle / (2.0 * 3.14159) * _BarCount);
                float barAngle = barIndex * (2.0 * 3.14159 / _BarCount);
                
                // 计算在当前条形内的位置（0-1）
                float barPosition = (angle - barAngle) / (2.0 * 3.14159 / _BarCount);
                
                // 获取当前条形的音频值
                float audioValue = getAudioValue(barAngle, _BarCount);
                
                // 添加脉冲效果
                float pulse = sin(_Time.y * _PulseSpeed) * 0.5 + 0.5;
                pulse = pulse * _PulseAmount * audioValue;
                
                // 计算条形高度
                float barHeight = _MinBarHeight + audioValue * (_MaxBarHeight - _MinBarHeight);
                barHeight += pulse; // 添加脉冲效果
                
                // 计算条形的内外半径
                float innerRadius = _InnerRadius;
                float outerRadius = innerRadius + barHeight;
                
                // 计算条形的宽度，考虑间隙
                float barWidthWithGap = _BarWidth * (1.0 - _BarGap);
                float halfBarWidth = barWidthWithGap * 0.5 * (2.0 * 3.14159 / _BarCount);
                float barCenter = barAngle + (2.0 * 3.14159 / _BarCount) * 0.5;
                float angleDiff = min(abs(angle - barCenter), abs(angle - barCenter - 2.0 * 3.14159));
                
                // 判断是否在条形内，使用平滑边缘
                float barEdge = smoothStep(halfBarWidth, halfBarWidth * (1.0 - _BarRounding), angleDiff);
                float distanceFromInner = dist - innerRadius;
                float distanceFromOuter = outerRadius - dist;
                
                // 计算条形内的位置（0-1，从内到外）
                float barRadialPosition = distanceFromInner / (outerRadius - innerRadius);
                
                // 判断是否在条形内
                bool insideBar = (dist >= innerRadius && dist <= outerRadius && angleDiff <= halfBarWidth);
                
                // 计算峰值位置
                float peakInnerRadius = outerRadius - _PeakHeight;
                float peakOuterRadius = outerRadius;
                bool insidePeak = (dist >= peakInnerRadius && dist <= peakOuterRadius && angleDiff <= halfBarWidth * (1.0 - _PeakWidth));
                
                // 计算颜色
                float4 barColor = lerp(_BaseColor, _PeakColor, audioValue);
                
                // 应用背景色
                float4 finalColor = _BgColor;
                
                // 如果在条形内，使用条形颜色
                if (insideBar) {
                    // 添加从内到外的渐变
                    float gradient = barRadialPosition;
                    finalColor = lerp(_BaseColor, barColor, gradient);
                    
                    // 添加边缘发光
                    float edgeGlow = 1.0 - abs(2.0 * barRadialPosition - 1.0);
                    edgeGlow = pow(edgeGlow, 3);
                    finalColor.rgb += _GlowColor.rgb * edgeGlow * _GlowIntensity * audioValue;
                    
                    // 应用条形边缘平滑
                    finalColor.a *= barEdge;
                }
                
                // 如果在峰值内，使用峰值颜色
                if (insidePeak) {
                    finalColor = _PeakColor;
                    finalColor.rgb += _GlowColor.rgb * _GlowIntensity * 0.5;
                }
                
                // 添加整体发光效果
                float overallGlow = 0;
                for (int i = 0; i < 8; i++) {
                    float sampleAngle = i * (2.0 * 3.14159 / 8);
                    float sampleAudio = getAudioValue(sampleAngle, 8);
                    overallGlow += sampleAudio;
                }
                overallGlow /= 8;
                
                // 在背景上添加微弱的发光
                if (!insideBar) {
                    float glowFalloff = 1.0 - saturate((dist - outerRadius) / 0.2);
                    finalColor.rgb += _GlowColor.rgb * glowFalloff * overallGlow * _GlowIntensity * 0.2;
                }
                
                // 应用透明度
                finalColor.a *= IN.color.a;
                
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
