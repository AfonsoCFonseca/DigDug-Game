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
    public Tile tile;

    // [SerializeField]
    // private GameObject pookas;
    // [SerializeField]
    // private GameObject fygars;

    
    private Tile[,] mapArray;
    private LevelMaps levelMaps;
    private GameValues gameValues;

    void Start()
    {
        levelMaps = GetComponent<LevelMaps>();
        gameValues = GetComponent<GameValues>();
        populateMap();
    }

    void populateMap() {
        tileSpawn();
        enemySpawn();
    }

    void tileSpawn() 
    {
        mapArray = new Tile[HEIGHT_MAP_TILES, WIDTH_MAP_TILES];
        for (int i = 0; i < HEIGHT_MAP_TILES; i++)
        {
            for (int j = 0; j < WIDTH_MAP_TILES; j++)
            {
                float currentY = INITIAL_MAP_Y - (i * 5);
                float currentX = INITIAL_MAP_X + (j * 5);

                Vector2 currentPosition = new Vector2(currentX, currentY);
                mapArray[i, j] = Instantiate(tile, currentPosition, Quaternion.identity);

                (int[] horizontal, int[] vertical) result = readTileFromLevelMap(i,j);

                mapArray[i, j].GetComponent<Tile>().Setup(result.vertical, result.horizontal, i, j);
            }
        }
    }

    void enemySpawn()
    {
        for(int i = 0; i < levelMaps.enemyPosition.Count; i++)
        {
            Position pos = levelMaps.enemyPosition[i];
            Tile enemyTile = GetCurrentTileByArrayPosition(pos.x, pos.y);
            Vector2 currentPosition = new Vector2(enemyTile.transform.position.x, enemyTile.transform.position.y);
            Instantiate(levelMaps.enemies[i], currentPosition, Quaternion.identity);
        }
    }

    public (int[] horizontal, int[] vertical) readTileFromLevelMap(int y, int x) 
    {
        int[] horizontal = new int[] { 0, 0, 0, 0 };
        int[] vertical = new int[] { 0, 0, 0, 0 };

        if(levelMaps.map[y][x] == 1) {
            horizontal = new int[] { 1, 1, 1, 1 };
        }
        if(levelMaps.map[y][x] == 2) {
            vertical = new int[] { 2, 2, 2, 2 };
        }
        if(levelMaps.map[y][x] == 3) {
            horizontal = new int[] { 3, 3, 3, 3 };
        }

        return (horizontal, vertical);
    }

    private Tile GetCurrentTileByArrayPosition(int posX, int posY)
    {
        return mapArray[posY, posX];
    }

    public Tile GetCurrentTile(Vector2 position)
    {
        Tile currentTile = null;
        float nearestDistance = float.MaxValue;

        for (int i = 0; i < HEIGHT_MAP_TILES; i++)
        {
            for (int j = 0; j < HEIGHT_MAP_TILES; j++)
            {
                Vector2 tilePosition = mapArray[i, j].transform.position;
                float distance = Vector2.Distance(position, tilePosition);

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    currentTile = mapArray[i, j];
                }
            }
        }
        return currentTile;
    }

    public Tile GetNeighbourTile(Tile currentTile, Direction direction)
    {
        int mapX = currentTile.getMapTilePosition().mapX;
        int mapY = currentTile.getMapTilePosition().mapY;

        if (!isBoardLimit(mapX, mapY, direction)){
            return null;
        }

        switch (direction)
        {
            case Direction.West:
                return mapArray[mapY, mapX - 1];
            case Direction.East:
                return mapArray[mapY, mapX + 1];
            case Direction.North:
                return mapArray[mapY - 1, mapX];
            case Direction.South:
                return mapArray[mapY + 1, mapX];
            default:
                return mapArray[mapX, mapY];
        }
    }

    private Tile FindTileById(string id)
    {
        Tile currentTile = null;
        for (int i = 0; i < WIDTH_MAP_TILES; i++)
        {
            for (int j = 0; j < WIDTH_MAP_TILES; j++)
            {
                if(mapArray[i, j].getId() == id) {
                    currentTile = mapArray[i, j];
                }
            }
        }

        return currentTile;
    }

    //I use this method instead of Tile.isFilled when the board isn't yet drawn but
    //i need to know the state of the future tile
    public bool IsFutureTileEmpty(int tileArrayPosX, int tileArrayPosY, bool isOrientationHorizontal)
    {
        (int[] horizontal, int[] vertical) result = readTileFromLevelMap(tileArrayPosY, tileArrayPosX);
        bool isEmpty = true;
        int[] arrSlots = isOrientationHorizontal ? result.horizontal : result.vertical;
        foreach (int slotNum in arrSlots) {
            if(slotNum == 0)
                isEmpty = false;
        }
        return isEmpty;
    }

    private bool isBoardLimit(int mapX, int mapY, Direction direction)
    {
        if(direction == Direction.West && mapX > 0 ||
            direction == Direction.East && mapX + 1 < mapArray.GetLength(1) )
            {
                return true;
            }
        
         if(direction == Direction.North && mapY > 0 || 
            direction == Direction.South && mapY + 1 < mapArray.GetLength(0))
            {
                return true;
            }

        return false;
    }
}
