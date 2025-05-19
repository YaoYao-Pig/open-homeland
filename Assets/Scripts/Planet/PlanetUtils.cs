using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetSettings
{
    public ShapeSettings shapeSettings;
    public ColourSettings colourSettings;
}
public static class PlanetUtils 
{
    public static IEnumerator CorotinuePlanet(GameObject dialogUI, Action<GameObject> onComplete)
    {
        yield return new WaitForSeconds(1f);
        int index = 2;
        for (int i = 0; i < 2; ++i)
        {
            DialogController.Instance.InitGradientSettings(i);
            yield return new WaitForSeconds(2f);
        }
        
        
        onComplete?.Invoke(dialogUI);
    }
}
