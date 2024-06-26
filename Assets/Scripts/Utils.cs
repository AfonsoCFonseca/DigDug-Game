using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    int currentColorDebugIndex = 0;

    public Direction GetOppositeDirection(Direction dir) {
        switch (dir)
        {
            case Direction.North:
                return Direction.South;
            case Direction.East:
                return Direction.West;
            case Direction.South:
                return Direction.North;
            case Direction.West:
                return Direction.East;
            default:
                return Direction.North;
        }
    }

    public bool IsVerticalAxis (Direction currentDirection)
    {
        return currentDirection == Direction.North || currentDirection == Direction.South;
    }

    public void DrawRandColorDebug(Tile ct)
    {
        string[] allDebugColors = new string[4] {"default", "target", "debug", "debug1"};
        if(currentColorDebugIndex >= 4) currentColorDebugIndex = 0;
        ct.setDebugToColor(allDebugColors[currentColorDebugIndex++]);
    }

    public void clearDebugBlocksFromMap(LevelManager lm)
    {
        for(int i = 0; i < 14; i++)
        {
            for(int j = 0; j < 14; j++) {
                lm.GetCurrentTileByArrayPosition(i, j).setDebugToColor("off");
            }
        }
    }

    public int GetRandomValueBetweenNumbers(int min, int max)
    {
        return Random.Range(min, max);
    }  
}
