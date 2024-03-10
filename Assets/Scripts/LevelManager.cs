using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public int WIDTH_MAP_TILES = 14;
    public int HEIGHT_MAP_TILES = 14;

    public const float INITIAL_MAP_X = -35.0f;
    public const float INITIAL_MAP_Y = 30.35f;

    [SerializeField]
    public Tile tile;

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

                int[] tileSlotsState = readTileFromLevelMap(i, j);

                mapArray[i, j].GetComponent<Tile>().Setup(tileSlotsState, i, j);

                if(i == 1 && j == 11) {
                    mapArray[i, j].GetSlot(3, true).SwitchToEndSlot(false);
                }
                if(i == 4 && j == 11) {
                    mapArray[i, j].GetSlot(0, true).SwitchToEndSlot(true);
                    mapArray[i, j].GetSlot(3, true).SwitchToEndSlot(false);
                }
                if(i == 6 && j == 11) {
                    mapArray[i, j].GetSlot(0, true).SwitchToEndSlot(true);
                }
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

    public int[] readTileFromLevelMap(int y, int x) 
    {
        int[] axisArrayState = new int[] { 0, 0, 0, 0 };

        int slotState = levelMaps.map[y][x];

        if(slotState == 1 || slotState == 2 || slotState == 3) {
            axisArrayState = new int[] { slotState, slotState, slotState, slotState };
        }

        return axisArrayState;
    }

    public Tile GetCurrentTileByArrayPosition(int posX, int posY)
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

    public Tile GetNeighbourTile(Tile tCurrentTile, Direction direction)
    {
        int mapX = tCurrentTile.getMapTilePosition().mapX;
        int mapY = tCurrentTile.getMapTilePosition().mapY;

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
    public bool IsFutureTileEmpty(int tileArrayPosX, int tileArrayPosY)
    {
        int[] tileSlotsState = readTileFromLevelMap(tileArrayPosY, tileArrayPosX);
        for(int i = 0; i < tileSlotsState.Length; i++)
        {
            if(tileSlotsState[i] == 0)
                return false;
        }
        return true;
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

    public void UpdateTileState(Tile currentTile)
    {
        int mapX = currentTile.getMapTilePosition().mapX;
        int mapY = currentTile.getMapTilePosition().mapY;
        mapArray[mapY, mapX] = currentTile;
    }
}
