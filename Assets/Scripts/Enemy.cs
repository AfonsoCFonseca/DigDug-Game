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
    private LevelMaps levelMaps;
    [SerializeField]
    private Sprite[] inflatingSpriteArray;
    private GameObject[] fireGameObjectsArr;
    [SerializeField]
    private Sprite[] movingAndGhostSpriteArray;

    private LevelManager levelManager;
    private GameValues gameValues;
    private UI ui;

    private float fygarsTimer;
    private int MIN_FYGAR_TIMER = 5;
    private int MAX_FYGAR_TIMER = 21; // this is 20
    private bool isShootingFire = false;
    private float FYGAR_SHOOTING_FIRE_TIME = 2.5f;
    private float fygarShootingFireTime;
    private float FYGAR_FIRE_PHASES_TIME = 0.6f;
    private float fygarFirePhaseTimer;
    private int fygarFirePhase = 0;

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
    private Vector3 initialPosition;
    private bool canGetNewNeighbour = true;
    private float currentSpeed;

    List<string> idTilesSinceLastTurningpoint = new List<string>();

    Tile currentPossibleTile;
    Direction currentPossibleDirection;

    private int MAX_HEALTH = 4;
    private int health;
    private bool isCooldownHealth = false;
    private float TIMER_COOLDOWN_HEALTH_VAL = 0.4f;
    private float timerCooldownHealth;
    private bool isDead = false;

    private float chaseTimer;
    private int TIME_CHASE_MAX = 10;
    private int TIME_CHASE_MIN = 30;
    private Tile playerTile;

    void Start()
    {
        pookasSprite = transform.Find("Sprite");

        animator = pookasSprite.GetComponent<Animator>();
        spriteRenderer = pookasSprite.GetComponent<SpriteRenderer>();
        GameObject gameManager = GameObject.FindGameObjectsWithTag("GameManager")[0];
        levelManager = gameManager.GetComponent<LevelManager>();
        gameValues = levelManager.GetComponent<GameValues>();
        utils = levelManager.GetComponent<Utils>();
        levelMaps = levelManager.GetComponent<LevelMaps>();
        ui = levelManager.GetComponent<UI>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();

        timerCooldownHealth = TIMER_COOLDOWN_HEALTH_VAL;
        health = MAX_HEALTH;

        currentTile = levelManager.GetCurrentTile(transform.position);
        initialPosition = transform.position;
        List<Direction> possibleDirections = GetAllPossibleDirections(currentTile);
        int randomIndex = UnityEngine.Random.Range(0, possibleDirections.Count);
        currentDirection = possibleDirections[randomIndex];
        currentPhase = Phase.Moving;

        currentPossibleTile = currentTile;
        currentPossibleDirection = currentDirection;

        if(IsFygar())
        {
            GetFireObjectAndPopulateArray();
            fygarShootingFireTime = FYGAR_SHOOTING_FIRE_TIME;
            fygarFirePhaseTimer = FYGAR_FIRE_PHASES_TIME;
            fygarsTimer = (float) utils.GetRandomValueBetweenNumbers(MIN_FYGAR_TIMER, MAX_FYGAR_TIMER);
        }

        chaseTimer = (float) utils.GetRandomValueBetweenNumbers(TIME_CHASE_MIN, TIME_CHASE_MAX);

        currentSpeed = levelMaps.GetEnemySpeed();
    }

    void Update()
    {
        switch(currentPhase)
        {
            case Phase.Chase:
                Chase();
                break;
            case Phase.Moving:
                ChaseTimer();
                if(IsFygar()) FygarsFireException();
                if(!isShootingFire) Move();
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

    public bool IsDead()
    {
        return isDead;
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

    private void Chase()
    {
        if(isDead) return;
        animator.SetBool("isChasing", true);
        MoveTransistion(playerTile.transform.position);
        float distanceToTarget = Vector2.Distance(transform.position, playerTile.transform.position);
        if (distanceToTarget <= 0.1f)
        {
            ResetChase();
            SetPhase(Phase.Moving);
        }
    }

    private void ResetChase()
    {
        canGetNewNeighbour = true;
        chaseTimer = utils.GetRandomValueBetweenNumbers(TIME_CHASE_MIN, TIME_CHASE_MAX);
        idTilesSinceLastTurningpoint = new List<string>();
        animator.SetBool("isChasing", false);
    }

    private void ChaseTimer()
    {
        if(isShootingFire) return;
        chaseTimer -= Time.deltaTime;
        if (chaseTimer <= 0)
        {
            playerTile = playerController.getCurrentTile();
            SetPhase(Phase.Chase);
        }
    }

    private void GetFireObjectAndPopulateArray()
    {
        string[] childNames = { "FireSprite_1", "FireSprite_2", "FireSprite_3" };
        fireGameObjectsArr = new GameObject[childNames.Length];
        for (int i = 0; i < childNames.Length; i++)
        {
            Transform childTransform = transform.Find(childNames[i]);
            if (childTransform != null)
            {
                fireGameObjectsArr[i] = childTransform.gameObject;
            }
        }
    }

    private void FygarsFireException()
    {
        fygarsTimer -= Time.deltaTime;
        if(fygarsTimer <= 0)
        {
            StartsFire();

            if(fygarFirePhaseTimer <= 0) 
            {
                ChangeFireState();
            }
            if(fygarShootingFireTime <= 0)
            {
                EndFire();
            }
        }
    }

    private void StartsFire()
    {
        isShootingFire = true;
        fygarShootingFireTime -= Time.deltaTime;
        fygarFirePhaseTimer -= Time.deltaTime;
        animator.SetBool("isFiring", true);
    }

    private void ChangeFireState()
    {
        fygarFirePhase++;
        if(fygarFirePhase - 1 <= fireGameObjectsArr.Length - 1) {
            if(fygarFirePhase - 2 >= 0) fireGameObjectsArr[fygarFirePhase - 2].SetActive(false);
            fireGameObjectsArr[fygarFirePhase - 1].SetActive(true);
        }
        fygarFirePhaseTimer = FYGAR_FIRE_PHASES_TIME;
    }

    private void EndFire()
    {
        fygarFirePhase = 0;
        animator.SetBool("isFiring", false);
        fygarShootingFireTime = FYGAR_SHOOTING_FIRE_TIME;
        fygarsTimer = (float) utils.GetRandomValueBetweenNumbers(MIN_FYGAR_TIMER, MAX_FYGAR_TIMER);
        fireGameObjectsArr[2].SetActive(false);
        isShootingFire = false;
    }

    private float GetFirePositionX(int index, Direction dir)
    {
        switch(index)
        {
            case 0:
                return dir == Direction.West ? -1.1f : 6.41f;
            case 1:
                return dir == Direction.West ? -2.0f : 8.00f;
            case 2: 
                return dir == Direction.West ? -2.95f : 9.30f;
            default: 
                Debug.Log("Went to the default value at GetFirePositionX");
                return dir == Direction.West ? -1.1f : 6.41f;
        }
    }

    private void Move()
    {
        if(isDead) return;
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

        Vector2 targetPosition = neighbourTile.transform.position;
        MoveTransistion(targetPosition); //makes movement transition

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

    private void MoveTransistion(Vector2 targetPosition)
    {
        Vector2 moveDirection = (targetPosition - (Vector2) transform.position).normalized;
        Vector2 newPosition = (Vector2)transform.position + moveDirection * currentSpeed * Time.deltaTime;
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
        if(IsFygar())
        {
            for(int i = 0; i < fireGameObjectsArr.Length; i++)
            {
                Vector3 firePos = new Vector3(GetFirePositionX(i, dir), -2.92f, -0.1f);
                fireGameObjectsArr[i].transform.rotation = Quaternion.Euler(0f, yRotate, 0f);
                fireGameObjectsArr[i].transform.localPosition = firePos;
            }
        }
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
        isDead = true;
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

    private bool IsFygar()
    {
        return gameObject.name == "Fygars(Clone)";
    }

    public void Restart()
    {
        transform.position = initialPosition;
        isShootingFire = false;
        canGetNewNeighbour = true;
        idTilesSinceLastTurningpoint = new List<string>();
        animator.SetBool("isChasing", false);
        fygarsTimer = (float) utils.GetRandomValueBetweenNumbers(MIN_FYGAR_TIMER, MAX_FYGAR_TIMER);
        chaseTimer = (float) utils.GetRandomValueBetweenNumbers(TIME_CHASE_MIN, TIME_CHASE_MAX);
        SetPhase(Phase.Moving);
    }
}
