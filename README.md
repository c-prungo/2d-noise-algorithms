# 2d-noise-algorithms

Using 2d noise samples to create interesting patterns, heightmaps, and landmasses.  

https://user-images.githubusercontent.com/97237166/151533699-0b50b575-f238-45e1-bbc8-82766eafe615.mp4

## Generating Noise

[Perlin Noise](https://en.wikipedia.org/wiki/Perlin_noise) is a form of coherent noise, meaning instead if purely random values, it generates values between 0 and 1 that flow in a smooth appearance, see below.

![basic-perlin-noise](https://user-images.githubusercontent.com/97237166/151533348-651dffc2-13bb-4ab4-8f7a-de3a6bc3a736.png)


This lends itself to the generation of a basic form of [Fractional Brownian Motion](https://en.wikipedia.org/wiki/Fractional_Brownian_motion).  
This is generated using the following code snippet:

```c#
float noiseHeight = 0;
float frequency = 1;
float amplitude = 1;

// apply noise once per octave
for (int i = 0; i < octaves; i++) {

    float perlinValue = Mathf.PerlinNoise (
        coords.x * frequency,
        coords.y * frequency
    );

    noiseHeight += perlinValue * amplitude;
    frequency *= lacunarity; // (1+) frequency is increased (more detail)
    amplitude *= persistence; // (0-1) amplitude is reduced (less effect)
}

return noiseHeight;
```
An excellent tutorial for this effect can be found [here](https://www.iquilezles.org/www/articles/fbm/fbm.htm) - credit to [Inigo Quilez](https://www.iquilezles.org/)

A basic explanation is as follows:

 - the **octaves** variable indicates how many layers of noise are used
 - the **lacunarity** variable indicates by how much the frequency is *increased* each octave (*although technically if this is any value other than 2, the term octaves is no longer accurate*)
 - the **persistence** variable indicates by how much the amplitude is *decreased* each octave (*measured multiplicitavely from 0 - 1*)

This describes the simple process of combining several layers of noise, each with differing levels of detail and effect.  
![image](https://user-images.githubusercontent.com/97237166/151534717-c41cc137-25b5-4e3f-809f-e87a57354128.png)
*credit to [Sebastian Lague's](https://www.youtube.com/channel/UCmtyQOKKmrMVaKuRXz02jbQ) video on procedural planets:
[Coding Adventure: Procedural Moons and Planets](https://youtu.be/lctXaT9pxA0?t=513)*

The result of this work gives us this noise-map, which we will use for almost everything going forward.  
![fractal-brownian-motion](https://user-images.githubusercontent.com/97237166/151535089-dff39e66-272a-4289-9a1d-7f780d0a148c.png)

