using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    [Header("Game State Checks")]
    public bool gameIsRunning = false;
    public bool gameCanStart = false;

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
    
    [Header("Game Start")]
    public GameObject FadeFromBlackToGamePanel;
    public float fadeSpeed;

    [Header("Game Over UI")]
    public GameObject ScreenFlashPanel;
    public float screenFlashFadeSpeed;
    public bool hasScreenFlashed = false;
    public Button restartButton;
    public Button returnToMainMenuButton;

    public delegate void StartNewGame();
    public static event StartNewGame startNewGame;
    
    public delegate void StopGame();
    public static event StopGame stopGame;

    public delegate void InGame();
    public static event InGame inGameEvent;

    public delegate void InMainMenu();
    public static event InMainMenu inMainMenuEvent;

    public AudioSource soundPlayer;
    public AudioClip scoreSound;
    public AudioClip deathSound;
    public AudioClip startSound;
    public AudioClip clickSound;

    // Start is called before the first frame update
    void Start()
    {
        gameIsRunning = false;
        gameCanStart = false;
        gameUIObject.SetActive(true);
        startGameText.SetActive(true);
        gameOverText.SetActive(false);
        soundPlayer = this.GetComponent<AudioSource>();

        scoreText = scoreObject.GetComponent<TextMeshProUGUI>();
        bestScoreText = bestScoreObject.GetComponent<TextMeshProUGUI>();

        ScoreData data = SaveSystem.LoadScore();
        if (data != null)
        {
            bestScore = data.bestScore;
        }
        bestScoreText.text = string.Format("Best: {0}", bestScore.ToString());

        restartButton.onClick.AddListener(Restart);
        returnToMainMenuButton.onClick.AddListener(QuitGame);

        FadeFromBlackToGamePanel.SetActive(true);
        StartCoroutine(FadeFromBlackToGame());

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

    public IEnumerator FadeFromBlackToGame()
    {
        soundPlayer.PlayOneShot(startSound);
        float fadeAmount = 1;
        Color FadeFromBlackToGameColor = FadeFromBlackToGamePanel.GetComponent<Image>().color;
        FadeFromBlackToGamePanel.GetComponent<Image>().color = new Color(FadeFromBlackToGameColor.r, FadeFromBlackToGameColor.b, FadeFromBlackToGameColor.g, 1);
        while (FadeFromBlackToGamePanel.GetComponent<Image>().color.a > 0)
        {
            fadeAmount = FadeFromBlackToGameColor.a - (fadeSpeed * Time.deltaTime);

            FadeFromBlackToGameColor = new Color(FadeFromBlackToGameColor.r, FadeFromBlackToGameColor.b, FadeFromBlackToGameColor.g, fadeAmount);
            FadeFromBlackToGamePanel.GetComponent<Image>().color = FadeFromBlackToGameColor;
            yield return null;
        }

        FadeFromBlackToGamePanel.SetActive(false);
        gameCanStart = true;
    }

    public IEnumerator RestartFadeToBlack()
    {
        FadeFromBlackToGamePanel.SetActive(true);
        float fadeAmount = 0;
        Color FadeFromBlackToGameColor = FadeFromBlackToGamePanel.GetComponent<Image>().color;
        FadeFromBlackToGamePanel.GetComponent<Image>().color = new Color(FadeFromBlackToGameColor.r, FadeFromBlackToGameColor.b, FadeFromBlackToGameColor.g, 0);
        while (FadeFromBlackToGamePanel.GetComponent<Image>().color.a < 1)
        {
            fadeAmount = FadeFromBlackToGameColor.a + (fadeSpeed * Time.deltaTime);

            FadeFromBlackToGameColor = new Color(FadeFromBlackToGameColor.r, FadeFromBlackToGameColor.b, FadeFromBlackToGameColor.g, fadeAmount);
            FadeFromBlackToGamePanel.GetComponent<Image>().color = FadeFromBlackToGameColor;
            yield return null;
        }
        yield return new WaitForSeconds(1);

        startGameText.SetActive(true);
        gameOverText.SetActive(false);
        foreach (Transform child in movingWallSpawnPosition.transform)
        {
            Destroy(child.gameObject);
        }
        timeToSpawnWallCountdown = 0;
        scoreCounter = 0;
        scoreText.text = scoreCounter.ToString();
        startNewGame();

        StartCoroutine(FadeFromBlackToGame());
    }

    void StartGame() {
        Debug.Log(startGameText.GetType());
        startGameText.SetActive(false);
        gameIsRunning = true;
    }

    void GameOver() {
        if (!hasScreenFlashed) {
            hasScreenFlashed = true;
            StartCoroutine(ScreenFlash());
        }
        
    }

    public IEnumerator ScreenFlash()
    {
        ScreenFlashPanel.SetActive(true);
        float fadeAmount = 1;
        Color ScreenFlashColor = ScreenFlashPanel.GetComponent<Image>().color;
        ScreenFlashPanel.GetComponent<Image>().color = new Color(ScreenFlashColor.r, ScreenFlashColor.b, ScreenFlashColor.g, 1);
        while (ScreenFlashPanel.GetComponent<Image>().color.a > 0)
        {
            fadeAmount = ScreenFlashColor.a - (screenFlashFadeSpeed * Time.deltaTime);

            ScreenFlashColor = new Color(ScreenFlashColor.r, ScreenFlashColor.b, ScreenFlashColor.g, fadeAmount);
            ScreenFlashPanel.GetComponent<Image>().color = ScreenFlashColor;
            yield return null;
        }
        ScreenFlashPanel.SetActive(false);
        ScreenFlashPanel.GetComponent<Image>().color = new Color(ScreenFlashColor.r, ScreenFlashColor.b, ScreenFlashColor.g, 1);
        yield return new WaitForSeconds(1);

        gameOverText.SetActive(true);
        gameIsRunning = false;
        SaveSystem.SaveScore(this);
        soundPlayer.PlayOneShot(deathSound);
        stopGame();
    }

    void Restart() {
        hasScreenFlashed = false;
        soundPlayer.PlayOneShot(clickSound);
        StartCoroutine(RestartFadeToBlack());
    }

    void QuitGame()
    {
        soundPlayer.PlayOneShot(clickSound);
        StartCoroutine(FadeToQuit());
    }

    public IEnumerator FadeToQuit()
    {
        FadeFromBlackToGamePanel.SetActive(true);
        float fadeAmount = 0;
        Color FadeFromBlackToGameColor = FadeFromBlackToGamePanel.GetComponent<Image>().color;
        FadeFromBlackToGamePanel.GetComponent<Image>().color = new Color(FadeFromBlackToGameColor.r, FadeFromBlackToGameColor.b, FadeFromBlackToGameColor.g, 0);
        while (FadeFromBlackToGamePanel.GetComponent<Image>().color.a < 1)
        {
            fadeAmount = FadeFromBlackToGameColor.a + (fadeSpeed * Time.deltaTime);

            FadeFromBlackToGameColor = new Color(FadeFromBlackToGameColor.r, FadeFromBlackToGameColor.b, FadeFromBlackToGameColor.g, fadeAmount);
            FadeFromBlackToGamePanel.GetComponent<Image>().color = FadeFromBlackToGameColor;
            yield return null;
        }
        yield return new WaitForSeconds(1);
        Application.Quit();
    }

    void ScorePoint()
    {
        soundPlayer.clip = scoreSound;
        soundPlayer.PlayOneShot(scoreSound);
        scoreCounter++;
        scoreText.text = scoreCounter.ToString();
        if (scoreCounter > bestScore) {
            bestScore++;
            bestScoreText.text = string.Format("Best:{0}", bestScore.ToString());
        }
    }
}
