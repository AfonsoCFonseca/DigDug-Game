using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Transform playerSprite;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    [SerializeField] LevelManager levelManager;
    private GameValues gameValues;

    [SerializeField] private float speed = 6.0f;

    private Tile currentTile;
    private Tile currentNeighbourTile;

    void Start()
    {
        playerSprite = transform.Find("Sprite");
        animator = playerSprite.GetComponent<Animator>();
        spriteRenderer = playerSprite.GetComponent<SpriteRenderer>();
        gameValues = levelManager.GetComponent<GameValues>();
        setPlayerToStartingPosition();
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
            Tile newCurrentTile = levelManager.GetCurrentTile(transform.position);
            if (newCurrentTile != null && newCurrentTile.getId() != currentTile.getId())
            {
                currentTile = newCurrentTile;
            }
        }

        if(Input.GetKey(KeyCode.LeftArrow))
        {
            MovePlayer(Direction.West);
        }
        else if(Input.GetKey(KeyCode.RightArrow))
        {
            MovePlayer(Direction.East);
        }
        else if(Input.GetKey(KeyCode.UpArrow))
        {
            MovePlayer(Direction.North);
        }
        else if(Input.GetKey(KeyCode.DownArrow))
        {
            MovePlayer(Direction.South);
        }
        else {
            animator.SetBool("isRunning", false);
        }

    }

    private void MovePlayer(Direction currentDirection)
    {
        currentNeighbourTile = calculateNextTileNeighbour(currentDirection);
        currentNeighbourTile.setDebugToColor("target");
        //sets the player movement
        Vector2 targetPosition = currentNeighbourTile.transform.position;
        Vector2 moveDirection = (targetPosition - (Vector2)transform.position).normalized;
        Vector2 newPosition = (Vector2)transform.position + moveDirection * speed * Time.deltaTime;
        transform.position = newPosition;
    }

    private Tile calculateNextTileNeighbour(Direction currentDirection)
    {
        // get the next tile
        Tile neighbourTile = levelManager.GetNeighbourTile(currentTile, currentDirection);
        //Check if neighbour Tile should be updated
        float distanceToTarget = Vector2.Distance(transform.position, currentNeighbourTile.transform.position);
        if(currentNeighbourTile.getId() !=  neighbourTile.getId() && distanceToTarget <= gameValues.PLAYER_TO_TILE_DISTANCE)
        {
            updatesPlayerRotation(currentDirection);
            return neighbourTile;
        }
        //if not, returns the current neighbour Tile
        return currentNeighbourTile;
    }

    private void updatesPlayerRotation(Direction dir)
    {
        switch(dir)
        {
            case Direction.East:
                spriteRenderer.flipX = false;
                spriteRenderer.flipY = false;
                playerSprite.rotation = Quaternion.Euler(0f, 0f, 0);
                break;
            case Direction.West:
                spriteRenderer.flipX = true;
                spriteRenderer.flipY = false;
                playerSprite.rotation = Quaternion.Euler(0f, 0f, 0);
                break;
            case Direction.North:
                playerSprite.rotation = Quaternion.Euler(0f, 0f, 90f);
                spriteRenderer.flipY = false;
                spriteRenderer.flipX = false;
                break;
            case Direction.South:
                playerSprite.rotation = Quaternion.Euler(0f, 0f, -90f);
                spriteRenderer.flipY = true;
                spriteRenderer.flipX = false;
                break;
            default:
                spriteRenderer.flipX = false;
                spriteRenderer.flipY = false;
                playerSprite.rotation = Quaternion.Euler(0f, 0f, 0);
                break;
        }
    }

    private void setPlayerToStartingPosition()
    {
        currentTile = levelManager.GetCurrentTile(gameValues.STARTING_TILE_POSITION);
        Vector2 tilePos = currentTile.transform.position;
        tilePos.x += 2.5f; //half images height
        tilePos.x -= 2.44f; //half images width
        transform.position = tilePos;

        currentNeighbourTile = levelManager.GetNeighbourTile(currentTile, 
            gameValues.STARTING_PLAYER_DIRECTION);
    }
}
