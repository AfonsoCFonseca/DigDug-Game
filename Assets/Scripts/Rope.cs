using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    public event System.Action<Collider2D> RopeOnTriggerEnter;
    private PlayerController playerController;

    [SerializeField] private GameObject rope1;
    [SerializeField] private GameObject rope2;

    public float durationToSecondRope = 0.1f;
    public float durationToThirdRope = 0.2f;
    private float currentTime = 0f;

    private void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    private void Update()
    {
        currentTime += Time.deltaTime;
        if(currentTime >= durationToSecondRope){
            rope2.SetActive(true);
        }
        if(currentTime >= durationToThirdRope){
            rope1.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        RopeOnTriggerEnter?.Invoke(otherCollider);
    }

    public void RestartState()
    {
        rope2.SetActive(false);
        rope1.SetActive(false);
    }
}
