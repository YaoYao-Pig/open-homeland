using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarController : MonoBehaviour
{
    [SerializeField] private Transform starPrefab;
    [SerializeField] private Transform centerPlanets;
    public static StarController Instance;
    private List<Repo_Read_DevelopNet_Node> nodes;
    private Dictionary<string, Repo_Read_DevelopNet_Node> nameToNodes;
    private List<Repo_Read_DevelopNet_Edge> edges;
    private Dictionary<string,GameObject> starObjects;
    private Dictionary<string,StarConnector> starConnectors;

    [SerializeField] private Button showDevelopNetWork;
    [SerializeField] private GameObject top_UI;
    [SerializeField] private GameObject ChartRoot;
    [SerializeField] private RepoDetailButton repoDetailButton;
    private bool isDetail = false;

    private void Awake()
    {
        Instance = this;
        nameToNodes = new Dictionary<string, Repo_Read_DevelopNet_Node>();
        starObjects = new Dictionary<string, GameObject>();
        starConnectors = new Dictionary<string, StarConnector>();
    }
    private void Start()
    {
        PlanetManager.Instance.OnPlanetEnd +=GenerateDeveloperStars;
        repoDetailButton.OnLeaveDetail += DestoryStars;
        showDevelopNetWork.onClick.AddListener(()=>{
            OnClickButton();
        });
    }
    public void GenerateDeveloperStars()
    {
        isDetail = false;
        Debug.Log("GenerateDeveloperStars");
        Repository repository = GameData.Instance.GetRepoDetail();
        if (repository == null) throw new System.Exception("Repository Is Null");
        Repo_Read_RepoDeveloperNet repo_Read_DevelopNet_Node = repository.developerNetwork;
        nodes = repo_Read_DevelopNet_Node.GetNodes();
        edges = repo_Read_DevelopNet_Node.GetEdges();

        GiveRandomPosition();
        //IniteEdgeLine();
    }

    private void OnClickButton()
    {
        top_UI.SetActive(isDetail);
        ChartRoot.SetActive(isDetail);
        foreach(var p in starObjects)
        {
            p.Value.SetActive(!isDetail);
        }
        isDetail = !isDetail;
    }


    private void GiveRandomPosition()
    {
        Vector3 areaCenter = centerPlanets.position; // 区域中心
        float radius = 2500f; // 半径，即球形轨道的大小

        foreach (var star in nodes)
        {
            // 生成随机的球面坐标
            float theta = Random.Range(0f, Mathf.PI * 2); // 随机角度 [0, 2π]
            float phi = Random.Range(0f, Mathf.PI); // 随机角度 [0, π]

            // 将球面坐标转换为笛卡尔坐标
            float x = radius * Mathf.Sin(phi) * Mathf.Cos(theta);
            float y = radius * Mathf.Sin(phi) * Mathf.Sin(theta);
            float z = radius * Mathf.Cos(phi);

            Vector3 randomPosition = areaCenter + new Vector3(x, y, z);
            var g = GameObject.Instantiate(starPrefab, randomPosition, Quaternion.identity, transform).gameObject;
            nameToNodes.Add(star._name, star);
            starObjects.Add(star._name, g);
            starConnectors.Add(star._name, g.GetComponentInChildren<StarConnector>());
            g.SetActive(false);
        }
    }


    public List<Repo_Read_DevelopNet_Node> GetNodes()
    {
        return nodes;
    }

    public List<Repo_Read_DevelopNet_Edge> GetEdges()
    {
        return edges;
    }
    public Dictionary<string, Repo_Read_DevelopNet_Node> GetNameToNodes()
    {
        return nameToNodes;
    }
    public Dictionary<string, GameObject> GetStarObjects()
    {
        return starObjects;
    }

    private void DestoryStars()
    {
        foreach(Transform child in gameObject.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void IniteEdgeLine()
    {

        foreach (var edge in edges)
        {
            GameObject fromNode = starObjects[edge._startNodeName];
            GameObject toNode = starObjects[edge._endNodeName];
            starConnectors[edge._startNodeName].IniteEdgeLine(fromNode, toNode);
        }

    }

}
