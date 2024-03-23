using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Position
{
    public int x;
    public int y;
}

public class LevelMaps : MonoBehaviour
{
    // LevelManager levelManager
    [SerializeField]
    private GameObject pookas;
    [SerializeField]
    private GameObject fygars;

    public List<Position> enemyPosition = new List<Position>();
    public List<GameObject> enemies = new List<GameObject>();
    public int[][] map;

    void Start()
    {
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

    public void Restart()
    {
        enemies.Clear();
        enemyPosition.Clear();
        Start();
    }
}