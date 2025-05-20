using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogController : MonoBehaviour
{
    public GameObject DialogPrefab;
    public static DialogController Instance;
    public Planet planet;

    private DialogUI dialogUI;
    public Gradient defaultColour;
    public Gradient oceanColour;
    public Gradient groundColour1;
    public Gradient groundColour2;
    public Gradient groundColour3;
    
    public List<string> dialogList;
    public int index = -1;
    public int checkPoint1;
    public int checkPoint2;
    public int checkPoint3;
    public int checkPoint4;
    public int checkPoint5;
    public int checkPoint6;
    public int checkPoint7;
    public List<PlanetSettings> settingsList;
    public GameObject star;
    public StarController starController;
    private void Awake()
    {
        Instance = this;
        dialogUI = DialogPrefab.GetComponent<DialogUI>();
        dialogUI.NextDialog();
        planet.shapeSettings.noiseLayers[0].enabled = false;
        planet.shapeSettings.noiseLayers[1].enabled = false;
        planet.shapeSettings.noiseLayers[2].enabled = false;
        planet.OnShapeSettingsUpdated();

        planet.colourSettings.oceanColour = defaultColour;
        planet.colourSettings.biomeColourSettings.biomes[0].gradient = defaultColour;
        planet.colourSettings.biomeColourSettings.biomes[1].gradient = defaultColour;
        planet.colourSettings.biomeColourSettings.biomes[2].gradient = defaultColour;
        planet.OnColourSettingsUpdated();
        
        
    }

    public void GenerateStar()
    {
        starController.GenerateStars();
    }
    public void InitGradientSettings(int index)
    {
        if (index == 0)
        {
            var t1 = new PlanetSettings();
            t1.shapeSettings = planet.shapeSettings;
            t1.colourSettings = planet.colourSettings;
            t1.shapeSettings.noiseLayers[0].enabled = true;
            t1.shapeSettings.noiseLayers[1].enabled = true;
            t1.shapeSettings.noiseLayers[2].enabled = false;
            t1.colourSettings.oceanColour = oceanColour;
            t1.colourSettings.biomeColourSettings.biomes[0].gradient = groundColour1;
            t1.colourSettings.biomeColourSettings.biomes[1].gradient = groundColour2;
            t1.colourSettings.biomeColourSettings.biomes[2].gradient = defaultColour;
            planet.OnColourSettingsUpdated();
            planet.OnShapeSettingsUpdated();

        }
        else
        {
            var t2 = new PlanetSettings();
            t2.shapeSettings = planet.shapeSettings;
            t2.colourSettings = planet.colourSettings;
            t2.shapeSettings.noiseLayers[0].enabled = true;
            t2.shapeSettings.noiseLayers[1].enabled = true;
            t2.shapeSettings.noiseLayers[2].enabled = false;
            t2.colourSettings.oceanColour = oceanColour;
            t2.colourSettings.biomeColourSettings.biomes[0].gradient = groundColour1;
            t2.colourSettings.biomeColourSettings.biomes[1].gradient = groundColour2;
            t2.colourSettings.biomeColourSettings.biomes[2].gradient = groundColour3;
            planet.OnColourSettingsUpdated();
            planet.OnShapeSettingsUpdated();
        }

        

        
    }

    
    
}
