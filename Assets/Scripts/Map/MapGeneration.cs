using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MapGeneration : MonoBehaviour
{
    [SerializeField] int mapHeight;
    [SerializeField] int mapWidth;
    [SerializeField] int density;
    [SerializeField] int iterations;
    [SerializeField] GameObject floorTile;
    [SerializeField] GameObject wallTile;
    [SerializeField] GameObject cloudTile;
    [SerializeField] GameObject seaTile;
    [SerializeField] GameObject mapHolder;

    private GameObject[,] noiseGrid;
    private GameObject creation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
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
        noiseGrid = new GameObject[mapHeight, mapWidth];
        for (int i = 0; i < mapHeight; i++)
        {
            for (int j = 0; j < mapWidth; j++)
            {
                int random = UnityEngine.Random.Range(1, 100);
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

    void cellularAutomation()// ciling will be clouds that can be passed, bottom will be sea (inta death), sides will be infite by have warning(no need for me to do warning)
    {
        for (int i = 1; i < iterations; i++)
        {
            GameObject[,] tempNoiseGrid = (GameObject[,])noiseGrid.Clone();
            for (int j = 1; j < mapHeight; j++)
            {
                for (int k = 1; k < mapWidth; k++)
                {
                    int neighborWallCount = 0;
                    for (int y = j - 1; y <= j + 1; y++)
                    {
                        for (int x = k - 1; x <= k + 1; x++)
                        {
                            if (y >= 0 && x >= 0 && y < mapHeight && x < mapWidth)//later (y >= 0 && x >= 1 && y <= mapHeight && x <= mapWidth - 2)
                            {
                                if (y != j || x != k)
                                {
                                    if (tempNoiseGrid[y, x] == wallTile)
                                    {
                                        neighborWallCount += 1;
                                    }
                                }
                            }
                            else
                            {
                                neighborWallCount += 1;
                            }
                        }
                    }
                    if (neighborWallCount > 4)
                    {
                        noiseGrid[j, k] = wallTile;
                    }
                    else
                    {
                        //UnityEngine.Debug.Log(neighborWallCount);
                        noiseGrid[j, k] = floorTile;
                    }
                }
            }
        }
    }

    void drawMap()
    {
        Vector2 tileSize = noiseGrid[0, 0].GetComponent<SpriteRenderer>().bounds.size;
        for (int x = 1; x < mapHeight; x++)
        {
            for (int y = 1; y < mapWidth; y++)
            {
                creation = Instantiate(noiseGrid[x, y], new Vector2(x * tileSize.x, y * tileSize.y), Quaternion.identity);
                creation.transform.parent = mapHolder.transform;
                creation.name = noiseGrid[x, y].name;
            }
        }
    }
}
