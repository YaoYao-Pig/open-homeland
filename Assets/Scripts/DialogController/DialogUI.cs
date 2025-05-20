using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogUI : MonoBehaviour
{
    public TextMeshProUGUI text;

    public void SetText(string t)
    {
        text.text = t;
    }
    public void NextDialog()
    {
        Debug.Log("NextDialog");
        if(DialogController.Instance.index == DialogController.Instance.checkPoint1)
        {
            DialogController.Instance.planet.shapeSettings.noiseLayers[0].enabled = true;
            DialogController.Instance.planet.OnShapeSettingsUpdated();
            DialogController.Instance.index++;
            SetText(DialogController.Instance.dialogList[DialogController.Instance.index]);
        }
        else if(DialogController.Instance.index == DialogController.Instance.checkPoint2)
        {
            DialogController.Instance.planet.shapeSettings.noiseLayers[1].enabled = true;
            DialogController.Instance.planet.OnShapeSettingsUpdated();
            DialogController.Instance.index++;
            SetText(DialogController.Instance.dialogList[DialogController.Instance.index]);
        }
        else if(DialogController.Instance.index == DialogController.Instance.checkPoint3)
        {
            DialogController.Instance.planet.colourSettings.oceanColour = DialogController.Instance.oceanColour;
            DialogController.Instance.planet.OnColourSettingsUpdated();
            DialogController.Instance.index++;
            SetText(DialogController.Instance.dialogList[DialogController.Instance.index]);
        }
        else if(DialogController.Instance.index == DialogController.Instance.checkPoint4)
        {
            DialogController.Instance.planet.colourSettings.biomeColourSettings.biomes[0].gradient = DialogController.Instance.groundColour1;
            DialogController.Instance.planet.OnColourSettingsUpdated();
            DialogController.Instance.index++;
            SetText(DialogController.Instance.dialogList[DialogController.Instance.index]);
        }
        else if (DialogController.Instance.index == DialogController.Instance.checkPoint5)
        {
            
            //gameObject.SetActive(false);
            StartCoroutine(PlanetUtils.CorotinuePlanet(()=>
            {
                DialogController.Instance.index = DialogController.Instance.checkPoint5 + 1;
                SetText(DialogController.Instance.dialogList[DialogController.Instance.index]);
            }));
        }
        else if (DialogController.Instance.index == DialogController.Instance.checkPoint6)
        {
            DialogController.Instance.star.SetActive(true);
            DialogController.Instance.index++;
            SetText(DialogController.Instance.dialogList[DialogController.Instance.index]);
        }
        else if (DialogController.Instance.index == DialogController.Instance.checkPoint7)
        {
            DialogController.Instance.star.transform.localScale = new Vector3(1, 1, 1);
            DialogController.Instance.GenerateStar();
            DialogController.Instance.index++;
            SetText(DialogController.Instance.dialogList[DialogController.Instance.index]);
        }
        else if(DialogController.Instance.index < DialogController.Instance.dialogList.Count -1)
        {
            DialogController.Instance.index++;
            SetText(DialogController.Instance.dialogList[DialogController.Instance.index]);
        }
        else if (DialogController.Instance.index == DialogController.Instance.dialogList.Count - 1)
        {
            SceneManager.LoadScene("MainScence");
            //gameObject.SetActive(false);
        }
        
    }
    
    
    
}
