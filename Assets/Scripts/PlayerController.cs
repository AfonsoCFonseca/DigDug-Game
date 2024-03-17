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

    [SerializeField] private float speed = 8.0f;
    Direction currentDirection;
    Direction previousDirection;

    private Tile currentTile;
    private Tile previousTile;
    private Tile currentNeighbourTile;

    private Excavator excavatorCollider;
    private Transform excavatorTransform;
    public bool isMovingHorizontally = true;

    private bool isMoving = false;


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
            Tile newCurrentTile = levelManager.GetCurrentTile(transform.position);
            if (newCurrentTile != null && newCurrentTile.getId() != currentTile.getId())
            {
                currentTile = newCurrentTile;
            }
        }

        if(Input.GetKey(KeyCode.LeftArrow))
        {
            isMoving = true;
            MovePlayer(Direction.West);
        }
        else if(Input.GetKey(KeyCode.RightArrow))
        {
            isMoving = true;
            MovePlayer(Direction.East);
        }
        else if(Input.GetKey(KeyCode.UpArrow))
        {
            isMoving = true;
            MovePlayer(Direction.North);
        }
        else if(Input.GetKey(KeyCode.DownArrow))
        {
            isMoving = true;
            MovePlayer(Direction.South);
        }
        else {
            animator.SetBool("isRunning", false);
            animator.SetBool("isDigging", false);
            isMoving = false;
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
            previousTile = currentTile;
            previousDirection = currentDirection;
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
        previousTile = currentTile;
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
        if (isCollidingWithSlot(otherCollider)){
            MovementAnimationPlayer(otherCollider);
            Slot slot = otherCollider.GetComponent<Slot>();
            slot.SetToDigged();
        }
    }

    private void HandleDigExit(Collider2D otherCollider)
    {
        Slot slot = otherCollider.GetComponent<Slot>();
        int slotPosition = slot.getSlotPositionInTile();
        bool isSlotZeroPosition = slotPosition == 0;

        Tile neighbourTile = levelManager.GetNeighbourTile(slot.GetParentTile(), previousDirection);
        bool isNeighbourFill = neighbourTile && neighbourTile.isFilled();
        bool isPlayerMovingVertically = utils.IsVerticalAxis(currentDirection);
        bool isLastSlotEndTile = slot.IsEndSlot();
        bool isSlotInVerticalPosition = slot.IsVertical();
        bool isCurrentSlotAndNeighbourTileValid = (isLastSlotEndTile || isNeighbourFill);

        // make sure the current slot is vertical and the player is now moving in the opposite direction
        // validate if the last slot of the current tile is an EndSlot or if the neighbour tile is filled with dirt
        if(isSlotInVerticalPosition && !isPlayerMovingVertically && isCurrentSlotAndNeighbourTileValid ||
            !isSlotInVerticalPosition && isPlayerMovingVertically && isCurrentSlotAndNeighbourTileValid)
        {
            slot.SwitchToEndSlot(isSlotZeroPosition);
        }
    }

    private void MovementAnimationPlayer(Collider2D otherCollider)
    {
        if (isMoving)
        {
            bool isRendererEnable = otherCollider.GetComponent<SpriteRenderer>().enabled == false;
            animator.SetBool("isRunning", !isRendererEnable);
            animator.SetBool("isDigging", isRendererEnable);
        }
    }

    private bool isCollidingWithSlot(Collider2D otherCollider)
    {
        return ((isMovingHorizontally && otherCollider.CompareTag("SlotHorizontal") 
        || !isMovingHorizontally && otherCollider.CompareTag("SlotVertical")));
    }


    public Tile getCurrentTile()
    {
        return currentTile;
    }

    public Tile getPreviousTile()
    {
        return previousTile;
    }
}
