using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyBoxController : MonoBehaviour
{
    private Material currentSkyBox;
    public static SkyBoxController Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentSkyBox = RenderSettings.skybox;
    }
    public void SetSkyBox(Material sky)
    {
        currentSkyBox = sky;
        RenderSettings.skybox = currentSkyBox;
    }
}
