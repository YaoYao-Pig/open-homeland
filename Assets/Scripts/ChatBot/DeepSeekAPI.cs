using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Collections;
using System.Text;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class DeepSeekAPI
{
    private const string API_URL = "http://202.120.92.104:8000/v1/chat/completions";  // DeepSeek API URL
    private const string MODEL = "/models/DeepSeek-R1-Distill-Llama-70B/";  // 使用的模型
    private string prompt = "你是一个对话机器人，这是一个开源项目的影响力可视化系统，可视化由星球+随机化地形生成操控。随机化生成的方式是通过开源项目的两个影响力控制的，一个是OpenRank值一个是活跃度值，OpenRank值类似于PageRank算法，越高说明项目影响力越高。" +
"这两个参数对于地形的控制如下：参数越高，地形越类似于地球，体现出平缓的地势和绿色。参数越低，项目活跃度越低，地形越崎岖，越红越不正常的颜色。OpenRank在0-1之间，活跃度在0-1之间。" +
"现在，我会给你一个项目的OpenRank值和活跃度，请你根据这个来回答问题。" +
"OpenRank：0.5，活跃度：0.2";
    private List<string> previousContext=new List<string>() ;
    private List<string> backGroundPrompt=new List<string>();



    public IEnumerator CallDeepSeekAPI(string userInput,Action<string> onComplete)
    {
        if(previousContext.Count==0)
            previousContext.Add(prompt);
        // 准备构造请求的messages
        List<object> messages = new List<object>();

        // 1. 添加背景要求，作为系统消息（system）
        foreach (var background in previousContext)
        {
            messages.Add(new { role = "system", content = background });
        }

        // 2. 添加上下文数据，作为用户和助手的消息（user/assistant）
        foreach (var context in backGroundPrompt)
        {
            messages.Add(new { role = "user", content = context });  // 上下文来自用户
            // 假设每个上下文后面是助手的回应
            messages.Add(new { role = "assistant", content = "Assistant's response to context." });
        }

        // 3. 添加当前用户输入
        messages.Add(new { role = "user", content = userInput });

        // 准备请求数据
        var requestData = new
        {
            model = MODEL,
            messages = messages
        };

        string jsonData = JsonConvert.SerializeObject(requestData);

        // 设置请求头和请求体
        using (UnityWebRequest www = new UnityWebRequest(API_URL, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            // 发送请求并等待响应
            yield return www.SendWebRequest();

            // 检查是否请求成功
            if (www.result == UnityWebRequest.Result.Success)
            {
                string responseText = www.downloadHandler.text;
                Debug.Log("Response: " + responseText);

                // 从响应中提取出聊天回复
                var responseJson = JsonConvert.DeserializeObject<ResponseData>(responseText);
                string assistantReply = responseJson.choices[0].message.content;
                string cleanedReply = ExtractAfterThink(assistantReply);
                AddToPreviousContext(cleanedReply);
                onComplete(cleanedReply);
            }
            else
            {
                Debug.LogError("Request failed: " + www.error);
                Debug.LogError("Response Code: " + www.responseCode);
                Debug.LogError("Response Text: " + www.downloadHandler.text); // 打印错误的响应内容
            }
        }
    }



    // 定义返回的数据结构
    [System.Serializable]
    public class ResponseData
    {
        public string id;
        public string objectType;
        public string model;
        public Choice[] choices;
    }

    [System.Serializable]
    public class Choice
    {
        public int index;
        public Message message;
    }

    [System.Serializable]
    public class Message
    {
        public string role;
        public string content;
    }


    // 提取</think>标签后的内容
    private string ExtractAfterThink(string content)
    {
        // 使用正则表达式提取</think>之后的部分
        string pattern = @"</think>\s*(.*)"; // 匹配</think>标签后的内容
        Match match = Regex.Match(content, pattern, RegexOptions.Singleline);

        if (match.Success)
        {
            // 返回</think>标签之后的部分
            return match.Groups[1].Value.Trim();
        }
        else
        {
            // 如果没有找到</think>标签，直接返回完整的内容
            return content.Trim();
        }
    }


    private void AddToPreviousContext(string str)
    {
        previousContext.Add(str);
    }
}
