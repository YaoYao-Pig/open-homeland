using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepoDetailButton : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject centerPlanet;
    private SceneTransition sceneTransition;
    
    private GameObject ship;

    private void Awake()
    {
        sceneTransition = FindObjectOfType<SceneTransition>();
    }
    public void BackToMain()
    {
        Debug.Log("Return");
        GameData.Instance.ClearParams();

        sceneTransition.FadeToScene(WorldInfo.mainScenceName);

        //º”‘ÿ≥°æ∞
        //UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(WorldInfo.mainScenceName);
    }
    /*
        private enum TravelStage
        {
            View,Travel
        }
        TravelStage stage = TravelStage.View;
        public void SwitchToTravell()
        {
            if (stage == TravelStage.View)
            {
                if (mainCamera == null) mainCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
                if (travelCamera == null) travelCamera = GameObject.Find("TravelCamera").GetComponent<Camera>();

                travelCamera.transform.position = mainCamera.transform.position;
                travelCamera.transform.rotation = mainCamera.transform.rotation;
                mainCamera.gameObject.SetActive(false);
                travelCamera.gameObject.SetActive(true);


                stage = TravelStage.Travel;
            }
            else if(stage== TravelStage.Travel)
            {
                mainCamera.transform.position = travelCamera.transform.position;
                mainCamera.transform.rotation = travelCamera.transform.rotation;
                travelCamera.gameObject.SetActive(false);
                mainCamera.gameObject.SetActive(true);

                stage = TravelStage.View;
            }
        }*/
    private int numberPlayer = 0;
    public void GeneratePlayer()
    {
        if (numberPlayer == 0)
        {
            ship = GameObject.Find("Ship");
            Instantiate(playerPrefab, centerPlanet.transform).transform.position = ship.transform.position;
            ship.SetActive(false);
            numberPlayer++;
        }

    }


}
