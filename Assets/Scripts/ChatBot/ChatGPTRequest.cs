using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using Newtonsoft.Json;
using System;

public class ChatGPTRequest
{
    // ChatGPT API Endpoint�����API��Կ
    private const string API_URL = "http://202.120.92.104:8000/v1/chat/completions";
    private const string API_KEY = "sk-faba2a644baa48bc9d7c6b39b84ab487"; // �滻Ϊ���Լ���API��Կ

    
    // ������������ݽṹ
    [System.Serializable]
    public class GPTRequestData
    {
        public string model = "/models/DeepSeek-R1-Distill-Llama-70B/"; // ѡ��ģ�ͣ������滻Ϊ����ģ��
        public string prompt;
        public int max_tokens = 150;
        public float temperature = 0.7f;
    }

    // ����API��Ӧ���ݽṹ
    [System.Serializable]
    public class GPTResponseData
    {
        public string id;
        public string objectType;
        public int created;
        public GPTChoice[] choices;
    }

    [System.Serializable]
    public class GPTChoice
    {
        public string text;
    }

    // ��Unity�е���ChatGPT API��Э��
    public IEnumerator CallChatGPT(string userPrompt,Action<string> onComplete)
    {
        // ������������
        GPTRequestData requestData = new GPTRequestData
        {
            prompt = userPrompt
        };

        // ����������ת��ΪJSON
        string jsonData = JsonConvert.SerializeObject(requestData);

        // ����UnityWebRequest��������ΪPOST����
        using (UnityWebRequest www = UnityWebRequest.Post(API_URL, "POST"))
        {
            // ��������ͷ
            www.SetRequestHeader("Authorization", "Bearer " + API_KEY);
            www.SetRequestHeader("Content-Type", "application/json");

            // ��JSON����д��������
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();

            // �ȴ�������Ӧ
            yield return www.SendWebRequest();

            // ����Ƿ�������
            if (www.result == UnityWebRequest.Result.Success)
            {
                // ����JSON��Ӧ
                string responseText = www.downloadHandler.text;
                GPTResponseData responseData = JsonConvert.DeserializeObject<GPTResponseData>(responseText);

                // ��ȡChatGPT���ɵ��ı�
                string chatGPTResponse = responseData.choices[0].text;
                Debug.Log("ChatGPT Response: " + chatGPTResponse);
                onComplete(chatGPTResponse);
            }
            else
            {
                // ���������Ϣ
                Debug.LogError("Error: " + www.error);
            }
        }
    }
}
