using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class TerrainSettings : ScriptableObject
{

    public TerrainType[] regions;

    [System.Serializable]
    public struct TerrainType {
        public string name;
        public float height;
        public Color colour;
    }
}
