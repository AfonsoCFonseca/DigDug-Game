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
    private Utils utils;

    [SerializeField] private float speed = 6.0f;
    Direction currentDirection;

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
        utils = levelManager.GetComponent<Utils>();
        excavatorCollider = GetComponentInChildren<Excavator>();
        
        SetPlayerToStartingPosition();

        if (excavatorCollider != null)
        {
            excavatorCollider.ExcavatorOnTriggerEnter += HandleDigCollision;
            excavatorCollider.ExcavatorOnTriggerExit += HandleDigExit;
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

    private void MovePlayer(Direction requestedDirection)
    {
        currentNeighbourTile = CalculateNextTileNeighbour(requestedDirection);

        //sets the player movement
        Vector2 targetPosition = currentNeighbourTile.transform.position;
        Vector2 moveDirection = (targetPosition - (Vector2)transform.position).normalized;
        Vector2 newPosition = (Vector2)transform.position + moveDirection * speed * Time.deltaTime;
        transform.position = newPosition;
    }

    private Tile CalculateNextTileNeighbour(Direction requestedDirection)
    {
        // get the next tile
        Tile neighbourTile = levelManager.GetNeighbourTile(currentTile, requestedDirection);
        //First input of the game
        if(currentNeighbourTile == null){
            SetPlayerStartingRotationAndDirection(requestedDirection, neighbourTile);
        }

        if(isPossibleNewNeighbourTile(requestedDirection, neighbourTile))
        {
            currentDirection = requestedDirection;
            UpdatesPlayerRotation(currentDirection);
            return neighbourTile;
        }
        //if not, returns the current neighbour Tile
        return currentNeighbourTile;
    }

    private void UpdatesPlayerRotation(Direction dir)
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
                playerSprite.rotation = Quaternion.Euler(0f, 0f, 90f);
                break;
            case Direction.South:
                isMovingHorizontally = false;
                playerSprite.rotation = Quaternion.Euler(0f, 180f, -90f);
                break;
            default:
                isMovingHorizontally = true;
                playerSprite.rotation = Quaternion.Euler(0f, 0f, 0f);
                break;
        }
    }

    private void SetPlayerToStartingPosition()
    {
        currentTile = levelManager.GetCurrentTile(gameValues.STARTING_TILE_POSITION);
        Vector2 tilePos = currentTile.transform.position;
        tilePos.x += 2.5f; //half images height
        tilePos.x -= 2.44f; //half images width
        transform.position = tilePos;
    }

    private void SetPlayerStartingRotationAndDirection(Direction requestedDirection, Tile neighbourTile)
    {
        currentDirection = requestedDirection;
        currentNeighbourTile = neighbourTile;
        UpdatesPlayerRotation(currentDirection);  
    }

    private bool isPossibleNewNeighbourTile(Direction requestedDirection, Tile neighbourTile)
    {
        float distanceToTarget = Vector2.Distance(transform.position, currentNeighbourTile.transform.position);
        return (neighbourTile && requestedDirection == utils.GetOppositeDirection(currentDirection) || 
        neighbourTile && currentNeighbourTile.getId() !=  neighbourTile.getId() && distanceToTarget <= gameValues.PLAYER_TO_TILE_DISTANCE);
    }

    private void HandleDigCollision(Collider2D otherCollider)
    {
        Slot slot = otherCollider.GetComponent<Slot>();
        slot.SetToDigged();
    }

    private void HandleDigExit(Collider2D otherCollider)
    {
        Slot slot = otherCollider.GetComponent<Slot>();
        bool isSlotZeroPosition = slot.getSlotPositionInTile() == 0;
        if(slot.IsVertical() == true && (currentDirection == Direction.East || currentDirection == Direction.West) ||
            slot.IsVertical() == false && (currentDirection == Direction.North || currentDirection == Direction.South))
        {
            slot.SwitchToEndSlot(isSlotZeroPosition);
        }
    }

    public Tile getCurrentTile()
    {
        return currentTile;
    }
}
