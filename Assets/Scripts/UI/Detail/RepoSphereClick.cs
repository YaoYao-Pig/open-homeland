using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using XCharts.Runtime;

public class RepoSphereClick : MonoBehaviour
{
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

        GameData.Instance.AddCurrentRepository(repository);

        
    }

    //�鿴�����˳�Repo�ڵ����ϸ��Ϣ��ͼ
    private void LookRepoDetails()
    {

        if (nodeSelf == null) nodeSelf = GetComponent<RepoNodeComponent>();
        CameraController.Instance.MoveCameraToSphereAndLoadScence(transform.position, nodeSelf.node.radius);
        TransferData();
    }
    

    private void OnMouseDown()
    {
        //WorldManager.Instance.SetPlanetUIActiveTrue();
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        repository = gameObject.GetComponent<RepoNodeComponent>().repository;
        LookRepoDetails();
        
    }
}

