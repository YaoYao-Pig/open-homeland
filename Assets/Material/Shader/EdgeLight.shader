Shader "yyz/OldEdgeLight"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _EdgeColor("EdgeColor",Color)=(0,0,0,0)
        _EdgeVisible("EdgeVisible",Range(0,1))= 0
    }
    SubShader
    {
        
        Pass{
            Name "ForwardLit"
            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"



            struct vertexIn{
                float4 vertex:POSITION;
                float3 normal:NORMAL;
                float2 uv:TEXCOORD0;

            };

            struct v2f{
                float4 vertex:SV_POSITION;
                float3 vertexWorldSpace:TEXCOORD2;
                float2 uv:TEXCOORD0;
                float3 normal:TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;

            v2f vert(vertexIn v){
                v2f o;
                o.vertex=TransformObjectToHClip(v.vertex);
                o.vertexWorldSpace=TransformObjectToWorld(v.vertex);

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = TransformObjectToWorldNormal(v.normal);
                return o;
            }

            float4 frag(v2f i):SV_Target{    

                float3 N=i.normal;
                //Light mainLight = GetMainLight();
                //float3 lightDirection = normalize(mainLight.direction);

                //float L=normalize(_WorldSpaceCameraPos-i.vertexWorldSpace);

                //float NdotL=dot(N,lightDirection)*10;
                
                float4 texColor = tex2D(_MainTex, i.uv) * _Color;
                float4 finalColor = texColor;//*NdotL;

                return finalColor;
            }
            ENDHLSL

        }

        Pass{
            Blend SrcAlpha One
            Cull Front
            Tags { "LightMode" = "SRPDefaultUnlit"}

            Name "OUTLINE"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            
            

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"



            struct vertexIn{
                float4 vertex:POSITION;
                float3 normal:NORMAL;
                float2 uv:TEXCOORD0;
            };

            struct v2f{
                float4 vertex:SV_POSITION;
                float2 uv:TEXCOORD0;
                float3 normal:TEXCOORD1;
                float4 worldPosition:TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _EdgeColor;

            float _EdgeVisible;//用于管理是否渲染边缘光

            v2f vert(vertexIn v){
                v2f o;
                v.vertex.xyz+=v.normal*0.15f;
                o.vertex=TransformObjectToHClip(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = TransformObjectToWorldNormal(v.normal);
                
                o.worldPosition=mul(unity_ObjectToWorld, v.vertex); 
                return o;
            }

            float4 frag(v2f i):SV_Target{    
                // 如果不满足条件，直接丢弃当前片段
                if (_EdgeVisible <= 0.5)
                {
                    discard; // 不渲染该片段
                }
                float4 texColor = tex2D(_MainTex, i.uv) * _EdgeColor;
                float4 finalColor = texColor;

                float3 viewDir=normalize(i.worldPosition-_WorldSpaceCameraPos.xyz);

                float dots=dot(i.normal,viewDir);
                    dots = saturate(dots);  // 将点积值限制在 [0, 1] 之间

    // 减小pow的指数，避免过度放大，调整边缘光平滑度
                finalColor.a = pow(dots, 3) * 5;  // 调整指数为3，减少透明度变化敏感度


                return finalColor;
            }
            ENDHLSL

        
        }
    }
    FallBack "Diffuse"
}
