using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCharts.Runtime;

public class RepoSphereClick : MonoBehaviour
{


    private bool flag = true;//flag�����´ε���Ǵ�Repo��Detailҳ�滹�ǹر�Detalҳ��


    private Repository repository;
    

    private void Awake()
    {
       // repository = GetComponent<RepoNodeComponent>().repository;


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


            CameraController.Instance.MoveCameraToSphereAndLoadScence(transform.position);


            TransferData();

            flag = false;
        }
        else if (!flag )
        {

            StartCoroutine(CameraController.Instance.backDetail());
            flag = true;
            //����ͼ��
            //DestoryCharts();
            
        }        
    }
    

    private void OnMouseDown()
    {
        LookRepoDetails();
        
    }
}

