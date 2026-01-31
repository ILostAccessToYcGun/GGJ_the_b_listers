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

    [Space]

    [SerializeField] Tilemap wallTilemap;
    [SerializeField] Tilemap floorTilemap;
    [SerializeField] TileBase floorTile;
    [SerializeField] TileBase wallTile;
    [SerializeField] TileBase cloudTile;
    [SerializeField] TileBase seaTile;

    [Space]
    public GameObject player;
    //[SerializeField] Tilemap mapHolder;

    private TileBase[,] noiseGrid;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        noiseGridGeneration();
        cellularAutomation();
        drawMap();
        randPlayerPos();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void noiseGridGeneration()
    {
        noiseGrid = new TileBase[mapHeight, mapWidth];
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
            TileBase[,] tempNoiseGrid = (TileBase[,])noiseGrid.Clone();
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
        //Vector2 tileSize = noiseGrid[0, 0].;
        float yRefection = mapHeight / 2f;
        float xRefection = mapWidth / 2f;
        for (int x = 1; x < mapWidth; x++)
        {
            for (int y = 1; y < mapHeight; y++)
            {
                if (noiseGrid[x, y] == wallTile)
                {
                    wallTilemap.SetTile(new Vector3Int((int)(x - xRefection), (int)(y - yRefection), 0), noiseGrid[x, y]);
                }
                else
                {
                    floorTilemap.SetTile(new Vector3Int((int)(x - xRefection), (int)(y - yRefection), 0), noiseGrid[x, y]);
                }
                //creation = Instantiate(noiseGrid[x, y], new Vector2(x * tileSize.x - xRefection, y * tileSize.y - yRefection), Quaternion.identity);
                //creation.transform.parent = mapHolder.transform;
                //creation.name = noiseGrid[x, y].name;
            }
        }
    }

    void randPlayerPos()
    {
        Vector3Int randomTilePos;
        do
        {
            randomTilePos = new Vector3Int(UnityEngine.Random.Range(-mapWidth / 2, mapWidth / 2), UnityEngine.Random.Range(-mapHeight / 2, mapHeight / 2), 0);
        } while (floorTilemap.GetTile(randomTilePos) == null || wallTilemap.GetTile(randomTilePos));
        player.transform.position = floorTilemap.CellToWorld(randomTilePos);
    }
}
