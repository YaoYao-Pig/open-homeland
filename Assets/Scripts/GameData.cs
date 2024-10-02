using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{

    private static GameData _Instance;
    public static GameData Instance { get
        {
            if (_Instance == null)
            {
                _Instance = new GameData();
            }
            return _Instance;
        }private set { } }

    public Dictionary<string,Object> gameParams;

    public bool AddgameParams(string key,Object value)
    {
        Object r=new Object();
        bool tmp= gameParams.TryGetValue(key, out r);
        if (tmp == false)
        {
            gameParams.Add(key, value);
            return true;
        }
        else return false;
    }

    public void ClearParams()
    {
        gameParams.Clear();
    }

    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }
}
