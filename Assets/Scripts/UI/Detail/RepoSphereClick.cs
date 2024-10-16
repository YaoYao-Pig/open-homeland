using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCharts.Runtime;

public class RepoSphereClick : MonoBehaviour
{


    private bool flag = true;//flag�����´ε���Ǵ�Repo��Detailҳ�滹�ǹر�Detalҳ��


    private Repository repository;

    private NodeComponent nodeSelf;

    private void Awake()
    {
        

    }



    private void TransferData()
    {
        //RepoName
        GameData.Instance.AddRepoName(gameObject.name);
        GameData.Instance.AddRepoDeveloperNumber(transform.childCount);

        GameData.Instance.AddgameParams("Repo_Develop_Net",(object)repository);
    }

    //�鿴�����˳�Repo�ڵ����ϸ��Ϣ��ͼ
    private void LookRepoDetails()
    {
        if (flag)
        {
            //StartCoroutine(WebController.GetRepoDetail(gameObject.name, m, ParseJsonToChart));
            //StartCoroutine(WebController.GetRepoOpenRank(gameObject.name));

            if (nodeSelf == null) nodeSelf = GetComponent<RepoNodeComponent>();

            
            CameraController.Instance.MoveCameraToSphereAndLoadScence(transform.position, nodeSelf.node.radius);
            TransferData();
            flag = false;
        }
        else if (!flag )
        {

            StartCoroutine(CameraController.Instance.backDetail());
            flag = true;
        }        
    }
    

    private void OnMouseDown()
    {
        repository = gameObject.GetComponent<RepoNodeComponent>().repository;
        LookRepoDetails();
        
    }
}

