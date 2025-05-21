using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourGenerator 
{
    ColourSettings settings;
    Texture2D texture;
    const int textureResolution = 50;
    INoiseFilter biomeNoiseFilter;


    public void UpdateSettings(ColourSettings settings)
    {
        if (settings == null)
        {
            if (PlanetManager.Instance.currentScence == ScenceType.Main)
            {
                settings = Resources.Load<ColourSettings>("Settings/Colour_main");
            }
            else if (PlanetManager.Instance.currentScence == ScenceType.Start)
            {
                settings = Resources.Load<ColourSettings>("Settings/Colour_start");
            }
        }
        this.settings = settings;
        if (texture == null || texture.height!=settings.biomeColourSettings.biomes.Length)
        {
            texture = new Texture2D(textureResolution*2, settings.biomeColourSettings.biomes.Length,TextureFormat.RGBA32,false);
        }
        biomeNoiseFilter = NoiseFilterFactory.CreateNoiseFilter(settings.biomeColourSettings.noise);

    }
    public void UpdateElevation(MinMax elevationMinMax)
    {
        //Debug.Log(elevationMinMax.Min);
        settings.planetMaterial.SetVector("_MinMax", new Vector4(elevationMinMax.Min, elevationMinMax.Max));
    }

    ///����������ÿһ���������ĸ�Ⱥϵ
    public float BiomePercentFromPoint(Vector3 pointOnUnitSphere)
    {
        float heightPercent = (pointOnUnitSphere.y + 1) / 2f;
        heightPercent += (biomeNoiseFilter.Evaluate(pointOnUnitSphere)-settings.biomeColourSettings.noiseOffset)*settings.biomeColourSettings.noiseStrength;

        float biomeIndex = 0;
        int numBiomes = settings.biomeColourSettings.biomes.Length;


        float blendRange = settings.biomeColourSettings.blendAmount / 2f +0.001f;
        for(int i = 0; i < numBiomes; ++i)
        {
            float dst = heightPercent - settings.biomeColourSettings.biomes[i].startHeight;
            float weight = Mathf.InverseLerp(-blendRange, blendRange, dst);
            biomeIndex *= (1 - weight);
            biomeIndex += i * weight;

        }
        return biomeIndex/Mathf.Max(1,(numBiomes-1));

    }


    public void OceanColour()
    {
        
    }

    public void UpdateColours()
    {
        Color[] colours = new Color[texture.width * texture.height];
        int colourIndex = 0;
        foreach(var biome in settings.biomeColourSettings.biomes)
        {
            for (int i = 0; i < textureResolution * 2; ++i)
            {
                Color gradientColor;
                //����Ǻ�����һ��Ļ�
                if (i < textureResolution)
                {
                    gradientColor = settings.oceanColour.Evaluate(i / (textureResolution-1f));
                }
                else
                {
                    //������Ǻ���
                    gradientColor = biome.gradient.Evaluate((i-textureResolution) / (textureResolution - 1f));
                    
                }
                Color tintColor = biome. tint;
                colours[colourIndex] = gradientColor * (1 - biome.tintPercecnt) + tintColor * biome.tintPercecnt;
                colourIndex++;
            }
        }

        texture.SetPixels(colours);
        texture.Apply();
        settings.planetMaterial.SetTexture("_texture", texture);
    }
}
 