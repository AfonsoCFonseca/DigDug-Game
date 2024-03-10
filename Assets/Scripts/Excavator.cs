using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Excavator : MonoBehaviour
{
    public event System.Action<Collider2D> ExcavatorOnTriggerEnter;
    public event System.Action<Collider2D> ExcavatorOnTriggerExit;

    private PlayerController playerController;

    private void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (isCollidingWithSlot(otherCollider))
        {
            ExcavatorOnTriggerEnter?.Invoke(otherCollider);
        }
    }

    private void OnTriggerExit2D(Collider2D otherCollider) {
        if(otherCollider.CompareTag("SlotHorizontal") || otherCollider.CompareTag("SlotVertical"))
        {
            GameObject slotGameObject = otherCollider.gameObject;
            Slot slotComponent = slotGameObject.GetComponent<Slot>();
            if(slotComponent.getSlotPositionInTile() == 0 || 
                slotComponent.getSlotPositionInTile() == 3)
            {
                ExcavatorOnTriggerExit?.Invoke(otherCollider);
            }
        }
    }

    private bool isCollidingWithSlot(Collider2D otherCollider)
    {
        return (playerController.isMovingHorizontally && otherCollider.CompareTag("SlotHorizontal") 
        || !playerController.isMovingHorizontally && otherCollider.CompareTag("SlotVertical"));
    }
}
