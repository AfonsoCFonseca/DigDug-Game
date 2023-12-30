using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public GameObject gm;
    const float SLOT_WIDTH = 1;
    const float SLOT_HEIGHT = 5.05f;

    void Start()
    {
        gm = GameObject.FindGameObjectsWithTag("GameManager")[0];
    }

    public int getCurrentTileX() 
    {
        return (int) ((transform.position.x - LevelManager.INITIAL_MAP_X) / SLOT_WIDTH);
    }

     public int getCurrentTileY() 
    {
        return Mathf.Abs((int) ((transform.position.y - LevelManager.INITIAL_MAP_Y) / SLOT_HEIGHT));
    }

    public int getCurrentSlotIndex() 
    {
        
        return 2;
    }
}
