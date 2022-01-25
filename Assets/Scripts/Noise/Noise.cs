using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public static float[,] GenerateNoiseMap(
        int resolution,
        int seed,
        float scale,
        int octaves,
        float lacunarity,
        float persistence,
        Vector2 offset,
        int warp
    )
    {

        // pseudorandom generation based on seed
        System.Random prng = new System.Random (seed);
        int randMax = Mathf.Max(octaves, warp);
        Vector2[] offsets = new Vector2[randMax];
        for (int i = 0; i < randMax; i++) {
            float offsetX = prng.Next (-100000, 100000) + offset.x;
            float offsetY = prng.Next (-100000, 100000) + offset.y;
            offsets[i] = new Vector2 (offsetX, offsetY);
        }

        float[] offsetList = {0f, 0f, 5,2f, 1.3f, 1.7f, 9.3f, 8.3f, 2.8f};
        Vector2[] warpOffsets = new Vector2[warp * 2];
        for (int i = 0; i < warp * 2; i++) {
            Vector2 newVector = new Vector2(offsetList[i*2], offsetList[i*2+1]);
            warpOffsets[i] = newVector;
        }

        float[,] noiseMap = CreateNoiseMap(resolution, scale, octaves, lacunarity, persistence, offsets, warp, warpOffsets);

        return noiseMap;
    }

    static float[,] CreateNoiseMap(
        int resolution,
        float scale,
        int octaves,
        float lacunarity,
        float persistence,
        Vector2[] offsets,
        int warp,
        Vector2[] warpOffsets
    )
    {
        float[,] noiseMap = new float[resolution, resolution];

        // noise min and max vals for normalisation
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        // generate the noisemap
        for (int y = 0; y < resolution; y++) {
            for (int x = 0; x < resolution; x++)
            {
                Vector2 coords = new Vector2(x/scale, y/scale);
                // float noiseHeight = fbm (coords, octaves, lacunarity, persistence, offsets);
                float noiseHeight = WarpPattern (coords, octaves, lacunarity, persistence, offsets, warp, warpOffsets);

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
        int octaves,
        float lacunarity,
        float persistence,
        Vector2[] offsets,
        int warp,
        Vector2[] warpOffsets
    )
    {

        Vector2 previousWarp = new Vector2(0, 0);

        // only executes if warp > 0
        for (int i = 0; i < warp; i++) {
            Vector2 currentWarp = new Vector2(
                fbm (coords + (4f * previousWarp) + warpOffsets[i*2], octaves, lacunarity, persistence, offsets),
                fbm (coords + (4f * previousWarp) + warpOffsets[i*2+1], octaves, lacunarity, persistence, offsets)
            );
            previousWarp = currentWarp;

        }

        float noiseHeight = 0f;
        if (warp > 0) {
            noiseHeight = fbm (coords + 4*previousWarp, octaves, lacunarity, persistence, offsets);
        }
        else {
            noiseHeight = fbm (coords, octaves, lacunarity, persistence, offsets);
        }
        return noiseHeight;
    }
}
