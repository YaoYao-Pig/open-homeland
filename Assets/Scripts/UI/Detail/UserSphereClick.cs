using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UserSphereClick : MonoBehaviour
{
    private UserNodeComponent nodeSelf;
    private UserUIClickedEvent userUIClickedEvent;
    private UserUIClickedOutEvent userUIClickedOutEvent;
    private int count = 0;

    private void Awake()
    {
        userUIClickedEvent = GetComponent<UserUIClickedEvent>();
        userUIClickedOutEvent = GetComponent<UserUIClickedOutEvent>();

    }


    //查看或者退出Repo节点的详细信息视图
    private void LookRepoDetails()
    {
        if (nodeSelf == null) nodeSelf = GetComponent<UserNodeComponent>();
        CameraController.Instance.MoveCameraToSphere(transform.position, nodeSelf.node.radius);

    }

    private void DealException()
    {
        Debug.Log("404");
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
        if (count == 0)
        {
            LookRepoDetails();
            userUIClickedEvent.CallOnUserUIClickedEvent(nodeSelf.node as UserNode, nodeSelf);
            //Debug.Log(UIManager.Instance.userBarChart);
            UIManager.Instance.userBarChart.ClearData();
            Debug.Log(gameObject.name);

            StartCoroutine(WebController.GetUserDeveloperOpenRank(gameObject.name, DealException));   
            count=1;
        }
        else
        {
            userUIClickedOutEvent.CallOnUserUIClickedOutEvent(nodeSelf.node as UserNode, nodeSelf);
            count=0;
        }
    }
}
