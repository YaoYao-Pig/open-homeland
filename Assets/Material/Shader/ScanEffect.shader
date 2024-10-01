Shader "yyz/ScanEffect"
{
    Properties{
        _MainTex("Texture",2D)="white"{}
    }
    SubShader{
        // No culling or depth
		Cull Off ZWrite Off ZTest Always

        Pass{
            Tags {"RenderPipeline" = "UniversalPipeline"
                "LightMode" = "UniversalForward"}
            HLSLPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            //#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            
            struct appdata{
                float4 vertex:POSITION;
                float2 uv:TEXCOORD0;
                float4 ray:TEXCOORD1;
            };

            struct v2f{
                float4 vertex:SV_POSITION;
                float2 uv:TEXCOORD0;
                float4 ray:TEXCOORD1;
                float4 screenPos : TEXCOORD2;
            };

            sampler2D _MainTex;
            float3 _ScanCenterPos;
			float _CurrentScanRadius;
			float _ScanWidth;
			float4 _HeadColor;
			float4 _TrailColor;

            TEXTURE2D(_CameraDepthTexture);
            SAMPLER(sampler_CameraDepthTexture);

            v2f vert(appdata v){
                v2f o;
                o.vertex=TransformObjectToHClip(v.vertex);

                o.screenPos = ComputeScreenPos(o.vertex);
                o.uv=v.uv;
                o.ray=v.ray;
                return o;
            }



            float4 frag(v2f i):SV_Target{

                float4 col = tex2D(_MainTex, i.uv);
                float2 screenPos = i.screenPos.xy / i.screenPos.w;
                float depth=  SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, screenPos).r; //采样深度
                depth= Linear01Depth(depth, _ZBufferParams);   
                float4 fromCameraToPos=depth*i.ray;

                float3 worldPos=_WorldSpaceCameraPos+fromCameraToPos;


                float outerRing = _CurrentScanRadius + _ScanWidth * 0.5;
                float innerRing = _CurrentScanRadius - _ScanWidth * 0.5;
                float distanceToCenter = distance(_ScanCenterPos, worldPos);
                float value = smoothstep(innerRing, outerRing, distanceToCenter); 

                float4 ringColor;

                //不在之间
                if(value >= 1 || value <= 0)
                { 
                    value = 0; 
                    ringColor = float4(1,1,1,1);
                }
                else //在圆环上
                {
                    ringColor = lerp(_TrailColor, _HeadColor, value);
                }

                return col * ringColor;
            }


            ENDHLSL
        
        
        }
    
    
    }


    FallBack "Diffuse"
}
