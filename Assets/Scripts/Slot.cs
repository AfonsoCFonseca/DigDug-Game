using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    const float SLOT_WIDTH = 1;
    const float SLOT_HEIGHT = 5.05f;

    [SerializeField] private Sprite normalSlot;
    [SerializeField] private Sprite endSlot;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public int getCurrentTileX() 
    {
        return (int) ((transform.position.x - LevelManager.INITIAL_MAP_X) / SLOT_WIDTH);
    }

     public int getCurrentTileY() 
    {
        return Mathf.Abs((int) ((transform.position.y - LevelManager.INITIAL_MAP_Y) / SLOT_HEIGHT));
    }

    public void SetToDigged()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = true;
        BoxCollider2D boxCollider2D = GetComponent<BoxCollider2D>();
        boxCollider2D.enabled = false;
    }

    public void SwitchToEndSlot(bool isLeftSide)
    {
        if(isLeftSide)
        {
            Vector3 scale = spriteRenderer.transform.localScale;
            scale.x *= -1;
            spriteRenderer.transform.localScale = scale;
        }
        spriteRenderer.sprite = endSlot;
    }
}
