using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class Enemy : MonoBehaviour
{
    private Transform pookasSprite;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Utils utils;
    private PlayerController playerController;
    [SerializeField]
    private Sprite[] inflatingSpriteArray;
    [SerializeField]
    private Sprite[] movingAndGhostSpriteArray;

    private LevelManager levelManager;
    private GameValues gameValues;
    private UI ui;

    [SerializeField]
    private int enemyScore;

    public enum Phase
    {
        Chase,
        Moving,
        Inflated,
    }
    private Phase currentPhase;

    private Direction currentDirection = Direction.West;
    private Tile currentTile;
    private Tile neighbourTile;
    private bool canGetNewNeighbour = true;
    [SerializeField] private float speed = 6.0f;

    List<string> idTilesSinceLastTurningpoint = new List<string>();

    Tile currentPossibleTile;
    Direction currentPossibleDirection;

    private int MAX_HEALTH = 4;
    private int health;
    private bool isCooldownHealth = false;
    private float TIMER_COOLDOWN_HEALTH_VAL = 0.4f;
    private float timerCooldownHealth;

    void Start()
    {
        pookasSprite = transform.Find("Sprite");

        animator = pookasSprite.GetComponent<Animator>();
        spriteRenderer = pookasSprite.GetComponent<SpriteRenderer>();
        GameObject gameManager = GameObject.FindGameObjectsWithTag("GameManager")[0];
        levelManager = gameManager.GetComponent<LevelManager>();
        gameValues = levelManager.GetComponent<GameValues>();
        utils = levelManager.GetComponent<Utils>();
        ui = levelManager.GetComponent<UI>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();

        timerCooldownHealth = TIMER_COOLDOWN_HEALTH_VAL;
        health = MAX_HEALTH;

        currentTile = levelManager.GetCurrentTile(transform.position);
        List<Direction> possibleDirections = GetAllPossibleDirections(currentTile);
        int randomIndex = UnityEngine.Random.Range(0, possibleDirections.Count);
        currentDirection = possibleDirections[randomIndex];
        currentPhase = Phase.Moving;

        currentPossibleTile = currentTile;
        currentPossibleDirection = currentDirection;
    }

    void Update()
    {
        switch(currentPhase)
        {
            case Phase.Chase:
                Chase();
                break;
            case Phase.Moving:
                Move();
                break;
            case Phase.Inflated:
                animator.SetBool("isRunning", false);
                HealthTimerWhileInflated();
                break;
        }
    }

    public void SetPhase(Phase newPhase)
    {
        currentPhase = newPhase;
    }

    private void HealthTimerWhileInflated()
    {
        if(timerCooldownHealth <= 0)
        {
            if(!isCooldownHealth) {
                UpdateHealth(true);
                if(health >= MAX_HEALTH)
                {
                    animator.enabled = true;
                    SetPhase(Phase.Moving);
                }
                else 
                {
                    spriteRenderer.sprite = inflatingSpriteArray[health];
                }
            }
            isCooldownHealth = false;
            timerCooldownHealth = TIMER_COOLDOWN_HEALTH_VAL;
        }
        timerCooldownHealth -= Time.deltaTime;
    }

    private void UpdateHealth(bool isIncr)
    {
        int updatedHealth = isIncr ? ++health : --health;
        health = Mathf.Clamp (updatedHealth, -1, MAX_HEALTH);
    }

    private void Move()
    {
        currentTile = levelManager.GetCurrentTile(transform.position);
        animator.SetBool("isRunning", true);

        float distanceToTarget = Vector2.Distance(transform.position, currentTile.transform.position);

        if(canGetNewNeighbour)
        {   
            idTilesSinceLastTurningpoint = new List<string>();
            Tile isPlayerTile = pathFinderPlayer();
            if(isPlayerTile != null)
            {
                neighbourTile = isPlayerTile;
                canGetNewNeighbour = false;
            } 
            else 
            {
                neighbourTile = GetNewNeighbour();
            }
        }

        MoveTransistion(); //makes movement transition

        float distanceToNeighbour = Vector2.Distance(transform.position, neighbourTile.transform.position);
        //waits until it's close enough to get a new neighbour tile on the GetNewNeighbour() above
        if(distanceToNeighbour <= 0.1f) {
            canGetNewNeighbour = true;
        }
    }

    private Tile pathFinderPlayer()
    {
        List<Tile> path = new List<Tile>();
        List<Direction> pathDirection = new List<Direction>();
        setPossibleTileAndDirection(currentTile, currentDirection);

        utils.clearDebugBlocksFromMap(levelManager);
        
        for(int i = 0; i < 14 * 14; i++)
        {
            List<Direction> possibleDirections = GetAllPossibleDirections(currentPossibleTile);
            Direction newDirection = PickOneOfTheDirections(possibleDirections, currentPossibleDirection);

            Tile nt = levelManager.GetNeighbourTile(currentPossibleTile, newDirection);
            path.Add(nt);
            pathDirection.Add(newDirection);
            setPossibleTileAndDirection(nt, newDirection);

            if(isTilePlayerTile(nt))
            {
                currentDirection = pathDirection[0];
                RotateSprite(currentDirection);
                return path[0];
            }

            if(GetAllPossibleDirections(nt).Count == 1) { // can only go backwards
                setPossibleTileAndDirection(currentTile, currentDirection);
                path = new List<Tile>();
                pathDirection = new List<Direction>();
                continue;
            }
        }
        return null;
    }

    private void setPossibleTileAndDirection(Tile ct, Direction cd)
    {
        currentPossibleTile = ct;
        currentPossibleDirection = cd;
    }
    
    private Tile GetNewNeighbour()
    {
        Tile currentNewNeighbourTile;
        List<Direction> possibleDirections = GetAllPossibleDirections(currentTile);
        currentDirection = PickOneOfTheDirections(possibleDirections, currentDirection);
        RotateSprite(currentDirection);
        currentNewNeighbourTile = levelManager.GetNeighbourTile(currentTile, currentDirection);

        canGetNewNeighbour = false;
        return currentNewNeighbourTile;
    }

    private void MoveTransistion()
    {
        Vector2 targetPosition = neighbourTile.transform.position;
        Vector2 moveDirection = (targetPosition - (Vector2) transform.position).normalized;
        Vector2 newPosition = (Vector2)transform.position + moveDirection * speed * Time.deltaTime;
        transform.position = newPosition;
    }
    
    //Get all directions in an array according with the current Tile where you are
    private List<Direction> GetAllPossibleDirections(Tile currentPossibleTile)
    {
        Direction[] allDirections = (Direction[])Enum.GetValues(typeof(Direction));
        List<Direction> possibleDirections = new List<Direction>();
        foreach (Direction direction in allDirections)
        {
            Tile neighbourTile = levelManager.GetNeighbourTile(currentPossibleTile, direction);
            bool isVertical = utils.IsVerticalAxis(direction);

            if(isTileValidForMoving(direction, isVertical, neighbourTile, currentPossibleTile))
            {
                possibleDirections.Add(direction);
            }
        }

        return possibleDirections;
    }

    //Pick one of the positions of the array but if it's bigger than 1 dont pick the oposite
    //to the current Direction
    private Direction PickOneOfTheDirections(List<Direction> possibleDirections, Direction currentPossibleDirection)
    {
        if(possibleDirections.Count > 1) {
            Direction specificDirection = possibleDirections.Find(
                dir => dir == utils.GetOppositeDirection(currentPossibleDirection));

            if (possibleDirections.Contains(specificDirection) )
            {
                possibleDirections.Remove(specificDirection);
            }
        }

        int randomIndex = UnityEngine.Random.Range(0, possibleDirections.Count);
        return possibleDirections[randomIndex];
    }

    private bool isTileValidForMoving(Direction dir, bool isVertical, 
        Tile neighbourTile, Tile currentPossibleTile)
    {
        //Check if the current tile and next tile match the slot state, validating if the slots are linked
        if (neighbourTile) 
        {
            // Define constants or variables for slot indices
            int currentSlotIndex = (dir == Direction.South || dir == Direction.East) ? 3 : 0;
            int neighborSlotIndex = (dir == Direction.South || dir == Direction.East) ? 0 : 3;

            bool isVerticalDirection = utils.IsVerticalAxis(dir);
            // Extracted common logic
            bool currentSlotActive = currentPossibleTile.GetSlot(currentSlotIndex, isVerticalDirection).isRendererActive();
            bool neighborSlotActive = neighbourTile.GetSlot(neighborSlotIndex, isVerticalDirection).isRendererActive();

            // Simplified condition checks
            if (!currentSlotActive || !neighborSlotActive)
            {
                return false;
            }
        }

        return neighbourTile && !neighbourTile.isFilled();
    }

    private void RotateSprite(Direction dir)
    {
        float yRotate = dir == Direction.West ? 180f : 0;
        pookasSprite.rotation = Quaternion.Euler(0f, yRotate, 0f);
    }

    private void Chase()
    {
        animator.SetBool("isChasing", true);
    }

    //returns if it's still alive
    public bool Inflate()
    {
        currentPhase = Phase.Inflated;
        if(!isCooldownHealth)
        {
            isCooldownHealth = true;
            UpdateHealth(false);
            if(health <= 0) return true;
            animator.enabled = false;
            spriteRenderer.sprite = inflatingSpriteArray[health];
            return false;
        }
        return false;
    }

    private bool isTilePlayerTile(Tile nt)
    {
        return nt.getId() == playerController.getCurrentTile().getId() || 
            nt.getId() == playerController.getPreviousTile().getId();
    }

    public void Die()
    {
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.enabled = false;

        TextMeshPro score = this.GetComponent<TextMeshPro>();
        score.text = enemyScore.ToString();
        ui.AddScore(enemyScore);

        isCooldownHealth = true;
        spriteRenderer.sprite = inflatingSpriteArray[0];
        Invoke("DestroyGameObject", 1.5f);
        Invoke("HideRenderer", 0.5f);
    }

    private void DestroyGameObject()
    {
        Destroy(gameObject);
    }

    private void HideRenderer()
    {
        TextMeshPro score = this.GetComponent<TextMeshPro>();
        score.enabled = true;
        spriteRenderer.enabled = false;
    }
}
