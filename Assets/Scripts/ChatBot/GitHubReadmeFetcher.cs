using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System;

public class GitHubReadmeFetcher : MonoBehaviour
{
    // ʾ���ֿ��ַ��https://github.com/<owner>/<repo>
    public string repositoryURL = "https://github.com/aasm/aasm";

    public static GitHubReadmeFetcher Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void GetReadMe()
    {
        StartCoroutine(GetReadmeContent(repositoryURL, (content, error) =>
        {
            if (!string.IsNullOrEmpty(error))
            {
                Debug.LogError($"Error: {error}");
                return;
            }

            Debug.Log("README content:\n" + content);
        }));
    }



    public static IEnumerator GetReadmeContent(string repoUrl, Action<string, string> callback)
    {
        // �����ֿ��ַ
        if (!TryParseGitHubUrl(repoUrl, out string owner, out string repo))
        {
            callback(null, "Invalid GitHub repository URL");
            yield break;
        }

        // ����GitHub API URL
        string apiUrl = $"https://api.github.com/repos/{owner}/{repo}/readme";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(apiUrl))
        {
            // ���ñ�Ҫ������ͷ
            webRequest.SetRequestHeader("User-Agent", "Unity-GitHub-Readme-Fetcher");
            webRequest.SetRequestHeader("Accept", "application/vnd.github.v3+json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                callback(null, $"API request failed: {webRequest.error}");
                yield break;
            }

            // ������Ӧ����
            GitHubReadmeResponse response = JsonUtility.FromJson<GitHubReadmeResponse>(webRequest.downloadHandler.text);

            // ����Base64����
            try
            {
                byte[] data = Convert.FromBase64String(response.content);
                string decodedContent = Encoding.UTF8.GetString(data);
                callback(decodedContent, null);
            }
            catch (Exception e)
            {
                callback(null, $"Base64 decoding failed: {e.Message}");
            }
        }
    }

    private static bool TryParseGitHubUrl(string url, out string owner, out string repo)
    {
        owner = "";
        repo = "";

        try
        {
            // ����URL��ʽ
            url = url.TrimEnd('/');
            if (url.EndsWith(".git"))
                url = url[..^4];

            // �ָ�·������
            string[] parts = url.Split('/');
            int repoIndex = Array.IndexOf(parts, "github.com") + 1;

            if (parts.Length < repoIndex + 2) return false;

            owner = parts[repoIndex];
            repo = parts[repoIndex + 1];
            return true;
        }
        catch
        {
            return false;
        }
    }

    [Serializable]
    private class GitHubReadmeResponse
    {
        public string content;
        public string encoding;
    }
}