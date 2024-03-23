using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menu_manager : MonoBehaviour
{
    [SerializeField] private GameObject arrow;
    private Vector2[] arrowPositions = new Vector2[] { new Vector2(-142.7f, -45.4f), new Vector2(-142.7f, -103.2f) };
    private string GAME_SCENE_NAME = "GameScene";

    void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            ChangeArrow();
        }

        if(Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene(GAME_SCENE_NAME);
        }
    }

    void ChangeArrow()
    {
        if (arrow.transform.localPosition.y == arrowPositions[0].y)
        {
            arrow.transform.localPosition = arrowPositions[1];
        }
        else
        {
            arrow.transform.localPosition = arrowPositions[0];
        }
    }
}
