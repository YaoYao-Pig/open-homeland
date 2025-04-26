Shader "Planet/AtmosphereGlow"
{
    Properties
    {
        //大气层的颜色
        _AtmoColor("Glow Color", Color) = (0.5, 0.7, 1.0, 1.0)
        //大气层反射效果的强化量
        _InnerRingFactor("Glow Factor", Range(1, 10)) = 5
        //大气层相对于地表的偏移
        _Offset("Vertex Offset", Range(0, 0.2)) = 0.05
        //光照强度
        _LightIntensity("Light Intensity", Range(0, 5)) = 1.5
        //边缘发光强度
        _RimPower("Rim Power", Range(0, 10)) = 3.0
        //光源方向
        _SunDir("Sun Direction", Vector) = (0, 1, 0, 0)
    }
    
    SubShader
    {
        Tags
        {
            "IgnoreProjector" = "True"
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
        }
        
        Pass
        {
            Blend One One
            AlphaTest Greater 0.1
            ColorMask RGBA
            Cull Back 
            Lighting Off 
            ZWrite Off 
            Fog{ Color(0,0,0,0) }
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #include "UnityCG.cginc"
            
            uniform float _InnerRingFactor;
            uniform float4 _AtmoColor;
            uniform float _Offset;
            uniform float _LightIntensity;
            uniform float _RimPower;
            uniform float3 _SunDir;
            
            struct VertexInput
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            
            struct VertexOutput
            {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
            };
            
            VertexOutput vert(VertexInput v)
            {
                VertexOutput o;
                // 获得顶点的法线方向
                o.normalDir = normalize(mul(unity_ObjectToWorld, float4(v.normal, 0)).xyz);
                // 对原来的顶点进行偏移
                v.vertex.xyz += (_Offset * v.normal);
                // 在世界中的顶点信息
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                // 转换后得到偏移后的顶点信息
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }
            
            float4 frag(VertexOutput i) : COLOR
            {
                // 视角方向
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                // 使用提供的太阳方向或默认方向
                float3 lightDirection = normalize(_SunDir);
                
                // 法线与光照方向的点积
                float ndl = dot(i.normalDir, lightDirection);
                // 法线与视角方向的点积
                float ndv = dot(i.normalDir, viewDirection);
                
                // 实现的效果为内侧描边
                // 当ndv为正数时，指数越大，效果越小
                // 当ndv为负数时，1-ndv为大于1的数，指数越大，效果越强
                float3 innerRing = saturate(_AtmoColor.xyz * pow(1.0 - ndv, _InnerRingFactor));
                
                // 光源方向参与到运算中，创建更真实的光照效果
                float sunViewEffect = max(0.2, dot(-lightDirection, viewDirection));
                float3 finalColor = saturate(pow(ndl + sunViewEffect, _RimPower)) * (innerRing * _LightIntensity);
                
                return float4(finalColor, 1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
