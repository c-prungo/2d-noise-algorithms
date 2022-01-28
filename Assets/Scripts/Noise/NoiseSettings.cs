using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class NoiseSettings : ScriptableObject
{
    [Range (-1000, 1000)]
    public int seed;
    [Range (1, 1000)]
    public float scale = 10;
    [Range (1, 10)]
    public int octaves = 1;
    [Range (1, 4)]
    public float lacunarity = 2f;
    [Range (0, 1)]
    public float persistence = 0.5f;
    public Vector2 offset;
    [Range (0, 1)]
    public int warpOctaves;
}
