using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    public event System.Action<Collider2D> RopeOnTriggerEnter;
    private PlayerController playerController;

    private void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        RopeOnTriggerEnter?.Invoke(otherCollider);
    }
}
