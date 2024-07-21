using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTerrain : MonoBehaviour
{
    [Tooltip("Size of the terrain (creates a square size x size)")]
    public int size = 2000;
    // coordinates of the terrain need to be (-) half of the width and height

    [Tooltip("Height of the terrain")]
    public float depth = 20f;

    [Tooltip("Scale of the noise")]
    public float scale = 20f;

    private float _offsetX;
    private float _offsetY;

    [ContextMenu("Generate Terrain")]
    public void GenerateTerrainMesh()
    {
        _offsetX = Random.Range(0f, 9999f);
        _offsetY = Random.Range(0f, 9999f);

        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrainData(terrain.terrainData);

        // move the terrain to the center
        float halfSize = (float)size / 2;
        terrain.transform.position = new Vector3(-halfSize, 0, -halfSize);
    }

    private TerrainData GenerateTerrainData(TerrainData terrainData)
    {
        terrainData.heightmapResolution = size + 1;

        terrainData.size = new Vector3(size, depth, size);

        terrainData.SetHeights(0, 0, GenerateHeights());

        return terrainData;
    }

    private float[,] GenerateHeights()
    {
        float[,] heights = new float[size, size];
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                heights[x, y] = CalculateHeight(x, y);
            }
        }

        return heights;
    }

    private float CalculateHeight(int x, int y)
    {
        float xCoord = (float)x / size * scale + _offsetX;
        float yCoord = (float)y / size * scale + _offsetY;

        return Mathf.PerlinNoise(xCoord, yCoord);
    }
}
