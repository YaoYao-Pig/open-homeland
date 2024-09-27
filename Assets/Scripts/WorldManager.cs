using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class WorldManager : MonoBehaviour
{


    public GameObject nodePrefab; //�ڵ��prefab
    public List<Repository> repositoryList;

    [SerializeField] private List<Node> repoNodeList;

    public Dictionary<Node,List<Node>> repo2UserNodeDic;//���ڱ�ʾrepoNode->userNodeList��ӳ���ϵ

    public Transform repoRoot;
    

    public Dictionary<string, Node> userNodeDic; //user�ڵ�Ĵ洢���洢һ��ͳһ��ʵ������������λ�ö��Ƕ����ʵ��������
    public Dictionary<string, GameObject> userNodeInstanceDic;//�洢user�ڵ��ʵ����ʵ����֮��ʹ��������ֹͬһ��user���ظ�����.

    private List<string> repoNameList;//���ڴ洢repo��name��
    public List<GameObject> repoObjectList;
    [SerializeField] private Material lineMaterial; //�洢�ӽڵ�͸��ڵ����ߵĲ���

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
    /// <param name="_filepath">�ļ���Ŀ¼</param>
    /// <param name="repositories"></param>
    public void LoadData(string _filepath)
    {

        List<string> filePaths = Utils.GetFles(_filepath, out repoNameList);
        foreach(string path in filePaths)
        {
            Repository repository = new Repository();
            repository.developerNetwork = LoadRepoDevelopNetData(path);
            //����Repo-DevelopNet����
            repositoryList.Add(repository);
        }
    }
    /// <summary>
    /// ���һ��repo��DeveloperNetData
    /// </summary>
    /// <param name="_filepath">�����λ��</param>
    /// <returns></returns>
    public RepoDeveloperNet LoadRepoDevelopNetData(string _filepath)
    {
        string data = Utils.LoadJsonFromResources(_filepath);
        LitJson.JsonData jsonData = LitJson.JsonMapper.ToObject(data);
        RepoDeveloperNet repoDNet = RepoDeveloperNet.ParseJson(jsonData);
        return repoDNet;
    }
    /// <summary>
    /// ͨ��RepoList��ʼ�����ֽڵ�
    /// </summary>
    /// ToDO:ȱ��ID��name�����ֳ�ʼ����������������
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

            //����ÿ��RepoNode����Ӧ��UserNodeList
            foreach (UserNode userNode in repo2UserNodeDic[repoNode])
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

    public bool InstanceNode()
    {
        foreach(var rn in repoNodeList)
        {
            GameObject rg = GameObject.Instantiate(nodePrefab, repoRoot);
            rg.name = rn.nodeName;
            //���NodeComponet
            RepoNodeComponent repoNode=rg.AddComponent<RepoNodeComponent>();
            repoNode.lineMaterial = lineMaterial;


            repoNode.node = rn;

            //�������ɽڵ�λ�ú�����
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
