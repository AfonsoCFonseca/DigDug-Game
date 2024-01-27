using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pookas : MonoBehaviour
{
    private Transform pookasSprite;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        pookasSprite = transform.Find("Sprite");

        animator = pookasSprite.GetComponent<Animator>();
        spriteRenderer = pookasSprite.GetComponent<SpriteRenderer>();

        animator.SetBool("isRunning", true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
