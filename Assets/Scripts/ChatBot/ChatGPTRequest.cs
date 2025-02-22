using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using Newtonsoft.Json;
using System;

public class ChatGPTRequest
{
    // ChatGPT API Endpoint和你的API密钥
    private const string API_URL = "http://202.120.92.104:8000/v1/chat/completions";
    private const string API_KEY = "sk-faba2a644baa48bc9d7c6b39b84ab487"; // 替换为你自己的API密钥

    
    // 定义请求的数据结构
    [System.Serializable]
    public class GPTRequestData
    {
        public string model = "/models/DeepSeek-R1-Distill-Llama-70B/"; // 选择模型，可以替换为其他模型
        public string prompt;
        public int max_tokens = 150;
        public float temperature = 0.7f;
    }

    // 定义API响应数据结构
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

    // 在Unity中调用ChatGPT API的协程
    public IEnumerator CallChatGPT(string userPrompt,Action<string> onComplete)
    {
        // 构造请求数据
        GPTRequestData requestData = new GPTRequestData
        {
            prompt = userPrompt
        };

        // 将请求数据转换为JSON
        string jsonData = JsonConvert.SerializeObject(requestData);

        // 创建UnityWebRequest对象，设置为POST请求
        using (UnityWebRequest www = UnityWebRequest.Post(API_URL, "POST"))
        {
            // 设置请求头
            www.SetRequestHeader("Authorization", "Bearer " + API_KEY);
            www.SetRequestHeader("Content-Type", "application/json");

            // 将JSON数据写入请求体
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();

            // 等待请求响应
            yield return www.SendWebRequest();

            // 检查是否发生错误
            if (www.result == UnityWebRequest.Result.Success)
            {
                // 解析JSON响应
                string responseText = www.downloadHandler.text;
                GPTResponseData responseData = JsonConvert.DeserializeObject<GPTResponseData>(responseText);

                // 获取ChatGPT生成的文本
                string chatGPTResponse = responseData.choices[0].text;
                Debug.Log("ChatGPT Response: " + chatGPTResponse);
                onComplete(chatGPTResponse);
            }
            else
            {
                // 输出错误信息
                Debug.LogError("Error: " + www.error);
            }
        }
    }
}
