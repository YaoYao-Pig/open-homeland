using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatBotSubmitButton : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;

    public enum ChatBotState
    {
        inRepo,
        inOut
    }
    public ChatBotState currentState = ChatBotState.inOut;

    public float activity;
    public float openRank;

    private string repoSearchContext;
    public string currentRepoContext;

    public void Submit()
    {

        Debug.Log("SubMit");
        if (currentState == ChatBotState.inRepo)
        {

        }
        ChatBotManager.Instance.SubmitUserInput(inputField.text);
    }



    private void SearchInDataset(string searchContext)
    {

    }
}
