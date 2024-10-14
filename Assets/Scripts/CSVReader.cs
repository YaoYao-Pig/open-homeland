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
    private string fileName = "nodes_position"; // ��������չ��
    private string folderPath = "repo_csv"; // �ļ���·��
    private string repofileName = "output";


    public List<Node_CSV> ReadPositionCSV()
    {
        // �� Resources/repo_csv �ļ��м��� CSV �ļ�
        TextAsset csvFile = Resources.Load<TextAsset>($"{folderPath}/{fileName}");
        if (csvFile != null)
        {
            string[] lines = csvFile.text.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
            List<Node_CSV> nodes = new List<Node_CSV>();

            // ����Ƿ����㹻����
            if (lines.Length < 2)
            {
                Debug.LogError("CSV file does not contain enough data.");
                return new List<Node_CSV>();
            }

            // ������ͷ
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                string[] values = line.Split(',');

                // �������
                if (values.Length < 4)
                {
                    Debug.LogWarning($"Line {i} does not have enough columns: {line}");
                    continue; // ��������
                }

                float _openRank = 0.0f;
                try
                {
                    _openRank = float.Parse(values[4].Trim());
                }
                catch
                {

                }
                    // ���� Node_CSV ����
                    Node_CSV node = new Node_CSV
                    {
                        nodeName = values[0].Trim(),
                        nodeType = values[1].Trim(),
                        parent = values[3].Trim(),
                        openrank = _openRank
                    };


                // ����λ���ַ���
                try
                {
                    string positionStr = values[2].Trim();
                    string[] positionValues = positionStr.Split('|');

                    if (positionValues.Length == 3)
                    {
                        // ���Խ���λ��
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
            
/*            // ��ӡ���
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
        // �� Resources/repo_csv �ļ��м��� output.csv �ļ�
        TextAsset outputCsvFile = Resources.Load<TextAsset>($"{folderPath}/{repofileName}");
            string[] lines = outputCsvFile.text.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

            // ���ڴ洢ÿ�� repo ���俪���ߵ��ֵ�
            Dictionary<string, List<string>> repoDevelopers = new Dictionary<string, List<string>>();

            // ������ͷ
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                string[] values = line.Split(',');

                // ���� repo_name �ڵ�һ��
                string repoName = values[0];
                List<string> developers = new List<string>();

                // �ӵڶ��п�ʼ��ȡ������
                for (int j = 1; j < values.Length; j++)
                {
                    if (!string.IsNullOrEmpty(values[j])) // ȷ������������Ϊ��
                    {
                        developers.Add(values[j]);
                    }
                }

                // �� repo �Ͷ�Ӧ�Ŀ������б���ӵ��ֵ���
                repoDevelopers[repoName] = developers;
            }

/*            // �ڿ���̨��ӡ repo ���Ƽ��俪����
            foreach (var entry in repoDevelopers)
            {
                string repoName = entry.Key;
                List<string> developers = entry.Value;
                Debug.Log($"Repo Name: {repoName}, Developers: {string.Join(", ", developers)}");
            }*/
        return repoDevelopers;

    }

}
