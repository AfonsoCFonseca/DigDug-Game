using UnityEngine;

public enum Direction
{
    North,
    South,
    East,
    West
}

public class GameValues : MonoBehaviour
{
    public Vector2 STARTING_TILE_POSITION = new Vector2(-5.00f, 0.35f);
    public Direction STARTING_PLAYER_DIRECTION = Direction.East;
    public float PLAYER_TO_TILE_DISTANCE = 0.1f;
    public float ENEMY_DISTANCE_TO_NEXT_TILE = 2.5f;
}