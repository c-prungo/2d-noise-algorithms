using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class TerrainSettings : ScriptableObject
{

    public bool includeTerrace;

    [Range(1, 50)]
    public float terraceDetail;

    [Range (0, 6)]
    public int levelOfDetail;

    public int heightMultiplier;
    public AnimationCurve heightCurve;
    public TerrainType[] regions;

    [System.Serializable]
    public struct TerrainType {
        public string name;
        public float height;
        public Color colour;
    }
}
