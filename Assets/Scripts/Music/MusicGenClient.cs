using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Threading.Tasks; // 对于异步操作
using UnityEngine.Networking; // 对于 UnityWebRequestMultimedia

public class MusicGenClient : MonoBehaviour
{
    public string serverHost = "192.168.1.114"; // 例如 "192.168.1.100" 或域名
    public int serverPort = 65432;
    public AudioSource audioSource; // 在Inspector中指定一个AudioSource组件

    private TcpClient client;
    private NetworkStream stream;
    private bool isProcessing = false;

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        SendTestPrompt();
    }

    public async void RequestMusic(string prompt)
    {
        if (isProcessing)
        {
            Debug.LogWarning("正在处理上一个请求，请稍候...");
            return;
        }

        isProcessing = true;
        Debug.Log($"准备向服务器发送Prompt: {prompt}");

        // 在后台任务中执行网络操作以避免阻塞主线程
        await Task.Run(() => ConnectAndProcess(prompt));

        isProcessing = false;
    }

    private async Task ConnectAndProcess(string prompt)
    {
        try
        {
            client = new TcpClient();
            Debug.Log($"尝试连接到服务器 {serverHost}:{serverPort}...");
            await client.ConnectAsync(serverHost, serverPort); // 异步连接
            stream = client.GetStream();
            Debug.Log("已连接到服务器！");

            // 1. 发送Prompt
            byte[] promptBytes = Encoding.UTF8.GetBytes(prompt + "\n"); // 添加换行符或遵循服务器协议
            await stream.WriteAsync(promptBytes, 0, promptBytes.Length);
            Debug.Log("Prompt已发送。等待服务器响应...");

            // 2. 接收服务器响应 (文件大小或错误)
            byte[] buffer = new byte[1024];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            string serverResponse = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
            Debug.Log($"收到服务器响应: {serverResponse}");

            if (serverResponse.StartsWith("SIZE:"))
            {
                long fileSize = long.Parse(serverResponse.Substring(5));
                Debug.Log($"预计音频文件大小: {fileSize} 字节");

                // 可选：发送准备好接收文件的信号给服务器 (如果服务器端协议需要)
                // byte[] readySignal = Encoding.UTF8.GetBytes("READY\n");
                // await stream.WriteAsync(readySignal, 0, readySignal.Length);

                // 3. 接收音频文件
                string tempAudioPath = Path.Combine(Application.persistentDataPath, "received_music.wav");
                using (FileStream fileStream = new FileStream(tempAudioPath, FileMode.Create, FileAccess.Write))
                {
                    long totalBytesReceived = 0;
                    byte[] dataBuffer = new byte[8192]; // 8KB 缓冲区

                    while (totalBytesReceived < fileSize)
                    {
                        int currentBytesRead = await stream.ReadAsync(dataBuffer, 0, dataBuffer.Length);
                        if (currentBytesRead == 0)
                        {
                            Debug.LogError("服务器在发送文件时提前关闭了连接。");
                            break;
                        }
                        await fileStream.WriteAsync(dataBuffer, 0, currentBytesRead);
                        totalBytesReceived += currentBytesRead;
                        // 更新进度 (可选)
                        // Debug.Log($"已接收: {totalBytesReceived}/{fileSize} 字节");
                    }
                    Debug.Log($"音频文件接收完毕，保存在: {tempAudioPath}");
                }

                if (File.Exists(tempAudioPath))
                {
                    // 由于UnityWebRequest需要在主线程上操作或通过协程，
                    // 我们使用 UnityMainThreadDispatcher (如果项目中有) 或简单的标志来通知主线程加载音频
                    // 为了简化，这里直接调用一个需要在主线程运行的方法（通过 Post 来模拟）
                    UnityMainThreadDispatcher.Instance().Enqueue(() => LoadAndPlayAudio(tempAudioPath));
                }
            }
            else if (serverResponse.StartsWith("ERROR:"))
            {
                Debug.LogError($"服务器错误: {serverResponse.Substring(6)}");
            }
            else
            {
                Debug.LogError($"未知的服务器响应: {serverResponse}");
            }
        }
        catch (SocketException ex)
        {
            Debug.LogError($"Socket异常: {ex.Message} (请检查服务器IP/端口和网络连接，以及服务器是否正在运行)");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"处理时发生错误: {ex.Message}\n{ex.StackTrace}");
        }
        finally
        {
            if (stream != null) stream.Close();
            if (client != null) client.Close();
            Debug.Log("与服务器的连接已关闭。");
        }
    }

    // 这个方法需要在主线程被调用来加载和播放音频
    private async void LoadAndPlayAudio(string audioPath)
    {
        // UnityWebRequestMultimedia.GetAudioClip 需要 "file://" 前缀
        string url = "file://" + audioPath;
        Debug.Log($"尝试从以下路径加载AudioClip: {url}");

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV))
        {
            // www.timeout = 60; // 设置超时
            www.SendWebRequest(); // 使用 AsTask() 来 await

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError || www.result == UnityWebRequest.Result.DataProcessingError)
            {
                Debug.LogError($"加载音频时出错: {www.error} 从路径: {url}");
            }
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                if (clip != null)
                {
                    if (clip.loadState == AudioDataLoadState.Loaded)
                    {
                        Debug.Log("音频加载成功！准备播放。");
                        audioSource.clip = clip;
                        audioSource.Play();
                    }
                    else
                    {
                        Debug.LogError("音频片段已创建但未能加载音频数据。");
                    }
                }
                else
                {
                    Debug.LogError("无法从下载的内容创建AudioClip。");
                }
            }
        }
    }

    // 简单的UI示例，用于从Inspector调用
    [Header("测试用")]
    public string testPrompt = "upbeat electronic music";
    [ContextMenu("发送测试Prompt")]
    public void SendTestPrompt()
    {
        RequestMusic(testPrompt);
    }
}
