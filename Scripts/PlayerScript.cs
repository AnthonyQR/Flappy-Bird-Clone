using System;
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
    public Vector3 playerStartingPosition;

    public Animator animator;

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

        this.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("Velocity", playerPhysics.velocity.y);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        playerPhysics.constraints = RigidbodyConstraints2D.FreezeRotation;
        animator.SetBool("PlayerAlive", false);
        animator.SetBool("GameStarted", false);
        if (gameManager.gameIsRunning && other.gameObject.tag != "Ground")
        {
            animator.Play("Player_Dead");
            playerPhysics.velocity = new Vector2(0, playerJumpHeight);
            if (other.gameObject.tag != "Ceiling")
            {
                Transform collidedObjects = other.gameObject.transform.parent.GetComponent<Transform>();
                collidedObjects.GetComponent<BoxCollider2D>().enabled = false;
                foreach (Transform pipeTransform in collidedObjects)
                {
                    Debug.Log("Hello");
                    pipeTransform.GetComponent<BoxCollider2D>().enabled = false;
                }
            }
            playerPhysics.constraints = RigidbodyConstraints2D.FreezePositionX;
        }
        else if (other.gameObject.tag == "Ground")
        {
            animator.speed = 0;
            playerPhysics.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        gameManager.gameIsRunning = false;
        gameManager.gameCanStart = false;
        gameOver();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        scorePoint();
    }

    void EnablePlayer()
    {
        this.enabled = true;
        animator.speed = 1;
        playerPhysics.constraints = RigidbodyConstraints2D.FreezeAll;
        player.transform.position = playerStartingPosition;
        animator.SetBool("InGame", true);
        animator.SetBool("PlayerAlive", true);
    }
    void DisablePlayer()
    {  
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
            animator.SetBool("InGame", true);
            animator.SetBool("PlayerAlive", true);
            animator.SetBool("GameStarted", true);
        }
        playerPhysics.constraints = RigidbodyConstraints2D.FreezeRotation;
        gameManager.gameIsRunning = true;
        playerPhysics.velocity = new Vector2(0, playerJumpHeight);
    }
}
