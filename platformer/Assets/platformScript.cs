using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class platformScript : MonoBehaviour
{

    // private Vector2 startPos;

    // Start is called before the first frame update
    void Start()
    {
        // startPos = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
    
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Player")) {
            other.transform.SetParent(transform);
        }
    }

    void OnCollisionExit2D(Collision2D coll) {
        if (coll.gameObject.CompareTag("Player")) {
            coll.transform.SetParent(null);
        }
    }
}
