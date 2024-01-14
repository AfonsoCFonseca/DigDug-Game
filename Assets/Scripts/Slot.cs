using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    const float SLOT_WIDTH = 1;
    const float SLOT_HEIGHT = 5.05f;

    public int getCurrentTileX() 
    {
        return (int) ((transform.position.x - LevelManager.INITIAL_MAP_X) / SLOT_WIDTH);
    }

     public int getCurrentTileY() 
    {
        return Mathf.Abs((int) ((transform.position.y - LevelManager.INITIAL_MAP_Y) / SLOT_HEIGHT));
    }

    public void setToDigged()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = true;
        BoxCollider2D boxCollider2D = GetComponent<BoxCollider2D>();
        boxCollider2D.enabled = false;
    }
}
