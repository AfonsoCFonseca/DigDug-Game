using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    const int TOTAL_SLOTS = 4;

    int[] verticalSlots = new int [TOTAL_SLOTS] { 0, 0, 0, 0 };
    int[] horizontalSlots = new int [TOTAL_SLOTS] { 0, 0, 0, 0 };

    public void drawInitialSlots(int[] vertSlots, int[] horiSlots)
    {
        verticalSlots = vertSlots;
        horizontalSlots = horiSlots;

        updateTileTextures();
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
            childTransform.gameObject.SetActive(true);
        }
    }
}
