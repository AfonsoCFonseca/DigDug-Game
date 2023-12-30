using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    const int WIDTH_MAP_TILES = 14;
    const int HEIGHT_MAP_TILES = 14;

    public const float INITIAL_MAP_X = -35.0f;
    public const float INITIAL_MAP_Y = 30.35f;

    [SerializeField]
    public GameObject tile;
    
    private GameObject[,] mapArray;
    LevelMaps levelMaps;

    void Start()
    {
        levelMaps = GetComponent<LevelMaps>();

        populateMap();
    }

    void populateMap() {
        tileSpawn();
    }

    void tileSpawn() 
    {
        mapArray = new GameObject[HEIGHT_MAP_TILES, WIDTH_MAP_TILES];
        for (int i = 0; i < WIDTH_MAP_TILES; i++)
        {
            for (int j = 0; j < HEIGHT_MAP_TILES; j++)
            {
                float currentX = INITIAL_MAP_X + (j * 5);
                float currentY = INITIAL_MAP_Y - (i * 5);

                Vector3 currentPosition = new Vector3(currentX, currentY, 0);
                mapArray[i, j] = Instantiate(tile, currentPosition, Quaternion.identity);

                (int[] horizontal, int[] vertical) result = readTileFromLevelMap(i,j);
                mapArray[i, j].GetComponent<Tile>().drawInitialSlots(result.vertical,
                    result.horizontal);
            }
        }
    }

    (int[] horizontal, int[] vertical) readTileFromLevelMap(int x, int y) 
    {
        int[] horizontal = new int[] { 0, 0, 0, 0 };
        int[] vertical = new int[] { 0, 0, 0, 0 };

        if(levelMaps.map[x][y] == 1) {
            horizontal = new int[] { 1, 1, 1, 1 };
        }
        if(levelMaps.map[x][y] == 2) {
            vertical = new int[] { 2, 2, 2, 2 };
        }
        if(levelMaps.map[x][y] == 3) {
            horizontal = new int[] { 3, 3, 3, 3 };
        }

        return (horizontal, vertical);
    }
}
