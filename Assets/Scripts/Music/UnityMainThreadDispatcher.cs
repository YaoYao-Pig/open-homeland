using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// =========================================================================
// UnityMainThreadDispatcher.cs - 一个帮助在主线程执行代码的工具类
// 你需要将这个脚本添加到你的项目中，通常附加到一个场景中持久存在的GameObject上。
// =========================================================================
public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static readonly System.Collections.Generic.Queue<System.Action> _executionQueue =
        new System.Collections.Generic.Queue<System.Action>();

    private static UnityMainThreadDispatcher _instance = null;

    public static UnityMainThreadDispatcher Instance()
    {
        if (!_instance)
        {
            // 尝试寻找一个已存在的实例
            _instance = FindObjectOfType<UnityMainThreadDispatcher>();
            if (!_instance)
            {
                // 如果没有找到，创建一个新的GameObject并附加此脚本
                var go = new GameObject("UnityMainThreadDispatcher");
                _instance = go.AddComponent<UnityMainThreadDispatcher>();
                DontDestroyOnLoad(go); // 使其在场景切换时不被销毁
            }
        }
        return _instance;
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject); //确保只有一个实例
        }
    }

    public virtual void Update()
    {
        lock (_executionQueue)
        {
            while (_executionQueue.Count > 0)
            {
                _executionQueue.Dequeue().Invoke();
            }
        }
    }

    public void Enqueue(System.Action action)
    {
        lock (_executionQueue)
        {
            _executionQueue.Enqueue(action);
        }
    }
}