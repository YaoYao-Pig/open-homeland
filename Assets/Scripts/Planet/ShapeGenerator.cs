using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeGenerator
{
    ShapeSettings settings;
    INoiseFilter[] noiseFilters;
    public MinMax elevationMinMax;


    public void UpdateSettings(ShapeSettings settings)
    {
        if (settings == null)
        {
            if (PlanetManager.Instance.currentScence == ScenceType.Main)
            {
                settings = Resources.Load<ShapeSettings>("Settings/Shape_main");
            }
            else if (PlanetManager.Instance.currentScence == ScenceType.Start)
            {
                settings = Resources.Load<ShapeSettings>("Settings/Shape_start");
            }
        }

        this.settings = settings;
        
        Debug.LogError(settings.noiseLayers);
        Debug.LogError(settings.noiseLayers.Length);
        noiseFilters = new INoiseFilter[settings.noiseLayers.Length];
        for (int i=0; i < noiseFilters.Length; ++i){
            noiseFilters[i] = NoiseFilterFactory.CreateNoiseFilter(settings.noiseLayers[i].noiseSettings); 
        }
        elevationMinMax = new MinMax();
    }


    /// <summary>
    /// ����ĳ��ĺ��Σ����������Լ������������ӣ����ؽ����һ���ڵ�λ�����ϵ�elevation�����ҪӦ���ڰ뾶���ǵ�λ�������ϣ���Ҫ��������
    /// </summary>
    public float CalculateUnscaledElevation(Vector3 pointOnUnitSphere)
    {
        float firstLayerValue = 0;
        float elevation = 0;
        //��һ������
        if (noiseFilters.Length > 0)
        {
            firstLayerValue = noiseFilters[0].Evaluate(pointOnUnitSphere);
            if (settings.noiseLayers[0].enabled)
            {
                elevation = firstLayerValue;
            }
        }


        
        for(int i = 1; i < noiseFilters.Length; ++i)
        {
            if (settings.noiseLayers[i].enabled)
            {
                float mask = (settings.noiseLayers[i].useFirstLayerAsMask) ? firstLayerValue : 1;

                elevation += noiseFilters[i].Evaluate(pointOnUnitSphere)*mask ;
            }
            
        }
        

        //��MinMax������¸߶�ֵ
        elevationMinMax.AddValue(elevation);
        return elevation;
    }

    /// <summary>
    //  ��������İ뾶�Ը߶�ֵ��������
    /// </summary>
    /// <param name="unscaleElevation"></param>
    /// <returns></returns>
    public float GetScaleElevation(float unscaleElevation)
    {
        float elevation = Mathf.Max(0, unscaleElevation);
        elevation = settings.planetRadius * (1 + elevation);
        return elevation;
    }
}
