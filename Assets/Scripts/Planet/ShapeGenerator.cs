using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeGenerator
{
    ShapeSettings shapeSettings;
    NoiseFilter noiseFilter;


    public ShapeGenerator(ShapeSettings settings)
    {
        shapeSettings = settings;
        noiseFilter = new NoiseFilter(settings.noiseSettings);
    }

    public Vector3 CalculatePointOnPlanet(Vector3 pointOnUnitSphere)
    {
        float elevation = noiseFilter.Evaluate(pointOnUnitSphere);
        return pointOnUnitSphere * shapeSettings.planetRadius*(1+elevation);
    }
}
