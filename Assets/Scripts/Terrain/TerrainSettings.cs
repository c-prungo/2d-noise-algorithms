using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class TerrainSettings : ScriptableObject
{

    public TerrainType[] regions;
    public int heightMultiplier;
    public AnimationCurve heightCurve;

    [System.Serializable]
    public struct TerrainType {
        public string name;
        public float height;
        public Color colour;
    }
}
