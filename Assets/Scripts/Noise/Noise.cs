using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public static float[,] GenerateNoiseMap(
        int resolution,
        NoiseSettings noiseSettings
    )
    {

        float[,] noiseMap = new float[resolution, resolution];

        Vector2[] offsets = generateRandOffsets(noiseSettings.seed, noiseSettings.octaves, noiseSettings.offset);

        // noise min and max vals for normalisation
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        // generate the noisemap
        for (int y = 0; y < resolution; y++) {
            for (int x = 0; x < resolution; x++)
            {
                Vector2 coords = new Vector2(x/noiseSettings.scale, y/noiseSettings.scale);
                float noiseHeight = WarpPattern (coords, noiseSettings, offsets);

                // get max and min noiseheight dynamically
                if (noiseHeight > maxNoiseHeight) {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight) {
                    minNoiseHeight = noiseHeight;
                }

                noiseMap [x, y] = noiseHeight;
            }
        }

        // normalise the noisemap
        for (int y = 0; y < resolution; y++) {
            for (int x = 0; x < resolution; x++)
            {
                noiseMap [x, y] = Mathf.InverseLerp (minNoiseHeight, maxNoiseHeight, noiseMap [x, y]);
            }
        }

        return noiseMap;
    }

    static float fbm (
        Vector2 coords,
        int octaves,
        float lacunarity,
        float persistence,
        Vector2[] offsets
    )
    {
        float noiseHeight = 0;
        float frequency = 1;
        float amplitude = 1;

        // apply noise once per octave
        for (int i = 0; i < octaves; i++) {

            float perlinValue = Mathf.PerlinNoise (
                coords.x * frequency + offsets[i].x,
                coords.y * frequency + offsets[i].y
            );

            noiseHeight += perlinValue * amplitude;
            frequency *= lacunarity; // (1+) frequency is increased (more detail)
            amplitude *= persistence; // (0-1) amplitude is reduced (less effect)
        }

        return noiseHeight;
    }

    // fractal pattern from 0 - 2 (two possible fractal settings)
    static float WarpPattern(
        Vector2 coords,
        NoiseSettings noiseSettings,
        Vector2[] offsets
    )
    {

        float noiseHeight;

        if (noiseSettings.applyWarp) {
            
            // generate new vector2 coordinates using offset noise
            Vector2 warpCoords = new Vector2(
                fbm (coords, noiseSettings.octaves, noiseSettings.lacunarity, noiseSettings.persistence, offsets),
                fbm (coords + new Vector2 (5.2f, 1.3f), noiseSettings.octaves, noiseSettings.lacunarity, noiseSettings.persistence, offsets)
            );

            // generate new noiseheight value with offset noise
            noiseHeight = fbm (warpCoords, noiseSettings.octaves, noiseSettings.lacunarity, noiseSettings.persistence, offsets);
        }
        else
        {
            noiseHeight = fbm (coords, noiseSettings.octaves, noiseSettings.lacunarity, noiseSettings.persistence, offsets);
        }

        return noiseHeight;
    }

    static Vector2[] generateRandOffsets(
        int seed,
        int randMax,
        Vector2 offset
    )
    {
        // pseudorandom generation based on seed
        System.Random prng = new System.Random (seed);
        Vector2[] offsets = new Vector2[randMax];
        for (int i = 0; i < randMax; i++) {
            float offsetX = prng.Next (-100000, 100000) + offset.x;
            float offsetY = prng.Next (-100000, 100000) + offset.y;
            offsets[i] = new Vector2 (offsetX, offsetY);
        }
        return offsets;
    }
}
