using UnityEngine;

public class SphereClick : MonoBehaviour
{


    private bool flag = true;//flag�����´ε���Ǵ�Repo��Detailҳ�滹�ǹر�Detalҳ��
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

