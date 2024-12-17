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


    //查看或者退出Repo节点的详细信息视图
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
