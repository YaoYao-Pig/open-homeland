using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class WorldManager : MonoBehaviour
{


    public GameObject nodePrefab; //�ڵ��prefab
    public List<Repository> repositoryList;

    public List<Node> repoNodeList;
    public Dictionary<Node,List<Node>> userNodeDic;

    public Transform repoRoot;
    public List<string> GetFles(string _path)
    {
        string[] _files = Directory.GetFiles(_path);
        List<string> files = new List<string>();

        foreach (var f in _files)
        {
            string directory = Path.GetDirectoryName(f);
            string fileName = Path.GetFileName(f); // ��ȡ�ļ���Ŀ¼��

            fileName = fileName.Split(".")[0];
            // �ָ�·��
            string[] parts = directory.Split(Path.DirectorySeparatorChar);
            if (parts.Length >= 1)
            {
                string lastPart = parts[^1]; // directory���һ����
                files.Add(lastPart+ Path.DirectorySeparatorChar + fileName);
             
            }
            else
            {
                Debug.Log("·�����ֲ�������");
            }

        }

        return files;
    }
    public void LoadData()
    {
        //����Repo-DevelopNet����
        LoadRepoDevelopNetData(WorldInfo.repo_developerNetFilePath, out repositoryList);

    }

    /// <summary>
    /// ����Repo-DevelopNet����
    /// </summary>
    /// <param name="_filepath">�ļ�·��</param>
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
    /// ͨ��RepoList��ʼ�����ֽڵ�
    /// </summary>
    /// ToDO:ȱ��ID��name�����ֳ�ʼ����������������
    public void IniteNodesByRepo()
    {

        int i = 0;
        foreach(var repo in repositoryList)
        {
            Node repoNode = new RepoNode(i.ToString(), i, NodeType.Repo); i++;
            repoNodeList.Add(repoNode);

            List<Node> nodes = new List<Node>();
            foreach(var u in repo.developerNetwork.nodes)
            {
                //TODO:���������⣬����Repo��User�ڵ㶼����һ����
                nodes.Add(new UserNode(u._name, 0, NodeType.User));
            }
            userNodeDic.Add(repoNode, nodes);
        }
    }


    private void Awake()
    {
        repoNodeList = new List<Node>();
        userNodeDic = new Dictionary<Node, List<Node>>();
        if (repoRoot == null) throw new System.Exception("WorldManager:Awake()=>repoRoot don't initialize");
        
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
        Vector3 areaCenter=Vector3.zero; // ��������
        Vector3 areaSize=new Vector3(100,100,100); // ����Ĵ�С

        foreach(RepoNode repoNode in repoNodeList)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(areaCenter.x - areaSize.x / 2, areaCenter.x + areaSize.x / 2),
                Random.Range(areaCenter.y - areaSize.y / 2, areaCenter.y + areaSize.y / 2),
                Random.Range(areaCenter.z - areaSize.z / 2, areaCenter.z + areaSize.z / 2)
                );

            // ���ýڵ��λ��
            repoNode.position = randomPosition;

            foreach (UserNode userNode in userNodeDic[repoNode])
            {
                // �������λ��
                Vector3 nodeRandomPosition = new Vector3(
                    Random.Range(areaCenter.x - areaSize.x / 2, areaCenter.x + areaSize.x / 2),
                    Random.Range(areaCenter.y - areaSize.y / 2, areaCenter.y + areaSize.y / 2),
                    Random.Range(areaCenter.z - areaSize.z / 2, areaCenter.z + areaSize.z / 2)
                );

                // ���ýڵ��λ��
                userNode.position = nodeRandomPosition;
            }
        }
        


    }

    private bool InstanceNode()
    {
        foreach(var rn in repoNodeList)
        {
            GameObject rg = GameObject.Instantiate(nodePrefab, repoRoot);
            //RepoNode rn= g.AddComponent<RepoNode>();
            //rn =(RepoNode) n;
            rg.transform.position = rn.position;
            foreach (var un in userNodeDic[rn])
            {
                GameObject ug = GameObject.Instantiate(nodePrefab, rg.transform);
                //UserNode un = g.AddComponent<UserNode>();
                //un = (UserNode)n;
                ug.transform.position = un.position;
            }
        }
        

        return true;
    }

}
