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

    private UnitySocketClient unitySocketClient = new UnitySocketClient();

    private DeepSeekRequest deepSeekRequest=new DeepSeekRequest();
    private ChatGPTRequest  chatGPTRequest=new ChatGPTRequest();
    private DeepSeekAPI deepSeekAPI = new DeepSeekAPI();

    private string prompt = "";

    //private string paramList;

    private ChatBotEnum currentStatus=ChatBotEnum.MainChat;

    private double openRank;
    private double activity;
    public void SetUp(ChatBotEnum status)
    {
        currentStatus = status;
        if (status == ChatBotEnum.RepoChat)
        {
            prompt = "背景:你是一个对话机器人，你所有的标点符号请都是英文标点,这是一个开源项目的影响力可视化系统，可视化由星球+随机化地形生成操控。随机化生成的方式是通过开源项目的两个影响力控制的，一个是OpenRank值一个是活跃度值，OpenRank值类似于PageRank算法，越高说明项目影响力越高。" +
"这两个参数对于地形的控制如下：参数越高，地形越类似于地球，体现出平缓的地势和绿色。参数越低，项目活跃度越低，地形越崎岖，越红越不正常的颜色。OpenRank在0-1之间，活跃度在0-1之间。" +
"现在，我会给你一个项目的OpenRank值和活跃度，请你根据这个来回答问题。";
            string paramList = "OpenRank: " + openRank + "活跃度: " + activity;
            prompt = prompt + paramList + "你的回答请根据以上背景，但是不要透露出上面的内容";
        }
        else if(status==ChatBotEnum.MainChat)
        {
            prompt = "背景:你是一个对话机器人，这是一个通过星球的方式可视化展示开源项目的连接关系的项目，用户可以进行以下行为：" +
                "1. 点击代表开源项目的蓝色星球，进入详细视图" +
                "2. 点击代表用户的黄色星球，了解单个用户的行为信息" +
                "3. 了解关于XLab实验室的相关内容" +
                "我还会给你提供一些本地知识库的知识，请你根据以上背景，给用户以相关提示和指引。";
        }
    }
    public void SetParam(params double[] ps)
    {
        openRank = ps[0];
        activity = ps[1];

    }
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
        SetUp(ChatBotEnum.MainChat);
    }

    public void Inite()
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
        //StartCoroutine(chatGPTRequest.CallChatGPT("API 测试", AddChatDialog));
        //deepSeekAPI.SetParam(1, 1);
        //StartCoroutine(deepSeekAPI.CallDeepSeekAPI(userInputContext, AddChatDialog));
        userInputContext = prompt + userInputContext;
        if (currentStatus == ChatBotEnum.MainChat)
        {
            SetParam(0.5f, 0.5f);
            string fullPrompt = userInputContext;
            deepSeekRequest.GetCompletion("deepseek-chat", fullPrompt, 0.5f, AddChatDialog);
        }
        else if (currentStatus == ChatBotEnum.RepoChat)
        {
            
            Debug.Log(unitySocketClient.SendQuery(userInputContext, AddChatDialog));
        }




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
