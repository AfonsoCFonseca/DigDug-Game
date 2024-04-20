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

    public List<Position> enemyPosition = new List<Position>();
    public List<GameObject> enemies = new List<GameObject>();
    public int[][] map;

    private int currentNumOfEnemies = 3;

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
        
        enemyPosition.Add(new Position { x = 6, y = 2 });
        enemyPosition.Add(new Position { x = 1, y = 2 });
        enemyPosition.Add(new Position { x = 11, y = 2 });

        enemies.Add(pookas);
        enemies.Add(pookas);
        enemies.Add(fygars);
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
            Debug.Log("new enemy");
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
        }

        return newMap;
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