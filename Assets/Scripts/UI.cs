using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    [SerializeField] private GameObject canvas;

    private TextMeshProUGUI highScoreTMP;
    private TextMeshProUGUI scoreTMP;
    private TextMeshProUGUI roundTMP;

    private int score = 0;
    private int highScore = 1000;
    private int currentRound = 0;
    private int currentLifes = 3;

    Transform life1ValueTransform;
    Transform life2ValueTransform;

    private string GAME_SCENE_NAME = "Menu";

    void Start()
    {
        Transform canvasTransform = canvas.transform;
        Transform highScoreValueTransform = canvasTransform.Find("highScoreValue");
        if(highScoreValueTransform != null)
        {
            highScoreTMP = highScoreValueTransform.GetComponent<TextMeshProUGUI>();
            AddHighScore(highScore);
        }

        Transform scoreValueTransform = canvasTransform.Find("scoreValue");
        if(scoreValueTransform != null)
        {
            scoreTMP = scoreValueTransform.GetComponent<TextMeshProUGUI>();
        }
        AddScore(10);

        Transform roundValueTransform = canvasTransform.Find("roundValue");
        if(roundValueTransform != null)
        {
            roundTMP = roundValueTransform.GetComponent<TextMeshProUGUI>();
        }
        NextRound();

        life1ValueTransform = canvasTransform.Find("life_1");
        life2ValueTransform = canvasTransform.Find("life_2");
    }

    void AddScore(int scoreToAdd)
    {   
        if(scoreTMP != null)
        {
            score += scoreToAdd;
            scoreTMP.text = score.ToString();
        }
    }

    void AddHighScore(int scoreToAdd)
    {   
        if(highScoreTMP != null)
        {
            highScoreTMP.text = highScore.ToString();
        }
    }

    void NextRound()
    {
        currentRound++;
        if(roundTMP)
        {
            roundTMP.text = currentRound.ToString();
        }
    }

    public void LooseLife()
    {
        currentLifes--;
        if(currentLifes == 2)
        {
            life2ValueTransform.gameObject.SetActive(false);
        }
        if(currentLifes == 1)
        {
            life1ValueTransform.gameObject.SetActive(false);
        }
        if(currentLifes == 0)
        {
            SceneManager.LoadScene(GAME_SCENE_NAME);
        }
    }

    public void ShowGameOver()
    {

    }

    public void RestartGame()
    {
        currentLifes = 3;
        score = 0;
        scoreTMP.text = score.ToString();
        life1ValueTransform.gameObject.SetActive(true);
        life2ValueTransform.gameObject.SetActive(true);
    }
}
