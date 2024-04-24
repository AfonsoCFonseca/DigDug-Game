using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private LevelManager levelManager;

    const int TOTAL_SLOTS = 4;
    string id = "";
    int tileArrayPosX = 0;
    int tileArrayPosY = 0;

    int[] verticalSlotsStates = new int [TOTAL_SLOTS] { 0, 0, 0, 0 };
    int[] horizontalSlotsStates = new int [TOTAL_SLOTS] { 0, 0, 0, 0 };
    // Slot[] verticalSlots = new Slot[4];
    List<Slot> verticalSlots = new List<Slot>();
    List<Slot> horizontalSlots = new List<Slot>();
    private bool isStone = false;

    //Debugging tool
    private Transform debugFlag;
    private Renderer debugFlagRenderer;
    private const string DEBUG_OBJ_NAME = "debug_flag";
    public Color targetColor = Color.green;
    public Color debugColor = Color.blue;
    public Color debugColor1 = Color.yellow;
    public Color defaultColor = Color.red;

    [SerializeField]
    private TextMesh textMesh;

    void Awake()
    {
        id = GenerateID(10);

        debugFlag = transform.Find(DEBUG_OBJ_NAME);
        debugFlagRenderer = debugFlag.GetComponent<Renderer>();
        debugFlagRenderer.material.color = defaultColor;

        GameObject gameManager = GameObject.FindGameObjectsWithTag("GameManager")[0];
        levelManager = gameManager.GetComponent<LevelManager>();
    }
    
    public void Setup(int[] slots, int mapY, int mapX)
    {
        tileArrayPosY = mapY;
        tileArrayPosX = mapX;
        setSlotVariables(slots);
        updateTileTextures();
    }

    public string getId()
    {
        return id;
    }

    public (int mapX, int mapY) getMapTilePosition()
    {
        return (tileArrayPosX, tileArrayPosY);
    }

    private void setSlotVariables(int[] slots)
    {
        verticalSlotsStates = slots;
        horizontalSlotsStates = slots;

        foreach (Transform childTransform in transform)
        {
            if (childTransform.CompareTag("SlotVertical"))
            {
                Slot slot = childTransform.GetComponent<Slot>();
                verticalSlots.Add(slot);
            }
            else if(childTransform.CompareTag("SlotHorizontal"))
            {
                Slot slot = childTransform.GetComponent<Slot>();
                horizontalSlots.Add(slot);
            }
        }
    }

    public void setDebugToColor(string type)
    {
        debugFlag.gameObject.SetActive(true);
        if(type == "default") debugFlagRenderer.material.color = defaultColor;
        if(type == "target") debugFlagRenderer.material.color = targetColor;
        if(type == "debug") debugFlagRenderer.material.color = debugColor;
        if(type == "debug1") debugFlagRenderer.material.color = debugColor1;
        if(type == "off") debugFlag.gameObject.SetActive(false);
    }

    void updateTileTextures() 
    {
        for(int i = 0; i < TOTAL_SLOTS; i++)
        {
            if(verticalSlotsStates[i] == 2) 
            {
                updateChildren("v_", i);
            }
            else if(horizontalSlotsStates[i] == 1 || horizontalSlotsStates[i] == 3) 
            {
                updateChildren("h_", i);
            }
        }

        if(horizontalSlotsStates[0] == 3)
        {
            updateChildren("v_", 0);
        }
    }

    public void updateChildren(string ob_child_name, int pos)
    {
        Slot slot = transform.Find(ob_child_name + pos).GetComponent<Slot>();
        if(slot) 
        {
            slot.SetToDigged();
            bool isVertical = slot.IsVertical();
            if(pos == 3)
            {
                int posTileX = isVertical ? tileArrayPosX : tileArrayPosX + 1;
                int posTileY = isVertical ? tileArrayPosY + 1 : tileArrayPosY;
                if(posTileX > levelManager.WIDTH_MAP_TILES - 1 || posTileY > levelManager.HEIGHT_MAP_TILES - 1
                    || !levelManager.IsFutureTileEmpty(posTileX, posTileY))
                {
                    slot.SwitchToEndSlot(false);
                }
            }
            if(pos == 0)
            {
                int posTileX = isVertical && tileArrayPosX > 0 ? tileArrayPosX : tileArrayPosX - 1;
                int posTileY = isVertical && tileArrayPosY > 0 ? tileArrayPosY - 1 : tileArrayPosY;
                if(tileArrayPosY == 0 || tileArrayPosX == 0 || !levelManager.IsFutureTileEmpty(posTileX, posTileY))
                {
                    slot.SwitchToEndSlot(true);
                }
            }
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
        bool isFilled = CheckSlots(horizontalSlotsStates);
        if(!isFilled) return isFilled;
        isFilled = CheckSlots(verticalSlotsStates);
        return isFilled;
    }

    public bool IsStone()
    {
        return isStone;
    }

    public void SetIsStone(bool isCurrentStone)
    { 
        isStone = isCurrentStone;
    }

    public bool isEmptyInDirection(bool isVertical)
    {
        int[] slots = isVertical ? verticalSlotsStates : horizontalSlotsStates;
        bool isFilled = CheckSlots(slots);
        return !isFilled;
    }

    private bool CheckSlots(int[] slots)
    {
        foreach (int slot in slots)
        {
            if (slot == 0)
            {
                return true;
            }
        }
        return false;
    }

    public void UpdateSlotState(int slotPos, int state)
    {
        verticalSlotsStates[slotPos] = 1;
        levelManager.UpdateTileState(this);
    }

    public Slot GetSlot(int pos, bool isVertical)
    {
        return isVertical ? verticalSlots[pos] : horizontalSlots[pos];
    }

}
