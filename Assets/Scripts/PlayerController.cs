using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private float speed = 10.0f;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    
    void Update()
    {
        keyMovement();
    }

    void keyMovement()
    {
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position += Vector3.left * speed * Time.deltaTime;
            animator.SetBool("isRunning", true);
            spriteRenderer.flipX = true;
            spriteRenderer.flipY = false;
            transform.rotation = Quaternion.Euler(0f, 0f, 0);
        }
        else if(Input.GetKey(KeyCode.RightArrow))
        {
            transform.position += Vector3.right * speed * Time.deltaTime;
            animator.SetBool("isRunning", true);
            spriteRenderer.flipX = false;
            spriteRenderer.flipY = false;
            transform.rotation = Quaternion.Euler(0f, 0f, 0);
        }
        else if(Input.GetKey(KeyCode.UpArrow))
        {
            transform.position += Vector3.up * speed * Time.deltaTime;
            animator.SetBool("isRunning", true);
            transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            spriteRenderer.flipY = false;
            spriteRenderer.flipX = false;
        }
        else if(Input.GetKey(KeyCode.DownArrow))
        {
            transform.position += Vector3.down * speed * Time.deltaTime;
            animator.SetBool("isRunning", true);
            transform.rotation = Quaternion.Euler(0f, 0f, -90f);
            spriteRenderer.flipY = true;
            spriteRenderer.flipX = false;
        }
        else {
             animator.SetBool("isRunning", false);
        }
    }
}
