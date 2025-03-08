using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;

public class SceneTransition : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 2f;

    public GameObject repoCameraPos;
    public GameObject repoObject;
    public Dictionary<string,Vector3> cameraPositonDic;


    public static SceneTransition Instance;
    public Action OnMainScenceUnLoad;
    public Action OnMainScenceLoad;
    private void Awake()
    {
        Instance = this;
    }
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
            OnMainScenceUnLoad?.Invoke();
            StartCoroutine(FadeIn());
            repoObject.SetActive(true);

            PlanetManager.Instance.InitializePlanet();
            WorldManager.Instance.SetPlanetUIActiveTrue();
            //WorldManager.Instance.moonController.GenerateMoon();
            //SolarSystemSpawner.Instance.Spawn(0);

            SetUpChatBot(ChatBotEnum.RepoChat);

        }
        else if (_name == WorldInfo.mainScenceName)
        {
            OnMainScenceLoad?.Invoke();
            CameraController.Instance.transform.position = Vector3.zero;
            
            repoObject = GameObject.Find("RepoDetailsPosition");

            
            repoObject.SetActive(false);

            WorldManager.Instance.SetPlanetUIActiveFalse();
            
            CameraController.Instance.SetCanMove(true);
            StartCoroutine(FadeIn());

            SetUpChatBot(ChatBotEnum.MainChat);
        }
    }

    public void SetUpChatBot(ChatBotEnum status)
    {
        ChatBotManager.Instance.SetUp(status);
    }
    public void UpdateCameraPosAndRotate()
    {
        CameraController.Instance.transform.position = repoCameraPos.transform.position;  // = cameraPositonDic[_name];
        CameraController.Instance.SetCameraToDetailPlanet(GameObject.Find("RepoDetailsPosition").transform.position - repoCameraPos.transform.position);
    }

}
