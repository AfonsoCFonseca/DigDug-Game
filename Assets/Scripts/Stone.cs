using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    private LevelManager levelManager;
    private PlayerController playerController;
    private Tile currentTile;
    private Tile startingTile;

    private bool isFalling = false;
    private float speed = 12.0f;
    private Tile nextTile;

    private bool isTrembling = false;
    private float currentTremblingTimer = 1.5f;
    private float degreeDifference = 8.0f;
    private Quaternion baseRotation;
    private Quaternion rotation1;
    private Quaternion rotation2;
    private float timeSinceLastSwitch;

    void Start()
    {
        GameObject gameManager = GameObject.FindGameObjectsWithTag("GameManager")[0];
        levelManager = gameManager.GetComponent<LevelManager>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();

        baseRotation = Quaternion.identity; 
        rotation1 = Quaternion.Euler(0, 0, degreeDifference) * baseRotation;
        rotation2 = Quaternion.Euler(0, 0, 0) * baseRotation;
        startingTile = levelManager.GetCurrentTile(transform.position);
        startingTile.SetIsStone(true);
    }

    void Update()
    {
        currentTile = levelManager.GetCurrentTile(transform.position);
        if(currentTile.getId() != startingTile.getId()) startingTile.SetIsStone(false);
        if (!isFalling)
        {
            nextTile = GetBelowTile();
        }
        else
        {
            FallDown();
        }

        if(isTrembling)
        {
            Tremble();
        }
    }

    private Tile GetBelowTile()
    {
        Tile belowTile = levelManager.GetNeighbourTile(currentTile, Direction.South);
        if(!belowTile.isFilled())
        {
            isTrembling = true;
        }

        return belowTile;
    }

    private void Tremble()
    {
        currentTremblingTimer -= Time.deltaTime;
        float switchInterval = 0.3f;
        timeSinceLastSwitch += Time.deltaTime;

        if (timeSinceLastSwitch >= switchInterval)
        {
            if (transform.rotation == rotation1)
            {
                transform.rotation = rotation2;
            }
            else
            {
                transform.rotation = rotation1;
            }

            timeSinceLastSwitch = 0.0f;
        }

        if(currentTremblingTimer <= 0)
        {
            isTrembling = false;
            isFalling = true;
            transform.rotation = rotation2;
        }
    }


    private void FallDown()
    {
        Vector2 targetPosition = (Vector2) nextTile.transform.position;
        Vector2 moveDirection = (targetPosition - (Vector2) transform.position).normalized;
        Vector2 newPosition = (Vector2)transform.position + moveDirection * speed * Time.deltaTime;
        transform.position = newPosition;
        nextTile = GetBelowTile();
    }

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (otherCollider.CompareTag("SlotHorizontal") || otherCollider.CompareTag("SlotVertical"))
        {
            Slot slotComponent = otherCollider.GetComponent<Slot>();

            if (slotComponent != null)
            {
                Transform parentTransform = slotComponent.transform.parent;

                if (parentTransform != null)
                {
                    Tile nextTile = parentTransform.GetComponent<Tile>();

                    if (nextTile != null && nextTile.isFilled() && startingTile)
                    {
                        isFalling = false;

                        string startingId = startingTile.getId();
                        if(startingId != nextTile.getId() && startingId != currentTile.getId())
                        {
                            Invoke("DestroyStone", 1.0f);
                        }
                    }
                }
            }
        }
        if (otherCollider.CompareTag("Enemy"))
        {
            Enemy enemy = otherCollider.GetComponent<Enemy>();
            if(enemy)
            {
                enemy.Die();
            } 
        }
    }
    
    private void DestroyStone()
    {
        Destroy(gameObject);
    }
}
