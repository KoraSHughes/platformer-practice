using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static int default_double_jumps = 1;
    public static int default_wall_jumps = 1;
    public int walk_speed = 2;  // TODO: fix how the player moves with these
    public int sprint_speed = 8;
    public int jump_height = 5;
    public int double_jump_height = 2;  // half the height of jump & wall_jump
    // TODO: fix weird floating issue when player is in the air and moving

    public enum state{  // TODO: attach sprites to states
        idle,
            // shooting,
        walking,
            // walk_shooting,
                sprinting,
                    // sprint_shooting,
        jumping,  // Note: anything past this counts as jumping (index: 3)
                jump_walking,
                jump_sprinting,
            wall_jumping,
            //     wall_jump_sprinting,
            //     wall_jump_walking
            double_jumping
            //     double_jump_walking,
            //     double_jump_sprinting
    }
    state player_state = state.idle;
    bool is_shooting = false;
    int double_jumps = default_double_jumps;
    int wall_jumps = default_wall_jumps;

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
        Debug.Log("Player State: " + player_state.ToString() + ", Jumps: "+ (double_jumps, wall_jumps).ToString() + ", Shooting: " + is_shooting.ToString());
        if (is_grounded() && is_jumping()){
            Debug.LogError("Contradiction in Jumping & Groundedness");
        }

        _rigidbody2D.angularVelocity = 0f; // TODO: make sure obj doesnt rotate


        if (is_grounded()){  // double jump reset
            double_jumps = default_double_jumps;
            wall_jumps = default_wall_jumps;

            if (_rigidbody2D.velocity.x == 0f){
                player_state = state.idle; // player idle
            }
        }
        else{
            if (_rigidbody2D.velocity.x == 0f){
                player_state = state.jumping; // player idle
            }
        }


        if (Input.GetButtonDown("Vertical")){  // jumping attempt
            if (is_grounded()){
                player_state = state.jumping;
                _rigidbody2D.velocity += new Vector2(0, jump_height);
            }
            else if (is_jumping()){
                if (wall_jumps > 0 && check_for_wall()){  // wall-jumping
                    // TODO: add velocity perpendicular to wall
                    player_state = state.wall_jumping;
                    wall_jumps -= 1;
                }
                else if(double_jumps > 0){   // double-jumping
                    _rigidbody2D.velocity += new Vector2(0, double_jump_height);
                    player_state = state.double_jumping;
                    double_jumps -= 1;
                }
            }
        }


        float xMove = Input.GetAxis("Horizontal");
        if (xMove != 0){  // horizontal movement attempt
            if(Input.GetButton("Fire3")) {  // sprinting
                if (is_jumping()){
                    player_state = state.jump_sprinting;
                }
                else{
                    player_state = state.sprinting;
                }
                _rigidbody2D.velocity = new Vector2(xMove*sprint_speed, _rigidbody2D.velocity.y);
            }
            else{  // walking
                if (is_jumping()){
                    player_state = state.jump_walking;
                }
                else{
                    player_state = state.walking;
                }
                _rigidbody2D.velocity = new Vector2(xMove*walk_speed, _rigidbody2D.velocity.y);
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
    private bool check_for_platform(){
        // TODO: check for nearby platform
        return true;
    }
    private bool is_grounded(){
        // =idle/walking/sprinting
        return _rigidbody2D.velocity.y == 0f && check_for_platform();
    }
    private bool is_jumping(){  // we are jumping
        // jumping/double-jumping/wall-jumping + any combo of sprinting/walking/shooting
        return (int)player_state >= (int)state.jumping;
    }


    public state get_state(){
        return player_state;
    }
    public (int, int) addJumps(int doubles, int walls){
        // TODO: add power-ups that change this value?
        double_jumps += doubles;
        wall_jumps += walls;
        return (double_jumps, wall_jumps);
    }
}
