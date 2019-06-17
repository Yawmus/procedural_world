﻿using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {

<<<<<<< HEAD
    public enum DrawMode
    {
        NoiseMap,
        ColorMap,
        Mesh
    }

    public const int MAP_CHUNK_SIZE = 241;

    [Range(0, 6)]
    public int LOD;

    public DrawMode drawMode;
=======
    public enum DrawMode
    {
        NoiseMap, ColorMap
    }
    public DrawMode drawMode;


    public int mapWidth;
	public int mapHeight;
>>>>>>> dca8136c7c7282d8308c4383d839d35acb8a8a24
	public float noiseScale;

    public int octaves;
    [Range (0, 1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;
    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;

    public bool autoUpdate;

<<<<<<< HEAD
    public TerrainType[] regions;

    Queue<MapThreadInfo<MapData>> mapDataThread = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<MeshData>> meshDataThread = new Queue<MapThreadInfo<MeshData>>();

    public void DrawMapInEditor()
    {
        MapData mapData = GenerateMapData();
        MapDisplay display = FindObjectOfType<MapDisplay>();

        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));
        }
        else if (drawMode == DrawMode.ColorMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColorMap(mapData.colors, MAP_CHUNK_SIZE, MAP_CHUNK_SIZE));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, LOD), TextureGenerator.TextureFromColorMap(mapData.colors, MAP_CHUNK_SIZE, MAP_CHUNK_SIZE));
        }
    }

    void Update()
    {
        if (mapDataThread.Count > 0)
        {
            for (int i = 0; i < mapDataThread.Count; i++)
            {
                MapThreadInfo<MapData> threadInfo = mapDataThread.Dequeue();
                threadInfo.cb(threadInfo.parameter);
            }
        }
        if (meshDataThread.Count > 0)
        {
            for (int i = 0; i < meshDataThread.Count; i++)
            {
                MapThreadInfo<MeshData> threadInfo = meshDataThread.Dequeue();
                threadInfo.cb(threadInfo.parameter);
            }
        }
    }

	 MapData GenerateMapData() {
		float[,] noiseMap = Noise.GenerateNoiseMap (MAP_CHUNK_SIZE, MAP_CHUNK_SIZE, seed, noiseScale, octaves, persistance, lacunarity, offset);

        Color[] colors = new Color[MAP_CHUNK_SIZE * MAP_CHUNK_SIZE];
        for (int y = 0; y < MAP_CHUNK_SIZE; y++)
        {
            for (int x = 0; x < MAP_CHUNK_SIZE; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        colors[y * MAP_CHUNK_SIZE + x] = regions[i].color;
                        break;
                    }
                }
            }
        }

        return new MapData(noiseMap, colors);
=======
    public TerrainType[] regions;

	public void GenerateMap() {
		float[,] noiseMap = Noise.GenerateNoiseMap (mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);

        Color[] colors = new Color[mapWidth * mapHeight];

        for(int y = 0; y < mapHeight; y++)
        {
            for (int x = 0;x  < mapWidth; x++)
            {
                float currentHeight = noiseMap[x, y];
                for(int i=0; i<regions.Length;i++)
                {
                    if(currentHeight <= regions[i].height)
                    {
                        colors[y * mapWidth + x] = regions[i].color;
                        break;
                    }
                }
            }
        }

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if(drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap)); 
        }
        else if(drawMode == DrawMode.ColorMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColorMap(colors, mapWidth, mapHeight));
        }
>>>>>>> dca8136c7c7282d8308c4383d839d35acb8a8a24
	}

    public void RequestMapData(Action<MapData> cb)
    {
        ThreadStart threadStart = delegate
        {
            MapDataThread(cb);
        };
        new Thread(threadStart).Start();
    }

    void MapDataThread(Action<MapData> cb)
    {
        MapData mapData = GenerateMapData();
        lock (mapDataThread) { 
            mapDataThread.Enqueue(new MapThreadInfo<MapData>(cb, mapData));
        }
    }

    public void RequestMeshData(MapData mapData, Action<MeshData> cb)
    {
        ThreadStart threadStart = delegate
        {
            MeshDataThread(mapData, cb);
        };
        new Thread(threadStart).Start();
    }

    void MeshDataThread(MapData mapData, Action<MeshData> cb)
    {
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, LOD);
        lock(meshDataThread)
        {
            meshDataThread.Enqueue(new MapThreadInfo<MeshData>(cb, meshData));
        }
    }

    void OnValidate()
    {
        if(lacunarity < 1)
        {
            lacunarity = 1;
        }
        if(octaves < 0) 
        {
            octaves = 0;
        }
    }

    struct MapThreadInfo<T>
    {
        public readonly Action<T> cb;
        public readonly T parameter;

        public MapThreadInfo(Action<T> cb, T parameter)
        {
            this.cb = cb;
            this.parameter = parameter;
        }
    }

}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}

public struct MapData
{
    public readonly float[,] heightMap;
    public readonly Color[] colors;

    public MapData(float[,] heightMap, Color[] colors)
    {
        this.heightMap = heightMap;
        this.colors = colors;
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}