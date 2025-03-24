Shader "Custom/Aerosphere" {
	Properties{
		   [Header(Texture)]
				  _MainTex("RGB:����ɫ A:��������",2D) = "white"{}
				  _NormMap("RGB������ͼ",2D) = "bump"{}
		   [Header(Diffuse)]
				  _MainCol("����ɫ",Color) = (0.5,0.5,0.5,1.0)
		   [Header(Fresnel)]
				  _FresnelPow("����������",Range(0,5)) = 1
				  _FresnelPow2("�ƶ����������",Range(0,20)) = 1
					  _Scale("����",Float) = 0.5
	}
		SubShader{
					  Tags{
					"RenderType" = "Transparent"
						   "Queue" = "Transparent"
						   "ForceNoShadowCasting " = "True"
						   "IgnoreProjector" = "True" }
		Pass{
			   Tags{"LightMode" = "ForwardBase"}
			   Blend SrcAlpha OneMinusSrcAlpha
			   Cull Front
			   CGPROGRAM
			   #pragma vertex vert
			   #pragma fragment frag
			   #include "Lighting.cginc"
			   #include "AutoLight.cginc"
			   #include "UnityCG.cginc"
			   #pragma multi_compile_fwdbase_fullshadows
			   #pragma target 3.0
					  //Texture
					  uniform sampler2D _MainTex;
					  uniform fixed4 _MainTex_ST;
					  uniform sampler2D _NormMap;
					  //Diffuse
					  uniform float3 _MainCol;
					  //Specular
					  uniform float _FresnelPow;
					  uniform float _FresnelPow2;
					  uniform float _Scale;
					  
					  //����ṹ
					  struct a2v {
							float4 vertex:       POSITION;            //������Ϣ
							float2 uv0:          TEXCOORD0;           //UV��Ϣ
							float4 normal:       NORMAL;                    //������Ϣ
							float4 tangent:      TANGENT;             //������Ϣ
					  };
					  //����ṹ
					  struct v2f {
							float4 pos:SV_POSITION;                  //��Ļ����λ��
							float2 uv0:TEXCOORD0;                    //UV
							float3 posWS:TEXCOORD1;                  //��������λ��
							float3 nDirWS:TEXCOORD2;          //�������귨��
							float3 tDirWS:TEXCOORD3;          //������������
							float3 bDirWS:TEXCOORD4;          //�������긱����
							LIGHTING_COORDS(5,6)              //ͶӰ���
					  };
					  v2f vert(a2v v) {
							v2f o;                                                                      //������ṹ
							o.pos = UnityObjectToClipPos(v.vertex);                //����λ��    OS>CS
							o.uv0 = v.uv0.xy * _MainTex_ST.xy + _MainTex_ST.zw;;                                                              //����UV
							//o.uv0.x = o.uv0.x -0.5 * o.uv0.y * (_Scale);//o.uv0.y*_
							o.posWS = mul(unity_ObjectToWorld, v.vertex);   //��λ��     CS>WS
							o.nDirWS = UnityObjectToWorldNormal(v.normal);  //����λ��   OS>WS
							o.tDirWS = normalize(mul(unity_ObjectToWorld, float4(v.tangent.xyz,0)).xyz);           //���߷���
							o.bDirWS = normalize(cross(o.nDirWS,o.tDirWS)*v.tangent.w);              //���з���
							TRANSFER_VERTEX_TO_FRAGMENT(o);                               //ͶӰ���
							return o;
					  }
					  float4 frag(v2f i) :SV_TARGET{
						  //����׼��
						  float3 nDirTS = UnpackNormal(tex2D(_NormMap,i.uv0)).rgb;
						  float3x3 TBN = float3x3(i.tDirWS,i.bDirWS,i.nDirWS);
						  float3 nDirWS = normalize(mul(nDirTS, TBN));
						  float3 vDirWS = normalize(_WorldSpaceCameraPos.xyz - i.posWS.xyz);
						  float3 vrDirWS = reflect(-vDirWS, nDirWS);
						  float3 lDirWS = _WorldSpaceLightPos0.xyz;
						  float3 lrDirWS = reflect(-lDirWS,nDirWS);
						  //�м���׼��
						  float ndotl = dot(nDirWS, lDirWS);
						  float vdotr = dot(vDirWS, lrDirWS);
						  float vdotn = dot(vDirWS, nDirWS);
						  //�������
						  float4 var_MainTex = tex2D(_MainTex, i.uv0);
						  //����ģ��
								 //�������淴��
								 float fresnel = pow(max(0.0, -vdotn), _FresnelPow);//������
								 float fresnel2 = pow(max(0.0,  - vdotn), _FresnelPow2);//������
								 //��Դ������
								 var_MainTex.a = var_MainTex.a *fresnel2;
								 float3 baseCol = var_MainTex.aaa+(_MainCol*(1 - var_MainTex.a));

								 //����ֵ
						   return fixed4(baseCol.rgb, max(0, (fresnel.r-0.5)*5));
					}
					ENDCG
			 }
				  }
}
