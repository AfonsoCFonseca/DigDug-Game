using UnityEngine;

public class LevelMaps : MonoBehaviour
{
    //1 endless horizontal
    //2 endless vertical
    //3 open top horizontal
    //4 closed right horizontal
    //4 closed left horizontal
    //5 closed top vertical
    //6 closed bottom vertical

    public int[][] map = new int[][]
    {
        new int[] {0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0},
        new int[] {0, 2, 0, 0, 0, 0, 2, 0, 0, 1, 1, 1, 1, 0},
        new int[] {0, 2, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0},
        new int[] {0, 2, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0},
        new int[] {0, 2, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0},
        new int[] {0, 2, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0},
        new int[] {0, 0, 0, 0, 0, 1, 3, 1, 0, 0, 0, 0, 0, 0},
        new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
        new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0},
        new int[] {0, 0, 1, 1, 1, 1, 0, 0, 0, 2, 0, 0, 0, 0},
        new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0},
        new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0},
        new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0},
        new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
    };
}