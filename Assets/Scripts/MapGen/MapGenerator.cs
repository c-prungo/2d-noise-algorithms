using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public NoiseSettings noiseSettings;
    public TerrainSettings terrainSettings;

    public enum DrawMode {NoiseMap, ColourMap, Mesh};
    public DrawMode drawMode;

    public bool autoUpdate;

    [HideInInspector]
    public bool noiseSettingsFoldout;

    [HideInInspector]
    public bool terrainSettingsFoldout;

    public void GenerateMap() {
        float[,] noiseMap = Noise.GenerateNoiseMap(
            noiseSettings.resolution,
            noiseSettings.seed,
            noiseSettings.scale,
            noiseSettings.octaves,
            noiseSettings.lacunarity,
            noiseSettings.persistence,
            noiseSettings.offset,
            noiseSettings.warp,
            noiseSettings.includeTerrace,
            noiseSettings.terraceDetail
        );

        Color[] colourMap = new Color [noiseSettings.resolution * noiseSettings.resolution];
        for (int y = 0; y < noiseSettings.resolution; y++) {
            for (int x = 0; x < noiseSettings.resolution; x++) {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < terrainSettings.regions.Length; i++) {
                    if (currentHeight <= terrainSettings.regions[i].height) {
                        colourMap [y * noiseSettings.resolution + x] = terrainSettings.regions[i].colour;
                        break;
                    }
                }
            }
        }
        MapDisplay display = FindObjectOfType<MapDisplay> ();

        if (drawMode == DrawMode.NoiseMap) {
            display.DrawTexture (TextureGenerator.TextureFromHeightMap(noiseMap));
        }
        else if (drawMode == DrawMode.ColourMap) {
            display.DrawTexture (TextureGenerator.TextureFromColourMap(colourMap, noiseSettings.resolution, noiseSettings.resolution));
        }
        else if (drawMode == DrawMode.Mesh) {
            display.DrawMesh(
                MeshGenerator.GenerateTerrainMesh(noiseMap, terrainSettings.heightMultiplier, terrainSettings.heightCurve),
                TextureGenerator.TextureFromColourMap(colourMap, noiseSettings.resolution, noiseSettings.resolution)
            );
        }
    }
}
