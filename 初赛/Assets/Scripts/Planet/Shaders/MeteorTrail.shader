Shader "Planet/MeteorTrail"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Main Color", Color) = (1,1,1,1)
        _TailColor ("Tail Color", Color) = (0.5,0.7,1,1)
        _FadeLength ("Fade Length", Range(0, 1)) = 0.5
        _Width ("Width", Range(0.01, 1)) = 0.1
        _Brightness ("Brightness", Range(0.1, 3)) = 1.5
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Blend SrcAlpha One
        Cull Off
        ZWrite Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float4 _TailColor;
            float _FadeLength;
            float _Width;
            float _Brightness;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                // Fade based on UV.y (along the trail)
                float fade = pow(1 - v.uv.y, _FadeLength * 3);
                
                // Combine colors
                o.color = lerp(_Color, _TailColor, v.uv.y) * v.color * _Brightness;
                o.color.a *= fade;
                
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // Sample texture
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;
                
                // Apply glow effect
                float glow = pow(i.uv.x * (1 - i.uv.x) * 4, 0.5);
                col.rgb *= glow;
                
                return col;
            }
            ENDCG
        }
    }
}
