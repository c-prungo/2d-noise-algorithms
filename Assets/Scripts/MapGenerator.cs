using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public NoiseSettings noiseSettings;

    public bool autoUpdate;

    [HideInInspector]
    public bool noiseSettingsFoldout;

    public void GenerateMap() {
        float[,] noiseMap = Noise.GenerateNoiseMap(
            noiseSettings.resolution,
            noiseSettings.seed,
            noiseSettings.scale,
            noiseSettings.octaves,
            noiseSettings.lacunarity,
            noiseSettings.persistence,
            noiseSettings.offset,
            noiseSettings.warp
        );
        MapDisplay display = FindObjectOfType<MapDisplay> ();
        display.DrawNoiseMap (noiseMap, noiseSettings.colour1, noiseSettings.colour2);
    }
}
