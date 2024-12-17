using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UserSphereClick : MonoBehaviour
{
    private UserNodeComponent nodeSelf;

    private void Awake()
    {


    }


    //�鿴�����˳�Repo�ڵ����ϸ��Ϣ��ͼ
    private void LookRepoDetails()
    {
        if (nodeSelf == null) nodeSelf = GetComponent<UserNodeComponent>();
        CameraController.Instance.MoveCameraToSphere(transform.position, nodeSelf.node.radius);
    }


    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (!AchievementSystemController.Instance.CheckAchieveStatus("a_clickStar"))
        {
            AchievementSystemController.Instance.UpdateProgress("a_clickStar", 0);
        }
        LookRepoDetails();

    }
}
