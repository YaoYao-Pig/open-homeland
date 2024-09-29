using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class NoiseSettings
{
    public float strength = 1;

    [Range(1, 8)] public int numLayers = 1; //ÔëÉùµÄµş¼Ó²ãÊı
    public float persistence = 0.5f;
    public float baseRoughness = 1;
    public float roughness = 2;
    public Vector3 centre;

    public float minValue;

}
