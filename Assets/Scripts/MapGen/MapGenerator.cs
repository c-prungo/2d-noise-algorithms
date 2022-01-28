using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    public enum DrawMode {NoiseMap, ColourMap, Mesh};
    public DrawMode drawMode;
    public bool autoUpdate;

    public int colourDetail = 100;

    public NoiseSettings noiseSettings;
    public TerrainSettings terrainSettings;

    const int mapChunkSize = 241;

    [HideInInspector]
    public bool noiseSettingsFoldout;

    [HideInInspector]
    public bool terrainSettingsFoldout;

    public void GenerateMap() {
        float[,] noiseMap = Noise.GenerateNoiseMap(
            mapChunkSize,
            noiseSettings
        );

        Color[] colourMap = new Color [mapChunkSize * mapChunkSize];
        Color[] colourTable = new Color [colourDetail];
        int terrainIndex = 0;

        // get a height list
        float[] heightList = new float [terrainSettings.regions.Length];
        for (int i = 0; i < terrainSettings.regions.Length; i++) {
            heightList[i] = terrainSettings.regions[i].height * (colourDetail - 1);
        }
        for (int i = 0; i < colourDetail; i++) {

            // ensure current colour index is within bounds
            if (i > heightList[terrainIndex] && terrainIndex < terrainSettings.regions.Length - 1) {
                terrainIndex++;
            }

            // only get below colour if there is one (not at index 0)
            int belowIndex = 0;
            if (terrainIndex > 0) {
                belowIndex = terrainIndex - 1;
            }

            float percentMod = Mathf.InverseLerp (heightList[belowIndex], heightList[terrainIndex], i);

            // blend the colour
            Color lerpedColor = Color.Lerp(terrainSettings.regions[belowIndex].colour, terrainSettings.regions[terrainIndex].colour, percentMod);

            // add the colour to colourTable
            colourTable[i] = lerpedColor;

            // print ("i=" + i);
            // print ("belowIndex=" + belowIndex);
            // print ("terrainIndex=" + terrainIndex);
            // print ("percentMod=" + percentMod);

        }

        // apply the colourTable to the colourMap
        for (int y = 0; y < mapChunkSize; y++) {
            for (int x = 0; x < mapChunkSize; x++) {
                float currentHeight = noiseMap[x, y] * (float)(colourDetail - 1);
                int colourIndex = y * mapChunkSize + x;
                // print("currentHeight=" + (int)currentHeight);
                colourMap [colourIndex] = colourTable[(int)currentHeight];
            }
        }
        MapDisplay display = FindObjectOfType<MapDisplay> ();

        if (drawMode == DrawMode.NoiseMap) {
            display.DrawTexture (TextureGenerator.TextureFromHeightMap(noiseMap));
        }
        else if (drawMode == DrawMode.ColourMap) {
            display.DrawTexture (TextureGenerator.TextureFromColourMap(colourMap, mapChunkSize, mapChunkSize));
        }
        else if (drawMode == DrawMode.Mesh) {
            display.DrawMesh(
                MeshGenerator.GenerateTerrainMesh(
                    noiseMap,
                    terrainSettings.heightMultiplier,
                    terrainSettings.heightCurve,
                    terrainSettings.includeTerrace,
                    terrainSettings.terraceDetail,
                    terrainSettings.levelOfDetail
                ),
                TextureGenerator.TextureFromColourMap(colourMap, mapChunkSize, mapChunkSize)
            );
        }
    }
}
