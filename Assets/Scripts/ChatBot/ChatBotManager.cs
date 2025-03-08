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
            prompt = "����:����һ���Ի������ˣ������еı������붼��Ӣ�ı��,����һ����Դ��Ŀ��Ӱ�������ӻ�ϵͳ�����ӻ�������+������������ɲٿء���������ɵķ�ʽ��ͨ����Դ��Ŀ������Ӱ�������Ƶģ�һ����OpenRankֵһ���ǻ�Ծ��ֵ��OpenRankֵ������PageRank�㷨��Խ��˵����ĿӰ����Խ�ߡ�" +
"�������������ڵ��εĿ������£�����Խ�ߣ�����Խ�����ڵ������ֳ�ƽ���ĵ��ƺ���ɫ������Խ�ͣ���Ŀ��Ծ��Խ�ͣ�����Խ��᫣�Խ��Խ����������ɫ��OpenRank��0-1֮�䣬��Ծ����0-1֮�䡣" +
"���ڣ��һ����һ����Ŀ��OpenRankֵ�ͻ�Ծ�ȣ��������������ش����⡣";
            string paramList = "OpenRank: " + openRank + "��Ծ��: " + activity;
            prompt = prompt + paramList + "��Ļش���������ϱ��������ǲ�Ҫ͸¶�����������";
        }
        else if(status==ChatBotEnum.MainChat)
        {
            prompt = "����:����һ���Ի������ˣ�����һ��ͨ������ķ�ʽ���ӻ�չʾ��Դ��Ŀ�����ӹ�ϵ����Ŀ���û����Խ���������Ϊ��" +
                "1. �������Դ��Ŀ����ɫ���򣬽�����ϸ��ͼ" +
                "2. ��������û��Ļ�ɫ�����˽ⵥ���û�����Ϊ��Ϣ" +
                "3. �˽����XLabʵ���ҵ��������" +
                "�һ�������ṩһЩ����֪ʶ���֪ʶ������������ϱ��������û��������ʾ��ָ����";
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
        //StartCoroutine(chatGPTRequest.CallChatGPT("API ����", AddChatDialog));
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
