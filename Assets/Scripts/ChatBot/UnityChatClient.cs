using UnityEngine;
using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine.UI;

public class UnitySocketClient
{
    public string host = "localhost";
    public int port = 5000;



    public string SendQuery(string text,Action<string> onComplete)
    {
        string query =text;
        try
        {
            using (var client = new TcpClient(host, port))
            {
                using (var stream = client.GetStream())
                {
                    // 发送查询
                    var queryBytes = Encoding.ASCII.GetBytes(query);
                    stream.Write(queryBytes, 0, queryBytes.Length);

                    // 接收回答
                    var responseBuilder = new StringBuilder();
                    var buffer = new byte[1024];
                    int bytesRead;
                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        responseBuilder.Append(Encoding.ASCII.GetString(buffer, 0, bytesRead));
                    }
                    var response = responseBuilder.ToString();
                    onComplete?.Invoke(response);
                    return response;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error sending query: " + e.ToString());
            return "";
        }
    }
}