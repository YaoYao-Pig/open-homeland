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
        if (count % 2 == 0)
        {
            LookRepoDetails();
            userUIClickedEvent.CallOnUserUIClickedEvent(nodeSelf.node as UserNode, nodeSelf);
            count++;
        }
        else
        {
            userUIClickedOutEvent.CallOnUserUIClickedOutEvent(nodeSelf.node as UserNode, nodeSelf);
            count++;
        }
    }
}
