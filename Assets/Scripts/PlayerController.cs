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
    Direction currentDirection = Direction.East;
    Direction previousDirection;

    private Tile currentTile;
    private Tile previousTile;
    private Tile currentNeighbourTile;

    private Excavator excavatorCollider;
    private Transform excavatorTransform;
    public bool isMovingHorizontally = true;
    private bool isMoving = false;

    [SerializeField] private GameObject rope_attack;
    private bool isAttacking = false;
    private float attackTimer = 0f;
    private float TIMER_ANIMATION_ATTACK = 0.3f;
    private float ATTACK_MOVING_SPEED = 50f;
    private GameObject ropeAttackInstance = null;

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

        if (isAttacking)
        {
            AttackTimerAndAnimation();
        }
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
        else if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Z))
        {
            Attack();
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

    private void Attack()
    {
        if(isAttacking == true) return;
        Vector2 ropeInitialPosition = new Vector3(transform.position.x + 7.5f, transform.position.y + -2.66f, 0);
        Quaternion quart = Quaternion.identity;
        switch(currentDirection) {
            case Direction.West:
                ropeInitialPosition = new Vector3(transform.position.x - 2.80f, transform.position.y + -2.66f, 0);
                quart = Quaternion.Euler(0, 0, 180);
                break;
            case Direction.North:
                ropeInitialPosition = new Vector3(transform.position.x + 2.43f, transform.position.y + 2.53f, 0);
                quart = Quaternion.Euler(0, 0, 90);
                break;
            case Direction.South:
                ropeInitialPosition = new Vector3(transform.position.x + 2.99f, transform.position.y + -8.19f, 0);
                quart = Quaternion.Euler(0, 0, -90);
                break;
        }

        ropeAttackInstance = Instantiate(rope_attack, ropeInitialPosition, quart);
        //set the z to be above the rest of the game
        ropeAttackInstance.transform.position = new Vector3(ropeAttackInstance.transform.position.x, 
            ropeAttackInstance.transform.position.y, ropeAttackInstance.transform.position.z);

        isAttacking = true;
    }

    private void AttackTimerAndAnimation()
    {
        if(ropeAttackInstance)
        {
            Rope ropeAttack = ropeAttackInstance.GetComponent<Rope>();
            if (ropeAttack != null)
            {
                ropeAttack.RopeOnTriggerEnter += RopeCollision;
            }
            Vector2 direction = new Vector2(1, 0);
            Vector2 translation = direction.normalized * ATTACK_MOVING_SPEED * Time.deltaTime;
            ropeAttackInstance.transform.Translate(translation);
        }

        attackTimer += Time.deltaTime;
        if (attackTimer >= TIMER_ANIMATION_ATTACK) 
        {
            isAttacking = false;
            DestroyRopeAttack();
            attackTimer = 0f;
        }
    }

    private void DestroyRopeAttack()
    {
        if(ropeAttackInstance)
        {
            Destroy(ropeAttackInstance);
            ropeAttackInstance = null;
        }
    }

    private void RopeCollision(Collider2D otherCollider)
    {
        collideWithEnemy(otherCollider);
        collidedWithWall(otherCollider);
    }

    private void collidedWithWall(Collider2D otherCollider)
    {
        bool isPlayerMovingVertically = utils.IsVerticalAxis(currentDirection);
        if(isPlayerMovingVertically && otherCollider.CompareTag("SlotVertical") ||
            !isPlayerMovingVertically && otherCollider.CompareTag("SlotHorizontal")) 
        {
            Tile tile = otherCollider.transform.parent.gameObject.GetComponent<Tile>();
            if(tile.isEmptyInDirection(false) == true)
                return;
            
            SpriteRenderer sr = otherCollider.GetComponent<SpriteRenderer>();
            if(sr.enabled == false){
                Destroy(ropeAttackInstance);
            }
        }
    }

    private void collideWithEnemy(Collider2D otherCollider)
    {
       if (otherCollider.CompareTag("Enemy"))
        {
            rope_attack.GetComponent<Rope>().RestartState();
            Destroy(otherCollider.gameObject);
        }
    }

    public Tile getCurrentTile()
    {
        return currentTile;
    }

    public Tile getPreviousTile()
    {
        return previousTile;
    }

    public void RestartPlayer()
    {
        SetPlayerToStartingPosition();
        Tile currentTile = levelManager.GetCurrentTile(gameValues.STARTING_TILE_POSITION);
        Tile neighbourTile = levelManager.GetNeighbourTile(currentTile, Direction.East);
        SetPlayerStartingRotationAndDirection(Direction.East, neighbourTile);
    }

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if(otherCollider.tag == "Enemy")
        {
            levelManager.RestartGame();
        }
    }
}
