using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum ScenceType
{
    Start,
    Main,
}
public class PlanetManager : MonoBehaviour
{
    public GameObject centerPlanet;
    public ColourSettings colourSettings;
    public ShapeSettings shapeSettings;

    private Repo_Read_OpenRank openRankList;
    public Planet planet;
    private Repository repository;

    private static PlanetManager _instance;
    private TopUITextController topUITextController;
    public Action OnPlanetEnd;
    public ScenceType currentScence;
    
    public string musicPrompt = "This is a GitHub project. The project's activity change curve is: 1, 2, 3, 4, 7, 10, 7, 5, 6, 1, 10.";

    public static PlanetManager Instance
    {
        get
        {
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
        SendTestPrompt();
        //��ȡGameData
        string repoName = GameData.Instance.GetRepoName();
        gameObject.name = repoName;
        yield return StartCoroutine(GetData(gameObject.name));
        
        InitializePlanets();
        OnPlanetEnd?.Invoke();
        
    }
    public void SendTestPrompt()
    {
        MusicGenClient.Instance.RequestMusic(musicPrompt);
    }

    public IEnumerator GetData(string _repoName, string _m = "openrank")
    {

        yield return StartCoroutine(WebController.GetRepoOpenRank(_repoName, _m));
        yield return StartCoroutine(WebController.GetRepoDetail(_repoName, "developer_network"));
        //�ҵ����µ�openRank
        openRankList = GameData.Instance.GetRepoOpenRankList();

        var repoOpenRankList = openRankList.repoOpenrankList;
        //��ȡ��Ŀ��Ŀǰ����Repository-DeveloperNet��
        repository =(Repository) GameData.Instance.gameParams["Repo_Develop_Net"];
        
        ChartManager.Instance.IniteTopKDeveloperChart(repository.developerNetwork);
        ChartManager.Instance.IniteRepoOpenRankChart(repoOpenRankList);
        ChartManager.Instance.IniteDeveloperPercentChart(repository.developerNetwork);

        ChatBotManager.Instance.SetParam(repository.developOpenRankAverage,repository.GetHeighestDeveloperOpenRank());

        topUITextController = GameObject.FindObjectOfType<TopUITextController>();
        if (topUITextController == null) throw new System.Exception("SceneTransition:topUITextController is null");
        topUITextController.SetTopUITextController(repository.GetRepoDevelopNetAverageOpenRank());

        StarController.Instance.GenerateStars();

    }

    private Color ColorTransfer(float r,float g,float b)
    {
        return new Color(r / 255.0f, g / 255.0f, b / 255.0f);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="param">ƽ����OpenRank</param>
    private void GenrateGradint(int layer,float param)
    {




        if (param <= 5f)
        {
            int index = 0;
            Gradient gradient = new Gradient();

            GradientColorKey[] colorKeys = new GradientColorKey[3];
            colorKeys[0] = new GradientColorKey(Color.black, 0.0f);    // 0% λ��Ϊ��ɫ
            colorKeys[1] = new GradientColorKey(Color.gray, 0.5f);  // 50% λ��Ϊ��ɫ
            colorKeys[2] = new GradientColorKey(Color.white, 1.0f);   // 100% λ��Ϊ��ɫ
            gradient.colorKeys = colorKeys;

            colourSettings.biomeColourSettings.biomes[layer].gradient = gradient;


            //����OceanColour
            colourSettings.oceanColour = colourSettings.oceanColours[index % colourSettings.oceanColours.Length];

        }
        else if (param <= 10f)
        {
            int index = 1;
            Gradient gradient = new Gradient();

            GradientColorKey[] colorKeys = new GradientColorKey[3];
            colorKeys[0] = new GradientColorKey(new Color(0.545f, 0.271f, 0.149f), 0.0f);    // 0% λ��Ϊ��ɫ
            colorKeys[1] = new GradientColorKey(new Color(0.65f, 0.16f, 0.16f), 0.5f);  // 50% λ��Ϊ����ɫ
            colorKeys[2] = new GradientColorKey(Color.blue, 1.0f);   // 100% λ��Ϊ��ɫ
            gradient.colorKeys = colorKeys;


            colourSettings.biomeColourSettings.biomes[layer].gradient = gradient;

            colourSettings.oceanColour = colourSettings.oceanColours[index % colourSettings.oceanColours.Length];
        }
        else if(param <= 20f)
        {
            int index = 2;
            Gradient gradient = new Gradient();

            GradientColorKey[] colorKeys = new GradientColorKey[3];
            colorKeys[0] = new GradientColorKey(Color.red, 0.0f);    // 0% λ��Ϊ��ɫ
            colorKeys[1] = new GradientColorKey(new Color(0.65f, 0.16f, 0.16f), 0.5f);  // 50% λ��Ϊ��ɫ
            colorKeys[2] = new GradientColorKey(Color.blue, 1.0f);   // 100% λ��Ϊ��ɫ
            gradient.colorKeys = colorKeys;

            colourSettings.biomeColourSettings.biomes[layer].gradient = gradient;

            colourSettings.oceanColour = colourSettings.oceanColours[index % colourSettings.oceanColours.Length];
        }
        else
        {
            int index = 3;
            Gradient gradient = new Gradient();
            GradientColorKey[] colorKeys = new GradientColorKey[5];
            colorKeys[0] = new GradientColorKey(ColorTransfer(218f, 226f, 50f), 0.0f);    // 0% λ��Ϊ��ɫ
            colorKeys[1] = new GradientColorKey(ColorTransfer(66.0f, 111f, 41f), 0.25f);  // 50% λ��Ϊ��ɫ
            colorKeys[2] = new GradientColorKey(ColorTransfer(73,125,17), 0.5f);  // 50% λ��Ϊ��ɫ
            colorKeys[3] = new GradientColorKey(ColorTransfer(94,28,5), 0.75f);   // 75%����ɫ
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
        if (currentScence == ScenceType.Main)
        {
            colourSettings = Resources.Load<ColourSettings>("Settings/Colour_main");
            shapeSettings = Resources.Load<ShapeSettings>("Settings/Shape_main");
        }
        else if (currentScence == ScenceType.Start)
        {
            colourSettings = Resources.Load<ColourSettings>("Settings/Colour_start");
            shapeSettings = Resources.Load<ShapeSettings>("Settings/Shape_start");
        }
        //���������ļ�


        ShapeSettings.NoiseLayer noiseLayer = shapeSettings.GetIndexNoiseLayer(0);
        if(noiseLayer.noiseSettings.filterType== NoiseSettings.FilterType.Simple)
        {
            NoiseSettings.SimpleNoiseSettings simpleNoiseSettings = noiseLayer.noiseSettings.simpleNoiseSettings;

            //ͨ��openRankֵ������baseRoughness
            simpleNoiseSettings.strength = Sigmoid(openRankList.lastOpenRank / 100.0f)/10f;
            simpleNoiseSettings.baseRoughness = Sigmoid(openRankList.lastOpenRank/100.0f);

            Debug.Log(repository.GetRepoDevelopNetAverageOpenRank());
            //ͨ��Openrank��ֵ��������ɫ
            GenrateGradint(1, repository.GetRepoDevelopNetAverageOpenRank());
            

            planet.GeneratePlanet();

        }
        else if(noiseLayer.noiseSettings.filterType == NoiseSettings.FilterType.Ridgid)
        {
            NoiseSettings.RidgidNoiseSettings ridgidNoiseSettings = noiseLayer.noiseSettings.ridgidNoiseSettings;

        }
    }
}
