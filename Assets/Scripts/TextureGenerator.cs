using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator
{
<<<<<<< HEAD
    public static Texture2D TextureFromColorMap(Color[] colors, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.SetPixels(colors);
        texture.Apply();

        return texture;
    }
    public static Texture2D TextureFromHeightMap(float[,] heightMap)
=======
    public static Texture2D TextureFromColorMap(Color[] colors, int width, int height)
    {
        Texture2D tex = new Texture2D(width, height);
        tex.SetPixels(colors);
        tex.Apply();
        return tex;
    }

    public static Texture2D TextureFromHeightMap(float[,] heightMap)
>>>>>>> dca8136c7c7282d8308c4383d839d35acb8a8a24
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

<<<<<<< HEAD
        Color[] colors = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colors[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
            }
        }

        return TextureFromColorMap(colors, width, height);
=======
        Texture2D texture = new Texture2D(width, height);

        Color[] colorMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
            }
        }

        return TextureFromColorMap(colorMap);
>>>>>>> dca8136c7c7282d8308c4383d839d35acb8a8a24
    }
}
