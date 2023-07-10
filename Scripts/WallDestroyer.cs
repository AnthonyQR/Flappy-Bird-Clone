using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallDestroyer : MonoBehaviour
{
    public BoxCollider2D wallCollider;
    // Start is called before the first frame update
    void Start()
    {
        wallCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D wallCollider)
    {
        Destroy(wallCollider);
    }
}
