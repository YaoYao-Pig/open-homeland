using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlanetManager : MonoBehaviour
{
    public GameObject centerPlanet;
    public ColourSettings colourSettings;
    public ShapeSettings shapeSettings;

    private Repo_Read_OpenRank openRankList;
    private Planet planet;
    private Repository repository;

    private static PlanetManager _instance;
    public static PlanetManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<PlanetManager>();
                if (_instance==null)
                {
                    GameObject g = new GameObject(typeof(PlanetManager).ToString());
                    g.AddComponent<PlanetManager>();
                }
            }
            return _instance;
        }
        private set { }
    }
    private void Awake()
    {
        if (_instance == null && _instance != this)
        {
            _instance = this;
        }
        else
        {
            Destroy(_instance);
        }
    }
    public void InitializePlanet()
    {
        if(planet==null)
            planet = GetComponentInChildren<Planet>();
        StartCoroutine(Initialize());
    }
    

    public IEnumerator Initialize(string _m = "openrank")
    {
        //获取GameData
        string repoName = GameData.Instance.GetRepoName();
        gameObject.name = repoName;
        yield return StartCoroutine(GetData(gameObject.name));
        
        InitializePlanets();
    }

    public IEnumerator GetData(string _repoName, string _m = "openrank")
    {

        yield return StartCoroutine(WebController.GetRepoOpenRank(_repoName, _m));

        //找到最新的openRank
        openRankList = GameData.Instance.GetRepoOpenRankList();

        var repoOpenRankList = openRankList.repoOpenrankList;
        //获取项目（目前就是Repository-DeveloperNet）
        repository =(Repository) GameData.Instance.gameParams["Repo_Develop_Net"];
        
        ChartManager.Instance.ParseJsonToChart(repository.developerNetwork);
        ChartManager.Instance.IniteRepoOpenRankChart(repoOpenRankList);
    }

    private Color ColorTransfer(float r,float g,float b)
    {
        return new Color(r / 255.0f, g / 255.0f, b / 255.0f);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="param">平均的OpenRank</param>
    private void GenrateGradint(int layer,float param)
    {




        if (param <= 2f)
        {
            int index = 0;
            Gradient gradient = new Gradient();

            GradientColorKey[] colorKeys = new GradientColorKey[3];
            colorKeys[0] = new GradientColorKey(Color.black, 0.0f);    // 0% 位置为红色
            colorKeys[1] = new GradientColorKey(Color.gray, 0.5f);  // 50% 位置为绿色
            colorKeys[2] = new GradientColorKey(Color.white, 1.0f);   // 100% 位置为蓝色
            gradient.colorKeys = colorKeys;

            colourSettings.biomeColourSettings.biomes[layer].gradient = gradient;


            //设置OceanColour
            colourSettings.oceanColour = colourSettings.oceanColours[index % colourSettings.oceanColours.Length];

        }
        else if (param <= 3f)
        {
            int index = 1;
            Gradient gradient = new Gradient();

            GradientColorKey[] colorKeys = new GradientColorKey[3];
            colorKeys[0] = new GradientColorKey(new Color(0.545f, 0.271f, 0.149f), 0.0f);    // 0% 位置为棕色
            colorKeys[1] = new GradientColorKey(new Color(0.65f, 0.16f, 0.16f), 0.5f);  // 50% 位置为深棕色
            colorKeys[2] = new GradientColorKey(Color.blue, 1.0f);   // 100% 位置为蓝色
            gradient.colorKeys = colorKeys;


            colourSettings.biomeColourSettings.biomes[layer].gradient = gradient;

            colourSettings.oceanColour = colourSettings.oceanColours[index % colourSettings.oceanColours.Length];
        }
        else if(param <= 5f)
        {
            int index = 2;
            Gradient gradient = new Gradient();

            GradientColorKey[] colorKeys = new GradientColorKey[3];
            colorKeys[0] = new GradientColorKey(Color.red, 0.0f);    // 0% 位置为红色
            colorKeys[1] = new GradientColorKey(new Color(0.65f, 0.16f, 0.16f), 0.5f);  // 50% 位置为绿色
            colorKeys[2] = new GradientColorKey(Color.blue, 1.0f);   // 100% 位置为蓝色
            gradient.colorKeys = colorKeys;

            colourSettings.biomeColourSettings.biomes[layer].gradient = gradient;

            colourSettings.oceanColour = colourSettings.oceanColours[index % colourSettings.oceanColours.Length];
        }
        else
        {
            int index = 3;
            Gradient gradient = new Gradient();
            GradientColorKey[] colorKeys = new GradientColorKey[5];
            colorKeys[0] = new GradientColorKey(ColorTransfer(218f, 226f, 50f), 0.0f);    // 0% 位置为白色
            colorKeys[1] = new GradientColorKey(ColorTransfer(66.0f, 111f, 41f), 0.25f);  // 50% 位置为绿色
            colorKeys[2] = new GradientColorKey(ColorTransfer(73,125,17), 0.5f);  // 50% 位置为绿色
            colorKeys[3] = new GradientColorKey(ColorTransfer(94,28,5), 0.75f);   // 75%深棕色
            colorKeys[4] = new GradientColorKey(ColorTransfer(34,17,17), 1.0f);   // 100% 

            gradient.colorKeys = colorKeys;

            colourSettings.biomeColourSettings.biomes[layer].gradient = gradient;

            colourSettings.oceanColour = colourSettings.oceanColours[2];
        }
    }

    private float Sigmoid(float f)
    {
        float t = Mathf.Pow(1 + Mathf.Pow((float)Math.E, -f), -1);
        Debug.Log("Sigmod:"+t);
        return Mathf.Pow(1 + Mathf.Pow((float)Math.E, -f), -1);
    }
    private void InitializePlanets()
    {

        //加载配置文件
        colourSettings = Resources.Load<ColourSettings>("Settings/Colour");
        shapeSettings = Resources.Load<ShapeSettings>("Settings/Shape");

        ShapeSettings.NoiseLayer noiseLayer = shapeSettings.GetIndexNoiseLayer(0);
        if(noiseLayer.noiseSettings.filterType== NoiseSettings.FilterType.Simple)
        {
            NoiseSettings.SimpleNoiseSettings simpleNoiseSettings = noiseLayer.noiseSettings.simpleNoiseSettings;

            //通过openRank值来控制baseRoughness
            simpleNoiseSettings.strength = Sigmoid(openRankList.lastOpenRank / 100.0f)/10f;
            simpleNoiseSettings.baseRoughness = Sigmoid(openRankList.lastOpenRank/100.0f);

            Debug.Log(repository.GetRepoDevelopNetAverageOpenRank());
            //通过Openrank均值来控制颜色
            GenrateGradint(1, repository.GetRepoDevelopNetAverageOpenRank());
            

            planet.GeneratePlanet();

        }
        else if(noiseLayer.noiseSettings.filterType == NoiseSettings.FilterType.Ridgid)
        {
            NoiseSettings.RidgidNoiseSettings ridgidNoiseSettings = noiseLayer.noiseSettings.ridgidNoiseSettings;

        }
    }
}
