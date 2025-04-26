Shader "Planet/AuroraEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (0,1,0.5,1)
        _SecondaryColor ("Secondary Color", Color) = (0,0.5,1,1)
        _Speed ("Speed", Range(0.1, 5.0)) = 1.0
        _Intensity ("Intensity", Range(0, 2.0)) = 1.0
        _Height ("Height", Range(0, 2.0)) = 0.1
        _NoiseScale ("Noise Scale", Range(0.1, 10.0)) = 1.0
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            // 顶点输入结构
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };
            
            // 顶点到片元结构
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
                float3 normal : TEXCOORD2;
            };
            
            sampler2D _MainTex;
            float4 _Color;
            float4 _SecondaryColor;
            float _Speed;
            float _Intensity;
            float _Height;
            float _NoiseScale;
            
            // 简单噪声函数
            float noise(float3 p)
            {
                float3 i = floor(p);
                float3 f = frac(p);
                f = f*f*(3.0-2.0*f);
                
                float n = i.x + i.y*57.0 + i.z*113.0;
                float r1 = frac(sin(n*753.5453)*7235.5453);
                float r2 = frac(sin((n+1.0)*753.5453)*7235.5453);
                float r3 = frac(sin((n+57.0)*753.5453)*7235.5453);
                float r4 = frac(sin((n+58.0)*753.5453)*7235.5453);
                float r5 = frac(sin((n+113.0)*753.5453)*7235.5453);
                float r6 = frac(sin((n+114.0)*753.5453)*7235.5453);
                float r7 = frac(sin((n+170.0)*753.5453)*7235.5453);
                float r8 = frac(sin((n+171.0)*753.5453)*7235.5453);
                
                float v1 = lerp(r1, r2, f.x);
                float v2 = lerp(r3, r4, f.x);
                float v3 = lerp(r5, r6, f.x);
                float v4 = lerp(r7, r8, f.x);
                
                float v5 = lerp(v1, v2, f.y);
                float v6 = lerp(v3, v4, f.y);
                
                return lerp(v5, v6, f.z);
            }
            
            v2f vert (appdata v)
            {
                v2f o;
                
                // 添加基于噪声的顶点偏移
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float n = noise(worldPos * _NoiseScale + _Time.y * _Speed);
                
                // 仅在极点区域应用效果
                float polarMask = pow(abs(normalize(worldPos).y), 8.0);
                v.vertex.xyz += v.normal * n * _Height * polarMask;
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = worldPos;
                o.normal = UnityObjectToWorldNormal(v.normal);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // 计算极点遮罩
                float polarMask = pow(abs(normalize(i.worldPos).y), 4.0);
                
                // 创建时间变化的噪声
                float n1 = noise(i.worldPos * _NoiseScale + _Time.y * _Speed * 0.5);
                float n2 = noise(i.worldPos * _NoiseScale * 2.0 + _Time.y * _Speed * 0.7);
                
                // 混合颜色
                float4 col = lerp(_Color, _SecondaryColor, n1);
                
                // 应用强度和遮罩
                float alpha = n1 * n2 * _Intensity * polarMask;
                
                return float4(col.rgb, alpha);
            }
            ENDCG
        }
    }
}
