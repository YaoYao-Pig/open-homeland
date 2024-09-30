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
        this.settings = settings;
        noiseFilters = new INoiseFilter[settings.noiseLayers.Length];
        for (int i=0; i < noiseFilters.Length; ++i){
            noiseFilters[i] = NoiseFilterFactory.CreateNoiseFilter(settings.noiseLayers[i].noiseSettings); 
        }
        elevationMinMax = new MinMax();
    }


    /// <summary>
    /// 计算某点的海拔，根据噪声以及各种缩放因子，返回结果是一个在单位球体上的elevation，如果要应用在半径不是单位的球体上，需要进行缩放
    /// </summary>
    public float CalculateUnscaledElevation(Vector3 pointOnUnitSphere)
    {
        float firstLayerValue = 0;
        float elevation = 0;
        //第一层遮罩
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
        

        //在MinMax里面更新高度值
        elevationMinMax.AddValue(elevation);
        return elevation;
    }

    /// <summary>
    //  根据星球的半径对高度值进行缩放
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
