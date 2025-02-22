using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatBotSubmitButton : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;

    public void Submit()
    {

        Debug.Log("SubMit");
        ChatBotManager.Instance.SubmitUserInput(inputField.text);
    }
}
