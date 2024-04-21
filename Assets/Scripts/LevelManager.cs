using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public int WIDTH_MAP_TILES = 14;
    public int HEIGHT_MAP_TILES = 14;

    public const float INITIAL_MAP_X = -35.0f;
    public const float INITIAL_MAP_Y = 30.35f;

    private int level = 1;

    [SerializeField]
    public Tile tile;

    private Tile[,] mapArray;
    private List<GameObject> enemyGameObjects = new List<GameObject>();
    private LevelMaps levelMaps;
    private int[][] currentLevelMap;
    private GameValues gameValues;
    private UI ui;
    [SerializeField] private PlayerController playerController;

    void Start()
    {
        levelMaps = GetComponent<LevelMaps>();
        gameValues = GetComponent<GameValues>();
        ui = GetComponent<UI>();

        populateMap();
    }

    void populateMap() {
        tileSpawn();
        enemySpawn();
    }

    void tileSpawn() 
    {
        mapArray = new Tile[HEIGHT_MAP_TILES, WIDTH_MAP_TILES];
        currentLevelMap = levelMaps.GetMap();
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
            }
        }
    }

    void enemySpawn()
    {
        enemyGameObjects = new List<GameObject>();
        List<Position> enemiesPosition = levelMaps.GetEnemiesPosition();
        for(int i = 0; i < enemiesPosition.Count; i++)
        {
            Position pos = enemiesPosition[i];
            Tile enemyTile = GetCurrentTileByArrayPosition(pos.x, pos.y);
            Vector2 currentPosition = new Vector2(enemyTile.transform.position.x, enemyTile.transform.position.y);
            enemyGameObjects.Add(Instantiate(levelMaps.GetEnemies()[i], currentPosition, Quaternion.identity));
        }
    }

    public int[] readTileFromLevelMap(int y, int x) 
    {
        int[] axisArrayState = new int[] { 0, 0, 0, 0 };

        int slotState = currentLevelMap[y][x];

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

    public void RestartGame(bool isGameOver)
    {
        for (int i = 0; i < enemyGameObjects.Count; i++)
        {
            if(enemyGameObjects[i] != null)
            {
                enemyGameObjects[i].GetComponent<Enemy>().Restart();
            }
        }
        playerController.RestartPlayer();
        ui.LooseLife();
    }

    public List<GameObject> GetAllEnemiesInLevel()
    {
        return enemyGameObjects;
    }

    public void ChangeLevel()
    {
        level++;
        ui.NextRoundLevelUpdate(level);
        playerController.RestartPlayer();
        ClearMap();
        ClearEnemies();
        levelMaps.Restart();
        populateMap();
    }

    private void ClearMap()
    {
        for (int i = 0; i < HEIGHT_MAP_TILES; i++)
        {
            for (int j = 0; j < WIDTH_MAP_TILES; j++)
            {
                Destroy(mapArray[i, j].gameObject);
                mapArray[i, j] = null;
            }
        }
    }

    private void ClearEnemies()
    {
        for (int i = 0; i < enemyGameObjects.Count; i++)
        {
            Destroy(enemyGameObjects[i].gameObject);
            enemyGameObjects[i] = null;
        }
    }

    public int GetLevel()
    {
        return level;
    }
}
