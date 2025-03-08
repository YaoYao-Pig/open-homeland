using System;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Networking;

public class DeepSeekRequest
{
    private string apiKey= "sk-fdabe410e2cd45fda569df4dd59d9fa2";

    public void Initialize(string apiKey)
    {
        this.apiKey = apiKey;
    }

    public void GetCompletion(string model, string prompt, float temperature, Action<string> callback)
    {
        if (string.IsNullOrWhiteSpace(prompt))
        {
            Debug.LogError("提示内容不能为空。");
            callback?.Invoke(null);
            return;
        }

        if (string.IsNullOrWhiteSpace(model))
        {
            Debug.LogError("模型名称不能为空。");
            callback?.Invoke(null);
            return;
        }

        if (string.IsNullOrWhiteSpace(this.apiKey))
        {
            Debug.LogError("API 密钥未设置。");
            callback?.Invoke(null);
            return;
        }

        var url = "https://api.deepseek.com/v1/chat/completions";
        var request = new UnityWebRequest(url, "POST");
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + this.apiKey);

        var requestBody = new ChatCompletionRequest
        {
            model = model,
            messages = new[] { new Message { role = "user", content = prompt } },
            temperature = temperature
        };

        var jsonBody = JsonConvert.SerializeObject(requestBody);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonBody));

        request.SendWebRequest().completed += (operation) =>
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                var responseJson = request.downloadHandler.text;
                var response = JsonConvert.DeserializeObject<ChatCompletionResponse>(responseJson);
                if (response.error != null)
                {
                    Debug.LogError("API 错误: " + response.error.message);
                    callback?.Invoke(null);
                }
                else
                {
                    string r = response.choices[0].message.content;
                    r.Replace('，', ',');
                    callback?.Invoke(r);
                }
            }
            else
            {
                Debug.LogError("错误: " + request.error);
                callback?.Invoke(null);
            }
        };
    }
}

public class ChatCompletionRequest
{
    public string model;
    public Message[] messages;
    public float temperature;
}

public class Message
{
    public string role;
    public string content;
}

public class ChatCompletionResponse
{
    public string id;
    public string @object;
    public int created;
    public string model;
    public Choice[] choices;
    public Usage usage;
    public Error error;
}

public class Choice
{
    public int index;
    public Message message;
    public string finish_reason;
}

public class Usage
{
    public int prompt_tokens;
    public int completion_tokens;
    public int total_tokens;
}

public class Error
{
    public string message;
    public string code;
    public string param;
}