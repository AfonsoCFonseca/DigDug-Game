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

    private Excavator excavatorCollider;
    private Transform excavatorTransform;
    public bool isMovingHorizontally = true;

    void Start()
    {
        playerSprite = transform.Find("Sprite");
        excavatorTransform = transform.Find("Excavator");

        animator = playerSprite.GetComponent<Animator>();
        spriteRenderer = playerSprite.GetComponent<SpriteRenderer>();
        gameValues = levelManager.GetComponent<GameValues>();
        excavatorCollider = GetComponentInChildren<Excavator>();
        
        setPlayerToStartingPosition();

        if (excavatorCollider != null)
        {
            excavatorCollider.OnCollisionEvent += HandleDigCollision;
        }
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
                isMovingHorizontally = true;
                playerSprite.rotation = Quaternion.Euler(0f, 0f, 0f);
                break;
            case Direction.West:
                isMovingHorizontally = true;
                playerSprite.rotation = Quaternion.Euler(0f, 180f, 0f);
                break;
            case Direction.North:
                isMovingHorizontally = false;
                excavatorTransform.rotation = Quaternion.Euler(0f, 0f, 90f);
                playerSprite.rotation = Quaternion.Euler(0f, 0f, 90f);
                break;
            case Direction.South:
                isMovingHorizontally = false;
                playerSprite.rotation = Quaternion.Euler(0f, 180f, -90f);
                excavatorTransform.rotation = Quaternion.Euler(0f, 0f, -90f);
                break;
            default:
                isMovingHorizontally = true;
                playerSprite.rotation = Quaternion.Euler(0f, 0f, 0f);
                excavatorTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
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

    private void HandleDigCollision(Collider2D otherCollider)
    {
        if(otherCollider.CompareTag("SlotVertical"))
        {
            Debug.Log("here");
        }
        Slot slot = otherCollider.GetComponent<Slot>();
        slot.setToDigged();
    }
}
