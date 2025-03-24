Shader "Custom/Aerosphere" {
	Properties{
		   [Header(Texture)]
				  _MainTex("RGB:基础色 A:环境遮罩",2D) = "white"{}
				  _NormMap("RGB法线贴图",2D) = "bump"{}
		   [Header(Diffuse)]
				  _MainCol("基本色",Color) = (0.5,0.5,0.5,1.0)
		   [Header(Fresnel)]
				  _FresnelPow("菲涅尔次幂",Range(0,5)) = 1
				  _FresnelPow2("云朵菲涅尔次幂",Range(0,20)) = 1
					  _Scale("缩放",Float) = 0.5
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
					  
					  //输入结构
					  struct a2v {
							float4 vertex:       POSITION;            //顶点信息
							float2 uv0:          TEXCOORD0;           //UV信息
							float4 normal:       NORMAL;                    //法线信息
							float4 tangent:      TANGENT;             //切线信息
					  };
					  //输出结构
					  struct v2f {
							float4 pos:SV_POSITION;                  //屏幕定点位置
							float2 uv0:TEXCOORD0;                    //UV
							float3 posWS:TEXCOORD1;                  //世界坐标位置
							float3 nDirWS:TEXCOORD2;          //世界坐标法线
							float3 tDirWS:TEXCOORD3;          //世界坐标切线
							float3 bDirWS:TEXCOORD4;          //世界坐标副切线
							LIGHTING_COORDS(5,6)              //投影相关
					  };
					  v2f vert(a2v v) {
							v2f o;                                                                      //新输出结构
							o.pos = UnityObjectToClipPos(v.vertex);                //顶点位置    OS>CS
							o.uv0 = v.uv0.xy * _MainTex_ST.xy + _MainTex_ST.zw;;                                                              //传弟UV
							//o.uv0.x = o.uv0.x -0.5 * o.uv0.y * (_Scale);//o.uv0.y*_
							o.posWS = mul(unity_ObjectToWorld, v.vertex);   //点位置     CS>WS
							o.nDirWS = UnityObjectToWorldNormal(v.normal);  //法线位置   OS>WS
							o.tDirWS = normalize(mul(unity_ObjectToWorld, float4(v.tangent.xyz,0)).xyz);           //切线方向
							o.bDirWS = normalize(cross(o.nDirWS,o.tDirWS)*v.tangent.w);              //副切方向
							TRANSFER_VERTEX_TO_FRAGMENT(o);                               //投影相关
							return o;
					  }
					  float4 frag(v2f i) :SV_TARGET{
						  //向量准备
						  float3 nDirTS = UnpackNormal(tex2D(_NormMap,i.uv0)).rgb;
						  float3x3 TBN = float3x3(i.tDirWS,i.bDirWS,i.nDirWS);
						  float3 nDirWS = normalize(mul(nDirTS, TBN));
						  float3 vDirWS = normalize(_WorldSpaceCameraPos.xyz - i.posWS.xyz);
						  float3 vrDirWS = reflect(-vDirWS, nDirWS);
						  float3 lDirWS = _WorldSpaceLightPos0.xyz;
						  float3 lrDirWS = reflect(-lDirWS,nDirWS);
						  //中间量准备
						  float ndotl = dot(nDirWS, lDirWS);
						  float vdotr = dot(vDirWS, lrDirWS);
						  float vdotn = dot(vDirWS, nDirWS);
						  //纹理采样
						  float4 var_MainTex = tex2D(_MainTex, i.uv0);
						  //光照模型
								 //环境镜面反射
								 float fresnel = pow(max(0.0, -vdotn), _FresnelPow);//菲涅尔
								 float fresnel2 = pow(max(0.0,  - vdotn), _FresnelPow2);//菲涅尔
								 //光源漫反射
								 var_MainTex.a = var_MainTex.a *fresnel2;
								 float3 baseCol = var_MainTex.aaa+(_MainCol*(1 - var_MainTex.a));

								 //返回值
						   return fixed4(baseCol.rgb, max(0, (fresnel.r-0.5)*5));
					}
					ENDCG
			 }
				  }
}
