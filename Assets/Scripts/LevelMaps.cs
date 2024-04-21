using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position
{
    public int x;
    public int y;
}

public class LevelMaps : MonoBehaviour
{
    LevelManager levelManager;
    [SerializeField]
    private GameObject pookas;
    [SerializeField]
    private GameObject fygars;
    [SerializeField]
    private GameObject stone;

    public List<Position> enemyPosition = new List<Position>();
    public List<GameObject> enemies = new List<GameObject>();
    public List<Position> stonesPositions = new List<Position>();
    public List<GameObject> stones = new List<GameObject>();

    public int[][] map;

    private int currentNumOfEnemies = 3;

    private float INCREMENTAL_DIFFICULTY_SPEED = 0.2f;
    private float enemySpeed = 6.0f;

    void Start()
    {
        levelManager = GetComponent<LevelManager>();
        //1 endless horizontal
        //2 endless vertical
        //3 open top horizontal
        //4 closed right horizontal
        //4 closed left horizontal
        //5 closed top vertical
        //6 closed bottom vertical

        map = new int[][]
        {
            new int[] {0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 2, 0, 0, 0, 0, 2, 0, 0, 1, 1, 1, 1, 0},
            new int[] {0, 2, 0, 0, 0, 0, 2, 0, 0, 0, 0, 2, 0, 0},
            new int[] {0, 2, 0, 0, 0, 0, 2, 0, 0, 0, 0, 2, 0, 0},
            new int[] {0, 2, 0, 0, 0, 0, 2, 0, 0, 1, 1, 1, 1, 1},
            new int[] {0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0},
            new int[] {0, 0, 0, 0, 0, 1, 3, 1, 1, 1, 1, 1, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0},
            new int[] {0, 0, 1, 1, 1, 1, 0, 0, 0, 2, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 2, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0}
        };
    }

    public int[][] GetMap()
    {
        return GenerateMap();
    }

    public List<Position> GetEnemiesPosition()
    {
        return enemyPosition;
    }

    public List<GameObject> GetEnemies()
    {
        return enemies;
    }


    public List<Position> GetStonesPosition()
    {
        return stonesPositions;
    }

    public List<GameObject> GetStones()
    {
        return stones;
    }

    public float GetEnemySpeed()
    {
        return enemySpeed;
    }

    public void Restart()
    {
        enemies.Clear();
        enemyPosition.Clear();
        Start();
    }

    private int[][] GenerateMap()
    {
        int level = levelManager.GetLevel();
        if(level % 2 == 0)
        {
            currentNumOfEnemies++;
        }

        int[][] newMap = GetCleanMap();

        List<string> combinations = new List<string>();

        for(int i = 0; i < currentNumOfEnemies; i++)
        {
            bool shouldRepeat;
            int x;
            int y;
            bool isVertical;
            string comb1, comb2, comb3;

            do{
                shouldRepeat = false;

                int rndNum = (int) (Random.Range(0,100));
                isVertical = rndNum % 2 == 0;

                x = (int) Random.Range(isVertical ? 1 : 3, isVertical ? 13 : 11);
                y = (int) Random.Range(!isVertical ? 1 : 3, !isVertical ? 13 :11);

                if(ValidateException(y, x)) shouldRepeat = true;
                if(ValidateException(isVertical ? y + 1 : y, isVertical ? x : x + 1)) shouldRepeat = true;
                if(ValidateException(isVertical ? y + 2 : y, isVertical ? x : x + 2)) shouldRepeat = true;

                comb1 = y.ToString() + x.ToString();
                comb2 = (isVertical ? (y + 1) : y).ToString() + (isVertical ? x : (x + 1)).ToString();
                comb3 = (isVertical ? (y + 2) : y).ToString() + (isVertical ? x : (x + 2)).ToString();

                if(ValidateIfExistsInNewOnes(new string[] { comb1, comb2, comb3 }, combinations)) shouldRepeat = true;
            }
            while(shouldRepeat);

            newMap[y][x] = isVertical ? 2 : 1;
            newMap[isVertical ? y + 1 : y][isVertical ? x : x + 1] = isVertical ? 2 : 1;
            newMap[isVertical ? y + 2 : y][isVertical ? x : x + 2] = isVertical ? 2 : 1;

            combinations.Add(comb1);
            combinations.Add(comb2);
            combinations.Add(comb3);

            AddEnemiesToMap(isVertical ? x : x + 1, isVertical ? y + 1 : y, level);
        }

        AddStonesToMap(combinations);

        return newMap;
    }

    private void AddEnemiesToMap(int x, int y, int level)
    {
        enemyPosition.Add(
            new Position {x = x, y = y}
        );

        int rndNum = (int) (Random.Range(0,100));
        bool isPookas = rndNum % 2 == 0;

        GameObject currentEnemy = isPookas ? pookas : fygars;
        Enemy enemyScript = currentEnemy.GetComponent<Enemy>();
        enemySpeed += INCREMENTAL_DIFFICULTY_SPEED;

        enemies.Add(currentEnemy);
    }

    private void AddStonesToMap(List<string> combinationsEmpty)
    {
        bool shouldRepeat;
        int randoPosX, randoPosY;

        for(int i = 0; i < 4; i++)
        {
            do {
                shouldRepeat = false;
                randoPosX = Random.Range(0, levelManager.WIDTH_MAP_TILES - 1); // -1 to avoid touching the floor
                randoPosY = Random.Range(0, levelManager.HEIGHT_MAP_TILES - 1);

                string comb = randoPosY.ToString() + randoPosX.ToString();
                string comb2 = (randoPosY + 1).ToString() + randoPosX.ToString(); // to check also if the pos bellow is empty

                if(ValidateIfExistsInNewOnes(new string[] { comb, comb2 }, combinationsEmpty)) shouldRepeat = true;
                if(ValidateException(randoPosY, randoPosX)) shouldRepeat = true;
                if(ValidateException(randoPosY + 1, randoPosX)) shouldRepeat = true; // also to check if it isn't empty bellow
            }
            while(shouldRepeat == true);

            stonesPositions.Add(
                new Position {x = randoPosX, y = randoPosY}
            );
            stones.Add(stone);
        }
    }

    private int [][] GetCleanMap()
    {
        return new int[][]{
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
        };
    }

    private bool ValidateException(int y, int x)
    {
        if(y == 6 && (x >= 3 && x <= 10))
        {
            return true;
        }
        return false;
    }

    private bool ValidateIfExistsInNewOnes(string[] combs, List<string> allCombinations)
    {
        foreach(string comb in combs)
        {
            foreach(string comb1 in allCombinations) {
                if (comb == comb1) return true;
            }
        }
        return false;
    }
}