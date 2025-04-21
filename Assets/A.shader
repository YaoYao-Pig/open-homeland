Shader "Unlit/AtmosphereShader"
{
    Properties
    {
        _MainTex("_default", 2D) = "white" {}
    }
        SubShader
    {
        //优化前的shader
        Tags { "Queue" = "Transparent" "RenderType" = "Opaque" }
        LOD 100
        Cull Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define PI 3.1415926
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 objVertex : TEXCOORD1;
                float4 vertex : POSITION;
            };

            /*外部变量*/
            sampler2D _AtmosSourceTexture; //截屏画面
            sampler2D _CameraDepthTexture; //默认深度图
            float3 _planetPos;
            float _heightCoef; //H0
            float _planetSeaLevel;
            float3 _sunDirection;
            float _atmosHeight;
            float3 _colorCoef;
            float3 _colorWaveLength;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            /*性能参数*/
            #define innerCounts  25
            #define outterCounts  5
            //由clip空间转化到NDC空间
            float2 getScreenUV(float4 vertex) {
                float2 uv;
                uv.xy = float2(vertex.x / vertex.w, vertex.y / vertex.w); //Clip to NDC space
                uv.xy = uv.xy + float2(1, 1); //(-1~1) to (0~2)
                uv.xy = uv.xy * 0.5; // (0~2) to (0~1) => screen pos
                uv.y = 1 - uv.y; //D3D y flip
                return uv;
            }

            //由深度值获取世界坐标
            float3 getWorldPosFromDepth(float eyeDepth,float4 objVertex) {
                //获取世界坐标下的射线向量
                float3 worldDir = WorldSpaceViewDir(objVertex);
                //转化到ViewSpace, 获取z分量, 进行z方向归一化
                float viewZ = mul(UNITY_MATRIX_V, float4(worldDir, 0.0)).z;
                worldDir = -worldDir / viewZ; //负号是因为原来的dir是从物体到摄像机的
                worldDir = worldDir * eyeDepth;
                return _WorldSpaceCameraPos + worldDir;
            }

            //给出波长对应常数K
            float getConstK(int lambda) {
                if (lambda > 625 & lambda < 740) return _colorCoef.x;
                if (lambda > 492 & lambda < 577) return _colorCoef.y;
                if (lambda > 440 & lambda < 475) return _colorCoef.z;
                return 0;
            }

            //给出exp(h/H0)
            float expHeight(float3 pos) {
                float h = length(pos - _planetPos) - _planetSeaLevel; //计算海拔
                return exp(-h * _heightCoef);
            }

            //给出方向系数
            float getFormationCoef(float3 vecCam)
            {
                float costheta = dot(normalize(vecCam), normalize(_sunDirection));
                return 0.75 * (1 + costheta * costheta);
            }
            //计算另一直角边
            float getAnotherSide(float c, float a) {
                return sqrt(abs(c*c - a * a));
            }
            //向量归一化
            float3 getNormalized(float3 vec) {
                return vec / length(vec);
            }
            //计算外散射, 事实上返回的是exp(-t(Pa,sun)-t(Pa,A))
            float outterScatter(int lambda, float3 Pa, float3 posA) {
                //计算Pa到太阳光出射点
                float costheta = dot(getNormalized(_sunDirection), getNormalized(_planetPos - Pa));
                float sintheta = sqrt(1 - costheta * costheta);
                float lenPa2Planet = length(Pa - _planetPos);
                if (lenPa2Planet*sintheta < _planetSeaLevel & (costheta > 0)) {
                    //如果Pa被星体遮挡, 则光强为0(相当于全部散射掉了)
                    //当costheta < 0时, 星体在Pa的后面, 并不造成遮挡
                    return 0;
                }
                else {
                    float lenSunLight = lenPa2Planet * costheta +
                        getAnotherSide(_atmosHeight, lenPa2Planet*sintheta);
                    float3 stepVec = getNormalized(_sunDirection) * lenSunLight / outterCounts; //步长向量
                    float3 nowPos = Pa;
                    float stepLen = length(stepVec);
                    float scatter2Sun = 0;
                    for (int i = 0; i < outterCounts - 0.01; i++) {
                        scatter2Sun = scatter2Sun + expHeight(nowPos)*stepLen;
                        nowPos = nowPos + stepVec;
                    }
                    //计算Pa到posA出射点
                    float scatter2posA = 0;
                    nowPos = Pa;
                    stepVec = (posA - Pa) / outterCounts;
                    stepLen = length(stepVec);
                    for (int j = 0; j < outterCounts - 0.01; j++) {
                        scatter2posA = scatter2posA + expHeight(nowPos)*stepLen;
                        nowPos = nowPos + stepVec;
                    }
                    return exp(-1 * getConstK(lambda) * (scatter2posA + scatter2Sun));
                }
            }

            float innerScatter(float originColor, float3 targetPos, int lambda) {
                float3 vecA, vecB; //光线入射点, 出射点
                //计算相机射线形成的弦中点
                float3 posMid;
                float reverseIntensity; //地面反射光强

                    float3 vecCam2Target = getNormalized(targetPos - _WorldSpaceCameraPos);
                    float3 vecCam2Planet = _planetPos - _WorldSpaceCameraPos;
                    float dstCam2Mid = dot(vecCam2Target, vecCam2Planet);
                    posMid = _WorldSpaceCameraPos + dstCam2Mid * vecCam2Target;
                    float dstMid2AB = getAnotherSide(_atmosHeight, length(posMid - _planetPos));
                    vecA = getNormalized(posMid - _WorldSpaceCameraPos) * (length(posMid - _WorldSpaceCameraPos) - dstMid2AB);
                    if (length(_WorldSpaceCameraPos - _planetPos) > _atmosHeight) {
                        //大气外部
                        //判断是否被遮挡
                        if (length(targetPos - _planetPos) > _atmosHeight) {
                        //没有被遮挡,posB是大气圈出射点
                        vecB = getNormalized(targetPos - _WorldSpaceCameraPos) * (length(posMid - _WorldSpaceCameraPos) + dstMid2AB);
                        reverseIntensity = 1;
                        }
                        else {
                            //有遮挡, posB就是target
                            vecB = targetPos - _WorldSpaceCameraPos;
                            reverseIntensity = 0.8; //TODO: 这里应该用外散射数据
                        }
                    }
                    else {
                        //大气内部
                        vecA = float3(0,0,0); //posA在摄像机处
                        //判断是否被遮挡
                        if (length(targetPos - _planetPos) > _atmosHeight) {
                            //没有被遮挡,posB是大气圈出射点
                            vecB = getNormalized(targetPos - _WorldSpaceCameraPos) * (length(posMid - _WorldSpaceCameraPos) + dstMid2AB);
                            reverseIntensity = 0.8;
                        }
                        else {
                            return originColor;
                        }
                    }
                    float3 stepVec = (vecB - vecA) / innerCounts; //取5个点做内散射
                    float stepLen = length(stepVec);
                    float3 nowOutterScatterPos = vecA + _WorldSpaceCameraPos; //当前外散射计算点
                    float innerScatter = 0;
                    float opticalLength = 0; //光学距离, 用来debug
                    // 计算内散射
                    for (int k = 0; k < innerCounts - 0.01; k++) {
                        nowOutterScatterPos = nowOutterScatterPos + stepVec;
                        innerScatter = innerScatter +
                            stepLen * expHeight(nowOutterScatterPos) * outterScatter(lambda, nowOutterScatterPos, vecA + _WorldSpaceCameraPos);
                        opticalLength = opticalLength + stepLen * expHeight(nowOutterScatterPos);
                    }
                    //有遮挡和无遮挡的反射光强不一致
                    innerScatter = 2.0f * innerScatter * getConstK(lambda) * getFormationCoef(vecB);
 
                    if (length(_WorldSpaceCameraPos - _planetPos) > _atmosHeight) {
                        return reverseIntensity * originColor + innerScatter;
                    }
                    else {
                        return  (originColor / (originColor + innerScatter * 50)) * originColor + innerScatter;
                    }
                }
                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = getScreenUV(o.vertex); //获取0-1贴图坐标
                    o.objVertex = v.vertex;
                    return o;
                }

                fixed4 frag(v2f inData) : SV_Target
                {
                    fixed4 col;
                //float depth = tex2D(_CameraDepthTexture, inData.uv).r; 等价
                float originDepth = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, inData.uv));
                //计算线性深度
                float depth = Linear01Depth(originDepth);
                //log: targetPos是摄像机射线入射到的星体的世界坐标
                float3 targetPos = getWorldPosFromDepth(LinearEyeDepth(originDepth),inData.objVertex); //深度图还原世界坐标
                //采样原截屏像素
                col = tex2D(_AtmosSourceTexture, inData.uv.xy);
                //计算各通道色光
                float4 atmosCol;
                atmosCol.x = innerScatter(col.r,targetPos, _colorWaveLength.x);
                atmosCol.y = innerScatter(col.g,targetPos, _colorWaveLength.y);
                atmosCol.z = innerScatter(col.b,targetPos, _colorWaveLength.z);

                atmosCol.w = 1;

                //1是白色,0是黑色
                //float testData = expHeight(targetPos);
                //return fixed4(testData,testData,testData,1);
                return atmosCol;
            }
            ENDCG
        }
    }
}