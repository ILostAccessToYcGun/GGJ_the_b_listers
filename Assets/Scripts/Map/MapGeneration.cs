using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MapGeneration : MonoBehaviour
{
    [SerializeField] int mapHeight;
    [SerializeField] int mapWidth;
    [SerializeField] int density;
    [SerializeField] int iterations;
    [SerializeField] Sprite floorTile;
    [SerializeField] Sprite wallTile;

    private Sprite[,] noiseGrid;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        noiseGridGeneration();
        cellularAutomation();
        drawMap();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void noiseGridGeneration()
    {
        noiseGrid = new Sprite[mapHeight, mapWidth];
        int random = UnityEngine.Random.Range(1, 100);
        for (int i = 0; i < mapHeight; i++)
        {
            for (int j = 0; j < mapWidth; j++)
            {
                if (random > density)
                {
                    noiseGrid[i, j] = floorTile;
                }
                else
                {
                    noiseGrid[i, j] = wallTile;
                }
            }
        }
    }

    void cellularAutomation()
    {
        for (int i = 1; i < iterations; i++)
        {
            Sprite[,] tempNoiseGrid = noiseGrid;
            for (int j = 0; j < mapHeight; j++)
            {
                for (int k = 0; k < mapWidth; k++)
                {
                    int neighborWallCount = 0;
                    for (int y = j - 1; y < j + 1; y++)
                    {
                        for (int x = k - 1; x < k + 1; x++)
                        {
                            if (y >= 1 && x >= 1 && y <= mapHeight && x <= mapWidth)
                            {
                                if (y != j && x != k)
                                {
                                    if (tempNoiseGrid[y, x] == wallTile)
                                    {
                                        neighborWallCount++;
                                    }
                                }
                            }
                            else
                            {
                                neighborWallCount++;//may be obsolete
                            }
                        }
                    }
                    if (neighborWallCount > 4)
                    {
                        noiseGrid[j, k] = wallTile;
                    }
                    else
                    {
                        noiseGrid[j, k] = floorTile;
                    }
                }
            }
        }
    }

    void drawMap()
    {
        for (int x = 1; x < mapHeight; x++)
        {
            for (int y = 1; y < mapWidth; y++)
            {
                noiseGrid.
            }
        }
    }
}
