using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class WorldManager : MonoBehaviour
{


    public GameObject nodePrefab; //节点的prefab
    public List<Repository> repositoryList;

    public List<Node> repoNodeList;
    public List<Node> userNodeList;

    public List<string> GetFles(string _path)
    {
        string[] _files = Directory.GetFiles(_path);
        List<string> files = new List<string>();

        foreach (var f in _files)
        {
            string directory = Path.GetDirectoryName(f);
            string fileName = Path.GetFileName(f); // 获取文件或目录名

            fileName = fileName.Split(".")[0];
            // 分割路径
            string[] parts = directory.Split(Path.DirectorySeparatorChar);
            if (parts.Length >= 1)
            {
                string lastPart = parts[^1]; // directory最后一部分
                files.Add(lastPart+ Path.DirectorySeparatorChar + fileName);
             
            }
            else
            {
                Debug.Log("路径部分不足两层");
            }

        }

        return files;
    }
    public void LoadData()
    {
        //加载Repo-DevelopNet数据
        LoadRepoDevelopNetData(WorldInfo.repo_developerNetFilePath, out repositoryList);

    }

    /// <summary>
    /// 加载Repo-DevelopNet数据
    /// </summary>
    /// <param name="_filepath">文件路径</param>
    /// <returns>RepoDeveloperNet</returns>
    public void LoadRepoDevelopNetData(string _filepath,out List<Repository> repositories)
    {
        repositories = new List<Repository>();
        List<string> filePaths = GetFles(_filepath);

        foreach(string path in filePaths)
        {
            //Debug.Log(path);
            string data = Utils.LoadJsonFromResources(path);
            LitJson.JsonData jsonData = LitJson.JsonMapper.ToObject(data);
            repositories.Add(new Repository(RepoDeveloperNet.ParseJson(jsonData)));
        }
    }
    

    /// <summary>
    /// 通过RepoList初始化各种节点
    /// </summary>
    /// ToDO:缺少ID，name，这种初始化方法还是有问题
    public void IniteNodesByRepo()
    {

        int i = 0;
        foreach(var repo in repositoryList)
        {
            repoNodeList.Add(new RepoNode(i.ToString(), i, NodeType.Repo));i++;
            
            foreach(var u in repo.developerNetwork.nodes)
            {
                //TODO:这里有问题，所有Repo的User节点都混在一起了
                userNodeList.Add(new UserNode(u._name, 0, NodeType.User));
            }

        }
    }


    private void Awake()
    {
        repoNodeList = new List<Node>();
        userNodeList = new List<Node>();
    }
    private void Start()
    {
        //LoadData()
        LoadData();
        repositoryList.ToString();
        IniteNodesByRepo();

        //tmp
        GiveRandomPosition();
        InstanceNode();
    }

    private void GiveRandomPosition()
    {
        Vector3 areaCenter=Vector3.zero; // 区域中心
        Vector3 areaSize=new Vector3(100,100,100); // 区域的大小

        foreach(RepoNode node in repoNodeList)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(areaCenter.x - areaSize.x / 2, areaCenter.x + areaSize.x / 2),
                Random.Range(areaCenter.y - areaSize.y / 2, areaCenter.y + areaSize.y / 2),
                Random.Range(areaCenter.z - areaSize.z / 2, areaCenter.z + areaSize.z / 2)
                );

            // 设置节点的位置
            node.position = randomPosition;
        }
        
        foreach (UserNode node in userNodeList)
        {
            // 计算随机位置
            Vector3 randomPosition = new Vector3(
                Random.Range(areaCenter.x - areaSize.x / 2, areaCenter.x + areaSize.x / 2),
                Random.Range(areaCenter.y - areaSize.y / 2, areaCenter.y + areaSize.y / 2),
                Random.Range(areaCenter.z - areaSize.z / 2, areaCenter.z + areaSize.z / 2)
            );

            // 设置节点的位置
            node.position = randomPosition;
        }

    }

    private bool InstanceNode()
    {
        foreach(var n in repoNodeList)
        {
            GameObject g = GameObject.Instantiate(nodePrefab, transform);
            //RepoNode rn= g.AddComponent<RepoNode>();
            //rn =(RepoNode) n;
            g.transform.position = n.position;
        }
        foreach(var n in userNodeList)
        {
            GameObject g = GameObject.Instantiate(nodePrefab, transform);
            //UserNode un = g.AddComponent<UserNode>();
            //un = (UserNode)n;
            g.transform.position = n.position;
        }

        return true;
    }

}
