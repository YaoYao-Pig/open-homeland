using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserUIClickedEvent : MonoBehaviour
{
    public Action<UserUIClickedEvent, OnUserUIClickedEventArgs> OnUserUIClickedEvent;

    public void CallOnUserUIClickedEvent(UserNode userNode,UserNodeComponent userNodeComponent)
    {
        OnUserUIClickedEvent?.Invoke(this, new OnUserUIClickedEventArgs()
        {
            userNode= userNode,
            nodeComponent = userNodeComponent
        });
    }
}
public class OnUserUIClickedEventArgs : EventArgs
{
    public UserNodeComponent nodeComponent;
    public UserNode userNode;
}
