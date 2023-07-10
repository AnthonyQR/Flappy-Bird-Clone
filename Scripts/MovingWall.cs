using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingWall : MonoBehaviour
{
    public GameObject wall;
    public Transform wallTransform;
    public GameManagerScript gameManager;
    public BoxCollider2D movingWallCollider;

    public float wallMoveSpeed;
    private ContactPoint2D[] contacts;
    public int wallYPositionFloor;
    public int wallYPositionCeiling;
    public Vector2 newPosition;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManagerScript>();
        newPosition = transform.position;
        newPosition.y = (Random.Range(wallYPositionFloor, wallYPositionCeiling));
        newPosition.y /= 10;
        wallTransform.position = newPosition;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (gameManager.gameIsRunning)
        {
            wallTransform.position = new Vector2((wallTransform.position.x - wallMoveSpeed), wallTransform.position.y);
        }
        
    }

    void OnTriggerEnter2D(Collider2D collidedObject)
    {
        Debug.Log("Collision");
        if (collidedObject.tag != "Player")
        {
            Debug.Log("Collide");
            Destroy(this.gameObject);
        }
    }
}
