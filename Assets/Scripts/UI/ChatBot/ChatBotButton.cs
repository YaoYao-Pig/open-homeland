using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatBotButton : MonoBehaviour
{
    [SerializeField] private GameObject chatDialog;

    [SerializeField] private GameObject outButton;
    int times = 0;
    public void OpenChatBotDialog()
    {
        if (!chatDialog.gameObject.activeSelf)
        {
            chatDialog.SetActive(true);

            outButton.gameObject.SetActive(false);
            if (times == 0)
            {
                ChatBotManager.Instance.Inite();
            }
            GitHubReadmeFetcher.Instance.GetReadMe();
        }
    }

    public void CloseChatBotDialog()
    {
        if (chatDialog.gameObject.activeSelf)
        {
            outButton.gameObject.SetActive(true);
            chatDialog.SetActive(false);

        }
    }
}
