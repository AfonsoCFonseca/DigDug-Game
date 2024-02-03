using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    const int TOTAL_SLOTS = 4;
    string id = "";
    int tileArrayPosX = 0;
    int tileArrayPosY = 0;

    int[] verticalSlots = new int [TOTAL_SLOTS] { 0, 0, 0, 0 };
    int[] horizontalSlots = new int [TOTAL_SLOTS] { 0, 0, 0, 0 };

    //Debugging tool
    private Transform debugFlag;
    private Renderer debugFlagRenderer;
    private const string DEBUG_OBJ_NAME = "debug_flag";
    public Color targetColor = Color.green;
    public Color debugColor = Color.blue;
    public Color debugColor1 = Color.yellow;
    public Color defaultColor = Color.red;


    void Start()
    {
        id = GenerateID(10);
        debugFlag = transform.Find(DEBUG_OBJ_NAME);
        debugFlagRenderer = debugFlag.GetComponent<Renderer>();
        debugFlagRenderer.material.color = defaultColor;
    }

    public void Setup(int[] vertSlots, int[] horiSlots, int mapY, int mapX)
    {
        DrawInitialSlots(vertSlots, horiSlots);
        tileArrayPosY = mapY;
        tileArrayPosX = mapX;
    }

    public string getId()
    {
        return id;
    }

    public (int mapX, int mapY) getMapTilePosition()
    {
        return (tileArrayPosX, tileArrayPosY);
    }

    public void DrawInitialSlots(int[] vertSlots, int[] horiSlots)
    {
        verticalSlots = vertSlots;
        horizontalSlots = horiSlots;

        updateTileTextures();
    }

    public void setDebugToColor(string type)
    {
        if(type == "default") debugFlagRenderer.material.color = defaultColor;
        if(type == "target") debugFlagRenderer.material.color = targetColor;
        if(type == "debug") debugFlagRenderer.material.color = debugColor;
        if(type == "debug1") debugFlagRenderer.material.color = debugColor1;
    }

    void updateTileTextures() 
    {
        for(int i = 0; i < TOTAL_SLOTS; i++)
        {
            if(verticalSlots[i] == 2) 
            {
                updateChildren("v_", i);
            }

            if(horizontalSlots[i] == 1 || horizontalSlots[i] == 3) 
            {
                updateChildren("h_", i);
            } 
        }

        if(horizontalSlots[0] == 3)
        {
            updateChildren("v_", 0);
        }
    }

    void updateChildren(string ob_child_name, int pos)
    {
        Transform childTransform = this.transform.Find(ob_child_name + pos);
        if(childTransform) 
        {
            SpriteRenderer spriteRenderer = childTransform.GetComponent<SpriteRenderer>();
            spriteRenderer.enabled = true;
            BoxCollider2D boxCollider2D = childTransform.GetComponent<BoxCollider2D>();
            boxCollider2D.enabled = false;
        }
    }

    private string GenerateID(int length)
    {
        System.Random random = new System.Random();  // Declare 'random' here
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        System.Text.StringBuilder idBuilder = new System.Text.StringBuilder();

        for (int i = 0; i < length; i++)
        {
            int index = random.Next(chars.Length);
            idBuilder.Append(chars[index]);
        }

        return idBuilder.ToString();
    }
    //if one of the slots is different than 0 it means it's empty
    public bool isFilled()
    {
        bool isFilled = false;
        for(int i = 0 ; i < horizontalSlots.Length; i++)
        {
            if(horizontalSlots[i] == 0) {
                isFilled = true;
            }
        }

        if(!isFilled) return isFilled;
        
        for(int j = 0 ; j < verticalSlots.Length; j++)
        {
            if(verticalSlots[j] == 0) {
                isFilled = true;
            }
        }

        return isFilled;
    }
}
