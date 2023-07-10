using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    // Player
    public GameObject player;
    public BoxCollider2D playerCollider;
    public Rigidbody2D playerPhysics;
    public int playerJumpHeight;
    public Vector2 playerStartingPosition;

    // Game Manager
    public GameManagerScript gameManager;

    // Events
    public delegate void StartGame();
    public static event StartGame startGame;

    public delegate void GameOver();
    public static event GameOver gameOver;

    public delegate void ScorePoint();
    public static event ScorePoint scorePoint;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerCollider = player.GetComponent<BoxCollider2D>();
        playerPhysics = player.GetComponent<Rigidbody2D>();
        playerStartingPosition = player.GetComponent<Transform>().position;
        playerPhysics.constraints = RigidbodyConstraints2D.FreezeAll;

        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManagerScript>();
        GameManagerScript.startNewGame += EnablePlayer;
        GameManagerScript.stopGame += DisablePlayer;

        this.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        gameManager.gameIsRunning = false;
        gameManager.gameCanStart = false;
        playerPhysics.constraints = RigidbodyConstraints2D.FreezeAll;
        gameOver();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        scorePoint();
    }

    void EnablePlayer()
    {
        this.enabled = true;
        player.transform.position = playerStartingPosition;
    }
    void DisablePlayer()
    {
        playerPhysics.constraints = RigidbodyConstraints2D.FreezeAll;
        this.enabled = false;
    }
    public void Jump(InputAction.CallbackContext value)
    {
        if(this.enabled == false || !value.started)
        {
            return;
        }
        if (gameManager.gameCanStart)
        {
            startGame();
        }
        playerPhysics.constraints = RigidbodyConstraints2D.FreezeRotation;
        gameManager.gameIsRunning = true;
        playerPhysics.velocity = new Vector2(0, playerJumpHeight);
    }
}
