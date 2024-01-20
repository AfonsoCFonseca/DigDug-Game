using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Excavator : MonoBehaviour
{
    private bool isMovingHorizontally = true;

    public event System.Action<Collider2D> OnCollisionEvent;

    private PlayerController playerController;

    private void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (isCollidingWithSlot(otherCollider))
        {
            OnCollisionEvent?.Invoke(otherCollider);
        }
    }

    private bool isCollidingWithSlot(Collider2D otherCollider)
    {
        return (playerController.isMovingHorizontally && otherCollider.CompareTag("SlotHorizontal") 
        || !playerController.isMovingHorizontally && otherCollider.CompareTag("SlotVertical"));
    }
}
