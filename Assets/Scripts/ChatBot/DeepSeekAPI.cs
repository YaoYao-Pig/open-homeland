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
    private const string MODEL = "/models/DeepSeek-R1-Distill-Llama-70B/";  // ʹ�õ�ģ��
    private string prompt = "����һ���Ի������ˣ�����һ����Դ��Ŀ��Ӱ�������ӻ�ϵͳ�����ӻ�������+������������ɲٿء���������ɵķ�ʽ��ͨ����Դ��Ŀ������Ӱ�������Ƶģ�һ����OpenRankֵһ���ǻ�Ծ��ֵ��OpenRankֵ������PageRank�㷨��Խ��˵����ĿӰ����Խ�ߡ�" +
"�������������ڵ��εĿ������£�����Խ�ߣ�����Խ�����ڵ������ֳ�ƽ���ĵ��ƺ���ɫ������Խ�ͣ���Ŀ��Ծ��Խ�ͣ�����Խ��᫣�Խ��Խ����������ɫ��OpenRank��0-1֮�䣬��Ծ����0-1֮�䡣" +
"���ڣ��һ����һ����Ŀ��OpenRankֵ�ͻ�Ծ�ȣ��������������ش����⡣" +
"OpenRank��0.5����Ծ�ȣ�0.2";
    private List<string> previousContext=new List<string>() ;
    private List<string> backGroundPrompt=new List<string>();



    public IEnumerator CallDeepSeekAPI(string userInput,Action<string> onComplete)
    {
        if(previousContext.Count==0)
            previousContext.Add(prompt);
        // ׼�����������messages
        List<object> messages = new List<object>();

        // 1. ��ӱ���Ҫ����Ϊϵͳ��Ϣ��system��
        foreach (var background in previousContext)
        {
            messages.Add(new { role = "system", content = background });
        }

        // 2. ������������ݣ���Ϊ�û������ֵ���Ϣ��user/assistant��
        foreach (var context in backGroundPrompt)
        {
            messages.Add(new { role = "user", content = context });  // �����������û�
            // ����ÿ�������ĺ��������ֵĻ�Ӧ
            messages.Add(new { role = "assistant", content = "Assistant's response to context." });
        }

        // 3. ��ӵ�ǰ�û�����
        messages.Add(new { role = "user", content = userInput });

        // ׼����������
        var requestData = new
        {
            model = MODEL,
            messages = messages
        };

        string jsonData = JsonConvert.SerializeObject(requestData);

        // ��������ͷ��������
        using (UnityWebRequest www = new UnityWebRequest(API_URL, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            // �������󲢵ȴ���Ӧ
            yield return www.SendWebRequest();

            // ����Ƿ�����ɹ�
            if (www.result == UnityWebRequest.Result.Success)
            {
                string responseText = www.downloadHandler.text;
                Debug.Log("Response: " + responseText);

                // ����Ӧ����ȡ������ظ�
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
                Debug.LogError("Response Text: " + www.downloadHandler.text); // ��ӡ�������Ӧ����
            }
        }
    }



    // ���巵�ص����ݽṹ
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


    // ��ȡ</think>��ǩ�������
    private string ExtractAfterThink(string content)
    {
        // ʹ��������ʽ��ȡ</think>֮��Ĳ���
        string pattern = @"</think>\s*(.*)"; // ƥ��</think>��ǩ�������
        Match match = Regex.Match(content, pattern, RegexOptions.Singleline);

        if (match.Success)
        {
            // ����</think>��ǩ֮��Ĳ���
            return match.Groups[1].Value.Trim();
        }
        else
        {
            // ���û���ҵ�</think>��ǩ��ֱ�ӷ�������������
            return content.Trim();
        }
    }


    private void AddToPreviousContext(string str)
    {
        previousContext.Add(str);
    }
}
