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
    private void Awake()
    {
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
            simpleNoiseSettings.baseRoughness = openRankList.lastOpenRank/100.0f;

            Debug.Log("DeveloperNumber: "+GameData.Instance.GetRepoDeveloperNumber());

            if (GameData.Instance.GetRepoDeveloperNumber()<=20)
            {
                Gradient gradient = new Gradient();

                GradientColorKey[] colorKeys = new GradientColorKey[3];
                colorKeys[0] = new GradientColorKey(Color.black, 0.0f);    // 0% 位置为红色
                colorKeys[1] = new GradientColorKey(Color.gray, 0.5f);  // 50% 位置为绿色
                colorKeys[2] = new GradientColorKey(Color.white, 1.0f);   // 100% 位置为蓝色
                gradient.colorKeys = colorKeys;


                colourSettings.oceanColour = gradient;

            }
            else if(GameData.Instance.GetRepoDeveloperNumber() <= 30)
            {
                Gradient gradient = new Gradient();

                GradientColorKey[] colorKeys = new GradientColorKey[3];
                colorKeys[0] = new GradientColorKey(Color.red, 0.0f);    // 0% 位置为红色
                colorKeys[1] = new GradientColorKey(Color.green, 0.5f);  // 50% 位置为绿色
                colorKeys[2] = new GradientColorKey(Color.blue, 1.0f);   // 100% 位置为蓝色
                gradient.colorKeys = colorKeys;


                colourSettings.oceanColour = gradient;
            }
            else
            {
                ;
            }
            planet.GeneratePlanet();

        }
        else if(noiseLayer.noiseSettings.filterType == NoiseSettings.FilterType.Ridgid)
        {
            NoiseSettings.RidgidNoiseSettings ridgidNoiseSettings = noiseLayer.noiseSettings.ridgidNoiseSettings;

        }
    }
}
