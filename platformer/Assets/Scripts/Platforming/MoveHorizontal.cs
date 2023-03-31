using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveHorizontal : MonoBehaviour
{
    private Vector2 startPos;
    private Vector2 newPos;
    public float speed = 0.5f;// SPEED OF MOVEMENT
    public float distance = 1f; // MAX DISTANCE FOR DIRECTION X
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        newPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        newPos.x = startPos.x + (distance * Mathf.Sin(Time.time * speed)); // Math.pingpong?
        transform.position = newPos;
    }
}
