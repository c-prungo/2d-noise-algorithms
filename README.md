# 2d-noise-algorithms

Using 2d noise samples to create interesting patterns, heightmaps, and landmasses.  
![offset-video](https://user-images.githubusercontent.com/97237166/151536462-17297341-adfc-4886-a9e3-0a1a16e32035.gif)

## Generating Noise

The term **noise** simply means generating a random value *between 0 and 1* for a given point on a *2D-plane*.

---

### Perlin Noise

[Perlin Noise](https://en.wikipedia.org/wiki/Perlin_noise) is a form of coherent noise, meaning instead if purely random values, it generates values that flow in a smooth appearance, see below.

![basic-perlin-noise](https://user-images.githubusercontent.com/97237166/151536648-66071baf-c911-4577-b979-6e45e778c885.png)

>*Perlin noise was chosen simply because it is included as a standard Unity library*

&nbsp;

### Fractional Brownian Motion

This lends itself to the generation of a basic form of [Fractional Brownian Motion](https://en.wikipedia.org/wiki/Fractional_Brownian_motion).  
The code for which seen below:

```c#
float fbm ()
{
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
}
```

>An excellent tutorial for this effect can be found [here](https://www.iquilezles.org/www/articles/fbm/fbm.htm) - credit to [Inigo Quilez](https://www.iquilezles.org/)

---

A basic explanation is as follows:

- the **octaves** variable indicates how many layers of noise are used
- the **lacunarity** variable indicates by how much the frequency is *increased* each octave (*although technically if this is any value other than 2, the term octaves is no longer accurate*)
- the **persistence** variable indicates by how much the amplitude is *decreased* each octave (*measured multiplicitavely from 0 - 1*)

---

This describes the simple process of combining several layers of noise, each with differing levels of detail and effect.  
![image](https://user-images.githubusercontent.com/97237166/151534717-c41cc137-25b5-4e3f-809f-e87a57354128.png)
*credit to [Sebastian Lague's](https://www.youtube.com/channel/UCmtyQOKKmrMVaKuRXz02jbQ) video on procedural planets:
[Coding Adventure: Procedural Moons and Planets](https://youtu.be/lctXaT9pxA0?t=513)*

The result of this work gives us this noise-map, which we will use for almost everything going forward.  
![fractal-brownian-motion](https://user-images.githubusercontent.com/97237166/151536714-e6871769-492f-40fd-9d33-f29d3f8b894c.png)

### Noise Settings

See below the noise settings used, and a brief description of each.

| Setting | Description |
| ------- | ----------- |
| Seed | a random offset for noise generation
| Scale | dividing the x and y location of noise data to increase the size of the noise
| Octaves | number of noise layers*
| Lacunarity | scaling factor for increasing frequency of the noise per octave*
| Persistence | scaling factor for decreasing amplitude of the noise per octave*
| Offset | Vector2 for changing the x and y coordinates used when sampling the noise-map
| Warp Octaves | special feature, see below for further details

>\* see *fractional brownian motion* for more details

### Noise Warping

**FBM** or Fractional Brownian Motion can be used to offset itself. This is the building block of noise warp.

Take the FBM function we built above, when we call it once, we can generate a simple noise pattern:

```c#
float WarpPattern (Vector2 coordinates)
{
    return fbm (coordinates);
}
```

![warped-fbm-0](https://user-images.githubusercontent.com/97237166/151548155-5be47cb8-abe5-48e8-bb04-c9efae78eddf.png)

> f(p) = fbm (p);

However, when we add domain warping, by applying an offset to the fbm call equal to a different fbm call for each direction (x, y), something interesting occurs.

```c#
float WarpPattern (Vector2 coordinates)
{
    Vector2 warpedCoordinates = Vector2 (
        fbm (coordinates + offset1),
        fbm (coordinates + offset2)
    ); // where offset1 and offset2 are small but arbitrary Vector2 offsets (maybe around 0 - 10)

    return fbm (coordinates + scale * warpedCoordinates);
    // where scale is a small but arbitrary scaling factor (maybe around 3 - 4)
}
```

![warped-fbm-1](https://user-images.githubusercontent.com/97237166/151547736-800a4d11-abc1-4251-9461-7a04a9c9411d.png)

> f(p) = fbm (p + fbm (p));

This can create some beautiful fractal style patterns.  

But it can be taken further.

```c#
float WarpPattern (Vector2 coordinates)
{

    // offsets are small but arbitrary Vector2 increases (0 - 10 for x and y)
    // scale is small but arbitrary scaling factor (3 - 4)

    Vector2 warpedCoordinates = Vector2 (
        fbm( coordinates + offset1 ),
        fbm( coordinates + offset2 ) );

    Vector2 warpedCoordinates = Vector2 (
        fbm (coordinates + scale * warpedCoordinates + offset3),
        fbm (coordinates + scale * warpedCoordinates + offset4)
    );

    return fbm (coordinates + scale * warpedCoordinates);
}
```

![warped-fbm-2](https://user-images.githubusercontent.com/97237166/151547685-f5d5852e-718f-42be-aaca-6913c411c9c5.png)

> f(p) = fbm (p + fbm (p + fbm (p)));

This can generate some very interesting landmasses, which we can be seen further down.  
*Although I have limited them to one level of warp, as it has diminishing returns for landmasses*
