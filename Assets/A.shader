Shader "Unlit/AtmosphereShader"
{
    Properties
    {
        _MainTex("_default", 2D) = "white" {}
    }
        SubShader
    {
        //�Ż�ǰ��shader
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

            /*�ⲿ����*/
            sampler2D _AtmosSourceTexture; //��������
            sampler2D _CameraDepthTexture; //Ĭ�����ͼ
            float3 _planetPos;
            float _heightCoef; //H0
            float _planetSeaLevel;
            float3 _sunDirection;
            float _atmosHeight;
            float3 _colorCoef;
            float3 _colorWaveLength;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            /*���ܲ���*/
            #define innerCounts  25
            #define outterCounts  5
            //��clip�ռ�ת����NDC�ռ�
            float2 getScreenUV(float4 vertex) {
                float2 uv;
                uv.xy = float2(vertex.x / vertex.w, vertex.y / vertex.w); //Clip to NDC space
                uv.xy = uv.xy + float2(1, 1); //(-1~1) to (0~2)
                uv.xy = uv.xy * 0.5; // (0~2) to (0~1) => screen pos
                uv.y = 1 - uv.y; //D3D y flip
                return uv;
            }

            //�����ֵ��ȡ��������
            float3 getWorldPosFromDepth(float eyeDepth,float4 objVertex) {
                //��ȡ���������µ���������
                float3 worldDir = WorldSpaceViewDir(objVertex);
                //ת����ViewSpace, ��ȡz����, ����z�����һ��
                float viewZ = mul(UNITY_MATRIX_V, float4(worldDir, 0.0)).z;
                worldDir = -worldDir / viewZ; //��������Ϊԭ����dir�Ǵ����嵽�������
                worldDir = worldDir * eyeDepth;
                return _WorldSpaceCameraPos + worldDir;
            }

            //����������Ӧ����K
            float getConstK(int lambda) {
                if (lambda > 625 & lambda < 740) return _colorCoef.x;
                if (lambda > 492 & lambda < 577) return _colorCoef.y;
                if (lambda > 440 & lambda < 475) return _colorCoef.z;
                return 0;
            }

            //����exp(h/H0)
            float expHeight(float3 pos) {
                float h = length(pos - _planetPos) - _planetSeaLevel; //���㺣��
                return exp(-h * _heightCoef);
            }

            //��������ϵ��
            float getFormationCoef(float3 vecCam)
            {
                float costheta = dot(normalize(vecCam), normalize(_sunDirection));
                return 0.75 * (1 + costheta * costheta);
            }
            //������һֱ�Ǳ�
            float getAnotherSide(float c, float a) {
                return sqrt(abs(c*c - a * a));
            }
            //������һ��
            float3 getNormalized(float3 vec) {
                return vec / length(vec);
            }
            //������ɢ��, ��ʵ�Ϸ��ص���exp(-t(Pa,sun)-t(Pa,A))
            float outterScatter(int lambda, float3 Pa, float3 posA) {
                //����Pa��̫��������
                float costheta = dot(getNormalized(_sunDirection), getNormalized(_planetPos - Pa));
                float sintheta = sqrt(1 - costheta * costheta);
                float lenPa2Planet = length(Pa - _planetPos);
                if (lenPa2Planet*sintheta < _planetSeaLevel & (costheta > 0)) {
                    //���Pa�������ڵ�, ���ǿΪ0(�൱��ȫ��ɢ�����)
                    //��costheta < 0ʱ, ������Pa�ĺ���, ��������ڵ�
                    return 0;
                }
                else {
                    float lenSunLight = lenPa2Planet * costheta +
                        getAnotherSide(_atmosHeight, lenPa2Planet*sintheta);
                    float3 stepVec = getNormalized(_sunDirection) * lenSunLight / outterCounts; //��������
                    float3 nowPos = Pa;
                    float stepLen = length(stepVec);
                    float scatter2Sun = 0;
                    for (int i = 0; i < outterCounts - 0.01; i++) {
                        scatter2Sun = scatter2Sun + expHeight(nowPos)*stepLen;
                        nowPos = nowPos + stepVec;
                    }
                    //����Pa��posA�����
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
                float3 vecA, vecB; //���������, �����
                //������������γɵ����е�
                float3 posMid;
                float reverseIntensity; //���淴���ǿ

                    float3 vecCam2Target = getNormalized(targetPos - _WorldSpaceCameraPos);
                    float3 vecCam2Planet = _planetPos - _WorldSpaceCameraPos;
                    float dstCam2Mid = dot(vecCam2Target, vecCam2Planet);
                    posMid = _WorldSpaceCameraPos + dstCam2Mid * vecCam2Target;
                    float dstMid2AB = getAnotherSide(_atmosHeight, length(posMid - _planetPos));
                    vecA = getNormalized(posMid - _WorldSpaceCameraPos) * (length(posMid - _WorldSpaceCameraPos) - dstMid2AB);
                    if (length(_WorldSpaceCameraPos - _planetPos) > _atmosHeight) {
                        //�����ⲿ
                        //�ж��Ƿ��ڵ�
                        if (length(targetPos - _planetPos) > _atmosHeight) {
                        //û�б��ڵ�,posB�Ǵ���Ȧ�����
                        vecB = getNormalized(targetPos - _WorldSpaceCameraPos) * (length(posMid - _WorldSpaceCameraPos) + dstMid2AB);
                        reverseIntensity = 1;
                        }
                        else {
                            //���ڵ�, posB����target
                            vecB = targetPos - _WorldSpaceCameraPos;
                            reverseIntensity = 0.8; //TODO: ����Ӧ������ɢ������
                        }
                    }
                    else {
                        //�����ڲ�
                        vecA = float3(0,0,0); //posA���������
                        //�ж��Ƿ��ڵ�
                        if (length(targetPos - _planetPos) > _atmosHeight) {
                            //û�б��ڵ�,posB�Ǵ���Ȧ�����
                            vecB = getNormalized(targetPos - _WorldSpaceCameraPos) * (length(posMid - _WorldSpaceCameraPos) + dstMid2AB);
                            reverseIntensity = 0.8;
                        }
                        else {
                            return originColor;
                        }
                    }
                    float3 stepVec = (vecB - vecA) / innerCounts; //ȡ5��������ɢ��
                    float stepLen = length(stepVec);
                    float3 nowOutterScatterPos = vecA + _WorldSpaceCameraPos; //��ǰ��ɢ������
                    float innerScatter = 0;
                    float opticalLength = 0; //��ѧ����, ����debug
                    // ������ɢ��
                    for (int k = 0; k < innerCounts - 0.01; k++) {
                        nowOutterScatterPos = nowOutterScatterPos + stepVec;
                        innerScatter = innerScatter +
                            stepLen * expHeight(nowOutterScatterPos) * outterScatter(lambda, nowOutterScatterPos, vecA + _WorldSpaceCameraPos);
                        opticalLength = opticalLength + stepLen * expHeight(nowOutterScatterPos);
                    }
                    //���ڵ������ڵ��ķ����ǿ��һ��
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
                    o.uv = getScreenUV(o.vertex); //��ȡ0-1��ͼ����
                    o.objVertex = v.vertex;
                    return o;
                }

                fixed4 frag(v2f inData) : SV_Target
                {
                    fixed4 col;
                //float depth = tex2D(_CameraDepthTexture, inData.uv).r; �ȼ�
                float originDepth = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, inData.uv));
                //�����������
                float depth = Linear01Depth(originDepth);
                //log: targetPos��������������䵽���������������
                float3 targetPos = getWorldPosFromDepth(LinearEyeDepth(originDepth),inData.objVertex); //���ͼ��ԭ��������
                //����ԭ��������
                col = tex2D(_AtmosSourceTexture, inData.uv.xy);
                //�����ͨ��ɫ��
                float4 atmosCol;
                atmosCol.x = innerScatter(col.r,targetPos, _colorWaveLength.x);
                atmosCol.y = innerScatter(col.g,targetPos, _colorWaveLength.y);
                atmosCol.z = innerScatter(col.b,targetPos, _colorWaveLength.z);

                atmosCol.w = 1;

                //1�ǰ�ɫ,0�Ǻ�ɫ
                //float testData = expHeight(targetPos);
                //return fixed4(testData,testData,testData,1);
                return atmosCol;
            }
            ENDCG
        }
    }
}