using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class WorldManager : MonoBehaviour
{


    public GameObject nodePrefab; //�ڵ��prefab
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
            repoNodeList.Add(new RepoNode(i.ToString(), i, NodeType.Repo));i++;
            
            foreach(var u in repo.developerNetwork.nodes)
            {
                //TODO:���������⣬����Repo��User�ڵ㶼����һ����
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
        Vector3 areaCenter=Vector3.zero; // ��������
        Vector3 areaSize=new Vector3(100,100,100); // ����Ĵ�С

        foreach(RepoNode node in repoNodeList)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(areaCenter.x - areaSize.x / 2, areaCenter.x + areaSize.x / 2),
                Random.Range(areaCenter.y - areaSize.y / 2, areaCenter.y + areaSize.y / 2),
                Random.Range(areaCenter.z - areaSize.z / 2, areaCenter.z + areaSize.z / 2)
                );

            // ���ýڵ��λ��
            node.position = randomPosition;
        }
        
        foreach (UserNode node in userNodeList)
        {
            // �������λ��
            Vector3 randomPosition = new Vector3(
                Random.Range(areaCenter.x - areaSize.x / 2, areaCenter.x + areaSize.x / 2),
                Random.Range(areaCenter.y - areaSize.y / 2, areaCenter.y + areaSize.y / 2),
                Random.Range(areaCenter.z - areaSize.z / 2, areaCenter.z + areaSize.z / 2)
            );

            // ���ýڵ��λ��
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
