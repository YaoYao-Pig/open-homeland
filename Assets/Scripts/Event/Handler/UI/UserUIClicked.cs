using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UserUIClickedEvent))]
public class UserUIClicked : MonoBehaviour
{
    private UserUIClickedEvent userUIClickedEvent;

    private void Awake()
    {
        userUIClickedEvent = GetComponent<UserUIClickedEvent>();
    }

    private void OnEnable()
    {
        userUIClickedEvent.OnUserUIClickedEvent += UserUIClickedEvent_OnUserUIClickedEvent;
        
    }


    private void OnDisable()
    {
        userUIClickedEvent.OnUserUIClickedEvent -= UserUIClickedEvent_OnUserUIClickedEvent;

    }


    private void UserUIClickedEvent_OnUserUIClickedEvent(UserUIClickedEvent userUIClickedEvent, OnUserUIClickedEventArgs userUIClickedEventArgs)
    {
        GameResource.Instance.mainUserUIRoot.gameObject.SetActive(true);
        GameResource.Instance.mainUserUIAnimator.SetBool("ui_out", true);
    }

    void Start()
    {
        
    }


}
