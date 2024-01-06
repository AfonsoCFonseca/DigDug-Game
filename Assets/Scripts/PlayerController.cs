using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Transform playerSprite;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    [SerializeField] LevelManager levelManager;

    [SerializeField] private float speed = 6.0f;
    private Vector2 moveDirection = Vector2.zero;

    private Tile currentTile;

    void Start()
    {
        playerSprite = transform.Find("Sprite");
        animator = playerSprite.GetComponent<Animator>();
        spriteRenderer = playerSprite.GetComponent<SpriteRenderer>();
        SnapToNearestTilePosition();
    }

    
    void Update()
    {
        KeyMovement();
    }

    void KeyMovement()
    {
        if(Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || 
            Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow)) 
        {
            animator.SetBool("isRunning", true);
            currentTile = levelManager.GetCurrentTile(transform.position);
        }

        if(Input.GetKey(KeyCode.LeftArrow))
        {
            spriteRenderer.flipX = true;
            spriteRenderer.flipY = false;
            playerSprite.rotation = Quaternion.Euler(0f, 0f, 0);

            MovePlayer(Direction.West);
        }
        else if(Input.GetKey(KeyCode.RightArrow))
        {
            spriteRenderer.flipX = false;
            spriteRenderer.flipY = false;
            playerSprite.rotation = Quaternion.Euler(0f, 0f, 0);

            MovePlayer(Direction.East);
        }
        else if(Input.GetKey(KeyCode.UpArrow))
        {
            playerSprite.rotation = Quaternion.Euler(0f, 0f, 90f);
            spriteRenderer.flipY = false;
            spriteRenderer.flipX = false;

            MovePlayer(Direction.North);
        }
        else if(Input.GetKey(KeyCode.DownArrow))
        {
            playerSprite.rotation = Quaternion.Euler(0f, 0f, -90f);
            spriteRenderer.flipY = true;
            spriteRenderer.flipX = false;

            MovePlayer(Direction.South);
        }
        else {
            animator.SetBool("isRunning", false);
        }

    }

    private void MovePlayer(Direction currentDirection)
    {
        // get the next tile
        Tile neighbourTile = levelManager.GetNeighbourTile(currentTile, currentDirection);
        neighbourTile.setDebugToColor("target");
        Vector2 targetPosition = neighbourTile.transform.position;

        //sets the movement
        Vector2 moveDirection = (targetPosition - (Vector2)transform.position).normalized;
        Vector2 newPosition = (Vector2)transform.position + moveDirection * speed * Time.deltaTime;

        //updates the position and checks when to update to next position
        float distanceToTarget = Vector2.Distance(transform.position, targetPosition);
        if (distanceToTarget <= 0.1f)
        {
            transform.position = targetPosition;
        }
        else {
            transform.position = newPosition;
        }

    }

    void SnapToNearestTilePosition()
    {
        currentTile = levelManager.GetCurrentTile(transform.position);
        Vector2 tilePos = currentTile.transform.position;
        tilePos.x += 2.5f; //half images height
        tilePos.x -= 2.44f; //half images width
        transform.position = tilePos;
    }
}
