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

    public void SetToDigged()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = true;
        spriteRenderer.sprite = normalSlot;
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

    public int getSlotPositionInTile()
    {
        string goName = gameObject.name;
        int underscoreIndex = goName.IndexOf('_');

        if (underscoreIndex != -1 && underscoreIndex < goName.Length - 1)
        {
            return int.Parse(goName.Substring(underscoreIndex + 1));
        }
        else
        {
            Debug.LogError("Failed to extract part after underscore from the name: " + goName);
            return 0;
        }
    }

    public bool IsVertical()
    {
        string goName = gameObject.name;
        int underscoreIndex = goName.IndexOf('_');

        if (underscoreIndex != -1 && underscoreIndex < goName.Length - 1)
        {
            return goName.Substring(0, underscoreIndex) == "v";
        }
        else
        {
            Debug.LogError("Failed to extract part after underscore from the name: " + goName);
            return false;
        }
    }
}
