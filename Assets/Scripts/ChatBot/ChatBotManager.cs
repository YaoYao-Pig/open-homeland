using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ChatBotManager : MonoBehaviour
{
    private static ChatBotManager _instance;
    private string userInputContext;

    private Dictionary<int, GameObject> chatBotDialogDict;
    private Dictionary<int, GameObject> userDialogDict;
    [SerializeField] private GameObject chatBotDialogPrefab;
    [SerializeField] private GameObject userDialogPrefab;
    [SerializeField] private Transform chatContent;

    private int chatBotID=0;
    private int userID=0;




    private ChatGPTRequest chatGPTRequest=new ChatGPTRequest();
    private DeepSeekAPI deepSeekAPI = new DeepSeekAPI();
    private enum DialogTurn
    {
        chatBot,
        user
    }
    private DialogTurn currentTurn=DialogTurn.chatBot;
    private void NextTurn()
    {
        if (currentTurn == DialogTurn.chatBot)
        {
            currentTurn = DialogTurn.user;
        }
        else if(currentTurn==DialogTurn.user)
        {
            currentTurn = DialogTurn.chatBot;
        }
    }
    public static ChatBotManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
        chatBotDialogDict = new Dictionary<int, GameObject>();
        userDialogDict = new Dictionary<int, GameObject>();
    }

    private void Start()
    {
        Inite();
    }

    private void Inite()
    {
        AddChatDialog("Hello!");
    }
    public void SubmitUserInput(string text)
    {
        if (text == "")
        {
            return;
        }
        userInputContext = text;
        AddUserDialog();
        //StartCoroutine(chatGPTRequest.CallChatGPT("API ≤‚ ‘", AddChatDialog));
        StartCoroutine(deepSeekAPI.CallDeepSeekAPI(userInputContext, AddChatDialog));


    }


    private void AddUserDialog()
    {
        if (currentTurn == DialogTurn.user)
        {

            GameObject userDialog = Instantiate(userDialogPrefab, chatContent);
            userDialog.GetComponent<UserDialog>().Inite(userInputContext);
            userDialogDict.Add(userID++, userDialog);

            NextTurn();
        }
        return;
    }

    private void AddChatDialog(string str)
    {
        if (currentTurn == DialogTurn.chatBot)
        {

            GameObject chatDialog = Instantiate(chatBotDialogPrefab, chatContent);
            chatDialog.GetComponent<ChatBotDialog>().Inite(str);
            chatBotDialogDict.Add(chatBotID++, chatDialog);
            NextTurn();
        }
        return;
    }



}
