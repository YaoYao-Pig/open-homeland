using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseFilter 
{
    private Noise noise = new Noise();
    private NoiseSettings settings;

    public NoiseFilter(NoiseSettings noiseSettings)
    {
        this.settings = noiseSettings;
    }

    public float Evaluate(Vector3 point)
    {
        float noiseValue = 0;

        float frequency = settings.baseRoughness;
        float amplitude = 1;
        for(int i = 0; i < settings.numLayers; ++i)
        {
            float v = noise.Evaluate(point * frequency + settings.centre);
            noiseValue += (v + 1) * 0.5f*amplitude;
            frequency *= settings.roughness;
            amplitude += settings.persistence;
        }

        noiseValue = Mathf.Max(0, noiseValue - settings.minValue);


        return noiseValue*settings.strength;
    }
}
