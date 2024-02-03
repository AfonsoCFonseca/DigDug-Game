using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    private Transform pookasSprite;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Utils utils;
    private PlayerController playerController;

    private LevelManager levelManager;
    private GameValues gameValues;

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


    void Start()
    {
        pookasSprite = transform.Find("Sprite");

        animator = pookasSprite.GetComponent<Animator>();
        spriteRenderer = pookasSprite.GetComponent<SpriteRenderer>();
        GameObject gameManager = GameObject.FindGameObjectsWithTag("GameManager")[0];
        levelManager = gameManager.GetComponent<LevelManager>();
        gameValues = levelManager.GetComponent<GameValues>();
        utils = levelManager.GetComponent<Utils>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();

        currentPhase = Phase.Chase;
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
                break;
        }
    }

    private void Move()
    {
        currentTile = levelManager.GetCurrentTile(transform.position);
        animator.SetBool("isRunning", true);

        //calculates if the player it's near the currentTile and near the neighbour tile
        float distanceToTarget = Vector2.Distance(transform.position, currentTile.transform.position);
        if(distanceToTarget <= 0.1f && canGetNewNeighbour) //if yes, gets new neighbour tile
        {
            neighbourTile = GetNewNeighbour();
        }

        MoveTransistion(); //makes movement transition

        float distanceToNeighbour = Vector2.Distance(transform.position, neighbourTile.transform.position);
        //waits until it's close enough to get a new neighbour tile on the GetNewNeighbour() above
        if(distanceToNeighbour <= 0.1f) {
            canGetNewNeighbour = true;
        }
    }

    private Tile GetNewNeighbour()
    {
        Tile currentNewNeighbourTile;
        if (neighbourTile != null) neighbourTile.setDebugToColor("default");
        List<Direction> possibleDirections = GetAllPossibleDirections();
        currentDirection = PickOneOfTheDirections(possibleDirections);
        RotateSprite(currentDirection);
        currentNewNeighbourTile = levelManager.GetNeighbourTile(currentTile, currentDirection);
        currentNewNeighbourTile.setDebugToColor("debug");
        canGetNewNeighbour = false;
        return currentNewNeighbourTile;
    }

    private void MoveTransistion()
    {
        Vector2 targetPosition = neighbourTile.transform.position;
        Vector2 moveDirection = (targetPosition - (Vector2)transform.position).normalized;
        Vector2 newPosition = (Vector2)transform.position + moveDirection * speed * Time.deltaTime;
        transform.position = newPosition;
    }
    
    //Get all directions in an array according with the current Tile where you are
    private List<Direction> GetAllPossibleDirections()
    {
        Direction[] allDirections = (Direction[])Enum.GetValues(typeof(Direction));
        List<Direction> possibleDirections = new List<Direction>();
        foreach (Direction direction in allDirections)
        {
            Tile possibleNeighbourTile = levelManager.GetNeighbourTile(currentTile, direction);
            if(!possibleNeighbourTile.isFilled()) {
                possibleDirections.Add(direction);
            }
        }

        return possibleDirections;
    }

    //Pick one of the positions of the array but if it's bigger than 1 dont pick the oposite
    //to the current Direction
    private Direction PickOneOfTheDirections(List<Direction> possibleDirections)
    {
        if(possibleDirections.Count > 1) {
            Direction specificDirection = possibleDirections.Find(
                dir => dir == utils.GetOppositeDirection(currentDirection));

            if (possibleDirections.Contains(specificDirection))
            {
                possibleDirections.Remove(specificDirection);
            }
        }

        int randomIndex = UnityEngine.Random.Range(0, possibleDirections.Count);
        return possibleDirections[randomIndex];
    }

    private void RotateSprite(Direction dir)
    {
        float yRotate = dir == Direction.West ? 180f : 0;
        pookasSprite.rotation = Quaternion.Euler(0f, yRotate, 0f);
    }

    private void Chase()
    {
        animator.SetBool("isChasing", true);
        Debug.Log(playerController.getCurrentTile());
    }
}
