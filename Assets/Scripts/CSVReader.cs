using System.Collections.Generic;
using UnityEngine;

public class Node_CSV
{
    public string nodeName;
    public string nodeType;
    public Vector3 nodePosition;
    public string parent;
    public float openrank;
}

public class CSVReader : MonoBehaviour
{
    private string fileName = "nodes_position"; // 不包含扩展名
    private string folderPath = "repo_csv"; // 文件夹路径
    private string repofileName = "output";


    public List<Node_CSV> ReadPositionCSV()
    {
        // 从 Resources/repo_csv 文件夹加载 CSV 文件
        TextAsset csvFile = Resources.Load<TextAsset>($"{folderPath}/{fileName}");
        if (csvFile != null)
        {
            string[] lines = csvFile.text.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
            List<Node_CSV> nodes = new List<Node_CSV>();

            // 检查是否有足够的行
            if (lines.Length < 2)
            {
                Debug.LogError("CSV file does not contain enough data.");
                return new List<Node_CSV>();
            }

            // 跳过表头
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                string[] values = line.Split(',');

                // 检查列数
                if (values.Length < 4)
                {
                    Debug.LogWarning($"Line {i} does not have enough columns: {line}");
                    continue; // 跳过该行
                }

                float _openRank = 0.0f;
                try
                {
                    _openRank = float.Parse(values[4].Trim());
                }
                catch
                {

                }
                    // 创建 Node_CSV 对象
                    Node_CSV node = new Node_CSV
                    {
                        nodeName = values[0].Trim(),
                        nodeType = values[1].Trim(),
                        parent = values[3].Trim(),
                        openrank = _openRank
                    };


                // 处理位置字符串
                try
                {
                    string positionStr = values[2].Trim();
                    string[] positionValues = positionStr.Split('|');

                    if (positionValues.Length == 3)
                    {
                        // 尝试解析位置
                        if (float.TryParse(positionValues[0], out float x) &&
                            float.TryParse(positionValues[1], out float y) &&
                            float.TryParse(positionValues[2], out float z))
                        {
                            node.nodePosition = new Vector3(x, y, z);
                        }
                        else
                        {
                            Debug.LogWarning($"Invalid position format in line {i}: {line}");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Position does not contain 3 values in line {i}: {line}");
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"Error parsing position in line {i}: {line}, Error: {ex.Message}");
                }

                nodes.Add(node);
            }
            
/*            // 打印结果
            foreach (Node_CSV node in nodes)
            {
                Debug.Log($"Node Name: {node.nodeName}, Type: {node.nodeType}, Position: {node.nodePosition}, Parent: {node.parent},OpenRank:{node.openrank}");
            }*/

            return nodes;
        }
        else
        {
            Debug.LogError("CSV file not found in Resources/repo_csv: " + fileName);
            return new List<Node_CSV>();
        }
        
    }

    public Dictionary<string, List<string>> ReadRepoCSV()
    {
        // 从 Resources/repo_csv 文件夹加载 output.csv 文件
        TextAsset outputCsvFile = Resources.Load<TextAsset>($"{folderPath}/{repofileName}");
            string[] lines = outputCsvFile.text.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

            // 用于存储每个 repo 及其开发者的字典
            Dictionary<string, List<string>> repoDevelopers = new Dictionary<string, List<string>>();

            // 跳过表头
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                string[] values = line.Split(',');

                // 假设 repo_name 在第一列
                string repoName = values[0];
                List<string> developers = new List<string>();

                // 从第二列开始读取开发者
                for (int j = 1; j < values.Length; j++)
                {
                    if (!string.IsNullOrEmpty(values[j])) // 确保开发者名不为空
                    {
                        developers.Add(values[j]);
                    }
                }

                // 将 repo 和对应的开发者列表添加到字典中
                repoDevelopers[repoName] = developers;
            }

/*            // 在控制台打印 repo 名称及其开发者
            foreach (var entry in repoDevelopers)
            {
                string repoName = entry.Key;
                List<string> developers = entry.Value;
                Debug.Log($"Repo Name: {repoName}, Developers: {string.Join(", ", developers)}");
            }*/
        return repoDevelopers;

    }

}
