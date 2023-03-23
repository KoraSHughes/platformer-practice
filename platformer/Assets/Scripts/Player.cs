using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int default_double_jumps = 1;
    public int default_wall_jumps = 1;
    float walk_speed = 5;
    float sprint_speed;
    float jump_height = 4f;
    float double_jump_height;

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
    int double_jumps;
    int wall_jumps;

    GameManager _gameManager;
    Rigidbody2D _rigidbody2D;
    GameObject foot;  // bottom of player, used to check if we are on the ground


    void Start()
    {
        _gameManager = GameObject.FindObjectOfType<GameManager>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        // foot = transform.Find("feet");
        foreach (Transform child in transform)
          {
              if (child.tag == "feet")
                  foot = child.gameObject;
          }

        double_jumps = default_double_jumps;
        wall_jumps = default_wall_jumps;
        updateSprint();
        updateJump();
    }

    void FixedUpdate()
    {
        
    }
    
    void Update()
    {
        Debug.Log("Player State: " + player_state.ToString() + ", Jumps: "+ (double_jumps, wall_jumps).ToString()
                  + ", Shooting: " + is_shooting.ToString() + ", Grounded|Jumping|Still?" + (is_grounded(), is_jumping(), is_still()).ToString());
        if (is_grounded() && is_jumping()){
            Debug.LogError("Contradiction in Jumping & Groundedness");
        }

        _rigidbody2D.angularVelocity = 0f; // TODO: make sure obj doesnt rotate


        if (is_grounded()){  // double jump reset
            double_jumps = default_double_jumps;
            wall_jumps = default_wall_jumps;

            if (is_still()){
                player_state = state.idle; // player idle
            }
        }


        if (Input.GetButtonDown("Vertical")){  // jumping attempt
            if (is_jumping()){
                int wall_dir = check_for_wall();
                if (wall_jumps > 0 && wall_dir != 0){  // wall-jumping
                    _rigidbody2D.velocity = new Vector2(wall_dir*sprint_speed, double_jump_height);
                    player_state = state.wall_jumping;
                    wall_jumps -= 1;
                }
                else if(double_jumps > 0){   // double-jumping
                    if (_rigidbody2D.velocity.y > double_jump_height){
                        _rigidbody2D.velocity += new Vector2(0, double_jump_height/2);
                    }
                    else{  // cancel current momentum if we aren't going up
                        _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, double_jump_height);
                    }
                    player_state = state.double_jumping;
                    double_jumps -= 1;
                }
            }
            else if (is_grounded()){
                player_state = state.jumping;
                _rigidbody2D.velocity += new Vector2(0, jump_height);
                // TODO: fix state where jump height is lower when moving
                // TODO: fix state where double jump counter decrements from normal jump
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

    private int check_for_wall(){
        // TODO: finds nearest wall and returns -1 if left of user, 0 if not close enough, and 1 if right of user
        return 0;
    }
    private bool check_for_platform(){
        // TODO: check for nearby platform
        // List<string> objs = foot.GetComponent<FindObjects>().getObjs();
        return true;
    }
    private bool is_grounded(){  // not moving much vertically
        // =idle/walking/sprinting
        return _rigidbody2D.velocity.y <= 0.5f && _rigidbody2D.velocity.y >= -0.5f && check_for_platform();
    }
    private bool is_jumping(){  // we are jumping
        // jumping/double-jumping/wall-jumping + any combo of sprinting/walking/shooting
        return (int)player_state >= (int)state.jumping;
    }
    private bool is_still(){  // not moving much horizontally
        return _rigidbody2D.velocity.x <= 0.5f && _rigidbody2D.velocity.x >= -0.5f;
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
    public void updateSprint(){
        sprint_speed = walk_speed*1.5f;
    }
    public void updateJump(){
        double_jump_height = jump_height/1.5f;
    }
}
