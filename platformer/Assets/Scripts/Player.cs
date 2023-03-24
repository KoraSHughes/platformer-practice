using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int default_double_jumps = 1;
    public int default_wall_jumps = 1;
    public int default_dashes = 1;
    float jump_cooldown = 0f;
    float dash_cooldown = 0f;
    float dash_dist = 7f;
    float walk_speed = 5;
    float jump_height = 4f;
    float double_jump_height = 3f;

    public enum state{  // TODO: attach sprites to states
        idle,
            // attacking,
        walking,
            // walk_attacking,
        jumping,  // Note: anything past this counts as jumping (index: 3)
                jump_walking,
            dashing,
            double_jumping,
            wall_jumping
    }
    state player_state = state.idle;
    bool is_attacking = false;
    int double_jumps;
    int wall_jumps;

    GameManager _gameManager;
    Rigidbody2D _rigidbody2D;
    
    public GameObject feet;  // bottom of player, used to check if we are on the ground
    public LayerMask whatIsGround;
    bool is_grounded = true;

    void Start()
    {
        _gameManager = GameObject.FindObjectOfType<GameManager>();
        _rigidbody2D = GetComponent<Rigidbody2D>();

        double_jumps = default_double_jumps;
        wall_jumps = default_wall_jumps;
    }

    void FixedUpdate()
    {
        
    }
    
    void Update()
    {
        is_grounded = Physics2D.OverlapCircle(feet.transform.position, .3f, whatIsGround);
        if (dash_cooldown > 0){
            dash_cooldown -= Time.deltaTime;
        }
        else{
            dash_cooldown = 0f;  // saftey in case time subtracts more than necessary
        }
        if (jump_cooldown > 0){
            jump_cooldown -= Time.deltaTime;
        }
        else{
            jump_cooldown = 0;
        }

        _rigidbody2D.angularVelocity = 0f; // TODO: make sure obj doesnt rotate


        Debug.Log("Player State: " + player_state.ToString() + ", Jumps: "+ (double_jumps, wall_jumps).ToString()
                  + ", Attacking: " + is_attacking.ToString() + ", Grounded|Still?" + (is_grounded, is_still()).ToString()
                  + ", Dash CD: " + dash_cooldown.ToString() + ", Jump CD: " + jump_cooldown.ToString());

        float yMove = Input.GetAxis("Vertical");
        float xMove = Input.GetAxis("Horizontal");

        if (is_grounded){  // on the ground
            double_jumps = default_double_jumps;
            wall_jumps = default_wall_jumps;

            if(Input.GetButton("Fire3") && dash_cooldown == 0){  // dashing
                int facing = (xMove >= 0) ? 1 : -1;
                _rigidbody2D.velocity = new Vector2(dash_dist*facing, _rigidbody2D.velocity.y/2);
                //Note: we half our vertical velocity (better feel)
                dash_cooldown += 2f;
                player_state = state.dashing;
            }
            else if (yMove > 0 && jump_cooldown == 0) { // jumping
                _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, jump_height);
                jump_cooldown += 0.7f;
                player_state = state.jumping;
            }
            else if (xMove != 0){  // walking
                _rigidbody2D.velocity = new Vector2(xMove*walk_speed, _rigidbody2D.velocity.y);
                player_state = state.walking;
            }
            else{
                if (is_still()){
                    player_state = state.idle; // player idle
                }
                else{
                    player_state = state.walking;
                }
            }
        }
        else{  // mid-air
            if(Input.GetButton("Fire3") && dash_cooldown == 0){  // dashing
                int facing = (xMove >= 0) ? 1 : -1;
                _rigidbody2D.velocity = new Vector2(dash_dist*facing, _rigidbody2D.velocity.y/2);
                //Note: we half our vertical velocity (better feel)
                dash_cooldown += 2f;
                player_state = state.dashing;
            }
            else if (yMove > 0 && jump_cooldown == 0) { // extra jumping
                _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, jump_height);
                jump_cooldown += 0.5f;
                player_state = state.jumping;

                int wall_dir = check_for_wall();
                if (wall_jumps > 0 && wall_dir != 0){  // wall-jumping
                    _rigidbody2D.velocity = new Vector2(wall_dir*walk_speed, double_jump_height);
                    player_state = state.wall_jumping;
                    wall_jumps -= 1;
                    // double_jumps += 1;
                }
                else if(double_jumps > 0){   // double-jumping
                    if (_rigidbody2D.velocity.y > double_jump_height){
                        _rigidbody2D.velocity += new Vector2(0, double_jump_height);
                    }
                    else{  // cancel current momentum if we aren't going up
                        _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, double_jump_height);
                    }
                    player_state = state.double_jumping;
                    double_jumps -= 1;
                }
                else{
                    player_state = state.jumping;
                }
            }
            else if (xMove != 0){  // walking
                _rigidbody2D.velocity = new Vector2(xMove*walk_speed, _rigidbody2D.velocity.y);
                player_state = state.jump_walking;
            }
            else{
                player_state = state.jumping;
            }
        }
        
        if (Input.GetButton("Jump")){ // attacking
            //TODO: maybe add a attacking cooldown
            is_attacking = true;
        }
        else{
            is_attacking = false;
        }
    }

    private int check_for_wall(){
        // TODO: finds nearest wall and returns -1 if left of user, 0 if not close enough, and 1 if right of user
        return 0;
    }
    private bool is_still(){  // not moving much horizontally
        return _rigidbody2D.velocity.x <= 0.5f && _rigidbody2D.velocity.x >= -0.5f && _rigidbody2D.velocity.y <= 0.3f && _rigidbody2D.velocity.y >= -0.3f;
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
