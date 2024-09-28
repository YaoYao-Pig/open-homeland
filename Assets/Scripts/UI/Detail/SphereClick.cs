using UnityEngine;

public class SphereClick : MonoBehaviour
{


    private bool flag = true;//flag代表下次点击是打开Repo的Detail页面还是关闭Detal页面
    //private static string clickNodeName = "";



    private void ViewChange()
    {
        if (flag)
        {
            StartCoroutine(WebController.GetRepoDetail(repoName, m));
            CameraController.Instance.MoveCameraToSphere(transform.position);
            flag = false;

            //clickNodeName = GetComponent<GameObject>().name;
           

        }
        else if (!flag)
        {

            StartCoroutine(CameraController.Instance.backDetail());
            flag = true;

        }


        
    }


    private string repoName = "CleverRaven/Cataclysm-DDA";
    private string m = "openrank";
    private void OnMouseDown()
    {
        ViewChange();
        






    }
}

