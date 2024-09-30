using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu()]
public class ColourSettings : ScriptableObject
{
    
    public Material planetMaterial;

    public BiomeColourSettings biomeColourSettings;




    /// <summary>
    /// 生物群系过渡
    /// </summary>
    [System.Serializable]
    public class BiomeColourSettings
    {
        public Biome[] biomes;

        public NoiseSettings noise;

        /// <summary>
        /// 噪声用来混合群系边界
        /// </summary>
        public float noiseOffset;
        public float noiseStrength;
        [Range(0,1)] public float blendAmount;

        [System.Serializable]
        public class Biome
        {
            public Gradient gradient;
            public Color tint;
            [Range(0,1)] public float startHeight;
            [Range(0, 1)] public float tintPercecnt;
        }
    }
}
