using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum state{  // TODO: attach sprites to states
        idle,
        walking,
                sprinting,
        jumping,
                jump_walking,
                jump_sprinting,
            wall_jumping,
                wall_jump_sprinting,
                wall_jump_walking,
            double_jumping,  // half the height of jump & wall_jump
                double_jump_walking,
                double_jump_sprinting
    }
    state player_state = state.idle;
    bool is_shooting = false;

    public int walk_speed = 2;  // TODO: fix how the player moves with these
    public int sprint_speed = 8;
    public int jump_height = 5;
    public int double_jump_height = 2;
    // TODO: fix weird floating issue when player is in the air and moving

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

        _rigidbody2D.angularVelocity = 0f; // TODO: make sure obj doesnt rotate

        if (Input.GetButtonDown("Vertical")){  // jumping
            if (player_state == state.idle || player_state == state.walking || player_state == state.sprinting){
                player_state = state.jumping;
                _rigidbody2D.velocity = new Vector2(0, jump_height);
            }
            else if (player_state == state.jumping && check_for_wall()){  // wall jumping
                // TODO: add velocity perpendicular to wall
                player_state = state.wall_jumping;
            }
            else if (player_state == state.jumping || player_state == state.jump_sprinting){ // double-jumping
                player_state = state.double_jumping;
                _rigidbody2D.velocity = new Vector2(0, double_jump_height);
            }
        }

        if (Input.GetAxis("Horizontal") == 0){
            if (is_grounded()){
                player_state = state.idle;
            }
        }
        else { // movement
            if(Input.GetButton("Fire3")) { // sprinting
                if (player_state == state.jumping){
                    player_state = state.jump_sprinting;
                }
                else if (player_state == state.jump_walking){
                    player_state = state.jump_sprinting;
                }
                else if (player_state == state.wall_jumping){
                    player_state = state.wall_jump_sprinting;
                }
                else if (player_state == state.double_jumping){
                    player_state = state.double_jump_sprinting;
                }
                else if (player_state == state.idle){
                    player_state = state.sprinting;
                }
                else if (player_state == state.walking){
                    player_state = state.sprinting;
                }
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
        else{
            is_shooting = false;
        }
    }

    private bool check_for_wall(){
        // TODO: check for nearby wall
        return false;
    }
    private bool is_grounded(){
        // check if player on platform or not moving?
        return _rigidbody2D.velocity == Vector2.zero;
    }

    public state get_state(){
        return player_state;
    }
}
