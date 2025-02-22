using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UserUIClickedOutEvent))]
public class UserUIClickedOut : MonoBehaviour
{
    private UserUIClickedOutEvent userUIClickedOutEvent;

    private void Awake()
    {
        userUIClickedOutEvent = GetComponent<UserUIClickedOutEvent>();
    }

    private void OnEnable()
    {
        userUIClickedOutEvent.OnUserUIClickedOutEvent += UserUIClickedEvent_OnUserUIClickedOutEvent;

    }


    private void OnDisable()
    {
        userUIClickedOutEvent.OnUserUIClickedOutEvent -= UserUIClickedEvent_OnUserUIClickedOutEvent;

    }


    private void UserUIClickedEvent_OnUserUIClickedOutEvent(UserUIClickedOutEvent userUIClickedOutEvent, OnUserUIClickedOutEventArgs userUIClickedOutEventArgs)
    {

        StartCoroutine(ClickedOut());
    }

    private IEnumerator ClickedOut()
    {
        GameResource.Instance.mainUserUIAnimator.SetBool("ui_out", false);
        GameResource.Instance.mainUserUIAnimator.SetBool("ui_back", true);
        yield return new WaitForSeconds(1);
        GameResource.Instance.mainUserUIRoot.gameObject.SetActive(true);
    }
    void Start()
    {

    }
}
