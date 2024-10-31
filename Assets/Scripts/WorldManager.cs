using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class WorldManager : MonoBehaviour
{

    private static WorldManager _instance;
    public static WorldManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<WorldManager>();
                if (_instance == null)
                {
                    GameObject g = new GameObject(typeof(WorldManager).Name);
                    g.AddComponent<WorldManager>();
                }
            }
            return _instance;
        }
        private set { }
    }

    public GameObject repoPrefab; //repo节点的prefab
    public GameObject userPrefab; //User节点的Prefab




    public List<Repository> repositoryList;

    [SerializeField] private List<Node> repoNodeList;

    public Dictionary<Node,List<Node>> repo2UserNodeDic;//用于表示repoNode->userNodeList的映射关系

    






    public Transform repoRoot;
    

    public Dictionary<string, Node> userNodeDic; //user节点的存储，存储一个统一的实例，所有其他位置都是对这个实例的引用
    public Dictionary<string, GameObject> userNodeInstanceDic;//存储user节点的实例，实例化之后就存入这里，防止同一个user被重复创建.

    public List<GameObject> repoObjectList;
    [SerializeField] private Material lineMaterial; //存储子节点和父节点连线的材质
    
    
    
    
    private CSVReader csvReader;
    private List<Node_CSV> node_CSVList;

    public Dictionary<string, List<string>> repoEdgeNameDic;


    public GameObject repoUIGameObject;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if(_instance!=null&& _instance!=this)
        {
            Destroy(_instance);
        }


        csvReader = GetComponent<CSVReader>();
        repoNodeList = new List<Node>();
        repo2UserNodeDic = new Dictionary<Node, List<Node>>();
        userNodeDic = new Dictionary<string, Node>();
        userNodeInstanceDic = new Dictionary<string, GameObject>();
        repositoryList = new List<Repository>();
        if (repoRoot == null) throw new System.Exception("WorldManager:Awake()=>repoRoot don't initialize");



    }


    

    public void IniteByCSV()
    {
        node_CSVList=csvReader.ReadPositionCSV();
        repoEdgeNameDic=csvReader.ReadEdgCSV();
        //建映射
        Dictionary<string, Node_CSV> nameToCSVNodeDic = new Dictionary<string, Node_CSV>();
        foreach(var node_csv in node_CSVList)
        {
            nameToCSVNodeDic[node_csv.nodeName]= node_csv;
        }

        Dictionary<string, List<string>> repoToDeveloper = csvReader.ReadRepoCSV();

        List<string> repoNameList = new List<string>();
        //填充repositoryList
        foreach(var developerNames in repoToDeveloper)
        {
            Repository repository = new Repository();
            repository.repoOpenRank = nameToCSVNodeDic[developerNames.Key].openrank;
            Repo_Read_RepoDeveloperNet read_RepoDeveloperNet = new Repo_Read_RepoDeveloperNet();
            repoNameList.Add(developerNames.Key);

            foreach (var dname in developerNames.Value)
            {
                Node_CSV nc =new Node_CSV();
                if(nameToCSVNodeDic.TryGetValue(dname, out nc))
                {
                    read_RepoDeveloperNet.nodes.Add(new Repo_Read_DevelopNet_Node(dname, nc.openrank));
                }
                else
                {
                    continue;
                }
            }
            repository.developerNetwork = read_RepoDeveloperNet;
            repositoryList.Add(repository);
        }

        IniteNodesByRepo(repoNameList,nameToCSVNodeDic);
        InstanceNode();

        //NodeConnector.IniteEdgeLine();
    }

    public void Inite()
    {
        LoadJsonData(WorldInfo.repo_developerNetFilePath);
        /*worldManager.repositoryList.ToString();*/
        IniteNodesByRepo();

        //tmp
        GiveRandomPosition();
        InstanceNode();

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_filepath">文件夹目录</param>
    /// <param name="repositories"></param>
    private void LoadJsonData(string _filepath)
    {
        List<string> filePaths = Utils.GetFles(_filepath);
        foreach(string path in filePaths)
        {
            Repository repository = new Repository();
            repository.developerNetwork = LoadRepoDevelopNetData(path);
            //加载Repo-DevelopNet数据
            repositoryList.Add(repository);
        }
    }





    /// <summary>
    /// 获得一个repo的DeveloperNetData
    /// </summary>
    /// <param name="_filepath">具体的位置</param>
    /// <returns></returns>
    public Repo_Read_RepoDeveloperNet LoadRepoDevelopNetData(string _filepath)
    {
        string data = Utils.LoadJsonFromResources(_filepath);
        LitJson.JsonData jsonData = LitJson.JsonMapper.ToObject(data);


        Repo_Read_RepoDeveloperNet repoDNet = Repo_Read_RepoDeveloperNet.ParseJson(jsonData);
        return repoDNet;
    }
    /// <summary>
    /// 通过RepoList初始化各种节点
    /// </summary>
    /// ToDO:缺少ID，name，这种初始化方法还是有问题
    private void IniteNodesByRepo()
    {
        int i = 0;
        foreach(Repository repo in repositoryList)
        {

            Node repoNode = new RepoNode(WorldInfo.initeRepoNameList[i], i, NodeType.Repo); i++;
            
            repoNodeList.Add(repoNode);

            List<Node> nodes = new List<Node>();

            foreach(Repo_Read_DevelopNet_Node u in repo.developerNetwork.nodes)
            {
                Node userNode =new UserNode();
                if (!userNodeDic.TryGetValue(u._name, out userNode))
                {
                    userNode = new UserNode(u._name, 0, NodeType.Repo);
                    userNodeDic[u._name] = userNode;
                }
                nodes.Add(userNode);

            }
            repo2UserNodeDic.Add(repoNode, nodes);
        }
    }
    private void IniteNodesByRepo(List<string> names, Dictionary<string, Node_CSV> nameToCSVNodeDic)
    {
        int i = 0;
        foreach (Repository repo in repositoryList)
        {

            Node repoNode = new RepoNode(names[i], i, NodeType.Repo,repo.repoOpenRank);             
            repoNodeList.Add(repoNode);
            repoNode.position = nameToCSVNodeDic[names[i]].nodePosition; i++;

            List<Node> nodes = new List<Node>();

            foreach (Repo_Read_DevelopNet_Node u in repo.developerNetwork.nodes)
            {
                Node userNode = new UserNode();
                if (!userNodeDic.TryGetValue(u._name, out userNode))
                {
                    userNode = new UserNode(u._name, 0, NodeType.Repo, 1/repoNode.scale); //平衡缩放
                    userNode.position = nameToCSVNodeDic[u._name].nodePosition;
                    userNodeDic[u._name] = userNode;
                }
                nodes.Add(userNode);

            }
            repo2UserNodeDic.Add(repoNode, nodes);
        }
    }



    private void Start()
    {
        //Inite();
        IniteByCSV();
    }

    public void GiveRandomPosition()
    {
        Vector3 areaCenter=Vector3.zero; // 区域中心
        Vector3 areaSize=new Vector3(1000, 1000, 1000); // 区域的大小

        Vector3 subAreaCenter = Vector3.zero;
        Vector3 subAreaSize = new Vector3(100, 100, 100);
        foreach(RepoNode repoNode in repoNodeList)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(areaCenter.x - areaSize.x / 2, areaCenter.x + areaSize.x / 2),
                Random.Range(areaCenter.y - areaSize.y / 2, areaCenter.y + areaSize.y / 2),
                Random.Range(areaCenter.z - areaSize.z / 2, areaCenter.z + areaSize.z / 2)
                );

            // 设置节点的位置
            repoNode.position = randomPosition;

            subAreaCenter = repoNode.position;
            //对于每个RepoNode，对应的UserNodeList
            foreach (UserNode userNode in repo2UserNodeDic[repoNode])
            {
                // 计算随机位置
                Vector3 nodeRandomPosition = new Vector3(
                    Random.Range(subAreaCenter.x - subAreaSize.x / 2, subAreaCenter.x + subAreaSize.x / 2),
                    Random.Range(subAreaCenter.y - subAreaSize.y / 2, subAreaCenter.y + subAreaSize.y / 2),
                    Random.Range(subAreaCenter.z - subAreaSize.z / 2, subAreaCenter.z + subAreaSize.z / 2)
                );

                // 设置节点的位置
                userNode.position = nodeRandomPosition;
            }
        }
    }

    public bool InstanceNode()
    {
        int i = 0;
        foreach(var rn in repoNodeList)
        {
            GameObject rg = GameObject.Instantiate(repoPrefab, repoRoot);
            rg.name = rn.nodeName;
            //添加NodeComponet
            RepoNodeComponent repoNode = rg.GetComponent<RepoNodeComponent>();
            repoNode.lineMaterial = lineMaterial;
            repoNode.repository = repositoryList[i++];

            repoNode.node = rn;

            //设置生成节点位置和缩放
            rg.transform.position = rn.position;
            rg.transform.localScale *= rn.scale;
            repoObjectList.Add(rg);
            foreach (var un in repo2UserNodeDic[rn])
            {
                GameObject ug = null;
                if(!userNodeInstanceDic.TryGetValue(un.nodeName,out ug))
                {
                    ug = Instantiate(userPrefab, rg.transform);

                    UserNodeComponent userNode = ug.GetComponent<UserNodeComponent>();
                    userNode.lineMaterial = lineMaterial;

                    var drawLine=ug.AddComponent<DrawLineToParent>();

                    userNode.node = un;
                    ug.name = un.nodeName;

                    ug.transform.position = un.position;
                    ug.transform.localScale *= un.scale;

                    userNodeInstanceDic[un.nodeName] = ug;
                }
                else
                {
                    continue;
                }
                
            }
        }
        

        return true;
    }

    public void SetPlanetUIActiveTrue()
    {
        ChartManager.Instance.DestoryCharts();
        repoUIGameObject.SetActive(true);
    }
    public void SetPlanetUIActiveFalse()
    {
        repoUIGameObject.SetActive(false);
    }

}
