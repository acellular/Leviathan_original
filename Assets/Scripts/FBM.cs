using System;
using UnityEngine;

//Modeled after https://www.iquilezles.org/www/articles/fbm/fbm.htm
//https://thebookofshaders.com/13/?fbclid=IwAR0A74lVJwhnypJnN-mQvXm9pFKcMV4tedFRrzdeGi08qVpRoSL6tP0XORc
//and https://gamedevacademy.org/complete-guide-to-procedural-level-generation-in-unity-part-1/
static class FBM 
{

    //for later use creating heightmaps and variable climate and soil zones
    public static float[,] FractalNoiseMap(int mapDepth, int mapWidth, float scale, float offsetX, float offsetZ,
                                        float lacunarity, float H, float frequency, float amplitude, int octaves)
    {
        // create an empty noise map with the mapDepth and mapWidth coordinates
        float[,] noiseMap = new float[mapDepth, mapWidth];

        for (int x = 0; x < mapDepth; x++)
        {
            for (int y = 0; y < mapWidth; y++)
            {
                // calculate sample indices based on the coordinates, the scale and the offset
                float sampleX = (x + offsetX) / scale;
                float sampleY = (y + offsetZ) / scale;

                //create gain from the Hurst exponent!-->lower the hurst the more volatile it becomes, when H = 1, G = .5, when H = 1/2, G = .7
                float gain = (float)Math.Pow(2, -H);
                //calculate noise for this value
                noiseMap[x, y] = Noise(sampleX, sampleY, lacunarity, gain, frequency, amplitude, octaves, true);
            }
        }
        return noiseMap;

    }

    //for 2d FMB noise--can use for 1d noise by only using one y or x (as seen in simplistic climate model via yield multiplier in this model)
    public static float Noise(float x, float y, float lacunarity, float gain, float frequency, float amplitude, int octaves, bool normed)
    {
        float normalization = 0;
        float noise = 0f;

        for (int i = 0; i < octaves; i++)
        {
            // generate noise value using PerlinNoise for a given wave
            noise += amplitude * Mathf.PerlinNoise(x * frequency, y * frequency);
            if (normed) { normalization += amplitude; }
            frequency *= lacunarity;
            amplitude *= gain;
        }

        //normalize the noise value within 0 and 1
        if (normed) { noise /= normalization; }
        return noise;
    }
}