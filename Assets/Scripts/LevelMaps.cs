using UnityEngine;

public class Position
{
    public int x;
    public int y;
}

public class LevelMaps : MonoBehaviour
{

    public Position[] enemyPosition = new Position[1];
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
            new int[] {0, 2, 0, 0, 0, 0, 2, 0, 0, 0, 0, 3, 0, 0},
            new int[] {0, 2, 0, 0, 0, 0, 2, 0, 0, 0, 0, 3, 0, 0},
            new int[] {0, 2, 0, 0, 0, 0, 2, 0, 0, 1, 1, 3, 1, 0},
            new int[] {0, 2, 0, 0, 0, 0, 2, 0, 0, 3, 0, 3, 0, 0},
            new int[] {0, 0, 0, 0, 0, 1, 3, 1, 1, 1, 1, 1, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0},
            new int[] {0, 0, 1, 1, 1, 1, 0, 0, 0, 2, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
        };
        
        enemyPosition[0] = new Position { x = 11, y = 1 };
        // enemyPosition[1] = new Position { x = 1, y = 2 };
        // enemyPosition[2] = new Position { x = 9, y = 9 };
    }
}