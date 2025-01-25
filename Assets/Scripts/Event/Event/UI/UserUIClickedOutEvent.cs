using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserUIClickedOutEvent : MonoBehaviour
{
    public Action<UserUIClickedOutEvent, OnUserUIClickedOutEventArgs> OnUserUIClickedOutEvent;

    public void CallOnUserUIClickedOutEvent(UserNode userNode, UserNodeComponent userNodeComponent)
    {
        OnUserUIClickedOutEvent?.Invoke(this, new OnUserUIClickedOutEventArgs()
        {
            userNode = userNode,
            nodeComponent = userNodeComponent
        });
    }
}
public class OnUserUIClickedOutEventArgs : EventArgs
{
    public UserNodeComponent nodeComponent;
    public UserNode userNode;
}
