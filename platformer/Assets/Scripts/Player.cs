using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum state{
        idle,
        walking,
            sprinting,
        jumping,
            jump_sprinting,
            wall_jumping,
            double_jumping,  // half the height of jump & wall_jump
    }
    public state player_state = state.idle;
    bool is_shooting = false;

    public int walk_speed = 10;
    public int sprint_speed = 10;
    public int jump_height = 10;

    GameManager _gameManager;
    Rigidbody2D _rigidbody2D;

    void Start()
    {
        _gameManager = GameObject.FindObjectOfType<GameManager>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        
    }
    
    void Update()
    {
        Debug.Log("Player State: " + player_state.ToString() + ", Shooting: " + is_shooting.ToString());
        float xVeloc = Input.GetAxis("Horizontal");

        if (Input.GetAxis("Vertical") != 0){  // jumping
            if (player_state == state.idle || player_state == state.walking || player_state == state.sprinting){
                player_state = state.jumping;
                _rigidbody2D.velocity = new Vector2(0, 10);
            }
            else if (player_state == state.jumping || player_state == state.jump_sprinting){ // double-jumping
                player_state = state.double_jumping;
                _rigidbody2D.velocity = new Vector2(0, 10);
            }
        }
        if (Input.GetAxis("Horizontal") != 0){ // movement
            if(Input.GetButtonDown("Fire3")) { // sprinting
                player_state = state.sprinting;
                _rigidbody2D.velocity = new Vector2(xVeloc*sprint_speed, 0);
            }
            else{  // walking
                player_state = state.walking;
                _rigidbody2D.velocity = new Vector2(xVeloc*walk_speed, 0);
            }
        }

        if (Input.GetButton("Jump")){ // shooting
            is_shooting = true;
        }
    }
}
