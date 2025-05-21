using System;
using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Threading.Tasks; // 对于异步操作
using UnityEngine.Networking; // 对于 UnityWebRequestMultimedia

public class MusicGenClient : MonoBehaviour
{
    public static MusicGenClient Instance;
    public string serverHost = "192.168.1.114"; // 例如 "192.168.1.100" 或域名
public int serverPort = 65432;
    public AudioSource audioSource; // 在Inspector中指定一个AudioSource组件

    private TcpClient client;
    private NetworkStream stream;
    private bool isProcessing = false;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (audioSource == null)
        {
            // 尝试获取附加的AudioSource，如果没有则添加一个新的
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        SendTestPrompt();
    }

    // --- 修改 RequestMusic 方法 ---
    public async void RequestMusic(string prompt)
    {
        if (isProcessing)
        {
            Debug.LogWarning("正在处理上一个请求，请稍候...");
            return;
        }

        isProcessing = true;
        Debug.Log($"准备向服务器发送Prompt: {prompt}");

        // 在主线程获取 persistentDataPath
        string currentPersistentDataPath = "";
        try
        {
            currentPersistentDataPath = Application.persistentDataPath;
            Debug.Log($"Main thread: persistentDataPath = {currentPersistentDataPath}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"在主线程获取 Application.persistentDataPath 时发生错误: {ex.Message}");
            isProcessing = false;
            return; // 获取路径失败，无法继续
        }


        // 将获取到的路径传递给后台任务
        await Task.Run(() => ConnectAndProcess(prompt, currentPersistentDataPath)); // 传递路径

        // isProcessing 状态将在 ConnectAndProcess 的 finally 块中重置
    }

    // --- 修改 ConnectAndProcess 方法以接受 persistentDataPath 参数 ---
    private async Task ConnectAndProcess(string prompt, string persistentDataPath) // 添加 string persistentDataPath 参数
    {
        try
        {
            client = new TcpClient();
            Debug.Log($"后台线程: 尝试连接到服务器 {serverHost}:{serverPort}...");
            await client.ConnectAsync(serverHost, serverPort);
            stream = client.GetStream();
            Debug.Log("后台线程: 已连接到服务器！");

            byte[] promptBytes = Encoding.UTF8.GetBytes(prompt + "\n");
            await stream.WriteAsync(promptBytes, 0, promptBytes.Length);
            Debug.Log("后台线程: Prompt已发送。等待服务器响应...");

            byte[] initialBuffer = new byte[1024];
            int initialBytesRead = await stream.ReadAsync(initialBuffer, 0, initialBuffer.Length);
            Debug.Log($"后台线程: 初始从流中读取了 {initialBytesRead} 字节。");

            if (initialBytesRead == 0) {
                Debug.LogError("后台线程: 服务器未发送任何数据或连接已关闭。");
                // isProcessing 将在 finally 中设置
                return; // 提前退出Task
            }

            int newlineIndex = -1;
            for (int i = 0; i < initialBytesRead; i++)
            {
                if (initialBuffer[i] == '\n')
                {
                    newlineIndex = i;
                    break;
                }
            }

            if (newlineIndex == -1)
            {
                Debug.LogError("后台线程: 未能在初始读取的数据中找到换行符。服务器响应格式不正确。");
                return;
            }

            string serverHeaderMessage = Encoding.UTF8.GetString(initialBuffer, 0, newlineIndex).Trim();
            Debug.Log($"后台线程: 提取到的服务器头部消息: '{serverHeaderMessage}'");

            long fileSize = 0;

            if (serverHeaderMessage.StartsWith("SIZE:"))
            {
                string sizeStr = serverHeaderMessage.Substring(5);
                try
                {
                    fileSize = long.Parse(sizeStr);
                    Debug.Log($"后台线程: 文件大小解析成功: {fileSize} 字节");
                }
                catch (System.Exception ex_parse)
                {
                    Debug.LogError($"后台线程: 解析文件大小失败: '{sizeStr}'. 错误: {ex_parse.Message}");
                    return;
                }
            }
            else if (serverHeaderMessage.StartsWith("ERROR:"))
            {
                Debug.LogError($"后台线程: 服务器返回错误: {serverHeaderMessage.Substring(6)}");
                return;
            }
            else
            {
                Debug.LogError($"后台线程: 未知的服务器头部消息格式: {serverHeaderMessage}");
                return;
            }

            // 使用从主线程传递过来的 persistentDataPath
            string tempAudioPath = Path.Combine(persistentDataPath, "received_music.wav");
            Debug.Log($"后台线程: 准备创建/打开本地文件用于写入: {tempAudioPath}");

            try
            {
                using (FileStream fileStream = new FileStream(tempAudioPath, FileMode.Create, FileAccess.Write))
                {
                    Debug.Log($"后台线程: 本地文件流已打开: {tempAudioPath}。准备接收数据...");
                    long totalBytesReceived = 0;

                    int fileDataStartIndexInInitialBuffer = newlineIndex + 1;
                    if (fileDataStartIndexInInitialBuffer < initialBytesRead)
                    {
                        int initialDataChunkLength = initialBytesRead - fileDataStartIndexInInitialBuffer;
                        await fileStream.WriteAsync(initialBuffer, fileDataStartIndexInInitialBuffer, initialDataChunkLength);
                        totalBytesReceived += initialDataChunkLength;
                        Debug.Log($"后台线程: 已从初始缓冲区写入 {initialDataChunkLength} 字节。总接收: {totalBytesReceived} / {fileSize}");
                    }

                    byte[] dataBuffer = new byte[8192];
                    while (totalBytesReceived < fileSize)
                    {
                        int bytesToRead = (int)Mathf.Min(dataBuffer.Length, (float)(fileSize - totalBytesReceived));
                        int currentBytesRead = await stream.ReadAsync(dataBuffer, 0, bytesToRead);
                        if (currentBytesRead == 0)
                        {
                            Debug.LogError("后台线程: 服务器在发送文件数据时关闭了连接 (currentBytesRead is 0). 可能文件未完整发送。");
                            break;
                        }
                        await fileStream.WriteAsync(dataBuffer, 0, currentBytesRead);
                        totalBytesReceived += currentBytesRead;
                        Debug.Log($"后台线程: 已接收并写入: {totalBytesReceived} / {fileSize} 字节");
                    }
                    Debug.Log($"后台线程: 音频文件接收循环结束。总接收: {totalBytesReceived} 字节。保存在: {tempAudioPath}");
                }

                if (File.Exists(tempAudioPath))
                {
                    FileInfo fileInfo = new FileInfo(tempAudioPath);
                    if (fileInfo.Length == fileSize)
                    {
                        Debug.Log("后台线程: 文件接收成功且大小符合预期，准备在主线程加载音频。");
                        UnityMainThreadDispatcher.Instance().Enqueue(() => LoadAndPlayAudio(tempAudioPath));
                    }
                    else
                    {
                        Debug.LogError($"后台线程: 文件接收完成但大小不符合预期。预期: {fileSize}, 实际: {fileInfo.Length}. 文件路径: {tempAudioPath}");
                    }
                }
                else
                {
                    Debug.LogError($"后台线程: 文件接收过程后，文件 {tempAudioPath} 未找到。");
                }
            }
            catch (System.Exception ex_filestream)
            {
                Debug.LogError($"后台线程: 处理文件流或接收数据时发生错误: {ex_filestream.Message}\nStackTrace: {ex_filestream.StackTrace}");
            }
        }
        catch (SocketException ex_socket)
        {
            Debug.LogError($"后台线程: Socket异常: {ex_socket.Message} (错误码: {ex_socket.SocketErrorCode}). 请检查服务器IP/端口和网络连接。");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"后台线程: 处理时发生一般性错误: {ex.Message}\nStackTrace: {ex.StackTrace}");
        }
        finally
        {
            if (stream != null)
            {
                try { stream.Close(); } catch { /* ignored */ }
                stream = null;
            }
            if (client != null)
            {
                try { client.Close(); } catch { /* ignored */ }
                client = null;
            }
            Debug.Log("后台线程: 与服务器的连接已关闭 (finally块执行)。");
            isProcessing = false; // 确保在所有情况下都重置此状态
        }
    }
    private async void LoadAndPlayAudio(string audioPath)
{
    // 这个方法由 UnityMainThreadDispatcher 调用，确保在主线程执行
    string url = "file://" + audioPath;
    Debug.Log($"主线程: 尝试从以下路径加载AudioClip: {url}");

    using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV))
    {
        Debug.Log($"主线程: UnityWebRequest 为 '{url}' 创建成功。正在发送请求 (SendWebRequest)...");
        var asyncOperation = www.SendWebRequest(); // 获取异步操作对象

        if (asyncOperation == null)
        {
            Debug.LogError("主线程: SendWebRequest() 返回了 null asyncOperation!");
            return; // <--- 对于 async void 方法，使用 return; 提前退出，而不是 yield break;
        }
        
        var tcs = new TaskCompletionSource<object>();
        asyncOperation.completed += operation => 
        {
            tcs.TrySetResult(null); 
        };
        
        await tcs.Task; // 等待 TaskCompletionSource 被设置为完成状态
        
        Debug.Log($"主线程: SendWebRequest 操作已完成 (通过TaskCompletionSource等待)。");
        Debug.Log($"主线程: www.isDone = {www.isDone}");
        Debug.Log($"主线程: www.result = {www.result}"); 
        Debug.Log($"主线程: www.error = {www.error}"); 

        if (www.downloadHandler != null)
        {
            Debug.Log($"主线程: www.downloadHandler.isDone = {www.downloadHandler.isDone}");
        }
        else
        {
            Debug.LogWarning($"主线程: www.downloadHandler 为 null！");
        }

        if (www.result == UnityWebRequest.Result.Success)
        {
            if (www.downloadHandler != null && www.downloadHandler.isDone)
            {
                Debug.Log("主线程: UnityWebRequest 和 DownloadHandler 都已完成且成功。尝试 GetContent...");
                try
                {
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                    if (clip != null)
                    {
                        if (clip.loadState == AudioDataLoadState.Loaded)
                        {
                            Debug.Log("主线程: 音频加载成功！准备播放。");
                            audioSource.clip = clip;
                            audioSource.Play();
                        }
                        else
                        {
                            Debug.LogError($"主线程: 音频片段已创建但未能加载音频数据。状态: {clip.loadState}.");
                        }
                    }
                    else
                    {
                        Debug.LogError($"主线程: DownloadHandlerAudioClip.GetContent(www) 返回了 null。错误信息: {www.error}");
                    }
                }
                catch (System.InvalidOperationException ioe)
                {
                    Debug.LogError($"主线程: 调用 GetContent 时发生 InvalidOperationException: {ioe.Message}. UnityWebRequest 状态: isDone={www.isDone}, result={www.result}, downloadHandler.isDone={www.downloadHandler?.isDone}. StackTrace: {ioe.StackTrace}");
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"主线程: 处理 AudioClip 时发生其他错误: {ex.Message}. StackTrace: {ex.StackTrace}");
                }
            }
            else
            {
                Debug.LogError($"主线程: UnityWebRequest 成功 (result={www.result})，但 downloadHandler 未完成 (isDone={www.downloadHandler?.isDone}) 或为 null。无法获取内容。错误: {www.error}");
            }
        }
        else 
        {
            Debug.LogError($"主线程: 加载音频时发生错误 (www.result 不是 Success): {www.result}, 错误: {www.error}, URL: {url}");
        }
    }
}

    // Test button in Inspector
    [Header("测试用")]
    public string testPrompt = "upbeat electronic music";
    [ContextMenu("发送测试Prompt")]
    public void SendTestPrompt()
    {
        RequestMusic(testPrompt);
    }
}