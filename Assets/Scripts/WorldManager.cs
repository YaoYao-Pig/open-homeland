using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class WorldManager : MonoBehaviour
{


    public GameObject nodePrefab; //节点的prefab
    public List<Repository> repositoryList;

    [SerializeField] private List<Node> repoNodeList;

    public Dictionary<Node,List<Node>> repo2UserNodeDic;//用于表示repoNode->userNodeList的映射关系

    public Transform repoRoot;
    

    public Dictionary<string, Node> userNodeDic; //user节点的存储，存储一个统一的实例，所有其他位置都是对这个实例的引用
    public Dictionary<string, GameObject> userNodeInstanceDic;//存储user节点的实例，实例化之后就存入这里，防止同一个user被重复创建.

    private List<string> repoNameList;//用于存储repo的name；
    public List<GameObject> repoObjectList;
    [SerializeField] private Material lineMaterial; //存储子节点和父节点连线的材质

    private void Awake()
    {
        repoNodeList = new List<Node>();
        repo2UserNodeDic = new Dictionary<Node, List<Node>>();
        userNodeDic = new Dictionary<string, Node>();
        userNodeInstanceDic = new Dictionary<string, GameObject>();
        repoNameList = new List<string>();
        repositoryList = new List<Repository>();
        if (repoRoot == null) throw new System.Exception("WorldManager:Awake()=>repoRoot don't initialize");



    }

    public void Inite()
    {
        LoadData(WorldInfo.repo_developerNetFilePath);
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
    public void LoadData(string _filepath)
    {

        List<string> filePaths = Utils.GetFles(_filepath, out repoNameList);
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
    public RepoDeveloperNet LoadRepoDevelopNetData(string _filepath)
    {
        string data = Utils.LoadJsonFromResources(_filepath);
        LitJson.JsonData jsonData = LitJson.JsonMapper.ToObject(data);
        RepoDeveloperNet repoDNet = RepoDeveloperNet.ParseJson(jsonData);
        return repoDNet;
    }
    /// <summary>
    /// 通过RepoList初始化各种节点
    /// </summary>
    /// ToDO:缺少ID，name，这种初始化方法还是有问题
    public void IniteNodesByRepo()
    {

        int i = 0;

        foreach(Repository repo in repositoryList)
        {

            Node repoNode = new RepoNode(repoNameList[i], i, NodeType.Repo); i++;
            repoNodeList.Add(repoNode);

            List<Node> nodes = new List<Node>();
            foreach(RepoDeveloperNet._Read_Node u in repo.developerNetwork.nodes)
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



    private void Start()
    {
        Inite();

    }

    public void GiveRandomPosition()
    {
        Vector3 areaCenter=Vector3.zero; // 区域中心
        Vector3 areaSize=new Vector3(100,100,100); // 区域的大小

        foreach(RepoNode repoNode in repoNodeList)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(areaCenter.x - areaSize.x / 2, areaCenter.x + areaSize.x / 2),
                Random.Range(areaCenter.y - areaSize.y / 2, areaCenter.y + areaSize.y / 2),
                Random.Range(areaCenter.z - areaSize.z / 2, areaCenter.z + areaSize.z / 2)
                );

            // 设置节点的位置
            repoNode.position = randomPosition;

            //对于每个RepoNode，对应的UserNodeList
            foreach (UserNode userNode in repo2UserNodeDic[repoNode])
            {
                // 计算随机位置
                Vector3 nodeRandomPosition = new Vector3(
                    Random.Range(areaCenter.x - areaSize.x / 2, areaCenter.x + areaSize.x / 2),
                    Random.Range(areaCenter.y - areaSize.y / 2, areaCenter.y + areaSize.y / 2),
                    Random.Range(areaCenter.z - areaSize.z / 2, areaCenter.z + areaSize.z / 2)
                );

                // 设置节点的位置
                userNode.position = nodeRandomPosition;
            }
        }
        


    }

    public bool InstanceNode()
    {
        foreach(var rn in repoNodeList)
        {
            GameObject rg = GameObject.Instantiate(nodePrefab, repoRoot);
            rg.name = rn.nodeName;
            //添加NodeComponet
            RepoNodeComponent repoNode=rg.AddComponent<RepoNodeComponent>();
            repoNode.lineMaterial = lineMaterial;


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
                    ug = GameObject.Instantiate(nodePrefab, rg.transform);

                    UserNodeComponent userNode = ug.AddComponent<UserNodeComponent>();
                    userNode.lineMaterial = lineMaterial;

                    ug.AddComponent<DrawLineToParent>();

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

}
