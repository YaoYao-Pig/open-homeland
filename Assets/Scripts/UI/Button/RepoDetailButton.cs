using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepoDetailButton : MonoBehaviour
{
    
    public void BackToMain()
    {
        Debug.Log("Return");
        GameData.Instance.ClearParams();

        //º”‘ÿ≥°æ∞
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(WorldInfo.mainScenceName);
    }
    

}
