using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RidgidNoiseFilfter : INoiseFilter
{
    private Noise noise = new Noise();
    private NoiseSettings.RidgidNoiseSettings settings;

    public RidgidNoiseFilfter(NoiseSettings.RidgidNoiseSettings noiseSettings)
    {
        this.settings = noiseSettings;
    }

    public float Evaluate(Vector3 point)
    {
        float noiseValue = 0;

        float frequency = settings.baseRoughness;
        float amplitude = 1;
        float weight = 1;


        for (int i = 0; i < settings.numLayers; ++i)
        {
            float v = 1-Mathf.Abs(noise.Evaluate(point * frequency + settings.centre));
            v *= v;
            v *= weight;
            weight = Mathf.Clamp01(v*settings.weightMultiplier);

            noiseValue += (v + 1) * 0.5f * amplitude;
            frequency *= settings.roughness;
            amplitude *= settings.persistence;
        }

        noiseValue = noiseValue - settings.minValue;


        return noiseValue * settings.strength;
    }
}
