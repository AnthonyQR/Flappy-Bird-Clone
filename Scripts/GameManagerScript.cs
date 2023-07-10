using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour
{
    [Header("Game State Checks")]
    public bool gameIsRunning = false;
    public bool gameCanStart = false;
    public bool inMainMenu = true;

    [Header("Wall Spawing")]
    public float timeToSpawnWall;
    public float timeToSpawnWallCountdown;
    public GameObject movingWallObject;
    public GameObject movingWallSpawnPosition;

    [Header("Score Tracking")]
    public int scoreCounter;
    public int bestScore;

    [Header("Game UI")]
    public GameObject gameUIObject;
    public GameObject startGameText;
    public GameObject gameOverText;
    public GameObject scoreObject;
    public TextMeshProUGUI scoreText;
    public GameObject bestScoreObject;
    public TextMeshProUGUI bestScoreText;

    [Header("Main Menu UI")]
    public GameObject mainMenuObject;
    public Button startButton;
    public Button quitButton;

    [Header("Game Over UI")]
    public Button restartButton;
    public Button returnToMainMenuButton;

    public delegate void StartNewGame();
    public static event StartNewGame startNewGame;
    
    public delegate void StopGame();
    public static event StopGame stopGame;

    // Start is called before the first frame update
    void Start()
    {
        gameIsRunning = false;
        gameCanStart = false;
        inMainMenu = true;
        gameUIObject.SetActive(false);
        mainMenuObject.SetActive(true);
        startGameText.SetActive(false);
        gameOverText.SetActive(false);

        scoreText = scoreObject.GetComponent<TextMeshProUGUI>();
        bestScoreText = bestScoreObject.GetComponent<TextMeshProUGUI>();

        ScoreData data = SaveSystem.LoadScore();
        if (data != null)
        {
            bestScore = data.bestScore;
        }
        bestScoreText.text = string.Format("Best: {0}", bestScore.ToString());

        startButton.onClick.AddListener(SetupGame);
        quitButton.onClick.AddListener(QuitGame);
        restartButton.onClick.AddListener(Restart);
        returnToMainMenuButton.onClick.AddListener(ReturnToMainMenu);

        PlayerScript.startGame += StartGame;
        PlayerScript.gameOver += GameOver;
        PlayerScript.scorePoint += ScorePoint;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameIsRunning)
        {
            timeToSpawnWallCountdown -= Time.deltaTime;
            if (timeToSpawnWallCountdown <= 0)
            {
                timeToSpawnWallCountdown = timeToSpawnWall;
                GameObject.Instantiate(movingWallObject, movingWallSpawnPosition.transform);
            }
        }
        
    }

    void SetupGame()
    {
        gameIsRunning = false;
        gameCanStart = true;
        inMainMenu = false;
        gameUIObject.SetActive(true);
        mainMenuObject.SetActive(false);
        startGameText.SetActive(true);
        gameOverText.SetActive(false);

        startNewGame();
    }

    void ReturnToMainMenu()
    {
        gameUIObject.SetActive(false);
        mainMenuObject.SetActive(true);
        startGameText.SetActive(false);
        gameOverText.SetActive(false);

        foreach (Transform child in movingWallSpawnPosition.transform)
        {
            Destroy(child.gameObject);
        }
        timeToSpawnWallCountdown = 0;
        scoreCounter = 0;
        scoreText.text = scoreCounter.ToString();

        stopGame();
    }

    void QuitGame()
    {
        Application.Quit();
    }

    void StartGame() {
        startGameText.SetActive(false);
        gameIsRunning = true;
    }

    void GameOver() {
        gameOverText.SetActive(true); 
        gameIsRunning = false;
        SaveSystem.SaveScore(this);
        stopGame();
    }

    void Restart() {
        gameCanStart = true;
        startGameText.SetActive(true);
        gameOverText.SetActive(false); 
        foreach(Transform child in movingWallSpawnPosition.transform)
        {
            Destroy(child.gameObject);
        }
        timeToSpawnWallCountdown = 0;
        scoreCounter = 0;
        scoreText.text = scoreCounter.ToString();
        startNewGame();
    }

    void ScorePoint()
    {
        scoreCounter++;
        scoreText.text = scoreCounter.ToString();
        if (scoreCounter > bestScore) {
            bestScore++;
            bestScoreText.text = string.Format("Best: {0}", bestScore.ToString());
        }
    }
}
