using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCharts.Runtime;

public class RepoSphereClick : MonoBehaviour
{


    private bool flag = true;//flag代表下次点击是打开Repo的Detail页面还是关闭Detal页面


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

    //查看或者退出Repo节点的详细信息视图
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
            //销毁图表
            //DestoryCharts();
            
        }        
    }
    

    private void OnMouseDown()
    {
        LookRepoDetails();
        
    }
}

