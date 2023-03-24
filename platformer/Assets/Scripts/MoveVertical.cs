using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveVertical : MonoBehaviour
{
    private Vector2 startPos;
    private Vector2 newPos;
    public float speed = 1;// SPEED OF MOVEMENT
    public float distance = 5; // MAX DISTANCE FOR DIRECTION Y
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        newPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        newPos.y = startPos.y + (distance * Mathf.Sin(Time.time * speed));
        transform.position = newPos;
    }
}
