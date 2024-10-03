using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class NoiseSettings
{
    public enum FilterType
    {
        Simple,Ridgid
    }

    public FilterType filterType;

    public SimpleNoiseSettings simpleNoiseSettings;
    public RidgidNoiseSettings ridgidNoiseSettings;


    [System.Serializable]
    public class SimpleNoiseSettings
    {
        public float strength = 1;

        [Range(1, 8)] public int numLayers = 1; //ÔëÉùµÄµþ¼Ó²ãÊý

        public float baseRoughness = 1;
        public float roughness = 2;
        public Vector3 centre;
        public float persistence = 0.5f;
        public float minValue = 1;
    }

    [System.Serializable]
    public class RidgidNoiseSettings:SimpleNoiseSettings
    {
        public float weightMultiplier = 0.8f;
    }

    
}
