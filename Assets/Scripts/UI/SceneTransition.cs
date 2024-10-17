using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class SceneTransition : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 2f;

    public GameObject repoCameraPos;
    public GameObject repoObject;
    public Dictionary<string,Vector3> cameraPositonDic;
    private void Start()
    {
        cameraPositonDic = new Dictionary<string, Vector3>() { { WorldInfo.mainScenceName,Vector3.zero},
            { WorldInfo.detailScenceName,repoCameraPos.transform.position} };

        StartCoroutine(FadeIn());
    }

    public void FadeToScene(string sceneName="")
    {
        StartCoroutine(FadeOut(sceneName));
    }

    IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = 1f - (elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
        color.a = 0f;
        fadeImage.color = color;
    }

    IEnumerator FadeOut(string sceneName)
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = elapsedTime / fadeDuration;
            fadeImage.color = color;
            yield return null;
        }
        color.a = 1f;
        fadeImage.color = color;
        //淡出，这里改成切换位置
        LoadScene(sceneName);
        //UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
    }

    private void LoadScene(string _name)
    {
        if (_name == WorldInfo.detailScenceName)
        {
            StartCoroutine(FadeIn());
            CameraController.Instance.transform.position = repoCameraPos.transform.position;  // = cameraPositonDic[_name];
            repoObject.SetActive(true);
            PlanetManager.Instance.InitializePlanet();
            WorldManager.Instance.SetPlanetUIActiveTrue();
        }
        else if (_name == WorldInfo.mainScenceName)
        {

            CameraController.Instance.transform.position = Vector3.zero;
            repoObject = GameObject.Find("RepoDetailsPosition");

            
            repoObject.SetActive(false);

            WorldManager.Instance.SetPlanetUIActiveFalse();
            
            CameraController.Instance.SetCanMove(true);
            StartCoroutine(FadeIn());
        }
    }

}
